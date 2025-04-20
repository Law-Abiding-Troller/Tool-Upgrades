using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UpgradesLIB
{
    [Menu("UpgradesLIB")]
    public class Config : ConfigFile
    {
        [Toggle("Enable Test Mode?")]
        public static bool EnableTestNode = true;
        [Keybind("Open Fabricator for HandHeldFabricator")]
        public KeyCode OpenFabKey = KeyCode.Mouse0;
    }
    
}