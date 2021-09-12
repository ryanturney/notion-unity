using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class BrainwavesPSDHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Brainwaves;
        public string Label => "psd";

        private readonly StringBuilder _builder;

        public BrainwavesPSDHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            PSD psd = JsonConvert.DeserializeObject<PSD>(metricData);

            _builder.AppendLine("Handling PSD Brainwaves")
                .Append("Label: ").AppendLine(psd.Label)
                .Append("Notch Frequency: ").AppendLine(psd.Info.NotchFrequency)
                .Append("Sampling Rate: ").AppendLine(psd.Info.SamplingRate.ToString())
                .Append("Star Time: ").AppendLine(psd.Info.StartTime.ToString());

            Debug.Log(_builder.ToString());
            _builder.Clear();
        }
    }
}