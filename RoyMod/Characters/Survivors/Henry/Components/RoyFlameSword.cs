using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Flame effect on Roy's sword, driven by one number: intensity (0 = no fire, 1 = fully ablaze).
    // Lives on the RoyFlameSword prefab, parented to the Sword1 bone. Mod copy of the editor script: here RoyController
    // sets intensity from his flame meter (the bundle prefab's own script reference doesn't survive into the game).
    public class RoyFlameSword : MonoBehaviour
    {
        [Range(0f, 1f)] public float intensity;
        // Set by whoever runs Roy's attacks (the skill states in the mod, RoyAnimatorTester here):
        // the swing trail only draws while an attack is playing.
        public bool attacking = true;

        public ParticleSystem flames;
        public ParticleSystem embers;
        public RoySwordTrail swingTrail;   // one trail spanning the whole blade
        public Light glow;

        // Fire rates and sizes at full intensity; they scale down linearly with intensity.
        public float maxFlameRate = 140f;
        public float maxEmberRate = 30f;
        public float maxFlameSize = 0.16f;
        public float maxGlow = 2.5f;
        // The swing trail only appears once the meter is this full.
        // Meter stages (Alex): soft effects (glow, embers) from 25, the flames from 50, the swing trail from 75.
        [Range(0f, 1f)] public float softThreshold = 0.25f;
        [Range(0f, 1f)] public float flameThreshold = 0.5f;
        [Range(0f, 1f)] public float trailThreshold = 0.75f;

        // Fire colour: red-orange normally, blue for the Awakening skin. Set by the skin in the mod.
        public bool blueFlames;
        public Material trailMaterialRed;
        public Material trailMaterialBlue;
        bool paletteApplied;
        bool appliedBlue;

        void ApplyPalette(bool blue)
        {
            paletteApplied = true;
            appliedBlue = blue;
            Color hot = blue ? new Color(0.85f, 0.95f, 1f) : new Color(1f, 0.95f, 0.6f);
            Color mid = blue ? new Color(0.25f, 0.55f, 1f) : new Color(1f, 0.5f, 0.1f);
            Color cool = blue ? new Color(0.15f, 0.12f, 0.8f) : new Color(0.8f, 0.12f, 0.05f);
            if (flames != null)
            {
                var col = flames.colorOverLifetime;
                var g = new Gradient();
                g.SetKeys(new[] { new GradientColorKey(hot, 0f), new GradientColorKey(mid, 0.35f), new GradientColorKey(cool, 1f) },
                          new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.1f), new GradientAlphaKey(0.6f, 0.5f), new GradientAlphaKey(0f, 1f) });
                col.color = g;
            }
            if (embers != null)
            {
                var main = embers.main;
                main.startColor = blue ? new Color(0.6f, 0.8f, 1f) : new Color(1f, 0.7f, 0.3f);
            }
            if (glow != null) glow.color = blue ? new Color(0.3f, 0.55f, 1f) : new Color(1f, 0.5f, 0.15f);
            if (swingTrail != null)
            {
                var m = blue ? trailMaterialBlue : trailMaterialRed;
                if (m != null) swingTrail.SetMaterial(m);
            }
        }

        void LateUpdate()
        {
            if (!paletteApplied || appliedBlue != blueFlames) ApplyPalette(blueFlames);
            float k = Mathf.Clamp01(intensity);
            bool soft = k >= softThreshold, hard = k >= flameThreshold;

            if (flames != null)
            {
                var emission = flames.emission;
                emission.rateOverTime = hard ? maxFlameRate * k : 0f;
                var main = flames.main;
                main.startSize = new ParticleSystem.MinMaxCurve(maxFlameSize * 0.4f * (0.5f + k), maxFlameSize * (0.5f + 0.5f * k));
                if (hard && !flames.isPlaying) flames.Play(true);
            }
            if (embers != null)
            {
                var emission = embers.emission;
                emission.rateOverTime = soft ? maxEmberRate * k : 0f;
                if (soft && !embers.isPlaying) embers.Play(true);
            }
            if (swingTrail != null)
            {
                swingTrail.emitting = attacking && k >= trailThreshold;
                swingTrail.alpha = Mathf.Lerp(0.5f, 1f, k);
            }
            if (glow != null)
            {
                glow.enabled = soft;
                glow.intensity = maxGlow * k;
            }
        }

        // Puts the bundle's RoyFlameSword prefab on Roy's Sword1 bone and wires up this mod copy of its scripts
        // (the prefab's own script references point at the editor project and don't survive into the game).
        public static GameObject prefab;
        public static Material trailMaterialRedAsset, trailMaterialBlueAsset;

        public static RoyFlameSword Attach(Transform modelRoot)
        {
            Material trailRed = trailMaterialRedAsset, trailBlue = trailMaterialBlueAsset;
            if (!modelRoot || !prefab) return null;
            Transform sword = null;
            foreach (Transform t in modelRoot.GetComponentsInChildren<Transform>(true)) if (t.name == "Sword1") { sword = t; break; }
            if (!sword) { Log.Warning("RoyFlameSword: no Sword1 bone"); return null; }

            GameObject instance = Object.Instantiate(prefab, sword, false);
            instance.name = "RoyFlameSword";
            Transform Find(string name) { foreach (Transform t in instance.GetComponentsInChildren<Transform>(true)) if (t.name == name) return t; return null; }

            Transform trailObject = Find("SwingTrail");
            RoySwordTrail trail = (trailObject ? trailObject.gameObject : instance).AddComponent<RoySwordTrail>();
            trail.basePoint = Find("BladeBase");
            trail.tipPoint = Find("BladeTip");
            trail.material = trailRed;

            RoyFlameSword flame = instance.AddComponent<RoyFlameSword>();
            flame.intensity = 0f;
            flame.attacking = false;
            flame.flames = Find("Flames") ? Find("Flames").GetComponent<ParticleSystem>() : null;
            flame.embers = Find("Embers") ? Find("Embers").GetComponent<ParticleSystem>() : null;
            flame.glow = Find("Glow") ? Find("Glow").GetComponent<Light>() : null;
            flame.swingTrail = trail;
            flame.trailMaterialRed = trailRed;
            flame.trailMaterialBlue = trailBlue;
            // values tuned on the editor prefab
            flame.maxFlameRate = 260f;
            flame.maxEmberRate = 30f;
            flame.maxFlameSize = 0.11f;
            flame.maxGlow = 2.5f;
            flame.trailThreshold = 0.75f;
            return flame;
        }
    }
}
