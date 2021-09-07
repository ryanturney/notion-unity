using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

namespace Notion.Unity.Example
{
    public class NotionTester : MonoBehaviour
    {
        [SerializeField]
        private Device _device;

        [SerializeField]
        Button _buttonLogin;

        [SerializeField]
        Button _buttonGetDevices;

        [SerializeField]
        Button _buttonGetStatus;

        [SerializeField]
        Button _buttonSubscribeCalm;

        [SerializeField]
        Button _buttonSubscribeFocus;

        FirebaseController _controller;
        Notion _notion;

        private void OnEnable()
        {
            if (_device == null)
            {
                Debug.LogError("Provide a device device instance. Assets -> Create -> Device", this);
                return;
            }
            if (!_device.IsValid)
            {
                Debug.LogError("Provide a valid device.", this);
                return;
            }

            _buttonLogin.onClick.AddListener(() =>
            {
                if (_notion != null && _notion.IsLoggedIn)
                {
                    _notion.Disconnect();
                    SetButtonStates();
                    Debug.Log("Logged out");
                }
                else
                {
                    Login();
                }
            });

            _buttonGetDevices.onClick.AddListener(() => GetDevices());
            _buttonGetStatus.onClick.AddListener(() => GetStatus());
            _buttonSubscribeCalm.onClick.AddListener(() => SubscribeCalm());
            _buttonSubscribeFocus.onClick.AddListener(() => SubscribeFocus());
        }

        private void SetButtonStates()
        {
            _buttonLogin.interactable = true;
            if(_notion.IsLoggedIn)
            {
                _buttonLogin.GetComponentInChildren<Text>().text = "Logout";
            }
            else
            {
                _buttonLogin.GetComponentInChildren<Text>().text = "Login";
            }

            _buttonGetDevices.interactable = _notion.IsLoggedIn;
            _buttonGetStatus.interactable = _notion.IsLoggedIn;
            _buttonSubscribeCalm.interactable = _notion.IsLoggedIn;
            _buttonSubscribeFocus.interactable = _notion.IsLoggedIn;
        }

        public async void Login()
        {
            _controller = new FirebaseController();
            await _controller.Initialize();

            _notion = new Notion(_controller);
            await _notion.Login(_device);

            Debug.Log("Logged in");
            SetButtonStates();
        }

        public async void GetDevices()
        {
            if (!_notion.IsLoggedIn) return;
            var devices = await _notion.GetDevices();
            Debug.Log(JsonConvert.SerializeObject(devices));
        }

        public void GetStatus()
        {
            if (!_notion.IsLoggedIn) return;
            Debug.Log(JsonConvert.SerializeObject(_notion.Status));
        }

        public void SubscribeCalm()
        {
            if (!_notion.IsLoggedIn) return;
            _notion.Subscribe(new CalmHandler());
            Debug.Log("Subscribed to calm");            
        }

        public void SubscribeFocus()
        {
            if (!_notion.IsLoggedIn) return;
            _notion.Subscribe(new FocusHandler());
            Debug.Log("Subscribed to focus");
        }

        private void OnDisable()
        {
            if (_notion == null) return;
            _notion.Disconnect();
            _controller.Logout();
        }
    }
}