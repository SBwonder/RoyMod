using UnityEngine;

// Editor-only test driver for animRoy. In the real mod, Risk of Rain 2's own code sets
// these same Animator parameters every frame, so this script is not shipped with the mod.
//
// Play mode controls:
//   W/A/S/D  move (sets forwardSpeed / rightSpeed / isMoving)
//   Ctrl     sprint (isSprinting); attacking cancels sprint until Ctrl is pressed again (ROR2's default sprint key)
//   Shift    utility dodge (ROR2's default utility key): no direction = spot dodge, S = back roll,
//            W/A/D = roll (A/D turn Roy sideways for a side roll), in the air = air dodge. Shift again during
//            (or just after) a ground roll = a second roll in any direction, which comes out into a sprint. Air dodge
//            hops Roy up 5/8 of a normal jump's height (once per airtime, like a second jump).
//            After the air dodge: left click = up air (same lift), a second left click = up air that hovers
//            briefly; right click = airborne up smash (2/3 lift), or Blazer (1.5 jumps) after an up air
//   Space    jump (fakes upSpeed and isGrounded for a simple arc)
//   Left click or J   attack string on the "Gesture, Override" layer:
//            tap = Pummel, hold = Pummel then Jab1/FTilt loop, three clicks within comboWindow = Pummel -> Jab1 -> FSmash
//   Click in the air   NAir (repeats while held); landing mid-NAir cuts into LandingAirN
//   Right click or K   Double-Edge Dance: each click during a hit queues the next (DED1 -> DED2 -> DED3 -> DED4);
//            hold Space while the next hit starts to get the upward version (DED2Up / DED3Up), and the
//            upward finisher is a juggle: jump + UpAir juggleHops times, each hop a fifth lower
//   Perfect dodge: Shift (either roll of a chain, or any dodge) within perfectDodgeWindow of H = Flare Blade
//            on the ground, or a hovering down air in the air
//   H        also: getting hit = a random flinch; whole body when standing, upper body only when moving/attacking
//   Flame meter: secondary hits, counters and perfect dodges fill it (F adds 25 for testing); it drains
//            after a pause and drives the RoyFlameSword effect on the blade
//   T        swap skins: default, or Awakening (with blue flames)
//   R        special: Counter (air version in the air). H = simulate an enemy hitting Roy:
//            hit in the counter window = counterattack; R and the hit within perfectCounterWindow = Final Smash
//   Sprint + click  DashAttack; right click after it = DED3Down, right click again = DED4Down
//   Sprint + click  DashAttack, then click again within dashFollowUpWindow of it ending for FSmash
//   +/-      attack speed for the attacks (Slash.playbackRate)
[RequireComponent(typeof(Animator))]
public class RoyAnimatorTester : MonoBehaviour
{
    public float jumpVelocity = 12f;
    public float gravity = 30f;
    public float attackSpeed = 1f;
    // How far through an attack (0-1) a queued click can cancel into the next hit.
    [Range(0f, 1f)] public float comboCancelPoint = 0.6f;

    // Three clicks within this many seconds of the first click play the full combo.
    [Range(1f, 5f)] public float comboWindow = 2f;

    // After the dash attack ends, a click this soon afterwards is the forward smash follow-up.
    [Range(0.5f, 1f)] public float dashFollowUpWindow = 0.75f;

    private static readonly string[] comboString = { "Pummel", "Jab1", "FSmash" };
    private static readonly string[] holdLoop = { "Jab1", "FTilt" };
    // Secondary (right click): Double-Edge Dance, forward version of each hit.
    private static readonly string[] secondaryString = { "DED1", "DED2", "DED3", "DED4" };
    // Holding jump during the string swaps hits 2-4 for Smash's upward versions.
    // The upward finisher is the up air with a jump (UpAir), not Smash's DED4Up.
    private static readonly string[] secondaryUpString = { "DED1", "DED2Up", "DED3Up", "UpAir" };

    // Sprint route: after the dash attack, right click goes into the last two hits of the down version.
    private static readonly string[] secondaryDownString = { "DED1", "DED2", "DED3Down", "DED4Down" };

    // Two ground rolls in a row: a Shift press during the first roll (or this soon after it) queues the
    // second, in any direction. The second roll comes out of it straight into a sprint.
    [Range(0f, 1f)] public float rollChainWindow = 0.3f;

    // Special (R): Counter. H stands in for "an enemy hits Roy" until the real game does.
    //   hit inside the counter window      -> counterattack
    //   R and the hit within perfectCounterWindow of each other -> Final Smash (start, then the strike)
    // Real timings get tuned in the mod; these just drive the preview.
    public float counterWindowStart = 0.1f;
    public float counterWindowEnd = 0.6f;
    public float perfectCounterWindow = 0.1f;

    // Perfect dodge: a utility press (first or second) within this long of an enemy hit (H) turns into
    // Flare Blade on the ground, or a down air that hovers Roy in the air.
    public float perfectDodgeWindow = 0.1f;
    [Range(0f, 1f)] public float perfectDodgeHoverTime = 0.35f;

    // Flame meter (0-100): secondary hits, counters and perfect dodges fill it; it drains after a pause.
    // It drives the RoyFlameSword effect and previews the damage bonus the mod would give.
    public float flameMeter;

    // Skin preview (T): the default look, or the Awakening skin with blue flames.
    public bool awakeningSkin;

    // Lingering fire left behind: a pillar on a perfect dodge, a tornado on a perfect counter.
    // Both grow and last longer with the flame meter, and burn blue on the Awakening skin.
    public GameObject firePillarPrefab;
    public GameObject fireTornadoPrefab;
    public float flamePerSecondaryHit = 8f;
    public float flamePerCounter = 25f;
    public float flamePerPerfectDodge = 20f;
    public float flameDecayDelay = 3f;      // seconds without gaining before it drains
    public float flameDecayPerSecond = 10f;
    public float flameMaxDamageBonus = 0.5f; // +50% damage at a full meter (preview number)
    [Range(0f, 100f)] public float enflamedThreshold = 50f;

    // Flip which side roll (left or right) uses the mirrored roll, if the spins feel backwards.
    public bool swapSideRollSpin = false;

    // The air dodge doubles as a small second jump: this fraction of the normal jump's height.
    [Range(0f, 1f)] public float airDodgeHeightFraction = 0.625f;   // 5/8
    // Air dodge follow-ups: left click = up air (same lift as the air dodge),
    // right click = airborne up smash with relaxed legs (2/3 of a jump's height).
    [Range(0f, 1f)] public float upSmashAirHeightFraction = 0.6667f;
    // A second up air in a row doesn't lift; Roy just hangs in the air this long.
    [Range(0f, 1f)] public float secondUpAirHoverTime = 0.35f;
    // Right click after one or two up airs: Blazer (Smash up special) with a big lift, in jump heights.
    [Range(0.5f, 3f)] public float blazerHeightFraction = 1.5f;

    // Upward finisher juggle: Roy hops and up-airs this many times in a row,
    // each hop reaching this fraction of the previous hop's height (0.8 = a fifth lower each time).
    public int juggleHops = 3;
    [Range(0.5f, 1f)] public float juggleHeightMultiplier = 0.8f;
    // How far through each up air (0-1) the next juggle hop starts.
    [Range(0.3f, 1f)] public float juggleNextPoint = 0.6f;

    // How far through a Double-Edge Dance hit (0-1) a queued right click moves on to the next hit.
    [Range(0f, 1f)] public float secondaryCancelPoint = 0.4f;

    private Animator animator;
    private int gestureLayer;       // upper body only: Jab1, FTilt, FSmash
    private int fullBodyLayer;      // whole body: Pummel, so the knee strike shows
    private bool attacking;
    private bool holdLooping;       // true once holding the button has taken over from the combo
    private string currentAttack = "-";
    private int comboStep = -1;     // last combo hit started (index into comboString), -1 = none
    private float comboStartTime;   // when the first click of the current combo happened
    private bool clickQueued;       // a click waiting for the current hit to reach the cancel point
    private int holdStep;           // index into holdLoop
    private float attackTimer;      // seconds into the current attack
    private float attackDuration;   // current attack's length at the current attack speed
    private bool sprintCancelled;   // attacking drops sprint until Ctrl is pressed again
    private string currentDodge = "";   // "" = not dodging
    private float dodgeTimer;
    private float dodgeDuration;
    private Quaternion dodgeBaseRotation;
    private bool airDodgeUsed;          // one air dodge (and its hop) per trip through the air
    private float specialPressTime = -999f;
    private float lastHitTime = -999f;
    private float dodgeStartTime = -999f;
    private string lastFlinch = "-";
    private float lastFlameGain = -999f;
    private RoyFlameSword flameSword;
    private GameObject awakeningRoot;                    // the AwakeningSkin meshes, bound to Roy's skeleton
    private SkinnedMeshRenderer[] defaultRenderers;      // the default skin's meshes
    private bool skinApplied;
    private bool appliedAwakening;
    private int rollChain;              // 0 = none, 1 = first ground roll, 2 = second ground roll
    private bool rollQueued;            // Shift pressed during the first roll
    private Vector2 rollQueuedInput;
    private float rollEndTime = -999f;  // when the first roll finished
    private bool rollSprint;            // came out of the second roll: sprinting without holding Ctrl
    private int airUpAirCount;          // up airs chained off the air dodge this airtime (max 2)
    private bool airFinisherUsed;       // airborne up smash or Blazer used this airtime
    private float hoverTimer;           // > 0: gravity paused (the second up air's short levitation)
    private float dashEndTime = -999f; // when the last dash attack finished
    private bool dashFollowUpUsed;  // the forward smash after a dash attack only happens once
    private int secondaryStep = -1;     // current Double-Edge Dance hit, -1 = not in the string
    private bool secondaryQueued;
    private int juggleCount;            // up-air hops done in the current finisher
    private bool secondaryDownRoute;    // this string started from the dash attack
    private float height;
    private float verticalSpeed;
    private bool grounded = true;

    void Awake()
    {
        animator = GetComponent<Animator>();
        gestureLayer = animator.GetLayerIndex("Gesture, Override");
        flameSword = GetComponentInChildren<RoyFlameSword>(true);
        var awakening = transform.Find("AwakeningSkin");
        awakeningRoot = awakening != null ? awakening.gameObject : null;
        var list = new System.Collections.Generic.List<SkinnedMeshRenderer>();
        foreach (var r in GetComponentsInChildren<SkinnedMeshRenderer>(true))
            if (awakening == null || !r.transform.IsChildOf(awakening)) list.Add(r);
        defaultRenderers = list.ToArray();
        fullBodyLayer = animator.GetLayerIndex("Gesture, Full Body");
    }

    float ClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0.5f;
    }

    void StartAttack(string stateName)
    {
        attacking = true;
        currentAttack = stateName;
        attackTimer = 0f;
        // LandingAirN is landing lag: it plays at a fixed 1.5x in the controller and ignores attack speed.
        // LandingAirN plays at a fixed 1.5x; the special (Counter / Final Smash) ignores attack speed.
        if (stateName.StartsWith("DED")) AddFlame(flamePerSecondaryHit);
        if (stateName.StartsWith("CounterHit") || stateName == "FinalStart" || stateName == "FinalAirStart") AddFlame(flamePerCounter);

        bool fixedSpeed = stateName.StartsWith("Counter") || stateName.StartsWith("Final")
            || stateName.StartsWith("FlareBlade") || stateName == "DownAir";
        attackDuration = stateName == "LandingAirN" ? ClipLength(stateName) / 1.5f
            : fixedSpeed ? ClipLength(stateName) : ClipLength(stateName) / attackSpeed;
        bool fullBody = stateName == "Pummel" || stateName == "DashAttack" || stateName == "NAir" || stateName == "LandingAirN"
            || stateName.StartsWith("DED") || stateName == "UpAir" || stateName == "UpSmashAir" || stateName == "Blazer"
            || stateName.StartsWith("Counter") || stateName.StartsWith("Final")
            || stateName.StartsWith("FlareBlade") || stateName == "DownAir";
        animator.CrossFadeInFixedTime(stateName, 0.05f, fullBody ? fullBodyLayer : gestureLayer, 0f);
        // Clear the other layer so a cancelled pummel doesn't keep driving the legs (and vice versa).
        animator.CrossFadeInFixedTime("BufferEmpty", 0.1f, fullBody ? gestureLayer : fullBodyLayer);
    }

    bool IsSprinting()
    {
        bool moving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;
        if (!moving) rollSprint = false;                     // stopping ends the roll-out sprint
        return moving && (Input.GetKey(KeyCode.LeftControl) || rollSprint) && !sprintCancelled;
    }

    // Sprint + click: the dash attack. Like Risk of Rain 2, attacking ends the sprint;
    // Roy goes back to walking until Ctrl is pressed again.
    void StartDashAttack()
    {
        sprintCancelled = true;
        dashEndTime = float.MaxValue;   // still dashing: a click now is always in time
        dashFollowUpUsed = false;
        comboStep = -1;
        holdLooping = false;
        clickQueued = false;
        StartAttack("DashAttack");
    }

    // A click during the dash attack, or within dashFollowUpWindow after it ends, is the forward smash.
    bool DashFollowUpAvailable()
    {
        return !dashFollowUpUsed && Time.time - dashEndTime <= dashFollowUpWindow;
    }

    void StartDashFollowUp()
    {
        dashFollowUpUsed = true;
        clickQueued = false;
        StartAttack("FSmash");
    }

    // Click while airborne: neutral air. Ends any ground combo.
    void StartAerial()
    {
        comboStep = -1;
        holdLooping = false;
        clickQueued = false;
        StartAttack("NAir");
    }

    void StopAttacking()
    {
        attacking = false;      // the layers return to BufferEmpty on their own
        holdLooping = false;
        currentAttack = "-";
    }

    void NextComboHit()
    {
        bool expired = comboStep < 0 || comboStep >= comboString.Length - 1 || Time.time - comboStartTime > comboWindow;
        if (expired)
        {
            comboStep = 0;
            comboStartTime = Time.time;
        }
        else
        {
            comboStep++;
        }
        holdLooping = false;
        clickQueued = false;
        StartAttack(comboString[comboStep]);
    }

    // Click = next hit of Pummel -> Jab1 -> FSmash, as long as all three clicks land within
    // comboWindow seconds of the first one (otherwise the string starts over at Pummel).
    // A single tap is just the Pummel. Holding the button after the Pummel loops Jab1 / FTilt.
    void UpdateAttacks()
    {
        bool pressed = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J);
        bool held = Input.GetMouseButton(0) || Input.GetKey(KeyCode.J);
        bool secondaryPressed = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K);

        if (UpdateSecondary(secondaryPressed)) return;

        if (!attacking)
        {
            if (!pressed) return;
            if (!grounded) StartAerial();
            else if (IsSprinting()) StartDashAttack();
            else if (DashFollowUpAvailable()) StartDashFollowUp();
            else NextComboHit();
            return;
        }

        if (pressed) clickQueued = true;
        attackTimer += Time.deltaTime;
        float progress = attackTimer / attackDuration;

        // Aerials live inside the jump/fall loop: touching the ground mid-attack cuts into landing lag,
        // and a held or queued click while still airborne repeats the aerial.
        if (currentAttack == "NAir")
        {
            if (grounded) { clickQueued = false; StartAttack("LandingAirN"); }
            else if (progress >= 1f)
            {
                if (held || clickQueued) StartAerial();
                else StopAttacking();
            }
            return;
        }
        // Air-dodge follow-ups play out in the air; landing cuts them off into the normal landing.
        if (currentAttack == "UpAir" || currentAttack == "UpSmashAir" || currentAttack == "Blazer")
        {
            if (grounded && attackTimer > 0.1f) { animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer); StopAttacking(); }
            else if (progress >= 1f) StopAttacking();
            return;
        }
        // Ground attacks don't carry into a jump: leaving the ground cancels them, and a click that was
        // already queued (spam-clicking) becomes the neutral air instead of the next ground hit.
        if (!grounded && currentAttack != "NAir" && currentAttack != "LandingAirN")
        {
            animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, gestureLayer);
            animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer);
            bool queued = clickQueued || pressed;
            StopAttacking();
            comboStep = -1;
            if (queued) StartAerial();
            return;
        }

        if (currentAttack == "LandingAirN")
        {
            if (!grounded)
            {
                // Jumped out of the landing lag: drop it so it doesn't play in the air.
                animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer);
                StopAttacking();
            }
            else if (clickQueued && progress >= 0.5f)
            {
                NextComboHit();         // spam-clicking goes straight into the ground string
            }
            else if (progress >= 1f)
            {
                StopAttacking();
            }
            return;
        }

        if (clickQueued && progress >= comboCancelPoint)
        {
            if (currentAttack == "DashAttack") StartDashFollowUp();
            else if (currentAttack == "FSmash" && dashFollowUpUsed && comboStep < 0) { clickQueued = false; }  // dash string is over
            else NextComboHit();
            return;
        }

        if (progress >= 1f)
        {
            if (currentAttack == "DashAttack") dashEndTime = Time.time;
            bool dashString = currentAttack == "DashAttack" || (currentAttack == "FSmash" && comboStep < 0);
            if (held && !clickQueued && !dashString)
            {
                // Holding: Jab1 and FTilt back and forth. This ends the click combo.
                holdStep = holdLooping ? (holdStep + 1) % holdLoop.Length : 0;
                holdLooping = true;
                comboStep = -1;
                StartAttack(holdLoop[holdStep]);
            }
            else
            {
                attacking = false;      // the layers return to BufferEmpty on their own
                holdLooping = false;
                currentAttack = "-";
            }
        }
    }

    // Right click (or K) on the ground: Double-Edge Dance. Each right click during a hit queues the next
    // one (up to four); with no further clicks the string ends. It takes priority over the primary,
    // cancels sprint like any attack, and leaving the ground cancels it. Returns true while it owns input.
    bool UpdateSecondary(bool secondaryPressed)
    {
        if (secondaryStep >= 0 && !(attacking && (currentAttack.StartsWith("DED") || currentAttack == "UpAir"))) secondaryStep = -1;

        if (secondaryStep < 0)
        {
            // Dash attack -> right click: DED3Down then (another right click) DED4Down. A right click during
            // the dash waits for the dash's cancel point; after the dash it uses the dash follow-up window.
            bool inDash = attacking && currentAttack == "DashAttack";
            if (inDash || DashFollowUpAvailable())
            {
                if (secondaryPressed) secondaryQueued = true;
                bool ready = !inDash || attackTimer / attackDuration >= comboCancelPoint;
                if (secondaryQueued && ready && grounded)
                {
                    dashFollowUpUsed = true;
                    comboStep = -1;
                    clickQueued = false;
                    secondaryQueued = false;
                    secondaryDownRoute = true;
                    secondaryStep = 2;
                    StartAttack(secondaryDownString[2]);
                    return true;
                }
                if (inDash) return false;   // let the dash keep running
            }
            secondaryDownRoute = false;
            if (!secondaryPressed || !grounded) return false;
            sprintCancelled = true;
            comboStep = -1;
            holdLooping = false;
            clickQueued = false;
            secondaryQueued = false;
            secondaryStep = 0;
            StartAttack(secondaryString[0]);
            return true;
        }

        if (secondaryPressed) secondaryQueued = true;
        attackTimer += Time.deltaTime;
        float progress = attackTimer / attackDuration;

        if (currentAttack == "UpAir")
        {
            // Juggle finisher: hop again partway through each up air until the hops run out,
            // then fall back down. Landing ends it and the normal landing plays.
            if (grounded && attackTimer > 0.1f) { animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer); StopAttacking(); secondaryStep = -1; }
            else if (juggleCount < juggleHops && progress >= juggleNextPoint) JuggleHop();
            else if (progress >= 1f) { StopAttacking(); secondaryStep = -1; }
            return true;
        }

        if (!grounded)
        {
            animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer);
            StopAttacking();
            secondaryStep = -1;
        }
        else if (secondaryQueued && progress >= secondaryCancelPoint && secondaryStep < secondaryString.Length - 1)
        {
            secondaryStep++;
            secondaryQueued = false;
            bool up = Input.GetKey(KeyCode.Space);
            string next = secondaryDownRoute ? secondaryDownString[secondaryStep]
                : up ? secondaryUpString[secondaryStep] : secondaryString[secondaryStep];
            if (next == "UpAir") { juggleCount = 0; JuggleHop(); }
            else StartAttack(next);
        }
        else if (progress >= 1f)
        {
            StopAttacking();
            secondaryStep = -1;
        }
        return true;
    }

    static bool IsGroundRoll(string dodge) { return dodge == "Roll" || dodge == "RollMirrored" || dodge == "RollBack"; }

    void StartDodge(Vector2 input)
    {
        string dodge;
        float sideYaw = 0f;
        if (!grounded)
        {
            dodge = "AirDodge";
            // Hop: jump height goes with the square of launch speed, so 5/8 of the height = sqrt(5/8) of the speed.
            verticalSpeed = jumpVelocity * Mathf.Sqrt(airDodgeHeightFraction);
            airDodgeUsed = true;
        }
        else if (input.y < -0.5f) dodge = "RollBack";
        else if (Mathf.Abs(input.x) > 0.5f && Mathf.Abs(input.y) <= 0.5f)
        {
            // Side rolls spin opposite ways: one side uses the left-right mirrored roll.
            bool right = input.x > 0;
            dodge = right != swapSideRollSpin ? "Roll" : "RollMirrored";
            sideYaw = right ? 90f : -90f;
        }
        else if (input.y > 0.5f) dodge = "Roll";
        else dodge = "SpotDodge";

        // cancel whatever attack was running
        StopAttacking();
        comboStep = -1;
        secondaryStep = -1;
        clickQueued = false;
        secondaryQueued = false;
        animator.CrossFadeInFixedTime("BufferEmpty", 0.05f, gestureLayer);
        animator.CrossFadeInFixedTime(dodge, 0.05f, fullBodyLayer, 0f);

        // Side roll: for this preview, turn Roy toward the roll. In ROR2 the character's facing
        // follows the movement direction during the roll instead.
        dodgeBaseRotation = transform.rotation;
        transform.rotation = dodgeBaseRotation * Quaternion.Euler(0f, sideYaw, 0f);

        currentDodge = dodge;
        dodgeStartTime = Time.time;
        dodgeTimer = 0f;
        dodgeDuration = ClipLength(dodge);
    }

    // Utility (Shift): which dodge depends on Roy's state and the movement keys. It cancels any attack.
    // Dodges ignore attack speed. Returns true while a dodge owns the character.
    bool UpdateDodge(Vector2 input)
    {
        bool shift = Input.GetKeyDown(KeyCode.LeftShift);
        if (currentDodge == "")
        {
            if (!shift) return false;
            if (Time.time - lastHitTime <= perfectDodgeWindow) { StartPerfectDodgeAttack(); return true; }
            if (!grounded && airDodgeUsed) return false;
            bool chained = grounded && rollChain == 1 && Time.time - rollEndTime <= rollChainWindow;
            StartDodge(chained && input.sqrMagnitude < 0.01f ? Vector2.up : input);   // second one is always a roll
            rollChain = IsGroundRoll(currentDodge) ? (chained ? 2 : 1) : 0;
            return true;
        }

        // Hit right as the dodge (first or second) started, or a utility press right as a hit lands: perfect dodge
        bool hitNow = lastHitTime == Time.time;
        if ((hitNow && Time.time - dodgeStartTime <= perfectDodgeWindow) || (shift && Time.time - lastHitTime <= perfectDodgeWindow))
        {
            transform.rotation = dodgeBaseRotation;
            currentDodge = "";
            StartPerfectDodgeAttack();
            return true;
        }

        // Shift during the first roll: line up the second one (direction read now)
        if (shift && rollChain == 1 && IsGroundRoll(currentDodge)) { rollQueued = true; rollQueuedInput = input; }

        dodgeTimer += Time.deltaTime;
        bool landedFromAirDodge = currentDodge == "AirDodge" && grounded && dodgeTimer > 0.1f;
        if (dodgeTimer >= dodgeDuration || landedFromAirDodge)
        {
            if (landedFromAirDodge) animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer);
            transform.rotation = dodgeBaseRotation;
            currentDodge = "";
            if (rollChain == 1 && rollQueued)
            {
                rollQueued = false;
                StartDodge(rollQueuedInput.sqrMagnitude < 0.01f ? Vector2.up : rollQueuedInput);
                rollChain = 2;
                return true;
            }
            if (rollChain == 2)
            {
                // out of the second roll and straight into a sprint
                rollChain = 0;
                sprintCancelled = false;
                rollSprint = true;
            }
            else if (rollChain == 1) rollEndTime = Time.time;
            return false;
        }
        return true;
    }

    // After an air dodge (during it or later in the same airtime):
    //   left click  = up air (5/8 lift); a second left click = another up air that only hovers briefly
    //   right click = airborne up smash (2/3 lift), or Blazer (big lift) if an up air came first
    // One finisher (up smash or Blazer) per airtime. Returns true on the frame an attack starts.
    bool TryAirDodgeFollowUp()
    {
        if (grounded || !airDodgeUsed || airFinisherUsed) return false;
        bool left = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J);
        bool right = Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K);
        if (right) { StartAirDodgeFollowUp(airUpAirCount > 0 ? "Blazer" : "UpSmashAir"); return true; }
        if (left && airUpAirCount < 2) { StartAirDodgeFollowUp("UpAir"); return true; }
        return false;
    }

    void StartAirDodgeFollowUp(string attack)
    {
        if (currentDodge != "") { transform.rotation = dodgeBaseRotation; currentDodge = ""; }
        StopAttacking();
        comboStep = -1;
        secondaryStep = -1;
        clickQueued = false;
        secondaryQueued = false;
        hoverTimer = 0f;
        if (attack == "UpAir")
        {
            airUpAirCount++;
            if (airUpAirCount == 1) verticalSpeed = jumpVelocity * Mathf.Sqrt(airDodgeHeightFraction);
            else { verticalSpeed = 0f; hoverTimer = secondUpAirHoverTime; }   // second one: hover, no lift
        }
        else
        {
            airFinisherUsed = true;
            float heightFraction = attack == "Blazer" ? blazerHeightFraction : upSmashAirHeightFraction;
            verticalSpeed = jumpVelocity * Mathf.Sqrt(heightFraction);
        }
        StartAttack(attack);
    }

    bool InSpecial()
    {
        return attacking && (currentAttack.StartsWith("Counter") || currentAttack.StartsWith("Final")
            || currentAttack.StartsWith("FlareBlade") || currentAttack == "DownAir");
    }

    // Perfect dodge payoff: Flare Blade (start, then the swing) on the ground, levitating down air in the air.
    void StartPerfectDodgeAttack()
    {
        AddFlame(flamePerPerfectDodge);
        SpawnFireZone(firePillarPrefab);
        rollChain = 0;
        rollQueued = false;
        if (grounded) StartSpecialAttack("FlareBladeStart", "FlareBladeStart");
        else
        {
            StartSpecialAttack("DownAir", "DownAir");
            verticalSpeed = 0f;
            hoverTimer = perfectDodgeHoverTime;
        }
    }

    void StartSpecialAttack(string ground, string air)
    {
        if (currentDodge != "") { transform.rotation = dodgeBaseRotation; currentDodge = ""; }
        StopAttacking();
        comboStep = -1;
        secondaryStep = -1;
        clickQueued = false;
        secondaryQueued = false;
        animator.CrossFadeInFixedTime("BufferEmpty", 0.05f, gestureLayer);
        StartAttack(grounded ? ground : air);
    }

    // Special (R) = Counter. Returns true while the special owns the character.
    bool UpdateSpecial()
    {
        bool pressed = Input.GetKeyDown(KeyCode.R);
        bool hit = Input.GetKeyDown(KeyCode.H);
        float now = Time.time;
        if (hit) lastHitTime = now;

        if (!InSpecial())
        {
            if (!pressed) { if (hit) PlayFlinch(); return false; }
            specialPressTime = now;
            // hit landed just before the press: perfect timing straight into the Final Smash
            if (now - lastHitTime <= perfectCounterWindow) { SpawnFireZone(fireTornadoPrefab); StartSpecialAttack("FinalStart", "FinalAirStart"); }
            else StartSpecialAttack("Counter", "CounterAir");
            return true;
        }

        attackTimer += Time.deltaTime;
        float progress = attackTimer / attackDuration;

        if (currentAttack.StartsWith("Counter") && !currentAttack.StartsWith("CounterHit") && hit)
        {
            float sincePress = now - specialPressTime;
            if (sincePress <= perfectCounterWindow) { SpawnFireZone(fireTornadoPrefab); StartSpecialAttack("FinalStart", "FinalAirStart"); }   // perfect
            else if (attackTimer >= counterWindowStart && attackTimer <= counterWindowEnd)
                StartSpecialAttack("CounterHit", "CounterHitAir");                                       // counter
            else PlayFlinch();                                                                       // whiffed: hit lands
            return true;
        }

        if (currentAttack == "DownAir" && grounded && attackTimer > 0.1f)
        {
            animator.CrossFadeInFixedTime("BufferEmpty", 0.08f, fullBodyLayer);
            StopAttacking();
            return true;
        }

        if (progress >= 1f)
        {
            if (currentAttack == "FlareBladeStart") StartAttack("FlareBlade");
            else if (currentAttack == "FinalStart") StartAttack("Final");
            else if (currentAttack == "FinalAirStart") StartAttack("FinalAir");
            else StopAttacking();
        }
        return true;
    }

    // Getting hit (H): one of three Smash hit reactions, picked at random (air versions in the air).
    // Standing still and not attacking: the whole body flinches. Moving or mid-attack: only the
    // upper body, on the masked Flinch layer, so the legs and the attack keep going (like ROR2).
    // Dodges ignore hits.
    void PlayFlinch()
    {
        if (currentDodge != "") return;
        string flinch = (grounded ? "Flinch" : "FlinchAir") + Random.Range(1, 4);
        bool moving = Mathf.Abs(Input.GetAxisRaw("Horizontal")) + Mathf.Abs(Input.GetAxisRaw("Vertical")) > 0.01f;
        if (!attacking && !moving && grounded) animator.CrossFadeInFixedTime(flinch, 0.03f, fullBodyLayer, 0f);
        else animator.CrossFadeInFixedTime(flinch, 0.03f, animator.GetLayerIndex("Flinch"), 0f);
        lastFlinch = flinch;
    }

    // Spawns on the ground under Roy (he may be in the air), sized by the current flame meter.
    void SpawnFireZone(GameObject prefab)
    {
        if (prefab == null) return;
        var spot = new Vector3(transform.position.x, transform.position.y - height, transform.position.z);
        var zone = Instantiate(prefab, spot, Quaternion.identity).GetComponent<RoyFireZone>();
        if (zone != null)
        {
            zone.level = flameMeter / 100f;
            zone.blue = awakeningSkin;
        }
    }

    void AddFlame(float amount)
    {
        flameMeter = Mathf.Clamp(flameMeter + amount, 0f, 100f);
        lastFlameGain = Time.time;
    }

    // Swap skins like a ROR2 SkinDef would: hide one set of meshes, show the other, recolour the fire.
    void UpdateSkin()
    {
        if (Input.GetKeyDown(KeyCode.T)) awakeningSkin = !awakeningSkin;
        if (skinApplied && appliedAwakening == awakeningSkin) return;
        skinApplied = true;
        appliedAwakening = awakeningSkin;
        if (awakeningRoot != null) awakeningRoot.SetActive(awakeningSkin);
        foreach (var r in defaultRenderers) if (r != null) r.enabled = !awakeningSkin;
        if (flameSword != null) flameSword.blueFlames = awakeningSkin;
    }

    void UpdateFlame()
    {
        UpdateSkin();
        if (Input.GetKeyDown(KeyCode.F)) AddFlame(25f);   // test key: add flame directly
        if (Time.time - lastFlameGain > flameDecayDelay)
            flameMeter = Mathf.Max(0f, flameMeter - flameDecayPerSecond * Time.deltaTime);
        if (flameSword != null)
        {
            flameSword.intensity = flameMeter / 100f;
            flameSword.attacking = attacking;   // swing trail only during attacks
        }
    }

    // One juggle hop: jump (each hop a fraction of the last one's height) and up air.
    // Jump height goes with the square of the launch speed, hence the square root.
    void JuggleHop()
    {
        float heightScale = Mathf.Pow(juggleHeightMultiplier, juggleCount);
        verticalSpeed = jumpVelocity * Mathf.Sqrt(heightScale);
        grounded = false;
        juggleCount++;
        StartAttack("UpAir");
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (input.sqrMagnitude > 1f) input.Normalize();
        bool moving = input.sqrMagnitude > 0.01f;

        // During Double-Edge Dance, jump is the "up" modifier instead of a jump; no jumping mid-dodge either.
        if (grounded && Input.GetKeyDown(KeyCode.Space) && secondaryStep < 0 && currentDodge == "")
        {
            grounded = false;
            verticalSpeed = jumpVelocity;
        }
        if (!grounded && hoverTimer > 0f)
        {
            hoverTimer -= Time.deltaTime;   // levitating: no gravity, no rise
            verticalSpeed = 0f;
        }
        else if (!grounded)
        {
            verticalSpeed -= gravity * Time.deltaTime;
            height += verticalSpeed * Time.deltaTime;
            if (height <= 0f)
            {
                height = 0f;
                verticalSpeed = 0f;
                grounded = true;
                airDodgeUsed = false;
                airUpAirCount = 0;
                airFinisherUsed = false;
                hoverTimer = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.KeypadPlus)) attackSpeed += 0.25f;
        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) attackSpeed = Mathf.Max(0.25f, attackSpeed - 0.25f);

        animator.SetFloat("rightSpeed", input.x);
        animator.SetFloat("forwardSpeed", input.y);
        animator.SetBool("isMoving", moving);
        if (Input.GetKeyDown(KeyCode.LeftControl)) sprintCancelled = false;   // pressing sprint again resumes it
        if (sprintCancelled) rollSprint = false;                               // attacking ends the roll-out sprint
        animator.SetBool("isSprinting", IsSprinting());
        animator.SetBool("isGrounded", grounded);
        animator.SetFloat("upSpeed", verticalSpeed);
        animator.SetFloat("Slash.playbackRate", attackSpeed);

        if (!UpdateSpecial() && !TryAirDodgeFollowUp() && !UpdateDodge(input)) UpdateAttacks();
        UpdateFlame();

        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, height, p.z);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 900, 220),
            "WASD move, Ctrl sprint, Space jump, Shift dodge (still = spot dodge, S = back roll, W/A/D = roll, air = air dodge)\n" +
            "Click/J: tap = Pummel, hold = Pummel then Jab/FTilt loop, 3 clicks within " + comboWindow.ToString("0.#") + "s = Pummel > Jab > FSmash\n" +
            "Sprint + click = DashAttack (ends sprint), click again within " + dashFollowUpWindow.ToString("0.0#") + "s of it ending = FSmash, or right click = DED3 Down > DED4 Down\n" +
            "Click while airborne = Neutral Air (hold to repeat; landing mid-attack = landing lag)\n" +
            "Right click/K in succession = Double-Edge Dance (up to 4 hits); hold Space during it for the upward hits\n" +
            "R = Counter; H = simulate an enemy hit (in window = counterattack, right as you press R = Final Smash)\n" +
            "Shift right as H lands = perfect dodge (Flare Blade on the ground, hovering down air in the air)\n" +
            "+/- attack speed (" + attackSpeed.ToString("0.00") + "x)   current attack: " + currentAttack + "   last flinch: " + lastFlinch
            + "   skin (T): " + (awakeningSkin ? "Awakening" : "Default") + "\n" +
            "Flame meter: " + flameMeter.ToString("0") + "/100  (F = +25)   damage x" + (1f + flameMaxDamageBonus * flameMeter / 100f).ToString("0.00")
            + (flameMeter >= enflamedThreshold ? "   ENFLAMED" : ""));
    }
}
