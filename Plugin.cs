using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Nautilus.Handlers;
using Nautilus.Utility;
using UpgradesLIB.Items.Equipment;
using UnityEngine;
using PluginInfo = ToolsUpgradesLIB.PluginInfo;

namespace UpgradesLIB;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
[BepInDependency("com.snmodding.nautilus")]
public class Plugin : BaseUnityPlugin
{
    public new static ManualLogSource Logger { get; private set; }

    public static bool EnableTestNodestatic;

    public Config ConfigOptions;
    
    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
    
    public IEnumerator CreateUpgradesContainer(TechType tech, string storageRootName, string storageRootClassId, int width, int height)
    {
        Logger.LogInfo("Coroutine started for CreateUpgradesContainer(TechType, string, string, int, int)!");//log EVERYTHING
        Logger.LogInfo($"Fetching {tech}'s Prefab...");
        CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(tech);//fetch the prefab
        yield return task;//wait for prefab to finish
        Logger.LogInfo("Prefab Fetched Successfully!");
        GameObject prefab = task.GetResult();//get the prefab
        Logger.LogInfo($"The prefab for {tech} is {prefab}. Creating container for the prefab.");//log it because why not
        PrefabUtils.AddStorageContainer(prefab, storageRootName, storageRootClassId, width, height);//use nautilus's method to create the storage container
        Logger.LogInfo("Storage Container Added. If it opens, the task was successful");//log it
    }

    public void Awake()
    {
        // set project-scoped logger instance
        Logger = base.Logger;

        Logger.LogInfo($"Awake method is running. Dependencies exist. Loading {PluginInfo.PLUGIN_NAME}");
        // Initialize custom prefabs
        InitializePrefabs();

        Logger.LogInfo("Initializing mod options");
        ConfigOptions = OptionsPanelHandler.RegisterModOptions<Config>();
        
        Logger.LogInfo("Methods are the following: CreateUpgradesContainer(TechType (the TechType to operate on), string (name), \n string (what you actually refer to for differences), int (width), int (height)");
        
        // register harmony patches, if there are any
        Harmony.CreateAndPatchAll(Assembly, $"{PluginInfo.PLUGIN_GUID}");
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded! Loading other plugins.");
    }

    public int timer = 0;
    public bool Initialreset = false;
    public PlayerTool HeldTool;
    private void Update()
    {
        timer++;
        if (timer == 5000 && !Initialreset)
        {
            timer = 0;
        }

        if (timer == 500)
        {
            timer = 0;
        }
        
        if (Inventory.main is null)
        {
            if (timer == 0)
            {
                Logger.LogError("No Inventory Loaded!");
            }
            return;
        }

        HeldTool = Inventory.main.GetHeldTool();

        if (HeldTool is null)
        {
            if (timer == 0)
            {
                Logger.LogError("No Tool held!");
            }
        }
        

        if (HeldTool is HandheldFabricator)
        {
            
        }
        
    }

    private void InitializePrefabs()
    {
        Logger.LogInfo($"Initializing prefabs:\n Loading HandHeldFabricator...");
        HandheldFabricator.Register();
        
    }
}