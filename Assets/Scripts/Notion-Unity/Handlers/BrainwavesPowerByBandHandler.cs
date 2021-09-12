using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class BrainwavesPowerByBandHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Brainwaves;
        public string Label => "powerByBand";

        private readonly StringBuilder _builder;

        public BrainwavesPowerByBandHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            PowerByBand powerByBand = JsonConvert.DeserializeObject<PowerByBand>(metricData);

            _builder.AppendLine("Handling Power By Band Brainwaves")
                .Append("Label: ").AppendLine(powerByBand.Label)
                .Append("Has Alpha: ").AppendLine((powerByBand.Data.Alpha.Length > 0).ToString())
                .Append("Has Beta: ").AppendLine((powerByBand.Data.Beta.Length > 0).ToString())
                .Append("Has Delta: ").AppendLine((powerByBand.Data.Delta.Length > 0).ToString())
                .Append("Has Gamma: ").AppendLine((powerByBand.Data.Gamma.Length > 0).ToString())
                .Append("Has Theta: ").AppendLine((powerByBand.Data.Theta.Length > 0).ToString());

            Debug.Log(_builder.ToString());
            _builder.Clear();
        }
    }
}