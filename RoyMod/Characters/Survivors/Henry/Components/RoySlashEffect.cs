using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HenryMod.Survivors.Henry.Components
{
    // Roy's swing effects, borrowed from vanilla:
    //   jab / forward tilt / forward smash / neutral air -> Mercenary's first and second swing slashes
    //   dash attack                                      -> Mercenary's third (finisher) slash, stretched and held longer
    //   knee strike                                      -> REX's DIRECTIVE: Disperse shockwave
    // Default Roy gets orange recolours; the Awakening skin gets blue (Mercenary's own colour; REX's tinted blue).
    // Each slash is laid along the path Roy's blade takes in that attack (see Mount).
    public static class RoySlashEffect
    {
        public enum Look { Swing, Finisher, Whirlwind, Shockwave }

        // Where a slash sits: like Mercenary's, a full-size slash centred on the body at chest height (his mounts sit at 1.35,
        // the finisher at 1.68). Roy's Smash swings happen in an upright plane pointing straight ahead (Smash is seen from the
        // side), so a crescent laid exactly in that plane is seen edge-on from behind as a thin streak. Instead each swing keeps
        // its direction (rising/falling, which side it comes from: the swing-plane normal fitted from the sword tip's path) but
        // the crescent is tipped toward the near-flat angle of Mercenary's slashes, so it reads as a wide arc in front of him.
        public struct Mount
        {
            public Look look;
            public Vector3 rotation;    // Euler angles relative to Roy's facing (used when there's no swing normal)
            public Vector3 swingNormal; // fitted normal of Roy's blade path (relative to his facing); its sign sets the sweep direction
            public float tilt;          // degrees the slash plane is tipped up from flat (Mercenary's are ~15-25)
            public Vector3 centre;      // offset from his feet, relative to his facing
            public float delay;         // fraction of the attack's duration when the blade starts its swing
            public float scale;         // size multiplier on Mercenary's slash (0 = as is)
            public bool followRoy;      // travels with Roy (the dash slash), instead of staying where it was swung
        }

        public static readonly Mount Jab = new Mount { look = Look.Swing, swingNormal = new Vector3(-0.89f, -0.32f, -0.32f), tilt = 35f, centre = new Vector3(0f, 1.35f, 0.3f), delay = 0.02f, scale = 1.15f };
        public static readonly Mount ForwardTilt = new Mount { look = Look.Swing, swingNormal = new Vector3(0.87f, -0.46f, 0.15f), tilt = 35f, centre = new Vector3(0f, 1.35f, 0.3f), delay = 0.03f, scale = 1.15f };
        public static readonly Mount ForwardSmash = new Mount { look = Look.Swing, swingNormal = new Vector3(0.84f, -0.54f, -0.03f), tilt = 45f, centre = new Vector3(0f, 1.35f, 0.3f), delay = 0.09f, scale = 1.3f };
        public static readonly Mount DashAttack = new Mount { look = Look.Finisher, rotation = new Vector3(65.1f, 121f, 276.1f), centre = new Vector3(0f, 1.68f, 0f), delay = 0.07f, followRoy = true };
        public static readonly Mount NeutralAir = new Mount { look = Look.Whirlwind, rotation = new Vector3(90f, 0f, 0f), centre = new Vector3(0f, 1f, 0f), delay = 0.20f, followRoy = true };   // Mercenary's whirlwind roundhouse: its circle lies across its local Z, so tip that axis upright to lay the circle flat around his waist
        public static readonly Mount KneeStrike = new Mount { look = Look.Shockwave, rotation = Vector3.zero, centre = new Vector3(0f, 1.1f, 0.3f), delay = 0.3f };

        private static GameObject swingOrange, swingBlue, finisherOrange, finisherBlue, whirlOrange, whirlBlue, shockOrange, shockBlue;

        // recolouring keeps each effect's own brightness and transparency and only moves its hue
        private const float orangeHue = 0.07f, blueHue = 0.6f;

        // effects are cosmetic: a failure here must never stop Roy from loading
        public static void Init()
        {
            try { LoadEffects(); }
            catch (System.Exception e) { Log.Error("Roy's swing effects failed to load: " + e); }
        }

        private static void LoadEffects()
        {
            GameObject mercSlash = Load("RoR2/Base/Merc/MercSwordSlash.prefab");
            GameObject mercFinisher = Load("RoR2/Base/Merc/MercSwordFinisherSlash.prefab");
            GameObject rexShockwave = Load("RoR2/Base/Treebot/TreebotShockwaveEffect.prefab");
            GameObject mercWhirlwind = Load("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab", "Assets/RoR2/Base/Characters/Merc/Skills/MercSwordSlashWhirlwind.prefab");

            // all slashes linger longer than Mercenary's (Alex); the dash slash is also a bit wider
            if (mercSlash)
            {
                swingOrange = Recolour(mercSlash, "RoySwingSlashOrange", orangeHue, 1f, 1.5f);
                swingBlue = Recolour(mercSlash, "RoySwingSlashBlue", null, 1f, 1.5f);
            }
            if (mercFinisher)
            {
                finisherOrange = Recolour(mercFinisher, "RoyDashSlashOrange", orangeHue, 1.25f, 1.6f, true);
                finisherBlue = Recolour(mercFinisher, "RoyDashSlashBlue", null, 1.25f, 1.6f, true);
            }
            if (mercWhirlwind)
            {
                whirlOrange = Recolour(mercWhirlwind, "RoyAirWhirlwindOrange", orangeHue, 1f, 1f, true);
                whirlBlue = Recolour(mercWhirlwind, "RoyAirWhirlwindBlue", null, 1f, 1f, true);
            }
            if (rexShockwave)
            {
                shockOrange = HoldRings(Recolour(rexShockwave, "RoyKneeShockwaveOrange", orangeHue, 1f, 1f));
                shockBlue = HoldRings(Recolour(rexShockwave, "RoyKneeShockwaveBlue", blueHue, 1f, 1f));
            }
        }

        // vanilla addresses are "RoR2/Base/<Character>/<Name>.prefab"; newer builds keep assets under Characters/, so try both
        private static GameObject Load(string key, string alternative = null)
        {
            foreach (string candidate in new[] { key, key.Replace("RoR2/Base/", "RoR2/Base/Characters/"), alternative })
            {
                if (candidate == null) continue;
                try
                {
                    if (Addressables.LoadResourceLocationsAsync(candidate).WaitForCompletion().Count == 0) continue;
                    GameObject prefab = Addressables.LoadAssetAsync<GameObject>(candidate).WaitForCompletion();
                    if (prefab) return prefab;
                }
                catch (System.Exception e) { Log.Warning("couldn't load " + candidate + ": " + e.Message); }
            }
            Log.Error("missing vanilla effect " + key);
            return null;
        }

        // copy of a vanilla effect with its colours hue-shifted (particle colours, material tints, Hopoo's remap ramps, lights),
        // optionally with every particle system's size and lifetime scaled. Distortion materials are left alone (REX's rings).
        // Hue-shifting the ramp rather than replacing it matters: the ramp's transparency is what cuts out the crescent shape.
        private static GameObject Recolour(GameObject source, string name, float? hue, float sizeScale, float lifetimeScale, bool localSpace = false)
        {
            GameObject copy = R2API.PrefabAPI.InstantiateClone(source, name, false);
            foreach (ParticleSystem ps in copy.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = ps.main;
                if (sizeScale != 1f) main.startSizeMultiplier *= sizeScale;
                if (lifetimeScale != 1f) main.startLifetimeMultiplier *= lifetimeScale;
                // particles that should move with their parent (the dash slash rides along with Roy)
                if (localSpace) main.simulationSpace = ParticleSystemSimulationSpace.Local;
                if (!hue.HasValue) continue;
                main.startColor = Shift(main.startColor, hue.Value);
                var col = ps.colorOverLifetime;
                if (col.enabled) col.color = Shift(col.color, hue.Value);
            }
            if (hue.HasValue)
            {
                foreach (Renderer renderer in copy.GetComponentsInChildren<Renderer>(true))
                {
                    Material[] materials = renderer.sharedMaterials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        if (!materials[i] || (materials[i].shader && materials[i].shader.name.Contains("Distort"))) continue;
                        Material m = new Material(materials[i]);
                        if (m.HasProperty("_TintColor")) m.SetColor("_TintColor", Shift(m.GetColor("_TintColor"), hue.Value));
                        if (m.HasProperty("_Color")) m.SetColor("_Color", Shift(m.GetColor("_Color"), hue.Value));
                        if (m.HasProperty("_RemapTex") && m.GetTexture("_RemapTex")) m.SetTexture("_RemapTex", ShiftTexture(m.GetTexture("_RemapTex"), hue.Value));
                        materials[i] = m;
                    }
                    renderer.sharedMaterials = materials;
                }
                foreach (Light light in copy.GetComponentsInChildren<Light>(true)) light.color = Shift(light.color, hue.Value);
            }
            // Mercenary's slashes aren't EffectManager effects (his attacks instantiate them directly); REX's shockwave is
            if (copy.GetComponent<EffectComponent>()) Modules.Content.CreateAndAddEffectDef(copy);
            return copy;
        }

        // REX flashes his two rings for a split second; Roy's stay up until just after the knee strike ends (~0.6 s)
        private static GameObject HoldRings(GameObject shockwave)
        {
            foreach (ParticleSystem ps in shockwave.GetComponentsInChildren<ParticleSystem>(true))
            {
                if (!ps.name.StartsWith("Distortion")) continue;
                var main = ps.main;
                if (main.startLifetime.constantMax < 0.6f) main.startLifetime = 0.6f;
                try { if (main.duration < 0.6f) main.duration = 0.6f; }
                catch (System.Exception) { }   // duration can't change on a system that's already playing; the lifetime is what matters
            }
            return shockwave;
        }

        // the crescent lies across its local Z axis with its arc toward local -Y: aim local Z along the tipped plane normal and
        // local -Y straight ahead (within that plane), so the arc sweeps across the front of him
        private static Quaternion TippedSwing(Vector3 swingNormal, float tilt)
        {
            Vector3 side = new Vector3(swingNormal.x, 0f, swingNormal.z);
            if (side.sqrMagnitude < 1e-4f) side = Vector3.right;
            side.Normalize();
            float s = swingNormal.y >= 0f ? 1f : -1f;
            Vector3 normal = (Vector3.up * s * Mathf.Cos(tilt * Mathf.Deg2Rad) + side * Mathf.Sin(tilt * Mathf.Deg2Rad)).normalized;
            Vector3 ahead = Vector3.ProjectOnPlane(Vector3.forward, normal).normalized;
            return Quaternion.LookRotation(normal, -ahead);
        }

        private static Color Shift(Color c, float hue)
        {
            Color.RGBToHSV(c, out _, out float s, out float v);
            Color shifted = Color.HSVToRGB(hue, s, v, true);
            shifted.a = c.a;
            return shifted;
        }

        private static Gradient Shift(Gradient g, float hue)
        {
            var keys = g.colorKeys;
            for (int i = 0; i < keys.Length; i++) keys[i].color = Shift(keys[i].color, hue);
            var shifted = new Gradient { mode = g.mode };
            shifted.SetKeys(keys, g.alphaKeys);
            return shifted;
        }

        private static ParticleSystem.MinMaxGradient Shift(ParticleSystem.MinMaxGradient g, float hue)
        {
            switch (g.mode)
            {
                case ParticleSystemGradientMode.Color: return new ParticleSystem.MinMaxGradient(Shift(g.color, hue));
                case ParticleSystemGradientMode.TwoColors: return new ParticleSystem.MinMaxGradient(Shift(g.colorMin, hue), Shift(g.colorMax, hue));
                case ParticleSystemGradientMode.Gradient: return new ParticleSystem.MinMaxGradient(Shift(g.gradient, hue));
                case ParticleSystemGradientMode.TwoGradients: return new ParticleSystem.MinMaxGradient(Shift(g.gradientMin, hue), Shift(g.gradientMax, hue));
                default: return g;
            }
        }

        // the game's textures aren't readable, so copy through a render texture before shifting the pixels
        private static readonly System.Collections.Generic.Dictionary<(Texture, float), Texture2D> shiftedTextures = new System.Collections.Generic.Dictionary<(Texture, float), Texture2D>();
        private static Texture ShiftTexture(Texture source, float hue)
        {
            if (shiftedTextures.TryGetValue((source, hue), out Texture2D cached)) return cached;
            RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            RenderTexture previous = RenderTexture.active;
            Graphics.Blit(source, rt);
            RenderTexture.active = rt;
            var tex = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false) { wrapMode = source.wrapMode, filterMode = source.filterMode, name = source.name + "_roy" };
            tex.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            Color[] pixels = tex.GetPixels();
            for (int i = 0; i < pixels.Length; i++) pixels[i] = Shift(pixels[i], hue);
            tex.SetPixels(pixels);
            tex.Apply();
            shiftedTextures[(source, hue)] = tex;
            return tex;
        }

        public static void Spawn(CharacterBody body, Vector3 aimForward, Mount mount)
        {
            if (!body) return;
            bool awakening = body.skinIndex > 0;
            GameObject prefab;
            switch (mount.look)
            {
                case Look.Swing: prefab = awakening ? swingBlue : swingOrange; break;
                case Look.Finisher: prefab = awakening ? finisherBlue : finisherOrange; break;
                case Look.Whirlwind: prefab = awakening ? whirlBlue : whirlOrange; break;
                default: prefab = awakening ? shockBlue : shockOrange; break;
            }
            if (!prefab) return;

            aimForward.y = 0f;
            if (aimForward.sqrMagnitude < 0.001f) aimForward = body.transform.forward;
            Quaternion facing = Quaternion.LookRotation(aimForward.normalized, Vector3.up);
            Vector3 origin = body.footPosition + facing * mount.centre;
            Quaternion rotation = facing * (mount.swingNormal != Vector3.zero ? TippedSwing(mount.swingNormal, mount.tilt) : Quaternion.Euler(mount.rotation));
            if (prefab.GetComponent<EffectComponent>())
            {
                // scale must be set: REX's rings are sized relative to the effect, and an unset scale collapses them to nothing
                EffectManager.SpawnEffect(prefab, new EffectData { origin = origin, rotation = rotation, scale = 1f }, false);
            }
            else
            {
                // like Mercenary's own swings: a plain instance (every client runs the attack state, so each spawns its own)
                GameObject slash = Object.Instantiate(prefab, origin, rotation, mount.followRoy ? body.transform : null);
                if (mount.scale > 0f)
                {
                    foreach (ParticleSystem ps in slash.GetComponentsInChildren<ParticleSystem>())
                    {
                        var main = ps.main;
                        main.startSizeMultiplier *= mount.scale;
                    }
                }
                Object.Destroy(slash, 1f);
            }
        }
    }
}
