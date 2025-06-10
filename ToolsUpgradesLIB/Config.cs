using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UpgradesLIB
{
    [Menu("UpgradesLIB")]
    public class Config : ConfigFile
    {
        [Toggle("Enable Debug Mode? (requires restart)"), OnChange(nameof(ToggleChangeEvent))]
        public bool DebugMode = false;

        public static bool debugMode = false;
        
        public void ToggleChangeEvent(ToggleChangedEventArgs newbind)
        {
            debugMode = newbind.Value;
        }
    }
    
}