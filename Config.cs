using Nautilus.Json;
using Nautilus.Options;
using Nautilus.Options.Attributes;
using UnityEngine;

namespace UpgradesLIB
{
    [Menu("UpgradesLIB")]
    public class Config : ConfigFile
    {
        [Toggle("Enable Debug Mode? (requires restart)")]
        public bool DebugMode = false;
    }
    
}