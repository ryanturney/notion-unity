using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Notion.Unity
{
    public class SubscriptionManager
    {
        private readonly NeurosityUser _user;
        private readonly Dictionary<IMetricHandler, string> _firebaseIDs;
        private readonly Dictionary<string, HashSet<IMetricHandler>> _subscriptionsMetrics;
        private readonly HashSet<ISettingsHandler> _subscriptionsSettings;
        private readonly Dictionary<string, DatabaseReference> _databaseReferences;
        private readonly DatabaseReference _clientRef;
        private readonly DatabaseReference _deviceRef;
        private readonly DatabaseReference _deviceSubsRef;

        public SubscriptionManager(FirebaseController firebase, Device credientials, NeurosityUser user)
        {
            _user = user;
            _firebaseIDs = new Dictionary<IMetricHandler, string>();
            _subscriptionsSettings = new HashSet<ISettingsHandler>();
            _subscriptionsMetrics = new Dictionary<string, HashSet<IMetricHandler>>();
            _databaseReferences = new Dictionary<string, DatabaseReference>();

            _deviceRef = firebase.NotionDatabase.GetReference($"devices/{credientials.DeviceId}");
            _deviceSubsRef = _deviceRef.Child("subscriptions");
            _clientRef = _deviceRef.Child($"clients/{_deviceSubsRef.Push().Key}");
        }

        public void Subscribe(ISettingsHandler handler)
        {
            if (_subscriptionsSettings.Contains(handler)) return;
            _deviceRef.Child("settings").ValueChanged += HandleSettingsValueChanged;
            _subscriptionsSettings.Add(handler);
        }

        public void Unsubscribe(ISettingsHandler handler)
        {
            _subscriptionsSettings.Remove(handler);
        }

        public async void Subscribe(IMetricHandler handler)
        {
            if (!await CanSubscribe(handler)) return;

            bool isAtomic = IsAtomic(handler);
            await AddFirebaseSubscription(handler, isAtomic);
            string key = $"metrics/{handler.Metric.GetMetricDescription()}";
            if (!isAtomic) key += $"/{handler.Label}";

            if (!_subscriptionsMetrics.ContainsKey(key))
            {
                DatabaseReference databaseRef = _deviceRef.Child(key);
                databaseRef.ValueChanged += HandleMetricValueChanged;
                _databaseReferences.Add(key, databaseRef);

                Debug.Log(databaseRef.Reference);

                HashSet<IMetricHandler> handlers = new HashSet<IMetricHandler>();
                handlers.Add(handler);
                _subscriptionsMetrics.Add(key, handlers);
            }
            else
            {
                _subscriptionsMetrics[key].Add(handler);
            }
        }

        public async void Unsubscribe(IMetricHandler handler)
        {
            string key = $"metrics/{handler.Metric.GetMetricDescription()}/{handler.Label}";

            if (_subscriptionsMetrics.TryGetValue(key, out HashSet<IMetricHandler> handlers))
            {
                if (_firebaseIDs.TryGetValue(handler, out string firebaseId))
                {
                    await _deviceRef.Child($"subscriptions/{firebaseId}").RemoveValueAsync();
                }

                bool success = handlers.Remove(handler);
                Debug.Log($"Removed {key} - {success}");
            }
        }

        public async Task Dispose()
        {
            foreach (var databaseRef in _databaseReferences)
            {
                databaseRef.Value.ValueChanged -= HandleMetricValueChanged;
            }

            _firebaseIDs.Clear();
            _databaseReferences.Clear();
            _subscriptionsMetrics.Clear();

            await _clientRef.OnDisconnect().RemoveValue();
        }

        private async Task<bool> CanSubscribe(IMetricHandler handler)
        {
            bool canSubscribe = true;
            switch (handler.Metric)
            {
                case Metrics.Accelerometer:
                    var selectedDevice = await _user.GetSelectedDevice();
                    canSubscribe = selectedDevice.ModelVersion > 2;
                    break;
            }

            return canSubscribe;
        }

        private bool IsAtomic(IMetricHandler handler)
        {
            return string.IsNullOrWhiteSpace(handler.Label);
        }

        /// <summary>
        /// Adds a subscription reference into the Firebase Database location of the current device.
        /// See deviceStore.js -> creativeDeviceStore -> subscribeToMetric
        /// </summary>
        private async Task AddFirebaseSubscription(IMetricHandler handler, bool isAtomic = false)
        {
            var subscriptionInfo = new Dictionary<string, object>
            {
                { "metric", handler.Metric.GetMetricDescription() },
                { "labels", isAtomic ? new string[]{ string.Empty } : new string[] { handler.Label } },
                { "atomic", isAtomic },
                { "serverType", "firebase" }
            };

            string id = _deviceSubsRef.Push().Key;
            string childPath = $"subscriptions/{id}";
            _firebaseIDs.Add(handler, id);

            await _deviceRef.Child(childPath).SetValueAsync(subscriptionInfo);
            await _deviceRef.Child(childPath).OnDisconnect().RemoveValue();
        }

        private void HandleMetricValueChanged(object sender, ValueChangedEventArgs args)
        {
            string fullPath = args.Snapshot.Reference.ToString();
            int delimiter = fullPath.LastIndexOf("metrics");
            string valuePath = fullPath.Substring(delimiter);

            if (_subscriptionsMetrics.TryGetValue(valuePath, out HashSet<IMetricHandler> handlers))
            {
                foreach (var handler in handlers)
                {
                    if (args.DatabaseError != null)
                    {
                        Debug.LogError(args.DatabaseError.Message);
                        continue;
                    }

                    if (!args.Snapshot.Exists) continue;

                    string json = args.Snapshot.GetRawJsonValue();
                    if (string.IsNullOrEmpty(json)) continue;

                    handler.Handle(json);
                }
            }
        }

        private void HandleSettingsValueChanged(object sender, ValueChangedEventArgs args)
        {
            foreach(var handler in _subscriptionsSettings)
            {
                if (args.DatabaseError != null)
                {
                    Debug.LogError(args.DatabaseError.Message);
                    continue;
                }

                if (!args.Snapshot.Exists) continue;
                string json = args.Snapshot.GetRawJsonValue();
                if (string.IsNullOrEmpty(json)) continue;

                handler.Handle(json);
            }
        }
    }
}