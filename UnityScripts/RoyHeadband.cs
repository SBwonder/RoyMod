// Editor copy of RoyMod's RoyHeadband component (Characters/Survivors/Henry/Components/RoyHeadband.cs), for previewing in Play mode.
using System.Collections.Generic;
using UnityEngine;

    // Smash swings Roy's headband tails and cape (the S_Headband* and S_Mantle* bones) with its own physics at runtime,
    // so the animations either never move them (headband: straight up) or hold them in one stiff keyed shape (cape).
    // This runs each strip as a simple verlet chain: gravity and drag pull it down, it trails behind when he moves,
    // a spring pulls it gently toward the animated pose, and spheres around his head, body and legs keep it outside him.
    public class RoyHeadband : MonoBehaviour
    {
        private class ChainSettings
        {
            public string root;
            public float stiffness;             // pull toward the animated pose per 60 Hz step (keep tiny: it fights gravity, 0.03 already holds a strip rigid)
            public float damping;               // fraction of velocity lost per 60 Hz step
            public bool collideBody;            // cape strips collide with the torso and legs, headband with the head
        }

        private static readonly ChainSettings[] chainSettings =
        {
            new ChainSettings { root = "S_HeadbandL1", stiffness = 0f, damping = 0.2f },
            new ChainSettings { root = "S_HeadbandR1", stiffness = 0f, damping = 0.2f },
            new ChainSettings { root = "S_MantleL1", stiffness = 0.005f, damping = 0.2f, collideBody = true },
            new ChainSettings { root = "S_MantleC1", stiffness = 0.005f, damping = 0.2f, collideBody = true },
            new ChainSettings { root = "S_MantleR1", stiffness = 0.005f, damping = 0.2f, collideBody = true },
        };

        // collision spheres: bone name and radius in model units (Roy authored 1.49 tall)
        private static readonly (string bone, float radius)[] bodySpheres =
        {
            ("Bust", 0.14f), ("Waist", 0.13f), ("Hip", 0.13f), ("LegL", 0.08f), ("LegR", 0.08f), ("KneeL", 0.07f), ("KneeR", 0.07f),
        };

        public Vector3 gravity = new Vector3(0f, -9.81f, 0f);
        // how much of the body's own travel the strips simply follow; the rest becomes drag (lower = more trailing)
        public float followMovement = 0.5f;
        // jumps launch instantly in ROR2, so vertical travel is followed almost fully or the cape flips over his head
        public float followVertical = 0.9f;
        // cap on how fast a strip point can move relative to the last frame (model units per 60 Hz step)
        public float maxSpeed = 0.06f;

        private class Chain
        {
            public ChainSettings settings;
            public Transform[] bones;           // root .. tip (tip is the *_null end bone)
            public Quaternion[] restLocal;
            public Quaternion[] written;        // what we set last frame, to tell whether the animation keyed the bone since
            public float[] lengths;
            public Vector3[] pos, prev;
        }

        private readonly List<Chain> chains = new List<Chain>();
        private readonly List<(Transform bone, float radius)> spheres = new List<(Transform, float)>();
        private Transform head;
        private Transform hairTop;
        private bool initialized;
        private Vector3 lastRootPosition;

        public void Awake()
        {
            var all = GetComponentsInChildren<Transform>(true);
            foreach (Transform t in all)
            {
                if (t.name == "Head") head = t;
                if (t.name == "S_HairC1") hairTop = t;
            }
            foreach (var s in bodySpheres)
            {
                Transform b = System.Array.Find(all, t => t.name == s.bone);
                if (b) spheres.Add((b, s.radius));
            }
            foreach (ChainSettings settings in chainSettings)
            {
                Transform b = System.Array.Find(all, t => t.name == settings.root);
                if (!b) continue;
                var bones = new List<Transform>();
                while (b)
                {
                    bones.Add(b);
                    b = b.childCount > 0 ? b.GetChild(0) : null;
                }
                var c = new Chain
                {
                    settings = settings,
                    bones = bones.ToArray(),
                    restLocal = new Quaternion[bones.Count],
                    written = new Quaternion[bones.Count],
                    lengths = new float[bones.Count],
                    pos = new Vector3[bones.Count],
                    prev = new Vector3[bones.Count],
                };
                for (int i = 0; i < bones.Count; i++) c.restLocal[i] = c.written[i] = bones[i].localRotation;
                chains.Add(c);
            }
        }

        private void OnEnable() => initialized = false;

        private void LateUpdate() => Simulate(Time.deltaTime);

        public void Simulate(float deltaTime)
        {
            if (!head || chains.Count == 0) return;
            float dt = Mathf.Min(deltaTime, 1f / 20f);
            if (dt <= 0f) return;
            float step = dt * 60f;
            float scale = transform.lossyScale.x;

            // head sphere: centred between the head bone and the top of the hair
            Vector3 top = hairTop ? hairTop.position : head.position + head.up * 0.15f * scale;
            Vector3 headCentre = Vector3.Lerp(head.position, top, 0.45f);
            float headRadius = Vector3.Distance(head.position, top) * 0.55f;

            Vector3 moved = initialized ? transform.position - lastRootPosition : Vector3.zero;
            Vector3 rootMove = new Vector3(moved.x * followMovement, moved.y * followVertical, moved.z * followMovement);
            lastRootPosition = transform.position;

            foreach (Chain c in chains)
            {
                for (int i = 1; i < c.bones.Length; i++)
                {
                    c.pos[i] += rootMove;
                    c.prev[i] += rootMove;
                }

                // the animated pose this frame: whatever the animation wrote, or the bind pose if it didn't key this bone
                for (int i = 0; i < c.bones.Length; i++)
                    if (c.bones[i].localRotation == c.written[i]) c.bones[i].localRotation = c.restLocal[i];

                if (!initialized || (c.pos[0] - c.bones[0].position).sqrMagnitude > 4f * scale * scale)
                {
                    for (int i = 0; i < c.bones.Length; i++)
                    {
                        c.pos[i] = c.prev[i] = c.bones[i].position;
                        if (i > 0) c.lengths[i] = Vector3.Distance(c.bones[i - 1].position, c.bones[i].position);
                    }
                }

                float keep = Mathf.Pow(1f - c.settings.damping, step);
                float pull = 1f - Mathf.Pow(1f - c.settings.stiffness, step);
                c.pos[0] = c.prev[0] = c.bones[0].position;
                for (int i = 1; i < c.bones.Length; i++)
                {
                    Vector3 velocity = Vector3.ClampMagnitude((c.pos[i] - c.prev[i]) * keep, maxSpeed * scale * step);
                    c.prev[i] = c.pos[i];
                    c.pos[i] += velocity + gravity * scale * dt * dt;
                    c.pos[i] += (c.bones[i].position - c.pos[i]) * pull;

                    Constrain(c, i);
                    if (c.settings.collideBody)
                        foreach (var s in spheres) PushOut(c, i, s.bone.position, s.radius * scale);
                    else
                        PushOut(c, i, headCentre, headRadius);
                }

                // aim each bone at its simulated child
                for (int i = 0; i < c.bones.Length - 1; i++)
                {
                    Vector3 current = c.bones[i + 1].position - c.bones[i].position;
                    Vector3 wanted = c.pos[i + 1] - c.bones[i].position;
                    if (current.sqrMagnitude > 1e-8f && wanted.sqrMagnitude > 1e-8f)
                        c.bones[i].rotation = Quaternion.FromToRotation(current, wanted) * c.bones[i].rotation;
                }
                for (int i = 0; i < c.bones.Length; i++) c.written[i] = c.bones[i].localRotation;
            }
            initialized = true;
        }

        private static void Constrain(Chain c, int i)
        {
            Vector3 d = c.pos[i] - c.pos[i - 1];
            c.pos[i] = c.pos[i - 1] + (d.sqrMagnitude > 1e-10f ? d.normalized : Vector3.down) * c.lengths[i];
        }

        private static void PushOut(Chain c, int i, Vector3 centre, float radius)
        {
            Vector3 fromCentre = c.pos[i] - centre;
            if (fromCentre.sqrMagnitude >= radius * radius) return;
            c.pos[i] = centre + (fromCentre.sqrMagnitude > 1e-10f ? fromCentre.normalized : Vector3.back) * radius;
            Constrain(c, i);
        }
    }
