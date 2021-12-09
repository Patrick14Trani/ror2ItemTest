using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RedHotRuby.Main;
using static RedHotRuby.Utils.ItemHelpers;
using System;

using RoR2.Projectile;
using UnityEngine.Networking;

namespace RedHotRuby.Items
{
    public class RedHotRuby : ItemBase
    {
        public override string ItemName => "The Crown Jewel";

        public override string ItemLangTokenName => "RED_HOT_RUBY";

        public override string ItemPickupDesc => "A flaming ruby surrounds your body with warmth";

        public override string ItemFullDescription => $"This item reduces damage from fireballs by 5%. +5% per stack";

        public override string ItemLore => "These flaming rubies are mined from the depths of hell. Their horns grow out of them and if picked up they shockingly don't burn you but rather they help the user's body become acustom to scalding temperatures.";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("RedHotRuby.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("RHRIcon.png");

        //Following are the variables we'd like to use
        public float FireBallCount = 10;
        public float AdditionalFireballsPerStack = 5;
        public float charge = 0;
        public float chargeDamage = 50f;
        public float chargeDamageIncreasePerStack = .03f;

        public static GameObject ItemBodyModelPrefab;
        public static GameObject RubyProjectile;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateCharge();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            FireBallCount = config.Bind<float>("Item: " + ItemName, "Starting amount of fireballs to let loose", 10f, "How many fireballs should be fired off?").Value;
            AdditionalFireballsPerStack = config.Bind<float>("Item: " + ItemName, "Increase the amount of fireballs fired off per stack", 5f, "How many fireballs should be fired off?").Value;
        }

        public void CreateCharge()
        {
            RubyProjectile = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/FireBall"), "RubyProjectile");
            var model = MainAssets.LoadAsset<GameObject>("RedHotRuby.prefab");
            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            RubyProjectile.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;

            var damage = RubyProjectile.GetComponent<ProjectileDamage>();
            damage.damageType = DamageType.Generic;
            damage.damage = 0;

            var applyTorqueOnStart = RubyProjectile.AddComponent<ApplyTorqueOnStart>();
            applyTorqueOnStart.localTorque = new Vector3(0, 1500, 0);


            if (RubyProjectile) PrefabAPI.RegisterNetworkPrefab(RubyProjectile);
            ProjectileAPI.Add(RubyProjectile);

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
                    localPos = new Vector3(.4f, .6f, -.2f),
                    localAngles = new Vector3(0,0,0),
                    localScale = new Vector3(1,1,1)
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
            try
            {
                if (!damageInfo.rejected || damageInfo == null)
                {
                    var inventoryCount = GetCount(self.body);
                    if (inventoryCount > 0)
                    {

                        if (damageInfo.inflictor.name == "Fireball(Clone)")
                        {
                            damageInfo.damage = damageInfo.damage - (damageInfo.damage / (damageInfo.damage + .05f * inventoryCount));
                            if(damageInfo.damage < 1)
                            {
                                damageInfo.damage = 1;
                            }
                        }
                    }
                }
                orig(self, damageInfo);
            }catch(NullReferenceException e)
            {
                orig(self, damageInfo);
            }

            
        }
    }
}
