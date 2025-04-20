using System.Collections.Generic;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using UnityEngine;

namespace UpgradesLIB.Items.Equipment;

public class HandheldFabricator : PlayerTool
{
    public static CustomPrefab Handheldfab;
    public static PrefabInfo HandheldfabInfo;
    public static CraftTree.Type HandheldfabTreeType;
    public static FabricatorGadget HandheldfabGadget;

    public static void Register()
    {
        HandheldfabInfo = PrefabInfo.WithTechType("HandHeldToolsFabricator", "Hand Held Upgrades Fabricator",
            "The hand held fabricator for Upgrades that are specific to tools and equipment.").WithIcon(SpriteManager.Get(TechType.Fabricator));
        Handheldfab = new CustomPrefab(HandheldfabInfo);
        var clone = new CloneTemplate(HandheldfabInfo, TechType.Fabricator)
        {
            ModifyPrefab = prefab => prefab.gameObject.AddComponent<Pickupable>()
        };
        clone.ModifyPrefab += obj =>
        {
            var model = obj.gameObject;
            model.transform.localScale = Vector3.one / 1.5f;
        };
        Handheldfab.SetGameObject(clone);
        Handheldfab.SetRecipe(new Nautilus.Crafting.RecipeData()
        {
            craftAmount = 1,
            Ingredients = new List<CraftData.Ingredient>()
            {
                new (TechType.Titanium, 2),
                new (TechType.Battery),
                new (TechType.Copper),
                new (TechType.JeweledDiskPiece, 2)
            }
        })
        .WithFabricatorType(CraftTree.Type.Fabricator)
        .WithStepsToFabricatorTab("Personal","Tools")
        .WithCraftingTime(3f);
        Handheldfab.SetUnlock(TechType.Fabricator);
        Handheldfab.SetEquipment(EquipmentType.Hand);
        HandheldfabGadget = Handheldfab.CreateFabricator(out HandheldfabTreeType)
            .AddTabNode("Equipment", "Equipment", SpriteManager.Get(TechType.Fabricator))
            .AddTabNode("Tools", "Tools", SpriteManager.Get(TechType.Fabricator))
            .AddCraftNode(TechType.HighCapacityTank, "Equipment")
            .AddCraftNode(TechType.Builder, "Tools");
        
        
        
        
        
        
        
        
        Handheldfab.Register();
        Plugin.Logger.LogInfo("Handheld fabricator loaded!");
        return;
    }
}