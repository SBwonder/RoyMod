using System.Collections.Generic;
using UnityEngine;

// One swing trail that spans the whole blade, from basePoint to tipPoint, like Smash's sword arcs.
// Each frame it records where the blade's base and tip are, keeps the last `lifetime` seconds,
// and builds a ribbon mesh through them (smoothed with Catmull-Rom so fast swings stay round).
// UVs: u = age (0 = the blade right now, 1 = the oldest end), v = 0 at the base, 1 at the tip.
public class RoySwordTrail : MonoBehaviour
{
    public Transform basePoint;
    public Transform tipPoint;
    public Material material;
    public float lifetime = 0.16f;
    [Range(1, 8)] public int subdivisions = 4;
    public bool emitting = true;
    [Range(0f, 1f)] public float alpha = 1f;
    // Only real swings draw: the blade tip has to move at least this fast (units/second),
    // so idle sway and walking don't leave a smear.
    // (Measured: idle sway peaks ~12, running ~20, attack swings ~90-150.)
    public float minTipSpeed = 15f;

    Vector3 lastTip;
    float lastStepTime = -1f;

    struct Sample { public Vector3 b, t; public float time; }
    readonly List<Sample> samples = new List<Sample>();
    Mesh mesh;
    Transform meshTransform;

    void Awake() { EnsureMesh(); }

    void EnsureMesh()
    {
        if (meshTransform != null) return;
        // clear out any leftover mesh objects (e.g. from editor previews before a script reload)
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            var child = transform.GetChild(i);
            if (child.name == "SwingTrailMesh") DestroyImmediate(child.gameObject);
        }
        var go = new GameObject("SwingTrailMesh");
        go.hideFlags = HideFlags.DontSave;   // runtime-only, never saved into the scene or prefab
        go.transform.SetParent(transform, false);
        meshTransform = go.transform;
        mesh = new Mesh { name = "RoySwingTrail" };
        mesh.MarkDynamic();
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        var renderer = go.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;
    }

    void OnDisable() { samples.Clear(); if (mesh != null) mesh.Clear(); }

    // Swap the trail's look (e.g. red fire vs. the Awakening skin's blue fire).
    public void SetMaterial(Material m)
    {
        material = m;
        if (meshTransform != null) meshTransform.GetComponent<MeshRenderer>().sharedMaterial = m;
    }

    void LateUpdate() { Step(Time.time); }

    // Record the blade at time `now` and rebuild the ribbon. Public so editor previews can drive it.
    public void Step(float now)
    {
        if (basePoint == null || tipPoint == null) return;
        EnsureMesh();
        // drop samples that are too old (or from a clock that has since been reset)
        samples.RemoveAll(s => now - s.time > lifetime || s.time > now);
        Vector3 tip = tipPoint.position;
        float dt = now - lastStepTime;
        float tipSpeed = (lastStepTime >= 0f && dt > 0f && dt < 0.5f) ? (tip - lastTip).magnitude / dt : 0f;
        lastTip = tip;
        lastStepTime = now;
        if (emitting && tipSpeed >= minTipSpeed) samples.Add(new Sample { b = basePoint.position, t = tip, time = now });
        Rebuild(now);
    }

    static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float s)
    {
        float s2 = s * s, s3 = s2 * s;
        return 0.5f * ((2f * p1) + (-p0 + p2) * s + (2f * p0 - 5f * p1 + 4f * p2 - p3) * s2 + (-p0 + 3f * p1 - 3f * p2 + p3) * s3);
    }

    void Rebuild(float now)
    {
        mesh.Clear();
        if (samples.Count < 2 || alpha <= 0f) return;

        var verts = new List<Vector3>();
        var uvs = new List<Vector2>();
        var colors = new List<Color>();
        // newest first so u runs from the blade outward
        for (int i = samples.Count - 1; i > 0; i--)
        {
            var p0 = samples[Mathf.Min(i + 1, samples.Count - 1)];
            var p1 = samples[i];
            var p2 = samples[i - 1];
            var p3 = samples[Mathf.Max(i - 2, 0)];
            for (int k = 0; k < subdivisions; k++)
            {
                float s = k / (float)subdivisions;
                Vector3 b = CatmullRom(p0.b, p1.b, p2.b, p3.b, s);
                Vector3 t = CatmullRom(p0.t, p1.t, p2.t, p3.t, s);
                float age = Mathf.Lerp(now - p1.time, now - p2.time, s) / lifetime;
                AddPair(verts, uvs, colors, b, t, age);
            }
        }
        var last = samples[0];
        AddPair(verts, uvs, colors, last.b, last.t, (now - last.time) / lifetime);

        var tris = new List<int>();
        for (int i = 0; i + 3 < verts.Count; i += 2)
        {
            tris.Add(i); tris.Add(i + 1); tris.Add(i + 2);
            tris.Add(i + 2); tris.Add(i + 1); tris.Add(i + 3);
        }
        mesh.SetVertices(verts);
        mesh.SetUVs(0, uvs);
        mesh.SetColors(colors);
        mesh.SetTriangles(tris, 0);
        mesh.RecalculateBounds();
    }

    void AddPair(List<Vector3> verts, List<Vector2> uvs, List<Color> colors, Vector3 b, Vector3 t, float age)
    {
        age = Mathf.Clamp01(age);
        verts.Add(meshTransform.InverseTransformPoint(b));
        verts.Add(meshTransform.InverseTransformPoint(t));
        uvs.Add(new Vector2(age, 0f));
        uvs.Add(new Vector2(age, 1f));
        var c = new Color(1f, 1f, 1f, alpha);
        colors.Add(c);
        colors.Add(c);
    }
}
