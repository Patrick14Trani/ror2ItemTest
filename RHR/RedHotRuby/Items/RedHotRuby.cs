using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RedHotRuby.Main;
using static RedHotRuby.Utils.ItemHelpers;

namespace RedHotRuby.Items
{
    public class RedHotRuby : ItemBase
    {
        public override string ItemName => "Red Hot Ruby";

        public override string ItemLangTokenName => "RED_HOT_RUBY";

        public override string ItemPickupDesc => "A flaming ruby surrounds your body with warmth";

        public override string ItemFullDescription => $"When you take fire damage, that damage will be reduced by <style=cIsUtility>{MainDamageReduced}</style> <style=cStack>+{AdditionalDamageReductionPerStack}</style>";

        public override string ItemLore => "These flaming rubies are mined from the depths of hell. Their horns grow out of them and if picked up they shockingly don't burn you but rather they help the user's body become acustom to scalding temperatures.";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("RedHotRuby.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("RHRIcon.png");

        //Following are the variables we'd like to use
        public float MainDamageReduced = 4;
        public float AdditionalDamageReductionPerStack = 4;

        public static GameObject ItemBodyModelPrefab;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            MainDamageReduced = config.Bind<float>("Item: " + ItemName, "Main Damage Reduction", 150f, "How much damage reduction should the item do?").Value;
            AdditionalDamageReductionPerStack = config.Bind<float>("Item: " + ItemName, "Increase to Damage Reduction per stack", 100f, "How much should the reduction increase by per additional stack?").Value;
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            ItemBodyModelPrefab = ItemModel;
            ItemBodyModelPrefab.AddComponent<RoR2.ItemDisplay>();
            ItemBodyModelPrefab.GetComponent<RoR2.ItemDisplay>().rendererInfos = ItemDisplaySetup(ItemBodyModelPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new RoR2.ItemDisplayRule[]
            {
                new RoR2.ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemBodyModelPrefab,
                    childName = "Chest",
                    localPos = new Vector3(1f, .6f, -.2f),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(1,1,1)
                }
            });
            return rules;
        }

        public override void Hooks()
        {

        }
    }
}
