using Newtonsoft.Json;
using System.Text;
using UnityEngine;

namespace Notion.Unity
{
    public class SignalQualityHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.SignalQuality;
        public string Label => string.Empty;

        private StringBuilder _builder;

        public SignalQualityHandler()
        {
            _builder = new StringBuilder();
        }

        public void Handle(string metricData)
        {
            var channelQuality = JsonConvert.DeserializeObject<ChannelQuality[]>(metricData);
            foreach(var channel in channelQuality)
            {
                _builder.Append("Quality - ").AppendLine(channel.Status.ToString());
            }

            Debug.Log(_builder.ToString());
            _builder.Clear();
        }
    }
}