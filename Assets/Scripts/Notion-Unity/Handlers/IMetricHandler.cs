namespace Notion.Unity
{
    public interface IMetricHandler
    {
        Metrics Metric { get; }
        string Label { get; }
        void Handle(string metricData);
    }
}