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
        private readonly Dictionary<string, HashSet<IMetricHandler>> _subscriptions;
        private readonly Dictionary<string, DatabaseReference> _databaseReferences;
        private readonly DatabaseReference _clientRef;
        private readonly DatabaseReference _deviceRef;
        private readonly DatabaseReference _deviceSubsRef;

        public SubscriptionManager(FirebaseController firebase, Device credientials, NeurosityUser user)
        {
            _user = user;
            _firebaseIDs = new Dictionary<IMetricHandler, string>();
            _subscriptions = new Dictionary<string, HashSet<IMetricHandler>>();
            _databaseReferences = new Dictionary<string, DatabaseReference>();

            _deviceRef = firebase.NotionDatabase.GetReference($"devices/{credientials.DeviceId}");
            _deviceSubsRef = _deviceRef.Child("subscriptions");
            _clientRef = _deviceRef.Child($"clients/{_deviceSubsRef.Push().Key}");
        }

        public async void Subscribe(IMetricHandler handler)
        {
            await AddFirebaseSubscription(handler);
            string key = $"metrics/{handler.Metric.ToString().ToLower()}/{handler.Label}";

            if (!_subscriptions.ContainsKey(key))
            {
                DatabaseReference databaseRef = _deviceRef.Child(key);
                databaseRef.ValueChanged += DatabaseRef_ValueChanged;
                _databaseReferences.Add(key, databaseRef);

                Debug.Log(databaseRef.Reference);

                HashSet<IMetricHandler> handlers = new HashSet<IMetricHandler>();
                handlers.Add(handler);
                _subscriptions.Add(key, handlers);
            }
            else
            {
                _subscriptions[key].Add(handler);
            }
        }

        public async void Unsubscribe(IMetricHandler handler)
        {
            string key = $"metrics/{handler.Metric.ToString().ToLower()}/{handler.Label}";

            if (_subscriptions.TryGetValue(key, out HashSet<IMetricHandler> handlers))
            {
                if(_firebaseIDs.TryGetValue(handler, out string firebaseId))
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
                databaseRef.Value.ValueChanged -= DatabaseRef_ValueChanged;
            }

            _firebaseIDs.Clear();
            _databaseReferences.Clear();
            _subscriptions.Clear();

            await _clientRef.OnDisconnect().RemoveValue();
        }

        /// <summary>
        /// Adds a subscription reference into the Firebase Database location of the current device.
        /// See deviceStore.js -> creativeDeviceStore -> subscribeToMetric
        /// </summary>
        private async Task AddFirebaseSubscription(IMetricHandler handler)
        {
            var subscriptionInfo = new Dictionary<string, object>
            {
                { "metric", handler.Metric.ToString().ToLower() },
                { "labels", new string[]{ handler.Label} },
                { "atomic", false },
                { "serverType", "firebase" }
            };

            string id = _deviceSubsRef.Push().Key;
            string childPath = $"subscriptions/{id}";
            _firebaseIDs.Add(handler, id);

            await _deviceRef.Child(childPath).SetValueAsync(subscriptionInfo);
            await _deviceRef.Child(childPath).OnDisconnect().RemoveValue();
        }

        private void DatabaseRef_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string fullPath = e.Snapshot.Reference.ToString();
            int delimiter = fullPath.LastIndexOf("metrics");
            string valuePath = fullPath.Substring(delimiter);

            if (_subscriptions.TryGetValue(valuePath, out HashSet<IMetricHandler> handlers))
            {
                foreach (var handler in handlers)
                {
                    if (e.DatabaseError != null)
                    {
                        Debug.LogError(e.DatabaseError.Message);
                        continue;
                    }

                    if (!e.Snapshot.Exists)
                    {
                        Debug.LogError(e.Snapshot.Reference + " doesn't exist.");
                        break;
                    }

                    if (handler.Label != e.Snapshot.Key) continue;

                    string json = e.Snapshot.GetRawJsonValue();
                    if (string.IsNullOrEmpty(json)) continue;

                    handler.Handle(json);
                }
            }
        }
    }
}
