using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static RedHotRuby.Main;
using static RedHotRuby.Utils.ItemHelpers;

using RoR2.Projectile;
using UnityEngine.Networking;
using System;

namespace RedHotRuby.Items
{
    public class MountainCrown : ItemBase
    {
        public override string ItemName => "N'Kuhana's Providence";

        public override string ItemLangTokenName => "MOUNTAIN_CROWN";

        public override string ItemPickupDesc => "This crown fills you with hope... and fear";

        public override string ItemFullDescription => $"Upon picking it up and reaching a new stage, the teleporter will receive a charge of shrine of the mountain.";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("mountainCrown.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MountCrownIcon.png");

        //Following are the variables we'd like to use
        public int charge;
        public int amount;
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
            charge = config.Bind<int>("Item: " + ItemName, "Initial amount of charges added", 1, "How many charges should it add?").Value;
            amount = config.Bind<int>("Item: " + ItemName, "Number started with", 0, "How many should you start with?").Value;
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
                    childName = "Head",
                    localPos = new Vector3(0f, .3f, 0f),
                    localAngles = new Vector3(0, 0, 0),
                    localScale = new Vector3(.05f,.05f,.05f)
                }
            });
            return rules;
        }

        public override void Hooks()
        {
            On.RoR2.CharacterMaster.OnInventoryChanged += CharacterMaster_OnInventoryChanged;
            On.RoR2.PlayerCharacterMasterController.OnBodyStart += PlayerCharacterMasterController_OnBodyStart;
        }

        private void PlayerCharacterMasterController_OnBodyStart(On.RoR2.PlayerCharacterMasterController.orig_OnBodyStart orig, PlayerCharacterMasterController self)
        {
            try
            {
                Chat.AddMessage($"Amount: {amount}");
                if (amount > 0)
                {
                    Chat.AddMessage("<style=cWorldEvent>N'Kuhana has provided a challenge</style>");
                    for (int i = 0; i < amount; i++)
                    {
                        TeleporterInteraction.instance.AddShrineStack();
                    }
                }
                orig(self);
            } catch (Exception e)
            {
                orig(self);
            }
        }

        private void CharacterMaster_OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            var count = GetCount(self.GetBody());
            if (count >= 1 && count > amount)
            {
                TeleporterInteraction.instance.AddShrineStack();
                Chat.AddMessage("<style=cWorldEvent>N'Kuhana has provided a challenge</style>");
                amount++;
            }
            orig(self);
        }

    }

}
