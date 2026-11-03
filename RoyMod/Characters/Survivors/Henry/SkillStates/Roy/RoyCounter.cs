using EntityStates;
using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Roy's special: a counter stance. A hit during the window is blocked and answered with the counterattack;
    // a hit right as he presses (perfect counter) launches his Final Smash at the attacker instead.
    // Whiffing leaves him open (the stance plays out).
    public class RoyCounter : BaseState, IRoyDefensive
    {
        private const float windowStart = 0.1f, windowEnd = 0.6f;
        // a hit this soon after pressing is a perfect counter (Alex: 0.2 s more than the dodge's 0.1 s, to be more forgiving)
        private const float perfectCounterWindow = 0.3f;
        private float duration = 1.17f;
        private bool airborne;
        private bool countered, perfect;
        private GameObject attacker;
        private float blockedDamage;
        private RoyController roy;

        public override void OnEnter()
        {
            base.OnEnter();
            roy = GetComponent<RoyController>();
            airborne = characterMotor && !characterMotor.isGrounded;
            PlayAnimation("Gesture, Override", "BufferEmpty");
            PlayAnimation("Gesture, Full Body", airborne ? "CounterAir" : "Counter");
            RoySound.Play("se_roy_swordin", gameObject);   // rings out over the perfect counter window
            if (isAuthority && roy && Time.fixedTime - roy.lastHitTime < RoyController.perfectWindow)
            {
                perfect = countered = true;
                attacker = roy.lastAttacker;
            }
        }

        public bool Intercept(DamageInfo damageInfo)
        {
            if (countered) return true;
            bool perfectTiming = fixedAge < perfectCounterWindow;
            if (!perfectTiming && (fixedAge < windowStart || fixedAge > windowEnd)) return false;
            countered = true;
            perfect = perfectTiming;
            attacker = damageInfo.attacker;
            blockedDamage = damageInfo.damage;
            return true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor)
            {
                characterMotor.velocity.x = 0f; characterMotor.velocity.z = 0f;
                if (airborne) characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, -2f);
            }
            if (!isAuthority) return;

            if (countered)
            {
                if (roy) roy.AddMeter(perfect ? 75f : 50f);   // a normal counter is half the meter, a perfect one three quarters
                if (perfect) outer.SetNextState(new RoyFinalSmash { target = attacker });
                else outer.SetNextState(new RoyCounterHit { blockedDamage = blockedDamage, attacker = attacker });
                return;
            }
            if (fixedAge >= duration) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }

    // The counterattack: a wide slash in front that hits harder the bigger the blocked hit was.
    public class RoyCounterHit : BaseState
    {
        public float blockedDamage;
        public GameObject attacker;
        private const float duration = 1.08f, hitTime = 0.2f;
        private bool fired;

        public override void OnEnter()
        {
            base.OnEnter();
            bool airborne = characterMotor && !characterMotor.isGrounded;
            PlayAnimation("Gesture, Full Body", airborne ? "CounterHitAir" : "CounterHit");
            RoySound.Play("se_roy_special_N03", gameObject);
            RoySound.Play("se_roy_swing_ll", gameObject);
            // immune for the whole counterattack: the player has no control during it (the Final Smash does the same)
            if (NetworkServer.active) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor) { characterMotor.velocity.x = 0f; characterMotor.velocity.z = 0f; }
            if (!fired && fixedAge >= hitTime)
            {
                fired = true;
                if (isAuthority)
                {
                    RoyController roy = GetComponent<RoyController>();
                    float coefficient = Mathf.Max(5f, 1.5f * blockedDamage / Mathf.Max(1f, damageStat));
                    int hits = RoyBlast.Fire(characterBody, characterBody.corePosition + characterDirection.forward * 2f, 5f, coefficient * (roy ? roy.DamageMultiplier : 1f), roy && roy.Enflamed);
                    if (hits > 0) RoySound.Play("se_roy_hit_l", gameObject);
                    if (attacker) RoyFireZone.Spawn(RoyFireZone.Kind.Pillar, characterBody, attacker, attacker.transform.position);
                }
            }
            if (isAuthority && fixedAge >= duration) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }

    // Perfect counter: Final Smash. Roy rushes the attacker, then a heavy finishing blow.
    public class RoyFinalSmash : BaseState
    {
        public GameObject target;
        private const float startDuration = 1.23f, finalDuration = 1.58f, hitTime = 0.45f;
        private const float rushTime = 0.4f, maxRushDistance = 60f, minRushSpeed = 30f;
        private bool started, fired, rushing;
        private bool airborne;
        private float rushSpeed;
        private CharacterBody targetBody;

        public override void OnEnter()
        {
            base.OnEnter();
            airborne = characterMotor && !characterMotor.isGrounded;
            PlayAnimation("Gesture, Full Body", airborne ? "FinalAirStart" : "FinalStart");
            RoySound.Play("effect_aura", gameObject);   // the wind-up
            if (NetworkServer.active) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, startDuration + finalDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            // movement keys do nothing here (this state replaces his normal movement); the rush below does all the moving
            if (characterMotor) { characterMotor.velocity = Vector3.zero; characterMotor.moveDirection = Vector3.zero; }

            if (!started && fixedAge >= startDuration)
            {
                started = true;
                targetBody = target ? target.GetComponent<CharacterBody>() : null;
                RoySound.Play("se_roy_special_S04S", gameObject);   // the rush to the attacker
                Vector3 to = RushOffset();
                // a target well above him (a flyer, or one up a slope) gets the aerial version
                if (to.y > 3f) airborne = true;
                PlayAnimation("Gesture, Full Body", airborne ? "FinalAir" : "Final");
                RoySound.Play("se_roy_swing_ll", gameObject);
                if (targetBody)
                {
                    rushing = true;
                    rushSpeed = Mathf.Max(minRushSpeed, Mathf.Min(to.magnitude, maxRushDistance) / rushTime);
                }
                else if (isAuthority && characterMotor)
                {
                    characterMotor.rootMotion += characterDirection.forward * 5f;   // no attacker to chase: a short lunge
                }
            }
            // the rush: straight at the attacker, in 3D, re-aimed every frame so a moving or flying enemy is followed.
            // He's lifted off the ground for it so slopes don't stop him (he slides up along them); walls still do.
            if (rushing && isAuthority && characterMotor)
            {
                Vector3 to = targetBody && targetBody.healthComponent && targetBody.healthComponent.alive ? RushOffset() : Vector3.zero;
                float stopAt = (targetBody ? targetBody.radius : 0f) + 1.5f;
                float remaining = to.magnitude - stopAt;
                if (remaining <= 0.05f || fixedAge > startDuration + hitTime + 0.3f) rushing = false;
                else
                {
                    if (characterMotor.Motor) characterMotor.Motor.ForceUnground();
                    characterMotor.rootMotion += to.normalized * Mathf.Min(remaining, rushSpeed * Time.fixedDeltaTime);
                    Vector3 flat = to; flat.y = 0f;
                    if (flat.sqrMagnitude > 0.01f) characterDirection.forward = flat.normalized;
                }
            }
            if (started && !fired && fixedAge >= startDuration + hitTime)
            {
                fired = true;
                if (isAuthority)
                {
                    RoyController roy = GetComponent<RoyController>();
                    Vector3 blastAt = characterBody.corePosition + characterDirection.forward * 2f;
                    RoyBlast.Fire(characterBody, blastAt, 8f, 14f * (roy ? roy.DamageMultiplier : 1f), true);
                    RoySound.Play("se_roy_criticalhit", target ? target : gameObject);
                    // then the fire tornado on the attacker (or where the blow landed, if it's gone)
                    RoyFireZone.Spawn(RoyFireZone.Kind.Tornado, characterBody, target, blastAt);
                }
            }
            if (isAuthority && fixedAge >= startDuration + finalDuration)
            {
                RoySound.Play("se_roy_final07", gameObject);   // once the Final Smash has played out
                outer.SetNextStateToMain();
            }
        }

        // from Roy to where he should end up: feet to feet for a grounded enemy, so he stays on the ground (following its
        // slope), centre to centre for one in the air
        private Vector3 RushOffset()
        {
            if (!targetBody) return Vector3.zero;
            bool targetGrounded = targetBody.characterMotor && targetBody.characterMotor.isGrounded;
            return targetGrounded ? targetBody.footPosition - characterBody.footPosition : targetBody.corePosition - characterBody.corePosition;
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }

    // Perfect dodge on the ground: Flare Blade, a burst of fire around him.
    public class RoyFlareBlade : BaseState
    {
        private const float startDuration = 0.18f, duration = 1.3f, hitTime = 0.35f;
        private bool fired, swung;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Full Body", "FlareBladeStart");
            if (NetworkServer.active) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor) { characterMotor.velocity.x = 0f; characterMotor.velocity.z = 0f; }
            if (!swung && fixedAge >= startDuration) { swung = true; PlayAnimation("Gesture, Full Body", "FlareBlade"); RoySound.Play("se_roy_swing_ll", gameObject); RoySound.Play("se_roy_smash_H02", gameObject); }
            if (!fired && fixedAge >= hitTime)
            {
                fired = true;
                if (isAuthority)
                {
                    RoyController roy = GetComponent<RoyController>();
                    if (RoyBlast.Fire(characterBody, characterBody.corePosition + characterDirection.forward * 1.5f, 6f, 6f * (roy ? roy.DamageMultiplier : 1f), true) > 0)
                        RoySound.Play("se_roy_hit_l", gameObject);
                }
            }
            if (isAuthority && fixedAge >= duration) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }

    // Perfect dodge in the air: a hovering Down Air that spikes everything below.
    public class RoyDownAir : BaseState
    {
        private const float duration = 1.15f, hover = 0.35f, hitTime = 0.4f;
        private bool fired;

        public override void OnEnter()
        {
            base.OnEnter();
            PlayAnimation("Gesture, Full Body", "DownAir");
            RoySound.Play("se_roy_swing_ll", gameObject);
            RoySound.Play("se_roy_smash_H02", gameObject);
            if (NetworkServer.active) characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, hover + 0.3f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (characterMotor && fixedAge < hover) characterMotor.velocity = Vector3.zero;
            if (!fired && fixedAge >= hitTime)
            {
                fired = true;
                if (isAuthority)
                {
                    RoyController roy = GetComponent<RoyController>();
                    if (RoyBlast.Fire(characterBody, characterBody.footPosition + Vector3.down * 1f, 4.5f, 5f * (roy ? roy.DamageMultiplier : 1f), true, Vector3.down * 2000f) > 0)
                        RoySound.Play("se_roy_hit_l", gameObject);
                }
            }
            if (isAuthority && fixedAge >= duration) outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }

    internal static class RoyBlast
    {
        // returns how many targets it hit
        public static int Fire(CharacterBody body, Vector3 position, float radius, float damageCoefficient, bool ignite, Vector3 force = default)
        {

            var blast = new BlastAttack
            {
                attacker = body.gameObject,
                inflictor = body.gameObject,
                teamIndex = body.teamComponent.teamIndex,
                baseDamage = damageCoefficient * body.damage,
                baseForce = 800f,
                bonusForce = force,
                position = position,
                radius = radius,
                procCoefficient = 1f,
                crit = body.RollCrit(),
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = ignite ? DamageType.IgniteOnHit : DamageType.Generic,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
            };
            return blast.Fire().hitCount;
        }
    }
}
