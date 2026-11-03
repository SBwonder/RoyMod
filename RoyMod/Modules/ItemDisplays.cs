using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace HenryMod.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();
        public static Dictionary<Object, ItemDisplayRule[]> KeyAssetDisplayPrefabs = new Dictionary<Object, ItemDisplayRule[]>();
        public static Dictionary<string, Object> KeyAssets = new Dictionary<string, Object>();
        // the display prefabs the source body uses for each key asset, in rule order
        private static Dictionary<string, List<GameObject>> keyAssetPrefabs = new Dictionary<string, List<GameObject>>();

        public static int queuedDisplays;

        public static bool initialized = false;

        public static void LazyInit()
        {
            if (initialized)
                return;
            initialized = true;

            PopulateDisplays();
        }

        internal static void DisposeWhenDone()
        {
            queuedDisplays--;
            if (queuedDisplays > 0)
                return;
            if (!initialized)
                return;
            initialized = false;

            itemDisplayPrefabs = null;
            KeyAssetDisplayPrefabs = null;
            KeyAssets = null;
            keyAssetPrefabs = null;
        }

        internal static void PopulateDisplays()
        {
            PopulateFromBody("LoaderBody");

            //PopulateCustomLightningArm();

            //if you have any custom item displays to add here I would be very impressed
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName).GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] itemRuleGroups = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < itemRuleGroups.Length; i++)
            {
                ItemDisplayRule[] rules = itemRuleGroups[i].displayRuleGroup.rules;

                KeyAssetDisplayPrefabs[itemRuleGroups[i].keyAsset] = rules;
                KeyAssets[itemRuleGroups[i].keyAsset.name] = itemRuleGroups[i].keyAsset;
                var keyPrefabs = keyAssetPrefabs[itemRuleGroups[i].keyAsset.name] = new List<GameObject>();

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    // newer game versions reference display prefabs by Addressables address instead
                    if (!followerPrefab && rules[j].followerPrefabAddress != null && rules[j].followerPrefabAddress.RuntimeKeyIsValid())
                    {
                        followerPrefab = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<GameObject>(rules[j].followerPrefabAddress).WaitForCompletion();
                    }
                    if (followerPrefab)
                    {
                        keyPrefabs.Add(followerPrefab);
                        string key = followerPrefab.name?.ToLowerInvariant();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        private static void PopulateCustomLightningArm()
        {
            #region IgnoreThisAndRunAway
            //seriously you don't need this
            //I see you're still here, well if you do need this here's what you do
            //but again you don't need this
            //capacitor is hardcoded to track your "UpperArmR", "LowerArmR", and "HandR" bones.
            //this is for having the lightning on custom bones in your childlocator

            GameObject display = R2API.PrefabAPI.InstantiateClone(itemDisplayPrefabs["displaylightningarmright"], "DisplayLightningCustom", false);

            LimbMatcher limbMatcher = display.GetComponent<LimbMatcher>();

            limbMatcher.limbPairs[0].targetChildLimb = "LightningArm1";
            limbMatcher.limbPairs[1].targetChildLimb = "LightningArm2";
            limbMatcher.limbPairs[2].targetChildLimb = "LightningArmEnd";

            itemDisplayPrefabs["displaylightningarmcustom"] = display;
            #endregion
        }

        public static GameObject LoadDisplay(string name) {

            if (itemDisplayPrefabs.ContainsKey(name.ToLowerInvariant())) {

                if (itemDisplayPrefabs[name.ToLowerInvariant()]) {

                    GameObject display = itemDisplayPrefabs[name.ToLowerInvariant()];

                    return display;
                }
            }
            Log.Error("item display " + name + " returned null");
            return null;
        }

        // for items whose display prefab name isn't known, use the one the source body shows for that item
        public static GameObject LoadDisplayFromKey(string keyAssetName, int index)
        {
            if (keyAssetPrefabs.TryGetValue(keyAssetName, out var list) && list.Count > 0)
                return list[Mathf.Min(index, list.Count - 1)];
            Log.Error("no item display found for " + keyAssetName);
            return null;
        }

        // Gives every item the source body has a rule for, but the character doesn't, a copy of the source body's
        // placement (re-parented to "Chest" when the character has no child of that name), so it shows up in
        // ItemDisplayPlacementHelper and can be placed there. Items named in skip stay without a display.
        public static void AddSourceRulesForMissing(List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules, HashSet<string> childNames, HashSet<string> skip)
        {
            var have = new HashSet<Object>();
            foreach (var group in itemDisplayRules) have.Add(group.keyAsset);
            var added = new List<string>();
            foreach (var pair in KeyAssetDisplayPrefabs)
            {
                if (pair.Key == null || have.Contains(pair.Key) || skip.Contains(pair.Key.name)) continue;
                var list = keyAssetPrefabs.TryGetValue(pair.Key.name, out var l) ? l : null;
                var rules = new List<ItemDisplayRule>();
                int prefabIndex = 0;
                foreach (ItemDisplayRule source in pair.Value)
                {
                    if (source.ruleType != ItemDisplayRuleType.ParentedPrefab) continue;
                    GameObject prefab = list != null && prefabIndex < list.Count ? list[prefabIndex] : null;
                    prefabIndex++;
                    if (!prefab) continue;
                    rules.Add(CreateDisplayRule(prefab, childNames.Contains(source.childName) ? source.childName : "Chest",
                        source.localPos, source.localAngles, source.localScale));
                }
                if (rules.Count == 0) continue;
                itemDisplayRules.Add(CreateDisplayRuleGroupWithRules(pair.Key, rules.ToArray()));
                added.Add(pair.Key.name + " [" + string.Join(" | ", rules.ConvertAll(r => r.followerPrefab.name + "@" + r.childName)) + "]");
            }
            Log.Info("item displays copied from the source body for placing: " + string.Join(", ", added));
        }

        #region add rule helpers

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateDisplayRuleGroupWithRules(string itemName, params ItemDisplayRule[] rules) => CreateDisplayRuleGroupWithRules(GetKeyAssetFromString(itemName), rules);
        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateDisplayRuleGroupWithRules(Object keyAsset_, params ItemDisplayRule[] rules)
        {
            if (keyAsset_ == null)
                Log.Error("could not find keyasset");

            return new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset_,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = rules
                }
            };
        }

        public static ItemDisplayRule CreateDisplayRule(string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale) => CreateDisplayRule(LoadDisplay(prefabName), childName, position, rotation, scale);
        public static ItemDisplayRule CreateDisplayRule(GameObject itemPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                childName = childName,
                followerPrefab = itemPrefab,
                limbMask = LimbFlags.None,
                localPos = position,
                localAngles = rotation,
                localScale = scale
            };
        }
        public static ItemDisplayRule CreateLimbMaskDisplayRule(LimbFlags limb)
        {
            return new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.LimbMask,
                limbMask = limb,
                childName = "",
                followerPrefab = null
                //localPos = Vector3.zero,
                //localAngles = Vector3.zero,
                //localScale = Vector3.zero
            };
        }

        private static Object GetKeyAssetFromString(string itemName)
        {
            Object itemDef = RoR2.LegacyResourcesAPI.Load<ItemDef>("ItemDefs/" + itemName);

            if (itemDef == null)
            {
                itemDef = RoR2.LegacyResourcesAPI.Load<EquipmentDef>("EquipmentDefs/" + itemName);
            }

            if (itemDef == null)
            {
                Log.Error("Could not load keyasset for " + itemName);
            }

            return itemDef;
        }

        #endregion add rule helpers
    }
}