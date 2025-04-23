using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
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
        HandheldfabGadget = Handheldfab.CreateFabricator(out HandheldfabTreeType)
            .AddTabNode("Equipment", "Equipment", SpriteManager.Get(TechType.Fabricator))
            .AddTabNode("Tools", "Tools", SpriteManager.Get(TechType.Fabricator))
            .AddCraftNode(HandheldfabInfo.TechType, "Tools");
            
        var clone = new FabricatorTemplate(HandheldfabInfo, HandheldfabTreeType)
        {
            FabricatorModel = FabricatorTemplate.Model.Fabricator,
            ModifyPrefab = prefab =>
            { 
                GameObject model = prefab.gameObject; 
                model.transform.localScale = Vector3.one / 2;
                PostScaleValue = model.transform.localScale;
                prefab.AddComponent<Pickupable>();
                prefab.AddComponent<HandHeldFabricator>();
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
        
        Handheldfab.Register();
        Plugin.Logger.LogInfo("Handheld fabricator loaded!");
        return;
    }

    public static CraftTree.Type HandheldfabTreeType;
    }

public class HandHeldFabricator : PlayerTool
{
    public override void OnDraw(Player p)
    {
        if (Plugin.HeldTool.transform.localScale != Handheldprefab.PostScaleValue)
        {
            Plugin.HeldTool.transform.localScale = Handheldprefab.PostScaleValue;
        }
        base.OnDraw(p);
    }
    public override bool OnRightHandDown()
    {
        if (Inventory.main.GetHeldTool() is HandHeldFabricator && Inventory.main is not null)
        {
            Plugin.HeldTool = Inventory.main.GetHeldTool().GetComponent<Fabricator>();
            uGUI.main.craftingMenu.Open(Handheldprefab.HandheldfabGadget.CraftTreeType, Plugin.HeldTool);
            return true;
        }

        return false;
    }
}