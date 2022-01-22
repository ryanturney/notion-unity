using System.Collections.Generic;

namespace Notion.Unity
{
    public class Kinesis
    {
        public float Confidence { get; set; }

        public string Label { get; set; }

        public string Metric { get; set; }

        public IEnumerable<BaseMetric> Predictions { get; set; }

        public int Streak { get; set; }

        public long Timestamp { get; set; }
    }
}
