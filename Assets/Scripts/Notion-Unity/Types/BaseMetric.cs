namespace Notion.Unity
{
    public class BaseMetric
    {
        public string Label { get; set; }
        public string Metric { get; set; }
        public float Probability { get; set; }
        public long Timestamp { get; set; }
    }
}