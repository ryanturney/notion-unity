using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notion.Unity
{
    public class NeurosityUser
    {
        public bool IsLoggedIn { get; private set; }
        public string UserId { get; private set; }

        private readonly FirebaseController _firebase;
        private readonly FirebaseUser _firebaseUsers;
        private readonly DatabaseReference _devicesReference;

        public NeurosityUser(FirebaseUser firebaseUser, FirebaseController firebase)
        {
            _firebase = firebase;
            _firebaseUsers = firebaseUser;
            UserId = _firebaseUsers.UserId;
            _devicesReference = _firebase.NotionDatabase.GetReference($"users/{UserId}/devices");
        }

        public async Task<IEnumerable<DeviceInfo>> GetDevices()
        {
            var devicesSnapshot = await _devicesReference.GetValueAsync();

            Dictionary<string, object> registeredDevices = devicesSnapshot.Value as Dictionary<string, object>;
            if (registeredDevices == null) return null;

            var deviceKeys = registeredDevices.Keys;
            List<DeviceInfo> devicesInfo = new List<DeviceInfo>(deviceKeys.Count);

            foreach (string deviceId in deviceKeys)
            {
                var infoSnapshot = await _firebase.NotionDatabase.
                    GetReference($"devices/{deviceId}/info").GetValueAsync();

                string json = infoSnapshot.GetRawJsonValue();
                DeviceInfo info = JsonConvert.DeserializeObject<DeviceInfo>(json);
                devicesInfo.Add(info);
            }

            return devicesInfo;
        }

        public async Task<DeviceInfo> GetSelectedDevice()
        {
            var devices = await GetDevices();
            return devices.FirstOrDefault();
        }

        public async Task<DeviceStatus> GetSelectedDeviceStatus()
        {
            var selectedDevice = await GetSelectedDevice();

            var statusSnapshot = await _firebase.NotionDatabase.
                GetReference($"devices/{selectedDevice.DeviceId}/status").GetValueAsync();
            string json = statusSnapshot.GetRawJsonValue();

            return JsonConvert.DeserializeObject<DeviceStatus>(json);
        }
        public async Task RemoveDevice(string deviceId)
        {
            string claimedByPath = $"devices/{deviceId}/status/claimedBy";
            string userDevicePath = $"users/{UserId}/devices/{deviceId}";
            var claimedByRef = _firebase.NotionDatabase.GetReference(claimedByPath);
            var userDeviceRef = _firebase.NotionDatabase.GetReference(userDevicePath);

            await claimedByRef.RemoveValueAsync();
            await userDeviceRef.RemoveValueAsync();
        }
    }
}