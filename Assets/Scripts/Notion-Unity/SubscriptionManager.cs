using Firebase.Database;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Notion.Unity
{
    public class SubscriptionManager
    {
        private Dictionary<string, HashSet<IMetricHandler>> _subscriptions;
        private Dictionary<string, DatabaseReference> _databaseReferences;

        private readonly DatabaseReference _clientRef;
        private readonly DatabaseReference _deviceRef;

        public SubscriptionManager(FirebaseController firebase, Device credientials)
        {
            _subscriptions = new Dictionary<string, HashSet<IMetricHandler>>();
            _databaseReferences = new Dictionary<string, DatabaseReference>();

            _deviceRef = firebase.NotionDatabase.GetReference($"devices/{credientials.DeviceId}");
            string clientId = _deviceRef.Child("subscriptions").Push().Key;
            _clientRef = _deviceRef.Child($"clients/{clientId}");
        }

        public void Subscribe(IMetricHandler handler)
        {
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

        public void Unsubscribe(IMetricHandler handler)
        {
            string key = $"metrics/{handler.Metric.ToString().ToLower()}/{handler.Label}";

            if (_subscriptions.TryGetValue(key, out HashSet<IMetricHandler> handlers))
            {
                bool success = handlers.Remove(handler);
                Debug.Log($"Removed {key} - {success}");
            }
        }

        public async Task Dispose()
        {
            foreach(var subscription in _subscriptions)
            {
                subscription.Value.Clear();
            }

            foreach(var databaseRef in _databaseReferences)
            {
                databaseRef.Value.ValueChanged -= DatabaseRef_ValueChanged;
                try
                {
                    await databaseRef.Value.OnDisconnect().RemoveValue();
                }
                catch { }
            }

            _databaseReferences.Clear();
            _subscriptions.Clear();

            await _clientRef.OnDisconnect().RemoveValue();
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
                    if(e.DatabaseError != null)
                    {
                        Debug.LogError(e.DatabaseError.Message);
                        continue;
                    }

                    if(!e.Snapshot.Exists)
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
