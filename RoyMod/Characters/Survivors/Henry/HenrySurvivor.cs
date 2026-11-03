using BepInEx.Configuration;
using HenryMod.Modules;
using HenryMod.Modules.Characters;
using HenryMod.Survivors.Henry.Components;
using HenryMod.Survivors.Henry.SkillStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Survivors.Henry
{
    public class HenrySurvivor : SurvivorBase<HenrySurvivor>
    {
        //used to load the assetbundle for this character. must be unique
        public override string assetBundleName => "roybundle"; //if you do not change this, you are giving permission to deprecate the mod

        //the name of the prefab we will create. conventionally ending in "Body". must be unique
        public override string bodyName => "RoyBody"; //if you do not change this, you get the point by now

        //name of the ai master for vengeance and goobo. must be unique
        public override string masterName => "RoyMonsterMaster"; //if you do not

        //the names of the prefabs you set up in unity that we will use to build your character
        public override string modelPrefabName => "mdlRoy";
        public override string displayPrefabName => "RoyDisplay";

        public const string HENRY_PREFIX = HenryPlugin.DEVELOPER_PREFIX + "_ROY_";

        //used when registering your survivor's language tokens
        public override string survivorTokenPrefix => HENRY_PREFIX;
        
        public override BodyInfo bodyInfo => new BodyInfo
        {
            bodyName = bodyName,
            bodyNameToken = HENRY_PREFIX + "NAME",
            subtitleNameToken = HENRY_PREFIX + "SUBTITLE",

            characterPortrait = assetBundle.LoadAsset<Texture>("texRoyIcon"),
            bodyColor = new Color(0.85f, 0.2f, 0.15f),
            sortPosition = 100,

            crosshair = Asset.LoadCrosshair("Standard"),
            podPrefab = null,  // Roy warps in with his Smash entrance (RoySpawnState) instead of a drop pod

            maxHealth = 110f,
            healthRegen = 1.1f,
            armor = 15f,   // (Alex) a little sturdier, since airborne enemies are harder for him to reach

            jumpCount = 1,
        };

        public const int ROY_RENDERER_COUNT = 54;

        // every renderer on mdlRoy is in its ChildLocator as RoyMesh0..RoyMesh16 (see the Unity prefab builder)
        public override CustomRendererInfo[] customRendererInfos
        {
            get
            {
                var infos = new CustomRendererInfo[ROY_RENDERER_COUNT];
                for (int i = 0; i < infos.Length; i++)
                    infos[i] = new CustomRendererInfo { childName = "RoyMesh" + i };
                return infos;
            }
        }

        public override UnlockableDef characterUnlockableDef => HenryUnlockables.characterUnlockableDef;
        
        public override ItemDisplaysBase itemDisplays => new HenryItemDisplays();

        //set in base classes
        public override AssetBundle assetBundle { get; protected set; }

        public override GameObject bodyPrefab { get; protected set; }
        public override CharacterBody prefabCharacterBody { get; protected set; }
        public override GameObject characterModelObject { get; protected set; }
        public override CharacterModel prefabCharacterModel { get; protected set; }
        public override GameObject displayPrefab { get; protected set; }

        public override void Initialize()
        {
            //uncomment if you have multiple characters
            //ConfigEntry<bool> characterEnabled = Config.CharacterEnableConfig("Survivors", "Henry");

            //if (!characterEnabled.Value)
            //    return;

            base.Initialize();
        }

        public override void InitializeCharacter()
        {
            //need the character unlockable before you initialize the survivordef
            HenryUnlockables.Init();

            base.InitializeCharacter();

            HenryConfig.Init();
            HenryStates.Init();
            RoyController.Init();
            HenryTokens.Init();

            HenryAssets.Init(assetBundle);
            HenryBuffs.Init(assetBundle);

            InitializeEntityStateMachines();
            InitializeSkills();
            InitializeSkins();
            InitializeCharacterMaster();

            AdditionalBodySetup();

            AddHooks();
        }

        // Alex's drawn icon from RoyMod\Icons (embedded in the dll) when there is one, otherwise the template's placeholder
        private Sprite SkillIcon(string drawnIcon, string placeholder)
        {
            return RoyIcons.Has(drawnIcon + ".png") ? RoyIcons.LoadSprite(drawnIcon + ".png") : assetBundle.LoadAsset<Sprite>(placeholder);
        }

        private void AdditionalBodySetup()
        {
            AddHitboxes();
            bodyPrefab.AddComponent<HenryWeaponComponent>();
            bodyPrefab.AddComponent<RoyController>();
            // Alex's drawn portrait (character select, HUD, logbook)
            Texture2D portrait = RoyIcons.LoadTexture("texRoyIcon.png");
            if (portrait) prefabCharacterBody.portraitIcon = portrait;
            // Smash jump and landing sounds (the game plays the body's SfxLocator jumpSound / landingSound)
            SfxLocator sfx = bodyPrefab.GetComponent<SfxLocator>();
            if (!sfx) sfx = bodyPrefab.AddComponent<SfxLocator>();
            sfx.jumpSound = "Play_se_roy_jump02";
            sfx.landingSound = "Play_se_roy_landing01";
            // Fire Emblem style death instead of a ragdoll
            bodyPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(RoyDeathState));
            RoySpawnState.warpEffectPrefab = assetBundle.LoadAsset<GameObject>("RoyEntryWarp");
            RoySpawnState.warpEffectPrefabBlue = assetBundle.LoadAsset<GameObject>("RoyEntryWarpBlue");
            RoyFireZone.pillarPrefab = assetBundle.LoadAsset<GameObject>("RoyFirePillar");
            RoyFireZone.tornadoPrefab = assetBundle.LoadAsset<GameObject>("RoyFireTornado");
            RoySlashEffect.Init();
            characterModelObject.AddComponent<RoyFace>();
            characterModelObject.AddComponent<RoyHeadband>();
            // the flame sword goes onto each spawned Roy (RoyController attaches it); a child instantiated into the bundle's
            // model asset at load time didn't carry over to the spawned bodies
            RoyFlameSword.prefab = assetBundle.LoadAsset<GameObject>("RoyFlameSword");
            RoySound.Load();
            RoyMeterHud.backSprite = assetBundle.LoadAsset<Sprite>("RoyMeterBack");
            RoyMeterHud.outlineSprite = assetBundle.LoadAsset<Sprite>("RoyMeterOutline");
            RoyMeterHud.maskSprite = assetBundle.LoadAsset<Sprite>("RoyMeterMask");
            RoyMeterHud.flameTexture = assetBundle.LoadAsset<Texture2D>("FlameArc");
            RoyMeterHud.flameTextureBlue = assetBundle.LoadAsset<Texture2D>("FlameArcBlue");
            prefabCharacterBody._defaultCrosshairPrefab = RoyCrosshair.Create(assetBundle.LoadAsset<Sprite>("RoyReticle"));
            RoyFlameSword.trailMaterialRedAsset = assetBundle.LoadAsset<Material>("FlameArc");
            RoyFlameSword.trailMaterialBlueAsset = assetBundle.LoadAsset<Material>("FlameArcBlue");
            if (displayPrefab)
            {
                displayPrefab.AddComponent<RoyFace>();
                displayPrefab.AddComponent<RoyHeadband>();
                displayPrefab.AddComponent<RoyDisplayEntrance>();
            }
            //bodyPrefab.AddComponent<HuntressTrackerComopnent>();
            //anything else here
        }

        public void AddHitboxes()
        {
            //example of how to create a HitBoxGroup. see summary for more details
            Prefabs.SetupHitBoxGroup(characterModelObject, "SwordGroup", "SwordHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "PummelGroup", "PummelHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "JabGroup", "JabHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "FTiltGroup", "FTiltHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "FSmashGroup", "FSmashHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DashGroup", "DashHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED1Group", "DED1Hitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED2Group", "DED2Hitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED3Group", "DED3Hitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED4Group", "DED4Hitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED2UpGroup", "DED2UpHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED3UpGroup", "DED3UpHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED3DownGroup", "DED3DownHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "DED4DownGroup", "DED4DownHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "UpAirGroup", "UpAirHitbox");
            Prefabs.SetupHitBoxGroup(characterModelObject, "JavelinGroup", "JavelinHitbox");
        }

        public override void InitializeEntityStateMachines() 
        {
            //clear existing state machines from your cloned body (probably commando)
            //omit all this if you want to just keep theirs
            Prefabs.ClearEntityStateMachines(bodyPrefab);

            //the main "Body" state machine has some special properties
            Prefabs.AddMainEntityStateMachine(bodyPrefab, "Body", typeof(EntityStates.GenericCharacterMain), typeof(RoySpawnState));
            bodyPrefab.GetComponent<CharacterBody>().preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(RoySpawnState));
            //if you set up a custom main characterstate, set it up here
                //don't forget to register custom entitystates in your HenryStates.cs

            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon");
            Prefabs.AddEntityStateMachine(bodyPrefab, "Weapon2");
        }

        #region skills
        public override void InitializeSkills()
        {
            //remove the genericskills from the commando body we cloned
            Skills.ClearGenericSkills(bodyPrefab);
            //add our own
            AddPassiveSkill();
            AddPrimarySkills();
            AddSecondarySkills();
            AddUtiitySkills();
            AddSpecialSkills();
        }

        // the Flame Meter passive: shown in the skills list on character select and in the logbook (it works through
        // RoyController, not as an activatable skill)
        private void AddPassiveSkill()
        {
            bodyPrefab.GetComponent<SkillLocator>().passiveSkill = new SkillLocator.PassiveSkill
            {
                enabled = true,
                skillNameToken = HENRY_PREFIX + "PASSIVE_NAME",
                skillDescriptionToken = HENRY_PREFIX + "PASSIVE_DESCRIPTION",
                icon = SkillIcon("texRoyPassiveIcon", "texPassiveIcon"),
            };
        }

        private void AddPrimarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Primary);
            SkillDef skillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RoyPrimary",
                skillNameToken = HENRY_PREFIX + "PRIMARY_NAME",
                skillDescriptionToken = HENRY_PREFIX + "PRIMARY_DESCRIPTION",
                skillIcon = SkillIcon("texRoyPrimaryIcon", "texPrimaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RoyPrimary)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Any,
                baseRechargeInterval = 0f, baseMaxStock = 1, rechargeStock = 1, requiredStock = 0, stockToConsume = 0,
                mustKeyPress = false, isCombatSkill = true,
                canceledFromSprinting = false, cancelSprintingOnActivation = false, forceSprintDuringState = false,
            });
            Skills.AddPrimarySkills(bodyPrefab, skillDef);
        }

        private void AddSecondarySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Secondary);
            SkillDef skillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RoyDoubleEdgeDance",
                skillNameToken = HENRY_PREFIX + "SECONDARY_NAME",
                skillDescriptionToken = HENRY_PREFIX + "SECONDARY_DESCRIPTION",
                skillIcon = SkillIcon("texRoySecondaryIcon", "texSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RoyDoubleEdgeDance)),
                activationStateMachineName = "Weapon",
                interruptPriority = EntityStates.InterruptPriority.Skill,
                baseRechargeInterval = 2.5f, baseMaxStock = 1, rechargeStock = 1, requiredStock = 1, stockToConsume = 1,
                mustKeyPress = true, isCombatSkill = true,
                canceledFromSprinting = false, cancelSprintingOnActivation = true, forceSprintDuringState = false,
            });
            Skills.AddSecondarySkills(bodyPrefab, skillDef);
        }

        private void AddUtiitySkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Utility);
            SkillDef skillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RoyDodge",
                skillNameToken = HENRY_PREFIX + "UTILITY_NAME",
                skillDescriptionToken = HENRY_PREFIX + "UTILITY_DESCRIPTION",
                skillIcon = SkillIcon("texRoyUtilityIcon", "texUtilityIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RoyDodge)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                baseRechargeInterval = 6f, baseMaxStock = 1, rechargeStock = 1, requiredStock = 1, stockToConsume = 1,
                mustKeyPress = true, isCombatSkill = false,
                canceledFromSprinting = false, cancelSprintingOnActivation = true, forceSprintDuringState = false,
            });
            Skills.AddUtilitySkills(bodyPrefab, skillDef);
        }

        private void AddSpecialSkills()
        {
            Skills.CreateGenericSkillWithSkillFamily(bodyPrefab, SkillSlot.Special);
            SkillDef skillDef = Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "RoyCounter",
                skillNameToken = HENRY_PREFIX + "SPECIAL_NAME",
                skillDescriptionToken = HENRY_PREFIX + "SPECIAL_DESCRIPTION",
                skillIcon = SkillIcon("texRoySpecialIcon", "texSpecialIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(RoyCounter)),
                activationStateMachineName = "Body",
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                baseRechargeInterval = 7.5f, baseMaxStock = 1, rechargeStock = 1, requiredStock = 1, stockToConsume = 1,
                mustKeyPress = true, isCombatSkill = true,
                canceledFromSprinting = false, cancelSprintingOnActivation = true, forceSprintDuringState = false,
            });
            Skills.AddSpecialSkills(bodyPrefab, skillDef);
        }
        #endregion skills
        
        #region skins
        public override void InitializeSkins()
        {
            ModelSkinController skinController = prefabCharacterModel.gameObject.AddComponent<ModelSkinController>();
            ChildLocator childLocator = prefabCharacterModel.GetComponent<ChildLocator>();

            CharacterModel.RendererInfo[] defaultRendererinfos = prefabCharacterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            // Roy's own body meshes vs the Awakening meshes (under "AwakeningSkin"); the face meshes (Roy_*) are shared and left to RoyFace
            List<GameObject> royBodyMeshes = new List<GameObject>();
            for (int i = 0; i < ROY_RENDERER_COUNT; i++)
            {
                Transform mesh = childLocator.FindChild("RoyMesh" + i);
                if (mesh && mesh.parent == prefabCharacterModel.transform && !mesh.name.StartsWith("Roy_")) royBodyMeshes.Add(mesh.gameObject);
            }
            GameObject awakeningMeshes = childLocator.FindChildGameObject("AwakeningSkin");
            SkinDef.GameObjectActivation[] Activations(bool awakening)
            {
                var activations = new List<SkinDef.GameObjectActivation>();
                foreach (GameObject mesh in royBodyMeshes) activations.Add(new SkinDef.GameObjectActivation { gameObject = mesh, shouldActivate = !awakening });
                if (awakeningMeshes) activations.Add(new SkinDef.GameObjectActivation { gameObject = awakeningMeshes, shouldActivate = awakening });
                return activations.ToArray();
            }

            #region DefaultSkin
            SkinDef defaultSkin = Skins.CreateSkinDef(HENRY_PREFIX + "DEFAULT_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texRoySkinDefault"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject);
            defaultSkin.gameObjectActivations = Activations(false);
            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            // Awakening: unlocked by beating the game or obliterating on Monsoon or harder as Roy (HenryMasteryAchievement)
            SkinDef awakeningSkin = Skins.CreateSkinDef(HENRY_PREFIX + "MASTERY_SKIN_NAME",
                assetBundle.LoadAsset<Sprite>("texRoySkinAwakening"),
                defaultRendererinfos,
                prefabCharacterModel.gameObject,
                HenryUnlockables.masterySkinUnlockableDef);
            awakeningSkin.gameObjectActivations = Activations(true);
            skins.Add(awakeningSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }
        #endregion skins

        //Character Master is what governs the AI of your character when it is not controlled by a player (artifact of vengeance, goobo)
        public override void InitializeCharacterMaster()
        {
            //you must only do one of these. adding duplicate masters breaks the game.

            //if you're lazy or prototyping you can simply copy the AI of a different character to be used
            //Modules.Prefabs.CloneDopplegangerMaster(bodyPrefab, masterName, "Merc");

            //how to set up AI in code
            HenryAI.Init(bodyPrefab, masterName);

            //how to load a master set up in unity, can be an empty gameobject with just AISkillDriver components
            //assetBundle.LoadMaster(bodyPrefab, masterName);
        }

        private void AddHooks()
        {
            R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args)
        {

            if (sender.HasBuff(HenryBuffs.armorBuff))
            {
                args.armorAdd += 300;
            }
        }
    }
}