using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Smash models keep each facial expression as separate meshes (eyes, eyelids/brows, mouth).
    // This shows one expression at a time, picked from what Roy is doing: hurt > heavy attack > attack > dodge > normal (with blinking).
    internal class RoyFace : MonoBehaviour
    {
        private enum Face { Open, Blink, Attack, HeavyAttack, Dodge, Hurt, HeavyHurt }

        // mesh name prefixes (Smash names, before "_VIS") for each expression
        private static readonly Dictionary<Face, string[]> facePrefixes = new Dictionary<Face, string[]>
        {
            { Face.Open,        new[] { "Roy_Openblink", "Roy_Eye", "Roy_Mouth_FaceN" } },
            { Face.Blink,       new[] { "Roy_Blink", "Roy_Mouth_FaceN" } },
            { Face.Attack,      new[] { "Roy_Blink_Attack", "Roy_Mouth_Attack" } },
            { Face.HeavyAttack, new[] { "Roy_Blink_HeavyAttack", "Roy_Mouth_HeavyAttack" } },
            { Face.Dodge,       new[] { "Roy_Blink_Escape", "Roy_Mouth_Escape" } },
            { Face.Hurt,        new[] { "Roy_Blink_Ouch", "Roy_Mouth_Ouch" } },
            { Face.HeavyHurt,   new[] { "Roy_Blink_HeavOouch", "Roy_Mouth_HeavyOuch" } },
        };

        private static readonly HashSet<string> heavyAttackStates = new HashSet<string>
        {
            "FSmash", "DED4", "DED4Up", "DED4Down", "UpSmashAir", "Blazer", "FlareBladeStart", "FlareBlade",
            "CounterHit", "CounterHitAir", "FinalStart", "Final", "FinalAirStart", "FinalAir",
        };
        private static readonly HashSet<string> attackStates = new HashSet<string>
        {
            "Pummel", "Jab1", "FTilt", "DashAttack", "NAir", "UpAir", "DownAir",
            "DED1", "DED2", "DED2Up", "DED3", "DED3Up", "DED3Down", "Counter", "CounterAir",
        };
        private static readonly HashSet<string> dodgeStates = new HashSet<string>
        {
            "Roll", "RollBack", "RollMirrored", "SpotDodge", "AirDodge",
        };
        private static readonly string[] hurtStates = { "Flinch1", "Flinch2", "FlinchAir1", "FlinchAir2" };
        private static readonly string[] heavyHurtStates = { "Flinch3", "FlinchAir3" };
        private static readonly string[] actionLayers = { "Gesture, Full Body", "Gesture, Override", "Body" };

        private const float hurtFaceDuration = 0.5f;
        private const float heavyHurtFraction = 0.1f;  // a hit taking 10% of max health or more
        private const float blinkDuration = 0.12f;

        private readonly Dictionary<Face, List<GameObject>> faceObjects = new Dictionary<Face, List<GameObject>>();
        private readonly List<GameObject> allFaceObjects = new List<GameObject>();
        private readonly Dictionary<int, string> stateNames = new Dictionary<int, string>();
        private Animator animator;
        private CharacterModel characterModel;
        private int[] actionLayerIndices;
        private int flinchLayer = -1;
        private int bodyLayer = -1;
        private static readonly int entryHash = Animator.StringToHash("Entry");
        private const float entryEyesOpenTime = 0.75f;
        private Face current = (Face)(-1);
        private float lastHealth = -1f;
        private float hurtTimer;
        private bool hurtHeavy;
        private float blinkTimer;
        private float nextBlink;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterModel = GetComponent<CharacterModel>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            {
                if (!r.name.StartsWith("Roy_")) continue;
                string prefix = r.name.Split(new[] { "_VIS" }, System.StringSplitOptions.None)[0];
                allFaceObjects.Add(r.gameObject);
                foreach (var pair in facePrefixes)
                {
                    if (System.Array.IndexOf(pair.Value, prefix) < 0) continue;
                    if (!faceObjects.TryGetValue(pair.Key, out var list)) faceObjects[pair.Key] = list = new List<GameObject>();
                    list.Add(r.gameObject);
                }
            }

            foreach (string s in heavyAttackStates) stateNames[Animator.StringToHash(s)] = s;
            foreach (string s in attackStates) stateNames[Animator.StringToHash(s)] = s;
            foreach (string s in dodgeStates) stateNames[Animator.StringToHash(s)] = s;
            foreach (string s in hurtStates) stateNames[Animator.StringToHash(s)] = s;
            foreach (string s in heavyHurtStates) stateNames[Animator.StringToHash(s)] = s;

            if (animator)
            {
                actionLayerIndices = new int[actionLayers.Length];
                for (int i = 0; i < actionLayers.Length; i++) actionLayerIndices[i] = animator.GetLayerIndex(actionLayers[i]);
                flinchLayer = animator.GetLayerIndex("Flinch");
                bodyLayer = animator.GetLayerIndex("Body");
            }
            nextBlink = Random.Range(2f, 5f);
            SetFace(Face.Open);
        }

        private void Update()
        {
            SetFace(PickFace());
        }

        private Face PickFace()
        {
            // a fresh hit shows the hurt face for a moment
            HealthComponent health = characterModel && characterModel.body ? characterModel.body.healthComponent : null;
            if (health)
            {
                float hp = health.combinedHealth;
                if (lastHealth >= 0f && hp < lastHealth - 0.01f)
                {
                    hurtTimer = hurtFaceDuration;
                    hurtHeavy = (lastHealth - hp) >= health.fullCombinedHealth * heavyHurtFraction;
                }
                lastHealth = hp;
            }
            if (hurtTimer > 0f)
            {
                hurtTimer -= Time.deltaTime;
                return hurtHeavy ? Face.HeavyHurt : Face.Hurt;
            }

            if (animator && animator.isActiveAndEnabled)
            {
                if (bodyLayer >= 0)
                {
                    AnimatorStateInfo body = animator.GetCurrentAnimatorStateInfo(bodyLayer);
                    if (body.shortNameHash == entryHash && body.normalizedTime * body.length < entryEyesOpenTime) return Face.Blink;
                }

                string flinch = flinchLayer >= 0 && animator.GetLayerWeight(flinchLayer) > 0.01f ? CurrentState(flinchLayer) : null;
                if (flinch != null && System.Array.IndexOf(heavyHurtStates, flinch) >= 0) return Face.HeavyHurt;
                if (flinch != null && System.Array.IndexOf(hurtStates, flinch) >= 0) return Face.Hurt;

                bool attack = false, dodge = false;
                foreach (int layer in actionLayerIndices)
                {
                    if (layer < 0) continue;
                    string state = CurrentState(layer);
                    if (state == null) continue;
                    if (heavyAttackStates.Contains(state)) return Face.HeavyAttack;
                    attack |= attackStates.Contains(state);
                    dodge |= dodgeStates.Contains(state);
                }
                if (attack) return Face.Attack;
                if (dodge) return Face.Dodge;
            }

            // idle blinking
            if (blinkTimer > 0f)
            {
                blinkTimer -= Time.deltaTime;
                return Face.Blink;
            }
            nextBlink -= Time.deltaTime;
            if (nextBlink <= 0f)
            {
                nextBlink = Random.Range(2f, 5f);
                blinkTimer = blinkDuration;
                return Face.Blink;
            }
            return Face.Open;
        }

        private string CurrentState(int layer)
        {
            // the state we're blending into counts as soon as the transition starts
            AnimatorStateInfo info = animator.IsInTransition(layer) ? animator.GetNextAnimatorStateInfo(layer) : animator.GetCurrentAnimatorStateInfo(layer);
            return stateNames.TryGetValue(info.shortNameHash, out string name) ? name : null;
        }

        private void SetFace(Face face)
        {
            if (face == current) return;
            current = face;
            faceObjects.TryGetValue(face, out var show);
            foreach (GameObject go in allFaceObjects)
            {
                bool on = show != null && show.Contains(go);
                if (go.activeSelf != on) go.SetActive(on);
            }
        }
    }
}
