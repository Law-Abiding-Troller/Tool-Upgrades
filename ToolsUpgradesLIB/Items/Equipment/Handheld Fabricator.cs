using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using UnityEngine;

namespace UpgradesLIB.Items.Equipment;

public class Handheldprefab
{
    public static CustomPrefab Handheldfab;
    public static PrefabInfo HandheldfabInfo;
    public static FabricatorGadget HandheldfabGadget;
    public static ITreeActionReceiver FabrInstance;
    public static Vector3 PostScaleValue;
    public static void Register()
    {
        HandheldfabInfo = PrefabInfo.WithTechType("HandHeldToolsFabricator", "Hand Held Upgrades Fabricator",
                "The hand held fabricator for Upgrades that are specific to tools and equipment.")
            .WithIcon(SpriteManager.Get(TechType.Fabricator));
        Handheldfab = new CustomPrefab(HandheldfabInfo);
        if (Config.debugMode)
        {
            HandheldfabGadget = Handheldfab.CreateFabricator(out HandheldfabTreeType)
                .AddTabNode("Equipment", "Equipment", SpriteManager.Get(TechType.Fabricator))
                .AddTabNode("Tools", "Tools", SpriteManager.Get(TechType.Fabricator))
                .AddCraftNode(HandheldfabInfo.TechType, "Tools")
                .AddCraftNode(HandheldfabInfo.TechType, "Equipment");
        }
        else
        {
            HandheldfabGadget = Handheldfab.CreateFabricator(out HandheldfabTreeType)
                .AddTabNode("Equipment", "Equipment", SpriteManager.Get(TechType.Fabricator))
                .AddTabNode("Tools", "Tools", SpriteManager.Get(TechType.Fabricator));
        }
        var clone = new FabricatorTemplate(HandheldfabInfo, HandheldfabTreeType)
        {
            FabricatorModel = FabricatorTemplate.Model.Fabricator,
            ModifyPrefab = prefab =>
            { 
                GameObject model = prefab.gameObject; 
                model.transform.localScale = Vector3.one / 2f;
                PostScaleValue = model.transform.localScale;
                prefab.AddComponent<Pickupable>();
                prefab.AddComponent<HandHeldFabricator>();
                List<TechType> compatbats = new List<TechType>()
                {
                    TechType.Battery,
                    TechType.PrecursorIonBattery
                };
                prefab.AddComponent<HandHeldRelay>().dontConnectToRelays = true;
                PrefabUtils.AddEnergyMixin<HandHeldBatterySource>(prefab, 
                    "'I don't really get why it exists, it just decreases the chance of a collision from like 9.399613e-55% to like 8.835272e-111%, both are very small numbers' - Lee23", 
                    TechType.Battery, compatbats);

            }
        };
        Handheldfab.SetGameObject(clone);
        Handheldfab.SetRecipe(new Nautilus.Crafting.RecipeData()
            {
                craftAmount = 1,
                Ingredients = new List<CraftData.Ingredient>()
                {
                    new(TechType.Titanium, 2),
                    new(TechType.Battery),
                    new(TechType.Copper),
                    new(TechType.JeweledDiskPiece, 2)
                    
                }
            })
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Tools")
            .WithCraftingTime(3f);
        Handheldfab.SetUnlock(TechType.Fabricator);
        Handheldfab.SetEquipment(EquipmentType.Hand);
        Handheldfab.SetPdaGroupCategory(Plugin.equipmentupgrademodules, Plugin.upgradelib);
        
        Handheldfab.Register();
        Plugin.Logger.LogInfo("Handheld fabricator loaded!");
        return;
    }

    public static CraftTree.Type HandheldfabTreeType;
    }

public class HandHeldFabricator : PlayerTool
{
    public Fabricator fab;
    public PowerRelay relay;
    public HandHeldBatterySource battery;
    public override void Awake()
    {
        fab = gameObject.GetComponent<Fabricator>();
        relay = gameObject.GetComponent<PowerRelay>();
        fab.powerRelay = relay;
        battery = gameObject.GetComponent<HandHeldBatterySource>();
        battery.connectedRelay = relay;
        relay.AddInboundPower(battery);
    }
    public override bool OnRightHandDown()
    {
        Plugin.Logger.LogDebug($"OnRightHandDown: {relay.inboundPowerSources.Count},{relay.GetPower()}, {battery.connectedRelay}, {battery.enabled}, {battery.charge}");
            fab.opened = true;
            uGUI.main.craftingMenu.Open(Handheldprefab.HandheldfabGadget.CraftTreeType, fab);
            return true;
    }

    public void Update()
    {
        gameObject.transform.localScale = Handheldprefab.PostScaleValue;
    }

    public override void OnDraw(Player p)
    {
        base.OnDraw(p);
        if (fab.animator == null) return;
        fab.animator.SetBool(AnimatorHashID.open_fabricator, fab.state);
    }
}

public class HandHeldRelay : PowerRelay
{
    public override void Start()
    {
        InvokeRepeating("UpdatePowerState", Random.value, 0.5f);
        lastCanConnect = CanMakeConnection();
        StartCoroutine(UpdateConnectionAsync());
        UpdatePowerState();
        if (WaitScreen.IsWaiting)
        {
            lastPowered = isPowered = true;
            powerStatus = PowerSystem.Status.Normal;
        }
    }
}

public class HandHeldBatterySource : BatterySource
{
    public override void Start()
    {
        RestoreBattery();
        
    }
}