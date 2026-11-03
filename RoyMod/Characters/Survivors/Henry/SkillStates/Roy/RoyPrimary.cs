using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Roy's primary. Which attack comes out depends on context:
    //   airborne                         -> Neutral Air
    //   sprinting                        -> Dash Attack (a click during it, or right after, -> Forward Smash)
    //   fresh clicks (3 within 2 s)      -> Pummel (knee) > Jab > Forward Smash
    //   holding the button               -> Pummel, then Jab / Forward Tilt on loop
    public class RoyPrimary : RoyMelee
    {
        public enum Move { Pummel, Jab, ForwardTilt, ForwardSmash, DashAttack, NeutralAir, AirDodgeUpAir }

        public Move move;
        public bool fromQueue;
        private int airDodgeUpAirIndex;
        private bool fromHoldLoop;   // the held jab/tilt loop hits a little harder than the clicked jab
        private bool decided;

        protected override void SetupAttack()
        {
            if (isAuthority && !decided) move = Decide();
            decided = true;

            // the standing and air attacks play well below Smash speed (Alex: Smash's pace felt too fast here); the dash attack keeps full speed.
            // Hit windows are long, Mercenary/False Son style: the hitbox stays out for most of the swing's arc.
            const float slower = 1f / (0.7f * 0.8f);        // standing attacks: 56% of Smash speed
            const float slowerAir = 1f / (0.7f * 0.7f);     // neutral air: 49%

            switch (move)
            {
                case Move.Pummel:
                    animationState = "Pummel"; baseDuration = 0.38f * slower; damageCoefficient = 0.9f;
                    hitboxGroupName = "PummelGroup"; attackStartPercentTime = 0.3f; attackEndPercentTime = 0.65f; earlyExitPercentTime = 0.6f;
                    pushForce = 300f;
                    break;
                case Move.Jab:
                    animationState = "Jab1"; layerName = Override; baseDuration = 0.68f * slower; damageCoefficient = fromHoldLoop ? 1.3f : 1.1f;
                    hitboxGroupName = "JabGroup"; attackStartPercentTime = 0.18f; attackEndPercentTime = 0.5f; earlyExitPercentTime = 0.5f;
                    break;
                case Move.ForwardTilt:
                    animationState = "FTilt"; layerName = Override; baseDuration = 0.82f * slower; damageCoefficient = 1.3f;
                    hitboxGroupName = "FTiltGroup"; attackStartPercentTime = 0.2f; attackEndPercentTime = 0.52f; earlyExitPercentTime = 0.55f;
                    break;
                case Move.ForwardSmash:
                    animationState = "FSmash"; layerName = Override; baseDuration = 1.23f * slower; damageCoefficient = 1.5f;
                    hitboxGroupName = "FSmashGroup"; attackStartPercentTime = 0.13f; attackEndPercentTime = 0.3f; // sword raised (13%) through the end of the downswing (~28%) earlyExitPercentTime = 0.7f;
                    pushForce = 1200f; hitStopDuration = 0.12f; heavyHit = true;
                    break;
                case Move.DashAttack:
                    animationState = "DashAttack"; baseDuration = 0.98f; damageCoefficient = 2.7f;
                    // starts early and stays out (travelling with him) until he settles back toward idle
                    hitboxGroupName = "DashGroup"; attackStartPercentTime = 0.06f; attackEndPercentTime = 0.75f; earlyExitPercentTime = 0.75f;
                    procCoefficient = 1.1f;
                    forwardDistance = 2.4f; forwardStart = 0.1f; forwardEnd = 0.5f; pushForce = 800f;
                    break;
                case Move.AirDodgeUpAir:
                    // after an air dodge: up to two up airs, the first lifting him 5/8 of a jump, the second hanging in place
                    animationState = "UpAir"; baseDuration = 0.92f; damageCoefficient = 1.6f;
                    hitboxGroupName = "UpAirGroup"; attackStartPercentTime = 0.15f; attackEndPercentTime = 0.45f; earlyExitPercentTime = 0.6f;
                    pushForce = 0f; bonusForce = Vector3.up * 1300f; procCoefficient = 0.9f;
                    if (airDodgeUpAirIndex == 0) liftFraction = 0.625f * 1.5f; else hoverDuration = 0.35f;   // utility lifts are 1.5x (Alex)
                    break;
                case Move.NeutralAir:
                    // two swings, each with the dash attack's wide box: the first sweep (5-22% of the clip), then the swing back (24-44%)
                    animationState = "NAir"; baseDuration = 0.82f * slowerAir; damageCoefficient = 1.7f;
                    hitboxGroupName = "DashGroup"; attackStartPercentTime = 0.05f; attackEndPercentTime = 0.22f; earlyExitPercentTime = 0.6f;
                    hitHopVelocity = 6f; procCoefficient = 0.9f;
                    break;
            }

            if (move == Move.DashAttack && roy)
            {
                roy.dashAttackActive = true;
                characterBody.isSprinting = false;
                RoySound.Play("se_roy_attackdash", gameObject);   // always, on top of any hit sounds (as in Smash)
            }
        }

        private Move Decide()
        {
            if (roy && roy.AirDodgeChainOpen && roy.airDodgeUpAirs < 2)
            {
                airDodgeUpAirIndex = roy.airDodgeUpAirs++;
                return Move.AirDodgeUpAir;
            }
            if (characterMotor && !characterMotor.isGrounded) return Move.NeutralAir;
            if (characterBody.isSprinting) return Move.DashAttack;
            if (!roy) return Move.Jab;

            float now = Time.fixedTime;
            if (roy.dashAttackActive || now - roy.dashAttackEndTime < RoyController.dashFollowUpWindow)
            {
                roy.dashAttackActive = false;
                roy.dashAttackEndTime = -99f;
                roy.primaryPresses = 0;
                return Move.ForwardSmash;
            }

            bool freshPress = fromQueue || inputBank.skill1.justPressed;
            if (freshPress)
            {
                if (now - roy.firstPressTime > RoyController.comboWindow || roy.primaryPresses >= 3)
                {
                    roy.primaryPresses = 0;
                    roy.firstPressTime = now;
                }
                roy.primaryPresses++;
                roy.holdAlternate = false;
                if (roy.primaryPresses == 2) return Move.Jab;
                if (roy.primaryPresses == 3) return Move.ForwardSmash;
                return Move.Pummel;
            }

            // held: loop Jab / Forward Tilt
            fromHoldLoop = true;
            roy.holdAlternate = !roy.holdAlternate;
            return roy.holdAlternate ? Move.Jab : Move.ForwardTilt;
        }

        // vanilla swing effects (see RoySlashEffect), spawned when the blade starts its swing rather than when the hit window opens
        private bool slashSpawned;

        protected override void PlaySwingEffect() { }

        private RoySlashEffect.Mount SlashMount()
        {
            switch (move)
            {
                case Move.Jab: return RoySlashEffect.Jab;
                case Move.ForwardTilt: return RoySlashEffect.ForwardTilt;
                case Move.ForwardSmash: return RoySlashEffect.ForwardSmash;
                case Move.DashAttack: return RoySlashEffect.DashAttack;
                case Move.NeutralAir: return RoySlashEffect.NeutralAir;
                case Move.AirDodgeUpAir: return RoySlashEffect.Jab;
                default: return RoySlashEffect.KneeStrike;
            }
        }

        private bool secondSwing, firstSwingSounded;

        // every hit of the standing string (knee, jab, tilt, forward smash, and the hold loop) holds off the next one until
        // 0.5 s after it started (Alex: follow-ups came out too fast); clicks meanwhile stay buffered, a held button waits too
        private const float comboFollowUpDelay = 0.5f;
        private const float holdJabHandOff = 0.4f, holdTiltHandOff = 0.1f;
        protected override bool CanExit() => move == Move.DashAttack || move == Move.NeutralAir || fixedAge >= comboFollowUpDelay;

        public override void FixedUpdate()
        {
            // neutral air's swing back: a second hit window with a fresh hit list, so the same enemies can be hit again
            if (move == Move.NeutralAir && !secondSwing && stopwatch >= duration * 0.23f)
            {
                secondSwing = true;
                attackStartPercentTime = 0.24f;
                attackEndPercentTime = 0.44f;
                attack.ResetIgnoredHealthComponents();
                RoySound.Play("se_roy_swing_s", gameObject);   // the swing back gets its own swing sound
                RoySound.Play("se_roy_swing_ll", gameObject);
            }
            // neutral air's first swing sounds as its first hit window opens
            if (move == Move.NeutralAir && !firstSwingSounded && stopwatch >= duration * 0.05f)
            {
                firstSwingSounded = true;
                RoySound.Play("se_roy_swing_s", gameObject);
                RoySound.Play("se_roy_swing_ll", gameObject);
            }
            base.FixedUpdate();
            if (!slashSpawned && stopwatch >= duration * SlashMount().delay)
            {
                slashSpawned = true;
                RoySlashEffect.Spawn(characterBody, GetAimForward(), SlashMount());
                // every primary swings with Smash's swing sound; the third hit of the string adds its forward smash sound
                // the knee (pummel) isn't a sword swing: it gets Smash's jump grunt instead
                if (move == Move.Pummel) RoySound.Play("se_roy_jump01", gameObject);
                else if (move != Move.NeutralAir) { RoySound.Play("se_roy_swing_s", gameObject); RoySound.Play("se_roy_swing_ll", gameObject); }
                if (move == Move.ForwardSmash) RoySound.Play("se_roy_smash_S01_win01", gameObject);
            }
            if (!isAuthority || outer.nextState != null) return;

            // a buffered click chains straight into the next attack at the cancel point
            // hold loop: the held jab hands off to the tilt 0.4 s before its animation ends, the tilt to the jab 0.1 s before
            // (Alex: the loop's attacks came out too late)
            if ((move == Move.Jab || move == Move.ForwardTilt) && !queuedPress && inputBank.skill1.down
                && stopwatch >= duration - (move == Move.Jab ? holdJabHandOff : holdTiltHandOff) && CanExit())
            {
                if (roy) roy.holdAlternate = move == Move.Jab;   // the hold loop's next pick: tilt after a jab, jab after a tilt
                outer.SetNextState(new RoyPrimary());
                return;
            }
            // dash attack: the follow-up comes out 0.1 s after his lunge stops; forward smash: a held button starts the loop
            // after only a quarter of the recovery that follows its hit (Alex: holding through the string felt too slow)
            float followUpAt = duration * earlyExitPercentTime;
            if (move == Move.DashAttack) followUpAt = duration * forwardEnd + 0.1f;
            if (move == Move.ForwardSmash) followUpAt = duration * (attackEndPercentTime + 0.25f * (1f - attackEndPercentTime));
            bool holding = (move == Move.DashAttack || move == Move.ForwardSmash) && inputBank.skill1.down;
            if ((queuedPress || holding) && stopwatch >= followUpAt && CanExit())
            {
                outer.SetNextState(new RoyPrimary { fromQueue = queuedPress });
                return;
            }
            // leaving the ground cancels a ground attack (a held/queued click becomes Neutral Air on the next activation)
            if (move != Move.NeutralAir && move != Move.AirDodgeUpAir && characterMotor && !characterMotor.isGrounded && move != Move.DashAttack)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (move == Move.DashAttack && roy)
            {
                roy.dashAttackActive = false;
                roy.dashAttackEndTime = Time.fixedTime;
            }
            if (move == Move.NeutralAir && characterMotor && characterMotor.isGrounded)
            {
                PlayAnimation(FullBody, "LandingAirN");
            }
            base.OnExit();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)move);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            move = (Move)reader.ReadByte();
            decided = true;
        }
    }
}
