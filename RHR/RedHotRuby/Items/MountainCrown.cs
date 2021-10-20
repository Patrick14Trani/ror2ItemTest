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
    public class MountainCrown : ItemBase
    {
        public override string ItemName => "Mountain Crown";

        public override string ItemLangTokenName => "MOUNTAIN_CROWN";

        public override string ItemPickupDesc => "Every teleporter will have shrin of the mountain activated.";

        public override string ItemFullDescription => $"Upon picking it up, the teleporter will receive {charge} charge of shrine of the mountain.";
        public override string ItemLore => "Whoever left these shrines about must've had a king as well.";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("mountainCrown.prefab");

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MountCrownIcon.png");

        //Following are the variables we'd like to use
        public int charge;
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
            //On.RoR2.SceneDirector.PopulateScene += PopulateScene;
            //On.RoR2.CharacterBody.Awake += Awake;
            //On.RoR2.PlayerCharacterMasterController.Awake += Awake;
            On.RoR2.HealthComponent.OnInventoryChanged += OnInventoryChanged;
        }

        /*private void PopulateScene(On.RoR2.SceneDirector.orig_PopulateScene orig, global::RoR2.SceneDirector self)
        {
            var inventoryCount = GetCount()
            if()
        }
        */
        private void OnInventoryChanged(On.RoR2.HealthComponent.orig_OnInventoryChanged orig, RoR2.HealthComponent self)
        {
            var inv = GetCount(self.body);
            if(inv > 0)
            {
               RoR2.TeleporterInteraction.AddShrineStack();
            }
        }
        
    }

}
