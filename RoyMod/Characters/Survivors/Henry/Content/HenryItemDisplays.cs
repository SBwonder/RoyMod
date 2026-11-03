using HenryMod.Modules;
using HenryMod.Modules.Characters;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

/* for custom copy format in keb's helper
{childName},
                    {localPos}, 
                    {localAngles},
                    {localScale})
*/

namespace HenryMod.Survivors.Henry
{
    public class HenryItemDisplays : ItemDisplaysBase
    {
        // placements from Alex's ItemDisplayPlacementHelper session ("Roy item asset location values.txt");
        // items not listed there have no display on Roy
        protected override void SetItemDisplayRules(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules)
        {
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["AlienHead"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAlienHead"),
                    "Pelvis",
                    new Vector3(0.01892F, 0.1369F, 0.08874F),
                    new Vector3(283.3374F, 56.37542F, 312.2703F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PersonalShield"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayShieldGenerator"),
                    "Chest",
                    new Vector3(-0.04039F, 0.16675F, 0.22516F),
                    new Vector3(72.13412F, 348.8022F, 158.259F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["UtilitySkillMagazine"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                    "Sword",
                    new Vector3(0.00215F, -0.09263F, 0.0998F),
                    new Vector3(31.17208F, 357.0118F, 356.4418F),
                    new Vector3(0.25F, 0.25F, 0.25F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAfterburnerShoulderRing"),
                    "Sword",
                    new Vector3(0.00063F, 0.09556F, 0.10325F),
                    new Vector3(323.8289F, 1.95068F, 177.6062F),
                    new Vector3(0.25F, 0.25F, 0.25F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CommandMissile"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMissileRack"),
                    "Chest",
                    new Vector3(-0.09994F, 0.15441F, -0.19069F),
                    new Vector3(34.95193F, 202.4198F, 344.5745F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ArmorReductionOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWarhammer"),
                    "HandL",
                    new Vector3(0.00285F, 0.11827F, -0.13687F),
                    new Vector3(342.2709F, 187.5137F, 232.5264F),
                    new Vector3(0.15F, 0.15F, 0.15F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["AttackSpeedAndMoveSpeed"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayCoffee"),
                    "Pelvis",
                    new Vector3(-0.04049F, 0.05786F, -0.13246F),
                    new Vector3(355.3504F, 218.9011F, 352.7613F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["AttackSpeedOnCrit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    "Head",
                    new Vector3(0.05648F, 0.07856F, 0.00178F),
                    new Vector3(13.51092F, 87.03545F, 0.29495F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["AutoCastEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFossil"),
                    "Stomach",
                    new Vector3(0.0798F, -0.00172F, 0.09154F),
                    new Vector3(5.6656F, 322.9655F, 355.4875F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Bandolier"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBandolier"),
                    "UpperArmR",
                    new Vector3(0.01495F, 0.08874F, 0.01047F),
                    new Vector3(77.65093F, 253.5414F, 195.2223F),
                    new Vector3(0.17F, 0.05F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BarrierOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBrooch"),
                    "Sword",
                    new Vector3(0.02276F, -0.00145F, 0.11038F),
                    new Vector3(2.64214F, 4.63509F, 280.6386F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BarrierOnOverHeal"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAegis"),
                    "UpperArmR",
                    new Vector3(0.02377F, 0.0591F, 0.08787F),
                    new Vector3(79.8101F, 109.8748F, 272.6695F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Bear"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBear"),
                    "Pelvis",
                    new Vector3(0.08181F, -0.00532F, -0.13019F),
                    new Vector3(351.219F, 132.0761F, 354.8598F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BearVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBearVoid"),
                    "Pelvis",
                    new Vector3(0.08181F, -0.00532F, -0.13019F),
                    new Vector3(351.219F, 132.0761F, 354.8598F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExtraLife"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHippo"),
                    "Pelvis",
                    new Vector3(0.08181F, -0.00532F, -0.13019F),
                    new Vector3(351.219F, 132.0761F, 354.8598F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExtraLifeVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHippoVoid"),
                    "Pelvis",
                    new Vector3(0.08181F, -0.00532F, -0.13019F),
                    new Vector3(351.219F, 132.0761F, 354.8598F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BeetleGland"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBeetleGland"),
                    "HandL",
                    new Vector3(0.01623F, 0.09468F, -0.02301F),
                    new Vector3(14.79902F, 286.4529F, 81.20745F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Behemoth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBehemoth"),
                    "Chest",
                    new Vector3(-0.16677F, 0.11475F, 0.00055F),
                    new Vector3(349.327F, 219.3543F, 348.7558F),
                    new Vector3(0.07F, 0.07F, 0.07F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BleedOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTriTip"),
                    "SwordTip",
                    new Vector3(0.00242F, -0.00007F, -0.65644F),
                    new Vector3(0.42084F, 0.66435F, 0.06859F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BleedOnHitVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTriTipVoid"),
                    "Sword",
                    new Vector3(0.00923F, -0.00269F, 0.5364F),
                    new Vector3(5.7671F, 39.83488F, 80.57198F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BounceNearby"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHook"),
                    "Neck",
                    new Vector3(-0.02689F, 0.02812F, 0.05203F),
                    new Vector3(9.78648F, 328.7785F, 14.72145F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ChainLightning"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayUkulele"),
                    "ThighL",
                    new Vector3(-0.00044F, 0.12682F, 0.08305F),
                    new Vector3(354.8873F, 353.6878F, 173.8436F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ChainLightningVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayUkuleleVoid"),
                    "ThighL",
                    new Vector3(-0.00044F, 0.12682F, 0.08305F),
                    new Vector3(354.8873F, 353.6878F, 173.8436F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Clover"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayClover"),
                    "Head",
                    new Vector3(0.06405F, 0.13759F, 0.06809F),
                    new Vector3(43.50097F, 354.869F, 328.2739F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CloverVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayCloverVoid"),
                    "Head",
                    new Vector3(0.06405F, 0.13759F, 0.06809F),
                    new Vector3(43.50097F, 354.869F, 328.2739F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CritDamage"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLaserSight"),
                    "Sword",
                    new Vector3(0.03081F, 0.0007F, 0.1142F),
                    new Vector3(79.08581F, 268.3683F, 179.8918F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CritGlasses"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGlasses"),
                    "Head",
                    new Vector3(0.07909F, 0.0382F, 0.0003F),
                    new Vector3(8.17772F, 89.62647F, 3.23139F),
                    new Vector3(0.19F, 0.199F, 0.16F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CritGlassesVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                    "Head",
                    new Vector3(0.07909F, 0.0382F, 0.0003F),
                    new Vector3(1.19504F, 85.43509F, 359.8778F),
                    new Vector3(0.15F, 0.16F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Crowbar"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayCrowbar"),
                    "Stomach",
                    new Vector3(-0.09713F, 0.00974F, -0.00105F),
                    new Vector3(300.1274F, 276.5169F, 82.92998F),
                    new Vector3(0.2F, 0.3F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Dagger"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDagger"),
                    "UpperArmR",
                    new Vector3(-0.08182F, 0.11478F, -0.02207F),
                    new Vector3(28.60671F, 302.3544F, 3.47121F),
                    new Vector3(0.7F, 0.7F, 0.7F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["DeathMark"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDeathMark"),
                    "Pelvis",
                    new Vector3(0.07993F, 0.09611F, 0.05477F),
                    new Vector3(278.6671F, 213.9344F, 221.1182F),
                    new Vector3(0.025F, 0.025F, 0.025F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ElementalRingVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayVoidRing"),
                    "HandR",
                    new Vector3(0.01827F, 0.10735F, -0.01157F),
                    new Vector3(5.3989F, 82.89082F, 3.63558F),
                    new Vector3(0.11F, 0.11F, 0.11F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EnergizedOnEquipmentUse"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWarHorn"),
                    "Pelvis",
                    new Vector3(-0.04906F, 0.01527F, -0.14594F),
                    new Vector3(15.31615F, 44.3898F, 3.71571F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EquipmentMagazine"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBattery"),
                    "ThighR",
                    new Vector3(-0.03892F, 0.15345F, -0.06729F),
                    new Vector3(309.5789F, 104.5781F, 2.50693F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EquipmentMagazineVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFuelCellVoid"),
                    "ThighR",
                    new Vector3(-0.03892F, 0.15345F, -0.06729F),
                    new Vector3(309.5789F, 104.5781F, 2.50693F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExecuteLowHealthElite"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGuillotine"),
                    "SwordTip",
                    new Vector3(-0.00004F, -0.00033F, -0.09691F),
                    new Vector3(0.09546F, 180.078F, 109.1484F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExplodeOnDeath"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWilloWisp"),
                    "Pelvis",
                    new Vector3(0.02548F, 0.01322F, -0.16373F),
                    new Vector3(3.30729F, 358.0177F, 10.09323F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExplodeOnDeathVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWillowWispVoid"),
                    "Pelvis",
                    new Vector3(0.02548F, 0.01322F, -0.16373F),
                    new Vector3(3.30729F, 358.0177F, 10.09323F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FallBoots"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGravBoots"),
                    "CalfL",
                    new Vector3(0.0049F, 0.34042F, 0.00805F),
                    new Vector3(359.1816F, 353.8907F, 359.5327F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGravBoots"),
                    "CalfR",
                    new Vector3(0.01295F, 0.37728F, -0.0067F),
                    new Vector3(7.88636F, 348.7487F, 356.9562F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Feather"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFeather"),
                    "Head",
                    new Vector3(-0.00847F, 0.09014F, -0.02251F),
                    new Vector3(348.4604F, 127.3303F, 327.4154F),
                    new Vector3(0.01F, 0.02F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FireballsOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
                    "HandL",
                    new Vector3(-0.01555F, 0.12248F, -0.00448F),
                    new Vector3(295.6318F, 85.7634F, 9.17097F),
                    new Vector3(0.01F, 0.01F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LightningStrikeOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayChargedPerforator"),
                    "HandL",
                    new Vector3(-0.0146F, 0.11009F, -0.02976F),
                    new Vector3(342.1431F, 309.2157F, 180.8991F),
                    new Vector3(0.11F, 0.11F, 0.11F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FireRing"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFireRing"),
                    "HandR",
                    new Vector3(-0.00129F, 0.11413F, 0.00612F),
                    new Vector3(297.2106F, 96.64824F, 326.1677F),
                    new Vector3(0.13F, 0.13F, 0.13F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FlatHealth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySteakCurved"),
                    "Head",
                    new Vector3(0.0923F, -0.02771F, 0.02685F),
                    new Vector3(310.7221F, 40.51475F, 152.7858F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FocusConvergence"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFocusedConvergence"),
                    "Neck",
                    new Vector3(-0.01644F, 0.08845F, 0.54932F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["FragileDamageBonus"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDelicateWatch"),
                    "HandL",
                    new Vector3(-0.00719F, -0.00872F, 0.00095F),
                    new Vector3(281.8886F, 346.4756F, 113.1781F),
                    new Vector3(0.4F, 0.5F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["GhostOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMask"),
                    "Head",
                    new Vector3(0.03774F, 0.04046F, -0.00108F),
                    new Vector3(2.33802F, 80.67154F, 4.35355F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["GoldOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                    "Head",
                    new Vector3(-0.00135F, 0.1076F, -0.00753F),
                    new Vector3(359.4242F, 2.74879F, 11.89618F),
                    new Vector3(0.6F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HalfAttackSpeedHalfCooldowns"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLunarShoulderNature"),
                    "Chest",
                    new Vector3(-0.07152F, 0.13941F, -0.20562F),
                    new Vector3(346.7759F, 298.3177F, 47.77142F),
                    new Vector3(0.5F, 0.5F, 0.6F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HalfSpeedDoubleHealth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLunarShoulderStone"),
                    "Chest",
                    new Vector3(-0.06388F, 0.15621F, 0.22882F),
                    new Vector3(350.1556F, 68.33543F, 351.0257F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HeadHunter"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySkullcrown"),
                    "Head",
                    new Vector3(-0.00787F, 0.07493F, -0.00836F),
                    new Vector3(359.9919F, 84.38554F, 358.4037F),
                    new Vector3(0.32F, 0.11F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HealingPotion"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHealingPotion"),
                    "Stomach",
                    new Vector3(0.09496F, -0.0145F, 0.01715F),
                    new Vector3(3.55074F, 356.3073F, 10.28063F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HealOnCrit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayScythe"),
                    "Scabbard",
                    new Vector3(-0.00207F, 0.19833F, 0.01165F),
                    new Vector3(83.70384F, 319.1665F, 276.4536F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HealWhileSafe"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySnail"),
                    "Chest",
                    new Vector3(0.074F, 0.1348F, 0.06352F),
                    new Vector3(42.02874F, 244.3177F, 180.8635F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Hoof"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHoof"),
                    "CalfL",
                    new Vector3(-0.09645F, 0.2748F, -0.01168F),
                    new Vector3(76.88034F, 92.89532F, 6.25583F),
                    new Vector3(0.08F, 0.1F, 0.075F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["IceRing"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayIceRing"),
                    "HandR",
                    new Vector3(0.019F, 0.09339F, -0.0319F),
                    new Vector3(25.16076F, 87.75369F, 12.77491F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Icicle"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayFrostRelic"),
                    "Neck",
                    new Vector3(-0.29225F, 0.37666F, 0.32981F),
                    new Vector3(53.18039F, 64.80971F, 101.4038F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["IgniteOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGasoline"),
                    "Scabbard",
                    new Vector3(-0.03966F, 0.17898F, -0.00156F),
                    new Vector3(87.24857F, 318.3643F, 320.8006F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ImmuneToDebuff"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayRainCoatBelt"),
                    "Pelvis",
                    new Vector3(0.01746F, 0.08979F, 0.00402F),
                    new Vector3(351.7897F, 89.36467F, 2.7222F),
                    new Vector3(0.9F, 0.5F, 0.65F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["IncreaseHealing"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAntler"),
                    "Head",
                    new Vector3(0.05281F, 0.08272F, -0.01257F),
                    new Vector3(358.642F, 132.774F, 358.8551F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayAntler"),
                    "Head",
                    new Vector3(0.03796F, 0.07986F, 0.03061F),
                    new Vector3(359.5843F, 25.48386F, 0.09986F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["JumpBoost"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    "Neck",
                    new Vector3(-0.0886F, -0.30672F, 0.10022F),
                    new Vector3(348.3029F, 48.18576F, 353.0205F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["KillEliteFrenzy"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                    "Head",
                    new Vector3(-0.01259F, 0.10455F, 0.02536F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.15F, 0.15F, 0.15F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LaserTurbine"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLaserTurbine"),
                    "Neck",
                    new Vector3(-0.34972F, 0.38054F, 0.5466F),
                    new Vector3(67.42687F, 236.9958F, 65.83738F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LunarDagger"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLunarDagger"),
                    "Scabbard",
                    new Vector3(-0.03464F, 0.13215F, 0.03167F),
                    new Vector3(313.2152F, 353.996F, 271.4974F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Medkit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMedkit"),
                    "Stomach",
                    new Vector3(0.08488F, -0.09919F, -0.03609F),
                    new Vector3(293.9908F, 338.2576F, 122.627F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MinorConstructOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDefenseNucleus"),
                    "Neck",
                    new Vector3(-0.15378F, 0.39583F, 0.7796F),
                    new Vector3(6.80784F, 348.1607F, 11.5469F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Missile"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMissileLauncher"),
                    "Neck",
                    new Vector3(-0.11358F, 0.12993F, -0.15207F),
                    new Vector3(333.7574F, 90.13618F, 333.924F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MissileVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMissileLauncherVoid"),
                    "Chest",
                    new Vector3(-0.08941F, 0.29651F, -0.10325F),
                    new Vector3(349.351F, 114.8049F, 349.2369F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Mushroom"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMushroom"),
                    "ThighR",
                    new Vector3(-0.03135F, 0.13719F, -0.05414F),
                    new Vector3(304.1568F, 328.2761F, 73.79642F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MushroomVoid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMushroomVoid"),
                    "ThighR",
                    new Vector3(-0.03135F, 0.13719F, -0.05414F),
                    new Vector3(304.1568F, 328.2761F, 73.79642F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["NearbyDamageBonus"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDiamond"),
                    "Sword",
                    new Vector3(-0.02358F, 0.00052F, 0.11568F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["NovaOnHeal"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                    "Head",
                    new Vector3(0.05338F, 0.07802F, -0.07019F),
                    new Vector3(299.8119F, 121.2304F, 45.62058F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDevilHorns"),
                    "Head",
                    new Vector3(0.02745F, 0.0737F, 0.06212F),
                    new Vector3(310.8142F, 301.7905F, 61.26521F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["NovaOnLowHealth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayJellyGuts"),
                    "LowerArmL",
                    new Vector3(-0.02233F, 0.12505F, 0.00708F),
                    new Vector3(335.0081F, 87.0704F, 344.5856F),
                    new Vector3(0.04F, 0.04F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["OutOfCombatArmor"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayOddlyShapedOpal"),
                    "Chest",
                    new Vector3(0.10445F, 0.02574F, 0.09599F),
                    new Vector3(346.9664F, 235.9892F, 17.3849F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PermanentDebuffOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayScorpion"),
                    "Neck",
                    new Vector3(0.00679F, -0.00033F, 0.0584F),
                    new Vector3(58.123F, 93.13254F, 123.5546F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Phasing"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayStealthkit"),
                    "LowerArmR",
                    new Vector3(-0.00027F, 0.23373F, 0.03723F),
                    new Vector3(281.6063F, 37.41669F, 305.3553F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PrimarySkillShuriken"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayShuriken"),
                    "Pelvis",
                    new Vector3(0.00242F, 0.03609F, -0.16348F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.13F, 0.13F, 0.13F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SecondarySkillMagazine"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                    "Sword",
                    new Vector3(-0.0056F, -0.00966F, -0.05381F),
                    new Vector3(293.5719F, 21.96214F, 339.5586F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ShockNearby"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTeslaCoil"),
                    "Chest",
                    new Vector3(0.10777F, 0.04533F, -0.08101F),
                    new Vector3(328.0006F, 38.1135F, 251.8406F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SprintArmor"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBuckler"),
                    "HandL",
                    new Vector3(0.01105F, 0.09169F, -0.02188F),
                    new Vector3(344.0821F, 294.7628F, 248.3226F),
                    new Vector3(0.1F, 0.1F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SprintBonus"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySoda"),
                    "Stomach",
                    new Vector3(0.0941F, 0.00102F, -0.01842F),
                    new Vector3(0.20073F, 3.5068F, 0.31338F),
                    new Vector3(0.1F, 0.1F, 0.15F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SprintOutOfCombat"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWhip"),
                    "Scabbard",
                    new Vector3(0.01873F, 0.01038F, 0.01607F),
                    new Vector3(303.3047F, 207.8704F, 152.6994F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SprintWisp"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBrokenMask"),
                    "Head",
                    new Vector3(0.05117F, 0.04294F, -0.01152F),
                    new Vector3(10.58155F, 84.66699F, 3.52252F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["StickyBomb"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayStickyBomb"),
                    "Sword",
                    new Vector3(0.00309F, 0.12822F, 0.15095F),
                    new Vector3(45.07119F, 0.66772F, 354.7885F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["StrengthenBurn"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGasTank"),
                    "Stomach",
                    new Vector3(0.02063F, -0.0745F, 0.13895F),
                    new Vector3(343.4153F, 74.63577F, 346.6254F),
                    new Vector3(0.15F, 0.15F, 0.15F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["StunChanceOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayStunGrenade"),
                    "Stomach",
                    new Vector3(0.04107F, -0.07689F, -0.15491F),
                    new Vector3(295.0052F, 275.0011F, 340.8122F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Syringe"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySyringeCluster"),
                    "Neck",
                    new Vector3(-0.01531F, 0.01192F, -0.02123F),
                    new Vector3(325.9291F, 117.455F, 285.9258F),
                    new Vector3(0.07F, 0.07F, 0.07F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Talisman"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTalisman"),
                    "Neck",
                    new Vector3(0.36207F, 0.27633F, 0.74352F),
                    new Vector3(351.5356F, 169.4974F, 337.0293F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Thorns"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayRazorwireLeft"),
                    "UpperArmL",
                    new Vector3(1.72547F, 1.50716F, 2.37881F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Tooth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothNecklaceDecal"),
                    "Neck",
                    new Vector3(0.03395F, 0.02459F, 0.00254F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(1F, 1F, 1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothMeshLarge"),
                    "Neck",
                    new Vector3(0.05556F, -0.00876F, -0.00567F),
                    new Vector3(345.428F, 77.34789F, 8.27967F),
                    new Vector3(1F, 1F, 1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothMeshSmall1"),
                    "Neck",
                    new Vector3(0.04933F, -0.00461F, 0.02394F),
                    new Vector3(331.6964F, 93.29562F, 327.6909F),
                    new Vector3(1F, 1F, 1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothMeshSmall2"),
                    "Neck",
                    new Vector3(0.03563F, -0.00777F, -0.03418F),
                    new Vector3(314.4677F, 131.5645F, 18.83721F),
                    new Vector3(1F, 1F, 1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothMeshSmall2"),
                    "Neck",
                    new Vector3(0.00152F, -0.00368F, -0.0567F),
                    new Vector3(358.9976F, 172.1812F, 324.2295F),
                    new Vector3(1F, 1F, 1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayToothMeshSmall1"),
                    "Neck",
                    new Vector3(0.0146F, -0.00335F, 0.04991F),
                    new Vector3(20.39085F, 198.5325F, 334.0324F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["WarCryOnMultiKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPauldron"),
                    "CalfL",
                    new Vector3(-0.06717F, 0.07659F, -0.00153F),
                    new Vector3(64.17693F, 274.1291F, 4.37853F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BFG"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBFG"),
                    "Neck",
                    new Vector3(-0.05703F, -0.02057F, 0.11057F),
                    new Vector3(320.8042F, 111.2262F, 7.79495F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Blackhole"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGravCube"),
                    "Neck",
                    new Vector3(-0.491F, 0.32881F, 0.13417F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BossHunterConsumed"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTricornUsed"),
                    "Head",
                    new Vector3(0.00704F, 0.17553F, -0.06726F),
                    new Vector3(7.5589F, 1.45096F, 359.9454F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BossHunter"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTricornGhost"),
                    "Head",
                    new Vector3(0.00704F, 0.17553F, -0.06726F),
                    new Vector3(7.5589F, 1.45096F, 359.9454F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBlunderbuss"),
                    "Neck",
                    new Vector3(0.75495F, -0.00181F, 0.08765F),
                    new Vector3(73.61008F, 57.58825F, 262.0458F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BurnNearby"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPotion"),
                    "Stomach",
                    new Vector3(0.08555F, -0.02977F, -0.07347F),
                    new Vector3(20.00938F, 302.0333F, 286.9827F),
                    new Vector3(0.01F, 0.01F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Cleanse"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWaterPack"),
                    "Stomach",
                    new Vector3(0.10596F, -0.11428F, 0.12124F),
                    new Vector3(330.0021F, 45.47925F, 356.625F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CritOnUse"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayNeuralImplant"),
                    "Head",
                    new Vector3(0.08419F, 0.04303F, 0.02998F),
                    new Vector3(353.593F, 86.72041F, 348.9697F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["GoldGat"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGoldGat"),
                    "Neck",
                    new Vector3(-0.17026F, 0.04459F, 0.1294F),
                    new Vector3(352.0971F, 184.7809F, 281.4641F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Jetpack"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBugWings"),
                    "Chest",
                    new Vector3(-0.00448F, 0.15133F, -0.01237F),
                    new Vector3(337.986F, 95.49573F, 356.1533F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LifestealOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLifestealOnHit"),
                    "Chest",
                    new Vector3(0.14278F, 0.00226F, 0.11056F),
                    new Vector3(327.4542F, 222.9013F, 334.8249F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Lightning"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLightningArmRight"),
                    "Chest",
                    new Vector3(3.09703F, 0.31349F, 1.85281F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.6F, 0.6F, 0.6F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Molotov"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMolotov"),
                    "Scabbard",
                    new Vector3(0.08378F, 0.16911F, -0.11962F),
                    new Vector3(335.8939F, 144.8751F, 183.0729F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Scanner"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayScanner"),
                    "Neck",
                    new Vector3(-0.07849F, -0.0047F, 0.18231F),
                    new Vector3(291.4768F, 264.1193F, 154.922F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Tonic"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTonic"),
                    "Stomach",
                    new Vector3(0.09099F, -0.00916F, 0.09532F),
                    new Vector3(338.6812F, 55.8671F, 355.5133F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Saw"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySawmerangFollower"),
                    "Neck",
                    new Vector3(0.82536F, 0.48016F, 0.20832F),
                    new Vector3(84.02339F, 180F, 180F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteLightningEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                    "Head",
                    new Vector3(0.05755F, 0.09711F, -0.04429F),
                    new Vector3(330.9063F, 130.8713F, 18.88409F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteRhinoHorn"),
                    "Head",
                    new Vector3(0.06479F, 0.09057F, 0.04351F),
                    new Vector3(328.4074F, 52.79839F, 347.2961F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteLunarEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplayFromKey("EliteLunarEquipment", 0),
                    "Head",
                    new Vector3(0.0745F, 0.0417F, -0.03012F),
                    new Vector3(1.14765F, 85.95602F, 349.8938F),
                    new Vector3(0.01F, 0.01F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ElitePoisonEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteUrchinCrown"),
                    "Head",
                    new Vector3(-0.02829F, 0.09757F, -0.01396F),
                    new Vector3(283.5099F, 232.3129F, 115.6707F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteIceEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteIceCrown"),
                    "Head",
                    new Vector3(-0.04673F, 0.15023F, -0.0027F),
                    new Vector3(287.9377F, 263.8183F, 186.6939F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteHauntedEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteStealthCrown"),
                    "Head",
                    new Vector3(0.0052F, 0.19669F, -0.01F),
                    new Vector3(80.77463F, 304.1191F, 29.52212F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteFireEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                    "Head",
                    new Vector3(0.07346F, 0.07718F, -0.03104F),
                    new Vector3(314.6048F, 37.61503F, 357.1991F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteHorn"),
                    "Head",
                    new Vector3(0.04772F, 0.07612F, 0.05601F),
                    new Vector3(342.6562F, 167.9895F, 332.6087F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteEarthEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteMendingAntlers"),
                    "Head",
                    new Vector3(0.06855F, 0.12683F, 0.00086F),
                    new Vector3(0.07441F, 273.4555F, 358.0757F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));

            // Seekers of the Storm, Alloyed Collective and Hollowed Concepts items, from Alex's second helper session
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["IncreaseDamageOnMultiKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayIncreaseDamageOnMultiKill"),
                    "Chest",
                    new Vector3(-0.15194F, 0.0795F, 0.00885F),
                    new Vector3(294.7691F, 94.05482F, 193.9792F),
                    new Vector3(0.15751F, 0.15751F, 0.15751F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteCollectiveEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteCollectiveHorn"),
                    "Head",
                    new Vector3(0.08882F, -0.04198F, -0.02297F),
                    new Vector3(1.31159F, 254.4808F, 0.25617F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteCollectiveHorn"),
                    "Head",
                    new Vector3(0.04356F, -0.043F, -0.00084F),
                    new Vector3(1.52099F, 320.6198F, 1.43675F),
                    new Vector3(-0.1F, 0.1F, 0.1F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteCollectiveRing"),
                    "Head",
                    new Vector3(0.06908F, 0.04086F, 0.02938F),
                    new Vector3(359.7016F, 83.77299F, 2.26802F),
                    new Vector3(0.01F, 0.01F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExtraStatsOnLevelUp"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPrayerBeads"),
                    "Neck",
                    new Vector3(-0.01024F, 0.02646F, -0.00012F),
                    new Vector3(349.2459F, 95.1865F, 355.0047F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["AttackSpeedPerNearbyAllyOrEnemy"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayRageCrystal"),
                    "Scabbard",
                    new Vector3(0.00124F, 0.64403F, -0.02057F),
                    new Vector3(1.20236F, 263.0822F, 125.0298F),
                    new Vector3(0.7F, 0.7F, 0.7F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["TeleportOnLowHealth"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTeleportOnLowHealth"),
                    "Sword",
                    new Vector3(0.01314F, -0.01163F, -0.26944F),
                    new Vector3(11.60509F, 84.79331F, 83.45893F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["DelayedDamage"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDelayedDamage"),
                    "Chest",
                    new Vector3(0.12712F, 0.02777F, -0.05996F),
                    new Vector3(12.88498F, 113.3761F, 355.1317F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["KnockBackHitEnemies"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayKnockbackFin"),
                    "Chest",
                    new Vector3(-0.03903F, 0.23711F, 0.02737F),
                    new Vector3(10.17253F, 101.8207F, 5.6862F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["IncreasePrimaryDamage"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayIncreasePrimaryDamage"),
                    "LowerArmR",
                    new Vector3(-0.00608F, 0.24512F, 0.03728F),
                    new Vector3(351.2442F, 351.6126F, 358.6879F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteBeadEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteBeadSpike"),
                    "Neck",
                    new Vector3(-0.06226F, -0.02677F, 0.09926F),
                    new Vector3(341.1818F, 163.5872F, 319.0284F),
                    new Vector3(0.01F, 0.01F, 0.01F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ExtraShrineItem"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayChanceDoll"),
                    "ThighR",
                    new Vector3(-0.06305F, 0.1519F, -0.04727F),
                    new Vector3(5.08356F, 223.6455F, 169.6321F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["JumpDamageStrike"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayJumpDamageStrike"),
                    "FootR",
                    new Vector3(0.02993F, -0.02653F, 0.00094F),
                    new Vector3(279.2483F, 73.4506F, 200.5309F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayJumpDamageStrike"),
                    "FootL",
                    new Vector3(0.02523F, -0.02722F, -0.0045F),
                    new Vector3(278.4267F, 95.99857F, 176.9675F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LowerPricedChests"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLowerPricedChests"),
                    "Neck",
                    new Vector3(0.0467F, 0.32264F, 1.15139F),
                    new Vector3(287.1772F, 235.692F, 280.4716F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["TriggerEnemyDebuffs"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayNoxiousThorn"),
                    "Chest",
                    new Vector3(0.01368F, 0.16062F, -0.10579F),
                    new Vector3(356.9885F, 271.7589F, 38.66858F),
                    new Vector3(0.34411F, 0.34411F, 0.34411F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteAurelioniteEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteAurelioniteEquipment"),
                    "Head",
                    new Vector3(0.0696F, 0.10574F, 0.00039F),
                    new Vector3(356.0397F, 96.20811F, 358.1334F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SpeedBoostPickup"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayElusiveAntlersLeft"),
                    "Head",
                    new Vector3(-0.00502F, 0.08245F, 0.03801F),
                    new Vector3(358.3357F, 125.9523F, 357.3068F),
                    new Vector3(0.35F, 0.35F, 0.35F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayElusiveAntlersRight"),
                    "Head",
                    new Vector3(0.00317F, 0.09312F, -0.04221F),
                    new Vector3(358.6385F, 47.86915F, 359.5246F),
                    new Vector3(0.35F, 0.35F, 0.35F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ShieldBooster"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayShieldBooster"),
                    "Chest",
                    new Vector3(-0.11324F, 0.13562F, 0.06833F),
                    new Vector3(24.24846F, 352.0555F, 49.19436F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BarrierOnCooldown"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBarrierOnCooldown"),
                    "Stomach",
                    new Vector3(0.13545F, -0.11997F, -0.07508F),
                    new Vector3(9.42769F, 25.19941F, 32.32237F),
                    new Vector3(0.04F, 0.056F, 0.04F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CritAtLowerElevation"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("CritAtLowerElevationDisplay"),
                    "Stomach",
                    new Vector3(-0.06639F, -0.00888F, 0.07855F),
                    new Vector3(352.377F, 319.8623F, 351.4144F),
                    new Vector3(0.15F, 0.18F, 0.187F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SpeedOnPickup"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("SpeedOnPickupDisplay"),
                    "Chest",
                    new Vector3(-0.1062F, 0.10523F, -0.12965F),
                    new Vector3(350.5033F, 247.0083F, 356.538F),
                    new Vector3(0.15F, 0.18F, 0.187F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SprintDamage"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySprintDamage"),
                    "Head",
                    new Vector3(-0.0168F, 0.02635F, 0.00561F),
                    new Vector3(343.1022F, 95.26679F, 353.5513F),
                    new Vector3(0.2F, 0.25F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["GhostBlades"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGhostBlades"),
                    "HandL",
                    new Vector3(0.01701F, 0.12898F, -0.09984F),
                    new Vector3(289.269F, 24.31144F, 338.6355F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MushShot"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMushShot"),
                    "CalfL",
                    new Vector3(-0.02058F, 0.02784F, 0.0125F),
                    new Vector3(330.4484F, 144.9012F, 296.9107F),
                    new Vector3(0.0631F, 0.0631F, 0.0631F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMushShot"),
                    "CalfR",
                    new Vector3(-0.01578F, 0.02947F, -0.00009F),
                    new Vector3(303.8221F, 142.8148F, 297.283F),
                    new Vector3(0.0631F, 0.0631F, 0.0631F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["EliteDeathEquipment"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEliteDeath"),
                    "Head",
                    new Vector3(-0.01213F, 0.09546F, -0.00245F),
                    new Vector3(2.18927F, 269.6617F, 1.744F),
                    new Vector3(0.28F, 0.28F, 0.28F)
                    )
                ));

            // remaining DLC items, from Alex's in-run check
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BarrageOnBoss"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayTreasuryDividends"),
                    "LowerArmR",
                    new Vector3(-0.0129F, 0.12592F, 0.04928F),
                    new Vector3(21.94709F, 157.141F, 281.3297F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BoostAllStats"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGrowthNectar"),
                    "Head",
                    new Vector3(-0.013F, 0.07618F, -0.00726F),
                    new Vector3(351.2723F, 77.85261F, 344.5592F),
                    new Vector3(0.5F, 0.5F, 0.5F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MeteorAttackOnHighDamage"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayMeteorAttackOnHighDamage"),
                    "Head",
                    new Vector3(0.05605F, -0.01067F, 0.07835F),
                    new Vector3(354.2134F, 83.70103F, 336.2761F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LunarSun"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySunHeadNeck"),
                    "Neck",
                    new Vector3(0.01595F, -0.07336F, 0.01455F),
                    new Vector3(3.75241F, 35.69159F, 10.90554F),
                    new Vector3(1.46F, 1.46F, 1.46F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySunHead"),
                    "Head",
                    new Vector3(-0.00244F, 0.06992F, 0.00459F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ShockDamageAura"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("ShockDamageAuraDisplay"),
                    "Head",
                    new Vector3(0.10511F, 0.15321F, -0.07672F),
                    new Vector3(325.1169F, 320.6286F, 359.0345F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ItemDropChanceOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySonorousEcho"),
                    "ThighL",
                    new Vector3(0.02466F, 0.27748F, 0.07732F),
                    new Vector3(17.33613F, 94.96349F, 174.8397F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PowerCube"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPowerCube"),
                    "Neck",
                    new Vector3(-0.79F, 0F, -0.849F),
                    new Vector3(-0.00015F, 180.079F, 180.4018F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PowerPyramid"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPowerPyramid"),
                    "Neck",
                    new Vector3(-0.79F, 0F, -0.849F),
                    new Vector3(-0.00015F, 180.079F, 180.4018F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PowerOrbSphere"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPowerOrbSphere"),
                    "Neck",
                    new Vector3(-0.79F, 0F, -0.849F),
                    new Vector3(-0.00015F, 180.079F, 180.4018F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MasterBattery"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPowerOrbSphere"),
                    "Neck",
                    new Vector3(-0.79F, 0F, -0.849F),
                    new Vector3(-0.00015F, 180.079F, 180.4018F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["MasterCore"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayPowerOrbSphere"),
                    "Neck",
                    new Vector3(-0.79F, 0F, -0.849F),
                    new Vector3(-0.00015F, 180.079F, 180.4018F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HealAndRevive"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHealAndRevive"),
                    "Neck",
                    new Vector3(0.0128F, 0.01674F, -0.03978F),
                    new Vector3(309.9742F, 19.0936F, 340.7981F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["HealAndReviveConsumed"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayHealAndReviveConsumed"),
                    "Neck",
                    new Vector3(0.0128F, 0.01674F, -0.03978F),
                    new Vector3(309.9742F, 19.0936F, 340.7981F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["OnLevelUpFreeUnlock"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayOnLevelUpFreeUnlockTablet"),
                    "Base",
                    new Vector3(1.1836F, -0.3469F, -0.03022F),
                    new Vector3(7.11974F, 172.7606F, 97.5547F),
                    new Vector3(0.8F, 0.8F, 0.8F)
                    ),
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayOnLevelUpFreeUnlock"),
                    "Neck",
                    new Vector3(-0.24808F, 0.00361F, -0.44107F),
                    new Vector3(7.50728F, 204.7542F, 333.1736F),
                    new Vector3(0.4F, 0.4F, 0.4F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["StunAndPierce"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayElectricBoomerang"),
                    "HandR",
                    new Vector3(-0.02608F, 0.07486F, -0.0041F),
                    new Vector3(39.50235F, 352.7544F, 86.19994F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["DronesDropDynamite"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DronesDropDynamiteDisplay"),
                    "Stomach",
                    new Vector3(0.06528F, -0.02308F, 0.09027F),
                    new Vector3(1.84707F, 38.64083F, 1.76678F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BonusHealthBoost"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayQuickFix"),
                    "Chest",
                    new Vector3(0.01575F, 0.17662F, 0.16914F),
                    new Vector3(339.1061F, 332.4063F, 341.4264F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Stew"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("StewDisplay"),
                    "Chest",
                    new Vector3(0.01575F, 0.17662F, 0.16914F),
                    new Vector3(339.1061F, 332.4063F, 341.4264F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CookedSteak"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayCookedSteakFlat"),
                    "Head",
                    new Vector3(0.09442F, -0.01263F, -0.01157F),
                    new Vector3(279.1414F, 48.1306F, 130.419F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Duplicator"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDuplicator"),
                    "LowerArmL",
                    new Vector3(0.00241F, 0.11639F, -0.06422F),
                    new Vector3(318.9315F, 177.9403F, 2.48966F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["PhysicsProjectile"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("PhysicsProjectileDisplay"),
                    "Head",
                    new Vector3(-0.03159F, 0.08112F, -0.00706F),
                    new Vector3(0.0678F, 272.7245F, 20.37274F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SharedSuffering"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("SharedSufferingDisplay"),
                    "Chest",
                    new Vector3(-0.11091F, 0.0532F, 0.00499F),
                    new Vector3(275.4637F, 283.9905F, 162.9904F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["UltimateMeal"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("UltimateMealDisplay"),
                    "Base",
                    new Vector3(0.78216F, 0.56999F, 0.06864F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(1F, 1F, 1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["WyrmOnHit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWyrmOnHit"),
                    "Neck",
                    new Vector3(0.00189F, 0.03075F, -0.00127F),
                    new Vector3(359.4529F, 25.9508F, 357.0813F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["Parry"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("ParryDisplay"),
                    "Sword",
                    new Vector3(0.02424F, -0.00086F, 0.10294F),
                    new Vector3(3.46467F, 87.27615F, 88.30054F),
                    new Vector3(0.2F, 0.2F, 0.2F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["ArtifactKey"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayArtifactKey"),
                    "Base",
                    new Vector3(0.87111F, -0.5401F, 0.10718F),
                    new Vector3(359.7207F, 4.99207F, 273.1783F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LunarBadLuck"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayStarSeed"),
                    "Base",
                    new Vector3(1.21144F, 0.0049F, -0.01672F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["RoboBallBuddy"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayEmpathyChip"),
                    "Stomach",
                    new Vector3(-0.08757F, -0.0032F, -0.02977F),
                    new Vector3(341.5872F, 352.0995F, 335.2873F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["BlightZone"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayBlightZone"),
                    "Chest",
                    new Vector3(0.14534F, 0.05689F, 0.05496F),
                    new Vector3(38.92514F, 344.5862F, 260.6878F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["DreamDemon"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDreamDemon"),
                    "Head",
                    new Vector3(0.01012F, 0.0225F, -0.08165F),
                    new Vector3(1.92028F, 160.3888F, 169.7666F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["GoldenGoose"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayGoldenGoose"),
                    "Head",
                    new Vector3(0.04431F, 0.14136F, -0.06285F),
                    new Vector3(311.2196F, 337.2399F, 7.95481F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["LowHealthOrbSpawner"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayLowHealthOrbSpawner"),
                    "Neck",
                    new Vector3(-0.06098F, 0.02094F, -0.192F),
                    new Vector3(321.7391F, 42.05848F, 3.72962F),
                    new Vector3(0.03F, 0.03F, 0.03F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SanguineChalice"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySanguineChalice"),
                    "LowerArmL",
                    new Vector3(0.00421F, 0.15014F, -0.05329F),
                    new Vector3(358.346F, 180F, 180F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["VengefulSpirit"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayVengefulSpirit"),
                    "Neck",
                    new Vector3(-0.00468F, 0.01173F, 0.00096F),
                    new Vector3(316.3125F, 87.43446F, 353.917F),
                    new Vector3(0.31F, 0.31F, 0.31F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["VileCenser"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayVileCenser"),
                    "Stomach",
                    new Vector3(-0.01056F, -0.07632F, -0.14979F),
                    new Vector3(3.47989F, 295.5712F, 342.7601F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["CausticPheromones"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayCausticPheromones"),
                    "Base",
                    new Vector3(0.41168F, -0.44253F, 0.077F),
                    new Vector3(-0.00227F, 359.8995F, 268.6924F),
                    new Vector3(0.3F, 0.3F, 0.3F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["RechargeableJump"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayRechargeableJump"),
                    "ThighR",
                    new Vector3(0.00446F, 0.15199F, -0.08129F),
                    new Vector3(348.3716F, 203.7292F, 176.4195F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["DroneWeapons"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayDroneWeapons"),
                    "CalfR",
                    new Vector3(-0.01495F, 0.10056F, -0.05864F),
                    new Vector3(1.90155F, 109.0735F, 173.0752F),
                    new Vector3(0.02F, 0.02F, 0.02F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["WispOnKill"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplayWispOnKill"),
                    "CalfL",
                    new Vector3(0.02895F, 0.22472F, 0.0667F),
                    new Vector3(0F, 0F, 0F),
                    new Vector3(0.1F, 0.1F, 0.1F)
                    )
                ));
            itemDisplayRules.Add(ItemDisplays.CreateDisplayRuleGroupWithRules(ItemDisplays.KeyAssets["SpawnAltar"],
                ItemDisplays.CreateDisplayRule(ItemDisplays.LoadDisplay("DisplaySpawnAltar"),
                    "ThighR",
                    new Vector3(0.03756F, 0.18636F, -0.07772F),
                    new Vector3(359.2914F, 287.8503F, 174.3376F),
                    new Vector3(0.05F, 0.05F, 0.05F)
                    )
                ));

            // newer items (Seekers of the Storm, Alloyed Collective...) start from Loader's placement so they can be placed in the helper
            ItemDisplays.AddSourceRulesForMissing(itemDisplayRules, royChildNames, notShownOnRoy);
        }

        private static readonly HashSet<string> royChildNames = new HashSet<string>
        {
            "Base", "Pelvis", "Stomach", "Chest", "Neck", "Head", "UpperArmL", "UpperArmR", "LowerArmL", "LowerArmR",
            "HandL", "HandR", "ThighL", "ThighR", "CalfL", "CalfR", "FootL", "FootR", "Sword", "SwordTip", "Scabbard",
        };

        // older items Alex left off Roy
        private static readonly HashSet<string> notShownOnRoy = new HashSet<string>
        {
            "ArmorPlate",
            "BleedOnHitAndExplode",
            "BonusGoldPackOnKill",
            "BossDamageBonus",
            "CooldownOnCrit",
            "CrippleWard",
            "DeathProjectile",
            "DroneBackup",
            "EliteVoidEquipment",
            "EmpowerAlways",
            "FireBallDash",
            "Firework",
            "FreeChest",
            "Fruit",
            "GainArmor",
            "Gateway",
            "GoldOnHurt",
            "GummyClone",
            "Incubator",
            "Infusion",
            "IrradiatingLaser",
            "Knurl",
            "LunarPortalOnUse",
            "LunarPrimaryReplacement",
            "LunarSecondaryReplacement",
            "LunarSpecialReplacement",
            "LunarTrinket",
            "LunarUtilityReplacement",
            "Meteor",
            "MonstersOnShrineUse",
            "MoreMissile",
            "MoveSpeedOnKill",
            "MultiShopCard",
            "ParentEgg",
            "Pearl",
            "Plant",
            "QuestVolatileBattery",
            "RandomDamageZone",
            "RandomEquipmentTrigger",
            "RandomlyLunar",
            "Recycle",
            "RegeneratingScrap",
            "RepeatHeal",
            "Seed",
            "ShieldOnly",
            "ShinyPearl",
            "SiphonOnLowHealth",
            "SlowOnHit",
            "SlowOnHitVoid",
            "Squid",
            "TPHealingNova",
            "TeamWarCry",
            "TitanGoldDuringTP",
            "TreasureCache",
            "TreasureCacheVoid",
            "VendingMachine",
            "VoidMegaCrabItem",
            "WardOnLevel",
            // unused in the current game (not on Alex's in-use list)
            "GroundEnemies",
            // Hexes and the Peace Flower stay hidden on Roy
            "Blindness",
            "HexCurse",
            "HexMountain",
            "IncreasedDifficulty",
            "LowerHealing",
            "OutOfTPRadius",
            "RandomDebuffOnInteract",
            "RoamingItemDisable",
            "SlipperyGround",
            "WeakAssHamstring",
            "Mimicry",
            "BarrierEnemiesOnKill",
            "HexHaunting",
            "ShareItemsWithEnemies",
            "HelpWake",
        };
    }
}
