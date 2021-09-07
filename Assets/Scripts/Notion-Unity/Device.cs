using UnityEngine;

namespace Notion.Unity
{
    [CreateAssetMenu]
    public class Device : ScriptableObject
    {
        [SerializeField]
        private string _email;

        [SerializeField]
        private string _password;

        [SerializeField]
        private string _deviceId;

        public string Email => _email;
        public string Password => _password;
        public string DeviceId => _deviceId;

        public bool IsValid => 
            !string.IsNullOrEmpty(_email) && 
            !string.IsNullOrEmpty(_password) && 
            !string.IsNullOrEmpty(DeviceId);
    }
}