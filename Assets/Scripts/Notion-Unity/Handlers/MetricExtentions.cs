using System.ComponentModel;

namespace Notion.Unity
{
    public static class MetricExtentions
    {
        public static string GetMetricDescription(this Metrics metric)
        {
            string metricName = metric.ToString();
            var info = metric.GetType().GetField(metricName);
            var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];
            
            string description;
            if (attributes != null && attributes.Length > 0) description = attributes[0].Description;
            else description = metricName.ToLower();

            return description;
        }
    }
}