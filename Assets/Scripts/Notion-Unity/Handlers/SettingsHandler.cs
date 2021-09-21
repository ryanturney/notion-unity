using Newtonsoft.Json;
using UnityEngine;

namespace Notion.Unity
{
    public class SettingsHandler : ISettingsHandler
    {
        public void Handle(string data)
        {
            Debug.Log(data);
            //Debug.Log(JsonConvert.DeserializeObject<Settings>(data));
        }
    }
}