using UnityEngine;

// Shared lifetime for Roy's lingering fire effects (the perfect-dodge pillar and the perfect-counter
// tornado): grow in, burn for `duration`, die down, then remove itself. `level` (0-1, Roy's flame meter
// when it was spawned) makes it bigger and last longer. In the mod, the damage zone that goes with it
// is set up by the skill code; this is only the look.
public class RoyFireZone : MonoBehaviour
{
    [Range(0f, 1f)] public float level;
    public bool blue;

    public float baseDuration = 2f;
    public float extraDuration = 2f;      // added at a full meter
    public float baseScale = 1f;
    public float extraScale = 0.8f;       // added at a full meter
    public float fadeIn = 0.25f;
    public float fadeOut = 0.6f;

    public ParticleSystem[] systems;      // fire systems recoloured by the palette
    public ParticleSystem[] embers;       // spark systems (start colour only)
    public Light glow;
    public float maxGlow = 4f;

    public float Duration { get { return baseDuration + extraDuration * Mathf.Clamp01(level); } }

    float age;
    bool stopped;

    void Start()
    {
        float s = baseScale + extraScale * Mathf.Clamp01(level);
        transform.localScale = Vector3.one * s;
        ApplyPalette();
        foreach (var ps in All()) ps.Play(true);
    }

    System.Collections.Generic.IEnumerable<ParticleSystem> All()
    {
        if (systems != null) foreach (var ps in systems) if (ps != null) yield return ps;
        if (embers != null) foreach (var ps in embers) if (ps != null) yield return ps;
    }

    public void ApplyPalette()
    {
        Color hot = blue ? new Color(0.85f, 0.95f, 1f) : new Color(1f, 0.95f, 0.6f);
        Color mid = blue ? new Color(0.25f, 0.55f, 1f) : new Color(1f, 0.5f, 0.1f);
        Color cool = blue ? new Color(0.15f, 0.12f, 0.8f) : new Color(0.8f, 0.12f, 0.05f);
        var g = new Gradient();
        g.SetKeys(new[] { new GradientColorKey(hot, 0f), new GradientColorKey(mid, 0.35f), new GradientColorKey(cool, 1f) },
                  new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.1f), new GradientAlphaKey(0.6f, 0.55f), new GradientAlphaKey(0f, 1f) });
        if (systems != null)
            foreach (var ps in systems)
            {
                if (ps == null) continue;
                var col = ps.colorOverLifetime;
                col.enabled = true;
                col.color = g;
            }
        if (embers != null)
            foreach (var ps in embers)
            {
                if (ps == null) continue;
                var main = ps.main;
                main.startColor = blue ? new Color(0.6f, 0.8f, 1f) : new Color(1f, 0.7f, 0.3f);
            }
        if (glow != null) glow.color = blue ? new Color(0.3f, 0.55f, 1f) : new Color(1f, 0.5f, 0.15f);
    }

    void Update()
    {
        age += Time.deltaTime;
        float d = Duration;
        if (glow != null)
        {
            float k = Mathf.Clamp01(age / fadeIn) * Mathf.Clamp01((d + fadeOut - age) / fadeOut);
            glow.intensity = maxGlow * k * (0.85f + 0.15f * Mathf.PerlinNoise(age * 8f, 0f));
        }
        if (!stopped && age >= d)
        {
            stopped = true;
            foreach (var ps in All()) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        if (age >= d + fadeOut + 1.5f) Destroy(gameObject);
    }
}
