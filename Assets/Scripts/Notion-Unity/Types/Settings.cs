using Newtonsoft.Json;
using System.Collections.Generic;

namespace Notion.Unity
{
    public class Settings
    {
        public bool ActivityLogging { get; set; }
        [JsonProperty("ble")]
        public bool BluetoothLowEnergy { get; set; }
        public bool HapticsSystem { get; set; }
        [JsonProperty("lsl")]
        public bool LabStreamingLayer { get; set; }
        [JsonProperty("osc")]
        public bool OpenSoundControl { get; set; }
        public bool SupportAccess { get; set; }

        public Dictionary<string, object> ToDictionary()
        {
            return new Dictionary<string, object>
            {
                { "activityLogging", ActivityLogging },
                { "ble", BluetoothLowEnergy},
                { "hapticsSystem", HapticsSystem },
                { "lsl", LabStreamingLayer },
                { "osc", OpenSoundControl },
                { "supportAccess", SupportAccess }
            };
        }
    }
}