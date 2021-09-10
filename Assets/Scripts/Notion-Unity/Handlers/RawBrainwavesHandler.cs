using Newtonsoft.Json;
using UnityEngine;

namespace Notion.Unity
{
    public class RawBrainwavesHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Brainwaves;
        public string Label => "raw";

        public void Handle(string metricData)
        {
            Epoch epoch = JsonConvert.DeserializeObject<Epoch>(metricData);
            Debug.Log($"Handling raw brainwaves, data points: {epoch.Data.Length}");
        }
    }
}