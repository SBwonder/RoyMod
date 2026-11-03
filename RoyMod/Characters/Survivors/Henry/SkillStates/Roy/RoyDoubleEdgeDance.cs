using HenryMod.Survivors.Henry.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace HenryMod.Survivors.Henry.SkillStates
{
    // Double-Edge Dance: up to four hits, each further press of the secondary continuing the string.
    //   holding Jump as hit 2 or 3 starts  -> the upward versions, finishing with the up-air juggle (RoyJuggle)
    //   used during/right after the dash attack -> the downward route: hit 3 (down), then hit 4 (down)
    // Every hit feeds the flame meter.
    public class RoyDoubleEdgeDance : RoyMelee
    {
        public int step = 1;
        public bool upRoute;
        public bool downRoute;
        public int airDodgeFinisher;   // 0 = none, 1 = upward stab, 2 = Blazer (secondary after an air dodge)
        private bool blazerForward;
        private bool decided;

        protected override void SetupAttack()
        {
            if (isAuthority && !decided && step == 1 && roy && roy.AirDodgeChainOpen)
            {
                airDodgeFinisher = roy.airDodgeUpAirs > 0 ? 2 : 1;
                roy.airDodgeFinisherUsed = true;
            }
            if (airDodgeFinisher > 0)
            {
                decided = true;
                step = 4;                                   // a finisher: nothing chains after it
                pushForce = 0f; earlyExitPercentTime = 0.7f;
                hitboxGroupName = "UpAirGroup";
                heavyHit = airDodgeFinisher == 1;   // the upward stab
                if (airDodgeFinisher == 2)
                {
                    // Blazer: the rising flame slash, lifting him 1.5 jumps. Multi-hit like Smash's: six hits (see blazerHits), every
                    // one burns and adds an eighth of the meter (3/4 in all)
                    animationState = "Blazer"; baseDuration = 0.82f; damageCoefficient = blazerHits[0].damage;
                    attackStartPercentTime = blazerHits[0].start; attackEndPercentTime = blazerHits[0].end; liftFraction = 1.5f * 1.5f; bonusForce = Vector3.up * 1500f;   // utility lifts are 1.5x (Alex)
                    meterGainOnHit = 12.5f; alwaysIgnite = true; procCoefficient = 0.9f;
                    // holding forward: Smash's angled Blazer, travelling forward instead of straight up
                    if (isAuthority && inputBank && Vector3.Dot(inputBank.moveVector, GetAimForward()) > 0.5f) blazerForward = true;
                    if (blazerForward) liftFraction = 0.5f * 1.5f;
                }
                else
                {
                    // the upward stab (airborne up smash) for an enemy above, rising 2/3 of a jump
                    animationState = "UpSmashAir"; baseDuration = 1.32f; damageCoefficient = 4.5f;
                    attackStartPercentTime = 0.2f; attackEndPercentTime = 0.45f; liftFraction = 0.667f * 1.5f; bonusForce = Vector3.up * 1200f;
                    meterGainOnHit = 100f / 3f;   // a third of the meter
                }
                return;
            }
            if (isAuthority && !decided)
            {
                if (step == 1 && roy && (roy.dashAttackActive || Time.fixedTime - roy.dashAttackEndTime < RoyController.dashFollowUpWindow))
                {
                    downRoute = true;
                    step = 3;
                    roy.dashAttackActive = false;
                    roy.dashAttackEndTime = -99f;
                }
                // in the air the string is the up path: its two rising hits, then the juggle (holding Jump on the ground lost out to
                // the jump itself, so the route is chosen by being airborne instead)
                if (!downRoute && step == 1 && characterMotor && !characterMotor.isGrounded)
                {
                    if (roy) roy.UseAirFollowUp();
                    upRoute = true;
                    step = 2;
                }
            }
            decided = true;

            // two full strings fill the meter: the ground string's 4 hits give 12.5 each, the dash follow-up's 2 give 25,
            // and the up path's 5 (two rising hits + three juggle hops) give 10
            meterGainOnHit = downRoute ? 25f : upRoute ? 10f : 12.5f;
            procCoefficient = 0.95f;
            heavyHit = step >= 4;   // the string's last hit
            earlyExitPercentTime = 0.4f;
            attackStartPercentTime = 0.2f;
            attackEndPercentTime = 0.4f;
            damageCoefficient = 1.6f;

            switch (step)
            {
                case 1:
                    animationState = "DED1"; baseDuration = 1.1f;
                    forwardDistance = 0.46f; forwardStart = 0.15f; forwardEnd = 0.35f;
                    break;
                case 2:
                    animationState = upRoute ? "DED2Up" : "DED2"; baseDuration = upRoute ? 0.85f : 0.8f;
                    if (upRoute) { bonusForce = Vector3.up * 800f; pushForce = 0f; hoverDuration = 0.35f; }   // up route: a slight lift (he hangs in the air while swinging)
                    break;
                case 3:
                    if (downRoute) { animationState = "DED3Down"; baseDuration = 0.88f; damageCoefficient = 2f; pushForce = -2000f; }   // the low repositioning sweep pulls enemies in toward Roy (negative push), so the five plunges that follow can all reach
                    else { animationState = upRoute ? "DED3Up" : "DED3"; baseDuration = upRoute ? 0.85f : 0.88f; forwardDistance = upRoute ? 0f : 0.33f; }
                    if (upRoute) { bonusForce = Vector3.up * 1400f; pushForce = 0f; hoverDuration = 0.35f; }   // up route: lifts them further
                    break;
                default:
                    if (downRoute) { animationState = "DED4Down"; baseDuration = 1.45f; forwardDistance = 0.5f; forwardStart = 0.2f; forwardEnd = 0.45f; }
                    else { animationState = "DED4"; baseDuration = 1.25f; forwardDistance = 0.23f; }
                    damageCoefficient = 3f; pushForce = 1000f; hitStopDuration = 0.1f;
                    earlyExitPercentTime = 0.75f;
                    break;
            }
            // each hit has its own box: the sprint attack's shape, tilted along that hit's swing (prefab builder: <state>Hitbox)
            hitboxGroupName = animationState + "Group";

            // the down route's last hit: five quick forward plunges, each a fresh hit with Acolyte's long, narrow javelin box,
            // then the closing slash with the tilted box (windows are fractions of the clip, traced from the sword tip)
            if (downRoute && step >= 4)
            {
                BuildPlungeWindows();
                damageCoefficient = 1f;
                procCoefficient = 0.8f;   // the stabs; the closing slash goes back to the string's 0.95
                hitboxGroupName = plungeWindows[0].group;
                attackStartPercentTime = plungeWindows[0].start;
                attackEndPercentTime = plungeWindows[0].end;
            }
        }

        // five stabs at base attack speed, more with attack speed (like Commando's Suppressive Fire: base count x attack speed),
        // packed into the clip's stabbing stretch (6-23%, spilling later if they need room), then the closing slash
        private const int baseStabs = 5;
        private const float stabsStart = 0.06f, stabsEnd = 0.23f, minStabSeconds = 0.04f;
        private (float start, float end, string group)[] plungeWindows;

        private void BuildPlungeWindows()
        {
            float attackSpeed = characterBody ? Mathf.Max(1f, characterBody.attackSpeed) : 1f;
            int stabs = Mathf.Max(baseStabs, Mathf.FloorToInt(baseStabs * attackSpeed));
            float clip = baseDuration / attackSpeed;
            float slot = Mathf.Max((stabsEnd - stabsStart) / stabs, minStabSeconds / clip);
            plungeWindows = new (float, float, string)[stabs + 1];
            for (int i = 0; i < stabs; i++)
            {
                float start = Mathf.Min(stabsStart + i * slot, 0.6f);
                plungeWindows[i] = (start, start + slot * 0.8f, "JavelinGroup");
            }
            plungeWindows[stabs] = (0.69f, 0.77f, "DED4DownGroup");
        }
        private int plunge;

        // Blazer's hits across its rise (start, end as fractions of the move, damage coefficient): a launching first hit,
        // four light ones, and a heavier last one (4x damage in all)
        private static readonly (float start, float end, float damage)[] blazerHits =
        {
            (0.1f, 0.17f, 1f), (0.18f, 0.25f, 0.5f), (0.26f, 0.33f, 0.5f), (0.34f, 0.41f, 0.5f), (0.42f, 0.5f, 0.5f), (0.51f, 0.6f, 1f),
        };
        private int blazerHit;

        protected override bool ChainButtonPressed() => inputBank && inputBank.skill2.justPressed;

        public override void OnEnter()
        {
            base.OnEnter();
            // Blazer has its own cry; each hit of the string gets Smash's Double-Edge Dance sound, except the dash route's
            // stabbing finisher, which has its own
            if (airDodgeFinisher == 2) RoySound.Play("se_roy_special_H01", gameObject);
            else if (airDodgeFinisher == 1)
            {
                // the upward stab
                RoySound.Play("se_roy_attackair_H01", gameObject);
                RoySound.Play("se_roy_smash_H02", gameObject);
            }
            else if (airDodgeFinisher == 0) RoySound.Play(downRoute && step >= 4 ? "se_roy_special_S03S" : "se_roy_special_S04H", gameObject);
        }

        public override void FixedUpdate()
        {
            // forward Blazer: carried forward exactly like a ground roll (move speed x3 easing to x1 over the roll's 0.65 s),
            // so it covers the same distance (Alex: 80% of the roll felt short)
            const float rollDuration = 0.65f;
            if (blazerForward && isAuthority && characterMotor && fixedAge < rollDuration)
            {
                characterMotor.rootMotion += GetAimForward() * 1.75f * moveSpeedStat * Mathf.Lerp(3f, 1f, fixedAge / rollDuration) * Time.fixedDeltaTime;   // 1.75x, like the rolls
            }
            // Blazer: next hit window, with its damage, a fresh hit list and a fresh meter gain
            if (airDodgeFinisher == 2 && blazerHit + 1 < blazerHits.Length && stopwatch >= duration * blazerHits[blazerHit].end)
            {
                blazerHit++;
                attackStartPercentTime = blazerHits[blazerHit].start;
                attackEndPercentTime = blazerHits[blazerHit].end;
                attack.damage = blazerHits[blazerHit].damage * damageStat * (roy ? roy.DamageMultiplier : 1f);
                attack.ResetIgnoredHealthComponents();
                gainedMeter = false;
            }
            // move on to the next plunge: new window, its box, and a fresh hit list so every plunge can land
            if (downRoute && step >= 4 && plunge + 1 < plungeWindows.Length && stopwatch >= duration * plungeWindows[plunge].end)
            {
                plunge++;
                attackStartPercentTime = plungeWindows[plunge].start;
                attackEndPercentTime = plungeWindows[plunge].end;
                attack.hitBoxGroup = FindHitBoxGroup(plungeWindows[plunge].group);
                attack.ResetIgnoredHealthComponents();
                attack.procCoefficient = plungeWindows[plunge].group == "JavelinGroup" ? 0.8f : 0.95f;
                if (plungeWindows[plunge].group == "JavelinGroup") RoySound.Play("se_roy_special_S03S", gameObject);   // one per stab
            }
            base.FixedUpdate();
            if (!isAuthority || outer.nextState != null) return;

            if (queuedPress && step < 4 && stopwatch >= duration * earlyExitPercentTime)
            {
                // the up route's finisher is the juggle instead of a fourth ground hit
                if (upRoute && step == 3) outer.SetNextState(new RoyJuggle());
                else outer.SetNextState(new RoyDoubleEdgeDance { step = step + 1, upRoute = upRoute, downRoute = downRoute, decided = true });
                return;
            }
            // leaving the ground cancels the ground string; jumping out of it keeps the secondary ready for a moment so a
            // press in the air carries on into the up path (RoyController hands the charge back and takes it away again)
            if (characterMotor && !characterMotor.isGrounded && !upRoute && airDodgeFinisher == 0)
            {
                if (roy && !downRoute) roy.OpenAirFollowUp(skillLocator ? skillLocator.secondary : null);
                outer.SetNextStateToMain();
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)step);
            writer.Write(upRoute);
            writer.Write(downRoute);
            writer.Write((byte)airDodgeFinisher);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            step = reader.ReadByte();
            upRoute = reader.ReadBoolean();
            downRoute = reader.ReadBoolean();
            airDodgeFinisher = reader.ReadByte();
            decided = true;
        }
    }

    // The up-route finisher: three jumping up-airs, each hop 0.8x as high as the last.
    public class RoyJuggle : RoyMelee
    {
        public int hop;

        public override void OnEnter()
        {
            base.OnEnter();
            RoySound.Play("se_roy_attackair_F01", gameObject);
        }

        protected override void SetupAttack()
        {
            animationState = "UpAir"; baseDuration = 0.92f; damageCoefficient = 2f; hitboxGroupName = "UpAirGroup";
            attackStartPercentTime = 0.15f; attackEndPercentTime = 0.45f; earlyExitPercentTime = 0.6f;
            meterGainOnHit = 10f;
            procCoefficient = 0.95f;
            // the up route's finisher: Roy launches himself 7/8 of his jump height carrying the enemy up with him,
            // then two more up airs keep them juggled, each hop 0.8x the last
            liftFraction = 0.875f * Mathf.Pow(0.8f, hop);
            bonusForce = Vector3.up * (hop == 0 ? 1700f : 1300f); pushForce = 0f;
        }

        protected override bool ChainButtonPressed() => false;

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (isAuthority && outer.nextState == null && hop < 2 && stopwatch >= duration * earlyExitPercentTime)
            {
                outer.SetNextState(new RoyJuggle { hop = hop + 1 });
            }
        }

        public override void OnSerialize(NetworkWriter writer) { base.OnSerialize(writer); writer.Write((byte)hop); }
        public override void OnDeserialize(NetworkReader reader) { base.OnDeserialize(reader); hop = reader.ReadByte(); }
    }
}
