using EntityStates;
using HenryMod.Modules.BaseStates;
using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Shared base for Roy's sword and knee attacks: plays a Smash clip on one of the gesture layers (clearing the other),
    // scales damage with the flame meter (and ignites when Enflamed), feeds the meter on hit, can push Roy forward
    // or lift him during part of the swing, and buffers a press of the same button to chain into the next attack.
    public abstract class RoyMelee : BaseMeleeAttack
    {
        protected const string FullBody = "Gesture, Full Body";
        protected const string Override = "Gesture, Override";

        protected string layerName = FullBody;
        protected string animationState;
        protected bool flameScaling = true;
        protected float meterGainOnHit;
        protected bool heavyHit;            // heavy blows land with Smash's large hit sound, the rest with the medium one

        // forward travel (world units) spread over a fraction of the swing; replaces the root motion stripped from the Smash clips
        protected float forwardDistance;
        protected float forwardStart = 0.1f, forwardEnd = 0.4f;
        // upward launch at the start of the attack (fraction of Roy's jump height, 0 = none)
        protected float liftFraction;
        protected float hoverDuration;      // hang in the air this long at the start

        protected float bufferOpensAt = 0.25f;  // fraction of the attack after which a press is buffered for the next one
        protected RoyController roy;
        protected bool queuedPress;
        protected bool gainedMeter;         // the meter gain is once per hit window; multi-hit moves clear this per window
        protected bool alwaysIgnite;        // burns on every hit, Enflamed or not

        public override void OnEnter()
        {
            roy = GetComponent<RoyController>();
            hitboxGroupName = "SwordGroup";
            playbackRateParam = "Slash.playbackRate";
            hitStopDuration = 0.06f;
            hitHopVelocity = 0f;
            SetupAttack();
            hitSoundString = heavyHit ? "Play_se_roy_hit_l" : "Play_se_roy_hit_m";
            base.OnEnter();
            if (roy) roy.swordAttacks++;

            if (liftFraction > 0f && characterMotor && isAuthority)
            {
                characterMotor.Motor.ForceUnground();
                characterMotor.velocity.y = characterBody.jumpPower * Mathf.Sqrt(liftFraction);
            }
        }

        // set the attack's fields (state, damage, timings) before the base sets up the hit
        protected abstract void SetupAttack();

        protected override void PlayAttackAnimation()
        {
            PlayAnimation(layerName == FullBody ? Override : FullBody, "BufferEmpty");
            PlayCrossfade(layerName, animationState, playbackRateParam, duration, 0.05f);
        }

        protected override void PlaySwingEffect()
        {
            RoySound.Play("se_roy_swing_ll", gameObject);   // every blade swing, hit or miss
            if (swingEffectPrefab) base.PlaySwingEffect();
        }

        protected override void ModifyOverlapAttack(OverlapAttack overlapAttack)
        {
            if (alwaysIgnite) overlapAttack.damageType.damageType |= DamageType.IgniteOnHit;
            if (!flameScaling || !roy) return;
            overlapAttack.damage *= roy.DamageMultiplier;
            if (roy.Enflamed) overlapAttack.damageType.damageType |= DamageType.IgniteOnHit;
        }

        protected override void OnHitEnemyAuthority()
        {
            base.OnHitEnemyAuthority();
            if (roy && meterGainOnHit > 0f && !gainedMeter)
            {
                gainedMeter = true;
                roy.AddMeter(meterGainOnHit);
            }
        }

        // which button chains this attack into the next one
        protected virtual bool ChainButtonPressed() => inputBank && inputBank.skill1.justPressed;

        public override void FixedUpdate()
        {
            // buffer a press for the next attack, but only once this swing is under way: the press that started this attack is
            // still "just pressed" on its first frame, and very quick double-taps shouldn't skip a step
            if (isAuthority && stopwatch >= duration * bufferOpensAt && ChainButtonPressed()) queuedPress = true;

            if (isAuthority && characterMotor && !inHitPause)
            {
                float t = stopwatch / duration;
                if (forwardDistance > 0f && t >= forwardStart && t < forwardEnd)
                {
                    float speed = forwardDistance / Mathf.Max(0.01f, (forwardEnd - forwardStart) * duration);
                    characterMotor.rootMotion += GetAimForward() * speed * Time.fixedDeltaTime;
                }
                if (hoverDuration > 0f && fixedAge < hoverDuration)
                {
                    characterMotor.velocity.y = Mathf.Max(characterMotor.velocity.y, 0f);
                }
            }

            base.FixedUpdate();
        }

        public override void OnExit()
        {
            if (roy) roy.swordAttacks = Mathf.Max(0, roy.swordAttacks - 1);
            base.OnExit();
        }

        protected Vector3 GetAimForward()
        {
            Vector3 forward = inputBank ? inputBank.aimDirection : transform.forward;
            forward.y = 0f;
            return forward.sqrMagnitude > 0.001f ? forward.normalized : transform.forward;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
