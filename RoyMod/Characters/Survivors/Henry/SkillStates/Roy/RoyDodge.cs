using EntityStates;
using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // States on the Body machine that react to an incoming hit (dodge i-frames, the counter window).
    // Called where damage is processed (server/host); returning true rejects the damage.
    public interface IRoyDefensive
    {
        bool Intercept(DamageInfo damageInfo);
    }

    // Roy's utility: one press, the move depends on how he's moving.
    //   standing still -> Spot Dodge          backing away -> Back Roll          any other direction -> Roll that way
    //   airborne       -> Air Dodge with a 5/8 hop (once per airtime); primary/secondary during it -> air follow-ups
    // A second press during a roll (or just after) chains one more roll, after which he exits into a sprint.
    // Getting hit within the first moment of a dodge (or just before pressing) is a perfect dodge:
    // on the ground Flare Blade, in the air a hovering Down Air.
    public class RoyDodge : BaseState, IRoyDefensive
    {
        public enum Kind { SpotDodge, Roll, BackRoll, AirDodge }

        public Kind kind;
        public bool secondRoll;
        public Vector3 rollDirection;

        private const float rollDistance = 3.6f;
        private const float airDodgeHopFraction = 0.625f * 1.5f;   // utility lifts are 1.5x (Alex)
        private const float rollSpeedScale = 1.75f;                 // rolls travel 1.75x as far (Alex)
        private const float extraPerfectWindow = 0.15f;             // seconds added to every perfect dodge window (Alex)
        private const float invulnerableStart = 0.05f, invulnerableEnd = 0.8f;

        private float duration;
        private bool perfectDodge;
        private GameObject attacker;
        private bool queuedRoll;
        private RoyController roy;

        public override void OnEnter()
        {
            base.OnEnter();
            roy = GetComponent<RoyController>();

            if (isAuthority)
            {
                Vector3 move = inputBank ? inputBank.moveVector : Vector3.zero;
                move.y = 0f;
                if (characterMotor && !characterMotor.isGrounded) kind = Kind.AirDodge;
                else if (move.sqrMagnitude < 0.04f) kind = secondRoll ? Kind.Roll : Kind.SpotDodge;
                else
                {
                    Vector3 aim = inputBank.aimDirection; aim.y = 0f; aim.Normalize();
                    kind = Vector3.Dot(move.normalized, aim) < -0.5f ? Kind.BackRoll : Kind.Roll;
                }
                rollDirection = move.sqrMagnitude > 0.04f ? move.normalized : FlatForward();

                // pressed just after being hit: still counts as perfect
                if (roy && Time.fixedTime - roy.lastHitTime < RoyController.perfectWindow) { perfectDodge = true; attacker = roy.lastAttacker; }
            }

            switch (kind)
            {
                case Kind.SpotDodge: duration = 0.65f; PlayAnimation("Gesture, Full Body", "SpotDodge"); break;
                case Kind.BackRoll: duration = 0.67f; PlayAnimation("Gesture, Full Body", "RollBack"); break;
                case Kind.AirDodge: duration = 0.82f; PlayAnimation("Gesture, Full Body", "AirDodge"); break;
                default:
                    duration = 0.65f;
                    PlayAnimation("Gesture, Full Body", "Roll");
                    if (characterDirection) characterDirection.forward = rollDirection;
                    break;
            }
            PlayAnimation("Gesture, Override", "BufferEmpty");
            RoySound.Play(kind == Kind.AirDodge ? "se_roy_escapeair" : "se_roy_escape", gameObject);

            if (kind == Kind.AirDodge && isAuthority && characterMotor)
            {
                if (roy) roy.airDodgedThisAirtime = true;
                characterMotor.velocity.y = characterBody.jumpPower * Mathf.Sqrt(airDodgeHopFraction);
            }
        }

        private Vector3 FlatForward()
        {
            Vector3 f = characterDirection ? characterDirection.forward : transform.forward;
            f.y = 0f;
            return f.normalized;
        }

        // perfect dodge window, as a fraction of the dodge: the first quarter of a moving dodge (rolls, air dodge); for the spot
        // dodge, everything up to the point where he's bent back furthest (~59% of the animation, traced from his head)
        private float PerfectWindowFraction => kind == Kind.SpotDodge ? 0.59f : 0.25f;

        public bool Intercept(DamageInfo damageInfo)
        {
            float t = fixedAge / duration;
            bool perfectTiming = fixedAge < PerfectWindowFraction * duration + extraPerfectWindow;
            if (perfectTiming) { perfectDodge = true; attacker = damageInfo.attacker; }
            return t >= invulnerableStart && t <= invulnerableEnd || perfectTiming;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isAuthority) return;

            if (perfectDodge)
            {
                if (roy) roy.AddMeter(50f);   // two perfect dodges fill the meter
                RoySound.Play("se_roy_special_N03", gameObject);
                // a fire pillar on whoever attacked: reaches enemies in the air or at range
                if (attacker) RoyFireZone.Spawn(RoyFireZone.Kind.Pillar, characterBody, attacker, attacker.transform.position);
                if (characterMotor && characterMotor.isGrounded) outer.SetNextState(new RoyFlareBlade());
                else outer.SetNextState(new RoyDownAir());
                return;
            }

            if (kind == Kind.Roll || kind == Kind.BackRoll)
            {
                // like Commando's slide: driven by his move speed, starting at 3x and easing to 1x over the roll (~9 units at base speed)
                float speed = rollSpeedScale * moveSpeedStat * Mathf.Lerp(3f, 1f, fixedAge / duration);
                // always travel the way the player is moving. (The back roll used to go "away from his facing", but when walking
                // backwards Roy has already turned to face the way he walks, so that sent him forward.) The back roll instead keeps
                // him facing away from the movement so its backward-roll animation reads right.
                Vector3 dir = rollDirection;
                if (characterMotor)
                {
                    characterMotor.moveDirection = Vector3.zero;
                    characterMotor.rootMotion += dir * speed * Time.fixedDeltaTime;
                }
                if (characterDirection) characterDirection.forward = kind == Kind.BackRoll ? -rollDirection : rollDirection;
                if (!secondRoll && inputBank.skill3.justPressed) queuedRoll = true;
            }
            else if (characterMotor && characterMotor.isGrounded)
            {
                characterMotor.velocity.x = 0f;
                characterMotor.velocity.z = 0f;
            }

            if (kind == Kind.AirDodge)
            {
                // normal air control during the air dodge (the dodge state replaces the usual movement state), so it can be used
                // to stretch out horizontal time in the air
                if (characterMotor && inputBank) characterMotor.moveDirection = inputBank.moveVector;
                if (characterDirection && inputBank && inputBank.moveVector.sqrMagnitude > 0.01f) characterDirection.moveVector = inputBank.moveVector;
                // primary/secondary during or after an air dodge are handled by those skills (RoyPrimary / RoyDoubleEdgeDance):
                // pressing either ends the dodge so the follow-up plays cleanly
                if (fixedAge > 0.1f && (inputBank.skill1.justPressed || inputBank.skill2.justPressed))
                {
                    outer.SetNextStateToMain();
                    return;
                }
            }

            if (fixedAge >= duration)
            {
                if (roy && (kind == Kind.Roll || kind == Kind.BackRoll))
                {
                    roy.lastRollEndTime = Time.fixedTime;
                    if (queuedRoll) { outer.SetNextState(new RoyDodge { secondRoll = true }); return; }
                    if (secondRoll) roy.rollSprint = true;
                }
                outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)kind);
            writer.Write(rollDirection);
            writer.Write(secondRoll);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            kind = (Kind)reader.ReadByte();
            rollDirection = reader.ReadVector3();
            secondRoll = reader.ReadBoolean();
        }
    }

    // Air-dodge follow-ups: up to two Up Airs (the first lifts 5/8 of a jump, the second hovers),
    // then one finisher: Blazer (rising, if an Up Air came first) or an airborne Up Smash.
    public class RoyAirFollowUp : RoyMelee
    {
        public enum Follow { UpAir, Blazer, UpSmash }
        public Follow follow;
        public int index;

        protected override void SetupAttack()
        {
            switch (follow)
            {
                case Follow.UpAir:
                    animationState = "UpAir"; baseDuration = 0.92f; damageCoefficient = 2f;
                    attackStartPercentTime = 0.15f; attackEndPercentTime = 0.45f;
                    if (index == 0) liftFraction = 0.625f; else hoverDuration = 0.35f;
                    break;
                case Follow.Blazer:
                    animationState = "Blazer"; baseDuration = 0.82f; damageCoefficient = 4f;
                    attackStartPercentTime = 0.1f; attackEndPercentTime = 0.6f; liftFraction = 1.5f;
                    bonusForce = Vector3.up * 1500f;
                    break;
                default:
                    animationState = "UpSmashAir"; baseDuration = 1.32f; damageCoefficient = 4.5f;
                    attackStartPercentTime = 0.2f; attackEndPercentTime = 0.45f; liftFraction = 0.667f;
                    bonusForce = Vector3.up * 1200f;
                    break;
            }
            earlyExitPercentTime = 0.7f;
            pushForce = 0f;
        }

        protected override bool ChainButtonPressed() => false;

        public override void OnSerialize(NetworkWriter writer) { base.OnSerialize(writer); writer.Write((byte)follow); writer.Write((byte)index); }
        public override void OnDeserialize(NetworkReader reader) { base.OnDeserialize(reader); follow = (Follow)reader.ReadByte(); index = reader.ReadByte(); }
    }
}
