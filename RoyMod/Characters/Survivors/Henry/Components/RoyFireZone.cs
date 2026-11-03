using RoR2;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Roy's lingering fire on an enemy that attacked him: the fire pillar (perfect dodge, normal counter) and the fire
    // tornado (perfect counter). The look comes from the bundle prefabs RoyFirePillar / RoyFireTornado (their editor
    // RoyFireZone script doesn't exist in the game, so this mirrors its lifetime and palette); the damage is repeated
    // burning blasts where it stands. Both get bigger, last longer and hit harder with a fuller flame meter.
    public class RoyFireZone : MonoBehaviour
    {
        public enum Kind { Pillar, Tornado }

        public static GameObject pillarPrefab, tornadoPrefab;

        // pillar: 6 burning hits; tornado: Kjaro's Band style, 12 burning hits over 3 s (12x damage in all, about what
        // Kjaro's tornado deals off a 400% hit)
        private const int pillarHits = 6, tornadoHits = 14;
        private const float pillarHitCoefficient = 0.375f, tornadoHitCoefficient = 1f;   // pillar 25% lower (Alex)
        private const float pillarInterval = 0.3f, tornadoInterval = 0.25f;

        private Kind kind;
        private CharacterBody owner;
        private float level;
        private float damageMultiplier;
        private bool blue;

        private float duration, fadeIn = 0.25f, fadeOut = 0.6f, maxGlow = 4f;
        private float age, nextHit;
        private int hitsLeft;
        private bool stopped;
        private ParticleSystem[] systems;
        private Light glow;

        // where it goes: a grounded enemy gets it at its feet; one in the air gets it centred on it, so it reaches flyers
        public static void Spawn(Kind kind, CharacterBody owner, GameObject target, Vector3 fallbackPosition)
        {
            GameObject prefab = kind == Kind.Pillar ? pillarPrefab : tornadoPrefab;
            if (!prefab || !owner) return;

            CharacterBody targetBody = target ? target.GetComponent<CharacterBody>() : null;
            Vector3 position = fallbackPosition;
            if (targetBody)
            {
                bool grounded = targetBody.characterMotor && targetBody.characterMotor.isGrounded;
                position = grounded ? targetBody.footPosition : targetBody.corePosition + Vector3.down * 2f;
            }

            RoyController roy = owner.GetComponent<RoyController>();
            GameObject instance = Instantiate(prefab, position, Quaternion.identity);
            RoyFireZone zone = instance.AddComponent<RoyFireZone>();
            zone.kind = kind;
            zone.owner = owner;
            zone.level = roy ? roy.MeterFraction : 0f;
            zone.damageMultiplier = roy ? roy.DamageMultiplier : 1f;
            zone.blue = owner.skinIndex == 1;   // Awakening
        }

        private void Start()
        {
            float baseDuration = kind == Kind.Pillar ? 2f : 3f, extraDuration = kind == Kind.Pillar ? 2f : 1.5f;
            float baseScale = 1f, extraScale = kind == Kind.Pillar ? 0.8f : 0.5f;
            duration = baseDuration + extraDuration * level;
            transform.localScale = Vector3.one * (baseScale + extraScale * level);

            systems = GetComponentsInChildren<ParticleSystem>(true);
            glow = GetComponentInChildren<Light>(true);
            ApplyPalette();
            foreach (var ps in systems) ps.Play(true);

            hitsLeft = kind == Kind.Pillar ? pillarHits : tornadoHits;
            // the game's own fire sounds: Gasoline's burst for the pillar, Kjaro's Band's tornado for the tornado
            Util.PlaySound(kind == Kind.Pillar ? "Play_item_proc_igniteOnKill" : "Play_item_proc_fireRingTornado_start", gameObject);
            nextHit = 0.15f;
        }

        private void ApplyPalette()
        {
            Color hot = blue ? new Color(0.85f, 0.95f, 1f) : new Color(1f, 0.95f, 0.6f);
            Color mid = blue ? new Color(0.25f, 0.55f, 1f) : new Color(1f, 0.5f, 0.1f);
            Color cool = blue ? new Color(0.15f, 0.12f, 0.8f) : new Color(0.8f, 0.12f, 0.05f);
            var g = new Gradient();
            g.SetKeys(new[] { new GradientColorKey(hot, 0f), new GradientColorKey(mid, 0.35f), new GradientColorKey(cool, 1f) },
                      new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.1f), new GradientAlphaKey(0.6f, 0.55f), new GradientAlphaKey(0f, 1f) });
            foreach (var ps in systems)
            {
                if (ps.name.Contains("Ember"))
                {
                    var main = ps.main;
                    main.startColor = blue ? new Color(0.6f, 0.8f, 1f) : new Color(1f, 0.7f, 0.3f);
                }
                else
                {
                    var col = ps.colorOverLifetime;
                    col.enabled = true;
                    col.color = g;
                }
            }
            if (glow) glow.color = blue ? new Color(0.3f, 0.55f, 1f) : new Color(1f, 0.5f, 0.15f);
        }

        private void FixedUpdate()
        {
            if (hitsLeft <= 0 || !owner || age < nextHit) return;
            hitsLeft--;
            nextHit += kind == Kind.Pillar ? pillarInterval : tornadoInterval;

            float coefficient = (kind == Kind.Pillar ? pillarHitCoefficient : tornadoHitCoefficient) * damageMultiplier;
            float radius = (kind == Kind.Pillar ? 4f : 6f) * transform.localScale.x;
            new BlastAttack
            {
                attacker = owner.gameObject,
                inflictor = gameObject,
                teamIndex = owner.teamComponent.teamIndex,
                baseDamage = coefficient * owner.damage,
                baseForce = 0f,
                position = transform.position + Vector3.up * 2f,
                radius = radius,
                procCoefficient = 0.5f,
                crit = owner.RollCrit(),
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.IgniteOnHit,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
            }.Fire();
        }

        private void Update()
        {
            age += Time.deltaTime;
            if (glow)
            {
                float k = Mathf.Clamp01(age / fadeIn) * Mathf.Clamp01((duration + fadeOut - age) / fadeOut);
                glow.intensity = maxGlow * k * (0.85f + 0.15f * Mathf.PerlinNoise(age * 8f, 0f));
            }
            if (!stopped && age >= duration)
            {
                stopped = true;
                foreach (var ps in systems) if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (kind == Kind.Tornado) Util.PlaySound("Play_item_proc_fireRingTornado_end", gameObject);
            }
            if (age >= duration + fadeOut + 1.5f) Destroy(gameObject);
        }
    }
}
