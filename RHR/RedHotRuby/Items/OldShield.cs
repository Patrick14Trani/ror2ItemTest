using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RedHotRuby.Main;
using static RedHotRuby.Utils.ItemHelpers;

using RoR2.Projectile;
using UnityEngine.Networking;

namespace RedHotRuby.Items
{
    public class OldShield : ItemBase
    {
        public override string ItemName => "Old Shield";

        public override string ItemLangTokenName => "OLD_SHIELD";

        public override string ItemPickupDesc => "Reduces the amount of damage taken from elites";

        public override string ItemFullDescription => $"Old Shield reduces the amount of damage you receive from elites by <style=cIsUtility>{DamageReduced}%</style>. <style=cStack>{PerStack}%</style>:Per stack";

        public override string ItemLore => "These flaming rubies are mined from the depths of hell. Their horns grow out of them and if picked up they shockingly don't burn you but rather they help the user's body become acustom to scalding temperatures.";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("OldShield.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("oldshieldicon.png");

        //Following are the variables we'd like to use
        public float DamageReduced;
        public float PerStack;

        public static GameObject ItemBodyModelPrefab;
        public static GameObject RubyProjectile;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            DamageReduced = config.Bind<float>("Item: " + ItemName, "Initial percentage of damage reduced", 10f, "What percent should it be reduced by?").Value;
            PerStack = config.Bind<float>("Item: " + ItemName, "Increased percentage reduced per stack", 6f, "How many fireballs should be fired off?").Value;
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
                    localPos = new Vector3(1f, 1, -2),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(.1f,.1f,.1f)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += TakeDamage;
        }

        private void TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
        {
            
            if (!damageInfo.rejected || damageInfo == null)
            {
                var inventoryCount = GetCount(self.body);
                if (inventoryCount > 0)
                {
                    Chat.AddMessage($"{damageInfo.damage}"); //debug
                    Chat.AddMessage($"{damageInfo.inflictor}"); //debug
                   
                    //we need a way to access the parent of the damage and check if they are an elite.
                    if ()
                    {
                        damageInfo.damage = damageInfo.damage - (damageInfo.damage * (DamageReduced + (PerStack * (inventoryCount - 1)) / 100));
                        Chat.AddMessage($"{damageInfo.damage}"); //debug
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}
