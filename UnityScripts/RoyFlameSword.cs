using UnityEngine;

// Flame effect on Roy's sword, driven by one number: intensity (0 = no fire, 1 = fully ablaze).
// Lives on the RoyFlameSword prefab, parented to the Sword1 bone. In the mod, Roy's skill code sets
// intensity from his flame-meter buff stacks; in the editor, RoyAnimatorTester sets it from its meter.
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
    [Range(0f, 1f)] public float trailThreshold = 0.25f;

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

        if (flames != null)
        {
            var emission = flames.emission;
            emission.rateOverTime = maxFlameRate * k;
            var main = flames.main;
            main.startSize = new ParticleSystem.MinMaxCurve(maxFlameSize * 0.4f * (0.5f + k), maxFlameSize * (0.5f + 0.5f * k));
        }
        if (embers != null)
        {
            var emission = embers.emission;
            emission.rateOverTime = maxEmberRate * k;
        }
        if (swingTrail != null)
        {
            swingTrail.emitting = attacking && k >= trailThreshold;
            swingTrail.alpha = Mathf.Lerp(0.5f, 1f, k);
        }
        if (glow != null)
        {
            glow.enabled = k > 0.01f;
            glow.intensity = maxGlow * k;
        }
    }
}
