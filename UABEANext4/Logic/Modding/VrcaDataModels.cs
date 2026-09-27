using System.Collections.Generic;
using Newtonsoft.Json;

namespace UABEANext4.Logic.Modding
{
    public class VrcaDump
    {
        [JsonProperty("meta")]
        public VrcaMeta Meta { get; set; } = new();

        [JsonProperty("parameters")]
        public List<VrcParameter> Parameters { get; set; } = new();

        [JsonProperty("menus")]
        public Dictionary<string, VrcMenu> Menus { get; set; } = new(); // Key is PathID as string
    }

    public class VrcaMeta
    {
        [JsonProperty("unity_version")]
        public string UnityVersion { get; set; } = "";
        
        [JsonProperty("root_menu_path_id")]
        public long RootMenuPathId { get; set; }
        
        [JsonProperty("parameters_path_id")]
        public long ParametersPathId { get; set; }
    }

    public class VrcParameter
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("type")]
        public int Type { get; set; } // 0=Int, 1=Float, 2=Bool

        [JsonProperty("default_val")]
        public float DefaultVal { get; set; }

        [JsonProperty("saved")]
        public bool Saved { get; set; } = true;

        [JsonProperty("synced")]
        public bool Synced { get; set; } = true;
    }

    public class VrcMenu
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("controls")]
        public List<VrcControl> Controls { get; set; } = new();
    }

    public class VrcControl
    {
        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("type")]
        public int Type { get; set; } // VRC SDK Enum

        [JsonProperty("icon_path_id")]
        public long IconPathId { get; set; }

        [JsonProperty("parameter_name")]
        public string Parameter { get; set; } = ""; // Parameter Name, NOT ID

        [JsonProperty("value")]
        public float Value { get; set; }

        [JsonProperty("submenu_path_id")]
        public long SubMenuPathId { get; set; } // 0 if none
        
        [JsonProperty("style")]
        public int Style { get; set; }
    }
}
