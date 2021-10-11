﻿using BepInEx.Configuration;
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

        public override string ItemFullDescription => $"This Item absorbs Lumerian Fireballs and stores them to be fired out at enemies.";

        public override string ItemLore => "These flaming rubies are mined from the depths of hell. Their horns grow out of them and if picked up they shockingly don't burn you but rather they help the user's body become acustom to scalding temperatures.";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("RedHotRuby.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("RHRIcon.png");

        //Following are the variables we'd like to use
        public float FireBallCount = 10;
        public float AdditionalFireballsPerStack = 5;
        public float charge = 0;
        public float chargeDamage;

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
            FireBallCount = config.Bind<float>("Item: " + ItemName, "Starting amount of fireballs to let loose", 10f, "How many fireballs should be fired off?").Value;
            AdditionalFireballsPerStack = config.Bind<float>("Item: " + ItemName, "Increase the amount of fireballs fired off per stack", 5f, "How many fireballs should be fired off?").Value;
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
                    localScale = new Vector3(20,20,20)
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
                    //Debug
                    //Chat.AddMessage($"Damage Type: {damageInfo.damageType}");
                    //Chat.AddMessage($"Inflictor : {damageInfo.inflictor}");
                    Chat.AddMessage($"Inflictor : {damageInfo.inflictor.tag}");
                    Chat.AddMessage($"Inflictor : {damageInfo.inflictor.name}");

                    if (damageInfo.inflictor.name == "Fireball(Clone)")
                    {
                        //particle effect?
                        Chat.AddMessage("Item Worked");
                        damageInfo.damage = 0f;
                        charge += 1;
                        if(charge >= 10)
                        {
                            Chat.AddMessage("Boom");
                            //blast balls
                        }
                    }
                }
            }

            orig(self, damageInfo);
        }
    }
}
