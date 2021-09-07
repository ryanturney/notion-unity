using Newtonsoft.Json;
using UnityEngine;

namespace Notion.Unity
{
    public class CalmHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Awareness;
        public string Label => "calm";

        public void Handle(string json)
        {
            BaseMetric metric = JsonConvert.DeserializeObject<BaseMetric>(json);
            Debug.Log($"Handling {metric.Label} : {metric.Probability}");
        }
    }
}