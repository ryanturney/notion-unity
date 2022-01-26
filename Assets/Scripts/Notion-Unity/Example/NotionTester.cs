using Newtonsoft.Json;
using System.Threading.Tasks;
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

        [SerializeField]
        Button _buttonSubscribeRawBrainwaves;

        [SerializeField]
        Button _buttonSubscribeAccelerometer;

        [SerializeField]
        Button _buttonSubscribeKinesis;

        [SerializeField]
        Text _textKinesisProbability;

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
                _buttonLogin.interactable = false;

                if (_notion != null && _notion.IsLoggedIn)
                {
                    Logout();
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
            _buttonSubscribeRawBrainwaves.onClick.AddListener(() => SubscribeBrainwaves());
            _buttonSubscribeAccelerometer.onClick.AddListener(() => SubscribeAccelerometer());
            _buttonSubscribeKinesis.onClick.AddListener(() => SubscribeKinesis(kinesisLabel: "leftArm"));
        }

        private void SetButtonStates()
        {
            _buttonLogin.interactable = true;

            string loginButtonText = _notion.IsLoggedIn ? "Logout" : "Login";
            _buttonLogin.GetComponentInChildren<Text>().text = loginButtonText;
            _buttonGetDevices.interactable = _notion.IsLoggedIn;
            _buttonGetStatus.interactable = _notion.IsLoggedIn;
            _buttonSubscribeCalm.interactable = _notion.IsLoggedIn;
            _buttonSubscribeFocus.interactable = _notion.IsLoggedIn;
            _buttonSubscribeRawBrainwaves.interactable = _notion.IsLoggedIn;
            _buttonSubscribeAccelerometer.interactable = _notion.IsLoggedIn;
            _buttonSubscribeKinesis.interactable = _notion.IsLoggedIn;
            _textKinesisProbability.text = _notion.IsLoggedIn ? string.Empty : "[KINESIS PROBABILITY]";
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

        public async void Logout()
        {
            await _notion.Logout();
            SetButtonStates();
            _controller = null;
            _notion = null;

            Debug.Log("Logged out");
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

        public void SubscribeBrainwaves()
        {
            if (!_notion.IsLoggedIn) return;
            _notion.Subscribe(new BrainwavesRawHandler());
            Debug.Log("Subscribed to raw brainwaves");
        }

        public void SubscribeAccelerometer()
        {
            if (!_notion.IsLoggedIn) return;
            _notion.Subscribe(new AccelerometerHandler());
            Debug.Log("Subscribed to accelerometer");
        }

        /// <summary>
        /// Add kinesisLabel based on the thought you're training.
        /// For instance: leftArm, rightArm, leftIndexFinger, etc
        /// </summary>
        /// <param name="kinesisLabel"></param>
        public void SubscribeKinesis(string kinesisLabel)
        {
            if (!_notion.IsLoggedIn) return;

            _notion.Subscribe(new KinesisHandler
            {
                Label = kinesisLabel,
                OnKinesisUpdated = (probability) => {
                    _textKinesisProbability.text = $"{kinesisLabel} : {probability}";
                }
            });
        }

        private async void OnDisable()
        {
            if (_notion == null) return;
            if (!_notion.IsLoggedIn) return;

            // Wrapping because Logout is meant to be invoked and forgotten about for use in button callbacks.
            await Task.Run(() => Logout());
            Debug.Log($"Logged out from {nameof(OnDisable)}");
        }
    }
}