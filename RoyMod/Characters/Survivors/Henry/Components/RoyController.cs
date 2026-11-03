using HenryMod.Survivors.Henry.SkillStates;
using RoR2;
using UnityEngine;

namespace HenryMod.Survivors.Henry.Components
{
    // Per-body memory for Roy's kit: combo/string windows, dodge and air-dodge chains, and the flame meter.
    // Everything here is read and written by the body's authority, which is also where melee damage is computed.
    public class RoyController : MonoBehaviour
    {
        // primary: three fresh clicks within this window = Pummel > Jab > Forward Smash
        public const float comboWindow = 2f;
        // a click during (or this long after) the dash attack follows up with Forward Smash
        public const float dashFollowUpWindow = 0.75f;
        // a second dodge press during a roll (or this long after it) chains a second roll
        public const float rollChainWindow = 0.3f;
        // perfect dodge / perfect counter: the hit lands within this long of the press
        public const float perfectWindow = 0.1f;

        public int primaryPresses;
        public float firstPressTime = -99f;
        public bool holdAlternate;
        public float dashAttackEndTime = -99f;
        public bool dashAttackActive;

        public int rollsInChain;
        public float lastRollEndTime = -99f;
        public bool rollSprint;

        public int airDodgeUpAirs;
        public bool airDodgeFinisherUsed;
        public bool airDodgedThisAirtime;
        // after an air dodge (until landing): primary = up air (twice at most), secondary = upward stab, or Blazer once an up air was used
        public bool AirDodgeChainOpen => airDodgedThisAirtime && !airDodgeFinisherUsed && motor && !motor.isGrounded;

        // sword sweet spot (Alex): an enemy hit by the lower half of the blade, i.e. close to Roy, takes more damage and is more
        // likely to be crit; that hit, and any sword crit, plays Smash's strong-hit sound
        public const float sweetSpotRange = 2f, sweetSpotDamage = 1.25f, sweetSpotCritChance = 25f;
        public int swordAttacks;   // sword attack states running (counted so overlapping states on both machines work)

        public float lastHitTime = -99f;    // set when an incoming hit is processed (server/host)
        public GameObject lastAttacker;

        // flame meter, 0-100: secondary hits, counters and perfect dodges build it; it drains when not fed
        public const float enflamedThreshold = 50f;
        // after this long without gaining, the meter drains: 3 per second at first, 1 more each second it keeps draining, up to 10
        public const float meterDrainDelay = 10f, meterDrainStart = 3f, meterDrainMax = 10f;
        public float meter;
        private float lastMeterGain = -99f;
        private bool drainStarted;

        private CharacterBody body;
        private CharacterMotor motor;

        public float DamageMultiplier => 1f + 0.5f * meter / 100f;
        public bool Enflamed => meter >= enflamedThreshold;
        public float MeterFraction => meter / 100f;

        private void Awake()
        {
            body = GetComponent<CharacterBody>();
            motor = GetComponent<CharacterMotor>();
        }

        public void AddMeter(float amount)
        {
            float before = meter;
            meter = Mathf.Clamp(meter + amount, 0f, 100f);
            if (before < 100f && meter >= 100f && body) RoySound.Play("btl_skill1", body.gameObject);   // full meter
            else if (before < enflamedThreshold && meter >= enflamedThreshold && body) RoySound.Play("sys_powerup1", body.gameObject);   // Enflamed
            lastMeterGain = Time.fixedTime;
        }

        // Double-Edge Dance cut short by a jump: the secondary gets its charge back for a moment so it can continue in the air
        // as the up path; if that window passes unused, the charge is taken back (its cooldown kept running all along)
        public const float airFollowUpWindow = 1f;
        private float airFollowUpUntil = -99f;
        private RoR2.GenericSkill refundedSkill;

        public void OpenAirFollowUp(RoR2.GenericSkill secondary)
        {
            if (!secondary || secondary.stock >= secondary.maxStock) return;
            secondary.AddOneStock();
            refundedSkill = secondary;
            airFollowUpUntil = Time.fixedTime + airFollowUpWindow;
        }

        public void UseAirFollowUp()
        {
            refundedSkill = null;
        }

        // the flame sword on his blade follows the meter (fire grows with it; the swing trail shows from 25%), burns blue
        // on the Awakening skin, and only trails while he's attacking
        private RoyFlameSword flameSword;
        private EntityStateMachine weaponMachine, bodyMachine;

        private RoyMeterHud meterHud;

        // footsteps (Alex's Smash step sounds): his Smash clips carry no footstep events, so steps are timed from how fast he's
        // actually moving on the ground, alternating feet with a random weight (m/l) each step
        // (Alex: one step per 1.6 units was 2-3 sounds per visible step; halved)
        private const float strideLength = 3.2f, minStepInterval = 0.25f, maxStepInterval = 1.2f;
        // step rate multipliers (Alex): walking steps 1.3x, sprinting steps 1.4x
        private const float walkStepRate = 1.3f, sprintStepRate = 1.4f;
        private static readonly string[] stepSizes = { "m", "l" };   // the small steps were quiet enough to read as missing
        private Vector3 lastPosition;
        private float nextStep, smoothedSpeed;
        private bool leftFoot;

        private void UpdateFootsteps()
        {
            Vector3 moved = transform.position - lastPosition;
            lastPosition = transform.position;
            moved.y = 0f;
            // frame-to-frame movement jitters, so the cadence follows a smoothed speed
            float speed = Time.deltaTime > 0f ? Mathf.Min(moved.magnitude / Time.deltaTime, 50f) : 0f;
            smoothedSpeed = Mathf.Lerp(smoothedSpeed, speed, 1f - Mathf.Exp(-8f * Time.deltaTime));
            if (!motor || !motor.isGrounded || smoothedSpeed < 1f) { nextStep = Time.time + 0.1f; return; }
            if (Time.time < nextStep) return;
            float rate = body && body.isSprinting ? sprintStepRate : walkStepRate;
            nextStep = Time.time + Mathf.Clamp(strideLength / (smoothedSpeed * rate), minStepInterval, maxStepInterval);
            leftFoot = !leftFoot;
            RoySound.Play("se_roy_step_" + (leftFoot ? "left_" : "right_") + stepSizes[Random.Range(0, stepSizes.Length)], gameObject);
        }

        private void Update()
        {
            UpdateFootsteps();
            if (!meterHud && body && body.hasEffectiveAuthority && body.isPlayerControlled) meterHud = RoyMeterHud.Create(this, body);
            if (!flameSword)
            {
                if (!body || !body.modelLocator || !body.modelLocator.modelTransform) return;
                flameSword = body.modelLocator.modelTransform.GetComponentInChildren<RoyFlameSword>(true);
                if (!flameSword) flameSword = RoyFlameSword.Attach(body.modelLocator.modelTransform);
                weaponMachine = EntityStateMachine.FindByCustomName(gameObject, "Weapon");
                bodyMachine = EntityStateMachine.FindByCustomName(gameObject, "Body");
                if (!flameSword) return;
            }
            flameSword.intensity = MeterFraction;
            flameSword.blueFlames = body.skinIndex == 1;
            flameSword.attacking = (weaponMachine && !(weaponMachine.state is EntityStates.Idle))
                || (bodyMachine && !(bodyMachine.state is EntityStates.GenericCharacterMain));
        }

        private void FixedUpdate()
        {
            if (refundedSkill && Time.fixedTime > airFollowUpUntil)
            {
                if (refundedSkill.stock > 0) refundedSkill.DeductStock(1);
                refundedSkill = null;
            }
            float draining = Time.fixedTime - lastMeterGain - meterDrainDelay;
            if (draining > 0f && meter > 0f && !drainStarted)
            {
                drainStarted = true;
                if (body) RoySound.Play("sys_powerdown1", body.gameObject);   // the meter starts going down
            }
            if (draining <= 0f) drainStarted = false;
            if (draining > 0f)
            {
                float rate = Mathf.Min(meterDrainMax, meterDrainStart + Mathf.Floor(draining));
                meter = Mathf.Max(0f, meter - rate * Time.fixedDeltaTime);
            }

            if (motor && motor.isGrounded)
            {
                airDodgeUpAirs = 0;
                airDodgeFinisherUsed = false;
                airDodgedThisAirtime = false;
            }

            // after a two-roll chain Roy keeps sprinting until he stops moving
            if (rollSprint && body && body.hasEffectiveAuthority)
            {
                InputBankTest input = body.inputBank;
                if (!input || input.moveVector.sqrMagnitude < 0.01f) rollSprint = false;
                else body.isSprinting = true;
            }
        }

        // Roy's sword hits (the attacker's own melee hit while one of his sword attacks is running)
        private static void ApplySweetSpot(HealthComponent victim, DamageInfo damageInfo)
        {
            if (damageInfo == null || damageInfo.rejected || !damageInfo.attacker || damageInfo.inflictor != damageInfo.attacker) return;
            RoyController attacker = damageInfo.attacker.GetComponent<RoyController>();
            if (!attacker || attacker.swordAttacks <= 0 || !attacker.body || victim.body == attacker.body) return;

            Vector3 from = attacker.body.corePosition;
            Vector3 nearest = victim.body && victim.body.mainHurtBox && victim.body.mainHurtBox.collider
                ? victim.body.mainHurtBox.collider.ClosestPoint(from) : victim.transform.position;
            Vector3 offset = nearest - from; offset.y = 0f;
            bool sweet = offset.magnitude <= sweetSpotRange;
            if (sweet)
            {
                damageInfo.damage *= sweetSpotDamage;
                if (!damageInfo.crit && Util.CheckRoll(sweetSpotCritChance, attacker.body.master)) damageInfo.crit = true;
            }
            if (sweet || damageInfo.crit) RoySound.Play("se_roy_hit_s", victim.gameObject);
        }

        // incoming damage: dodges and the counter can intercept it (runs where damage is processed: the server/host)
        internal static void Init()
        {

            On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
            {
                ApplySweetSpot(self, damageInfo);
                RoyController roy = self.body ? self.body.GetComponent<RoyController>() : null;
                if (roy && damageInfo != null && !damageInfo.rejected)
                {
                    roy.lastHitTime = Time.fixedTime;
                    roy.lastAttacker = damageInfo.attacker;
                    EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(self.gameObject, "Body");
                    if (bodyMachine && bodyMachine.state is IRoyDefensive defensive && defensive.Intercept(damageInfo))
                    {
                        damageInfo.rejected = true;
                    }
                }
                orig(self, damageInfo);
            };

            // Fire Emblem's "support get" jingle when a run is launched from character select with Roy picked. It plays on the
            // application object so it carries on through the loading screen.
            Run.onRunStartGlobal += run =>
            {
                NetworkUser user = LocalUserManager.GetFirstLocalUser()?.currentNetworkUser;
                if (user && user.bodyIndexPreference == BodyCatalog.FindBodyIndex("RoyBody") && RoR2Application.instance)
                    RoySound.Play("fe_support_get", RoR2Application.instance.gameObject);
            };

            // Roy's level-up jingle replaces the team level-up sound for a player playing Roy
            On.RoR2.LevelUpEffectManager.OnTeamLevelUp += (orig, teamIndex) =>
            {
                TeamDef team = TeamCatalog.GetTeamDef(teamIndex);
                CharacterBody local = LocalUserManager.GetFirstLocalUser()?.cachedBody;
                if (team == null || teamIndex != TeamIndex.Player || !local || !local.GetComponent<RoyController>()) { orig(teamIndex); return; }
                string sound = team.levelUpSound;
                team.levelUpSound = "Play_jingle_rarityup";
                try { orig(teamIndex); }
                finally { team.levelUpSound = sound; }
            };

            // flinch on hit (every client plays it on its copy of Roy)
            GlobalEventManager.onClientDamageNotified += message =>
            {
                if (!message.victim || message.damage <= 0f) return;
                CharacterBody victim = message.victim.GetComponent<CharacterBody>();
                if (!victim || !victim.GetComponent<RoyController>()) return;
                EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(victim.gameObject, "Body");
                if (bodyMachine && bodyMachine.state is IRoyDefensive) return;
                Animator animator = victim.modelLocator && victim.modelLocator.modelTransform ? victim.modelLocator.modelTransform.GetComponent<Animator>() : null;
                if (!animator) return;
                bool grounded = victim.characterMotor && victim.characterMotor.isGrounded;
                string flinch = (grounded ? "Flinch" : "FlinchAir") + Random.Range(1, 4);
                animator.CrossFadeInFixedTime(flinch, 0.05f, animator.GetLayerIndex("Flinch"), 0f);
            };
        }
    }
}
