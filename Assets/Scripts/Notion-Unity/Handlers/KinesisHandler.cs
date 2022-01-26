using Newtonsoft.Json;
using System;
using System.Linq;
using UnityEngine;

namespace Notion.Unity
{
    public class KinesisHandler : IMetricHandler
    {
        public Metrics Metric => Metrics.Kinesis;

        public string Label { get; set; }

        public Action<float> OnKinesisUpdated { get; set; }

        public void Handle(string json)
        {
            Kinesis metric = JsonConvert.DeserializeObject<Kinesis>(json);
            var prediction = metric.Predictions.FirstOrDefault();

            if(prediction != null)
            {
                Debug.Log($"Handling {metric.Label} : Prediction: {prediction.Probability}");
                OnKinesisUpdated?.Invoke(prediction.Probability);
            }
        }
    }
}