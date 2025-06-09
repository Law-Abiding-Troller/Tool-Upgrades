using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UpgradesLIB.Items.Equipment;
using UnityEngine;
using PluginInfo = ToolsUpgradesLIB.PluginInfo;

namespace UpgradesLIB; // remember link: https://github.com/tinyhoot/Nautilus/blob/debugging-tutorial/Nautilus/Documentation/guides/debugging.md for debugging
/*
 * Remember:
 * CHECK THE GOD DAMN COMPONENT REQUIREMENTS
 * requirements contain anything that CANNOT BE NULL. NRE otherwise
 * Keywords:
 * Fields with AssertNotNull attribute
 * Fields not public (that don't have the NonSerialized attribute)
 * Public properties
 * "'I don't really get why it exists, it just decreases the chance of a collision from like 9.399613e-55% to like 8.835272e-111%, both are very small numbers' - Lee23"
 * Todo list:
 */
[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    public static bool EnableTestNodestatic;

    public Config ConfigOptions;
    
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    
    public static IEnumerator CreateUpgradesContainer(TechType tech, string storageRootName, string storageRootClassId, int width, int height, TechType[] allowedTechTypes = null, bool preventDeconstuctionIfNotEmpty = false)
    {
        Logger.LogInfo($"Coroutine started for CreateUpgradesContainer(TechType, string, string, int, int, TechType[], bool)!");//log EVERYTHING
        Logger.LogInfo($"Fetching {tech}'s Prefab...");
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(tech);//fetch the prefab
        yield return task;//wait for prefab to finish
        Logger.LogInfo("Prefab Fetched Successfully!");
        GameObject prefab = task.GetResult();//get the prefab
        Logger.LogInfo($"The prefab for {tech} is {prefab}. Creating container for the prefab.");//log it because why not
        PrefabUtils.AddStorageContainer(prefab, storageRootName, storageRootClassId, width, height, preventDeconstuctionIfNotEmpty).container.SetAllowedTechTypes(allowedTechTypes);//use nautilus's method to create the storage container
        Logger.LogInfo("Storage Container Added. If it opens, the task was successful");//log it
    }
    public static TechGroup toolupgrademodules = EnumHandler.AddEntry<TechGroup>("ToolsUpgrades")
        .WithPdaInfo("Tools Upgrade Modules");
    public static TechGroup equipmentupgrademodules = EnumHandler.AddEntry<TechGroup>("EquipmentUpgrades")
        .WithPdaInfo("Equipment Upgrade Modules");
    public static TechCategory upgradelib = EnumHandler.AddEntry<TechCategory>("UpgradesLIB").WithPdaInfo("UpgradesLIB").RegisterToTechGroup(equipmentupgrademodules);
    public void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        Logger.LogInfo($"Awake method is running. Dependencies exist. Loading {PluginInfo.PLUGIN_NAME}");
        
        // Initialize custom prefabs
        InitializePrefabs();
        
        Logger.LogInfo("Initializing mod options");
        ConfigOptions = OptionsPanelHandler.RegisterModOptions<Config>();
        
        Logger.LogInfo("Methods are the following: ");
        Logger.LogInfo("(Coroutine) CreateUpgradesContainer(TechType (the TechType to operate on), string (name), string (what you actually refer to for differences), int (width), int (height)), TechType[] (what you want to be the allowed tech, not required), bool (Prevent deconstruction if not empty, not required)");
        
        if (ConfigOptions.DebugMode)
        {
            Logger.LogInfo("Debug Mode Enabled. Begin counting.");
            Logger.LogWarning("This WILL clutter your log! Only keep this enabled if you are actually debugging!");
        }
        
        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_NAME}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded! Loading other plugins." );
    }

    public int timer = 0;
    public bool Initialreset = false;
    public static Fabricator HeldTool;
    private void Update()
    {
        timer++;
        if (timer == 5000 && !Initialreset)
        {
            timer = 0;
        }

        if (ConfigOptions.DebugMode)
        {
            Logger.LogDebug(timer.ToString());
        }
        if (timer == 500)
        {
            timer = 0;
        }
        
        if (Inventory.main is null)
        {
            if (timer == 0)
            {
                Logger.LogError("No Inventory Loaded!" );
            }
            return;
        }
        
        if (Inventory.main.GetHeldTool() is HandHeldFabricator)
        { 
            Inventory.main.GetHeldTool().transform.localScale = Handheldprefab.PostScaleValue;
        }
        
    }

    private void InitializePrefabs()
    {
        Logger.LogInfo($"Initializing prefabs: " );
        Logger.LogInfo("Loading HandHeldFabricator..." );
        Handheldprefab.Register();
        
    }
}