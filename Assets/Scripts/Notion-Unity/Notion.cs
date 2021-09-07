using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notion.Unity
{
    public class Notion
    {
        public bool IsLoggedIn { get; private set; }
        public DeviceStatus Status { get; private set; }

        private readonly FirebaseController _firebase;
        private SubscriptionManager _subscriptionManager;
        private NeurosityUser _user;

        public Notion(FirebaseController firebaseController)
        {
            _firebase = firebaseController;
        }

        public async Task Login(Device credientials)
        {
            var u = await _firebase.Login(credientials);
            _user = new NeurosityUser(u, _firebase);
            _subscriptionManager = new SubscriptionManager(_firebase, credientials);

            Status = await _user.GetSelectedDevice();

            IsLoggedIn = true;
        }

        public void Logout()
        {
            _firebase.Logout();
            IsLoggedIn = false;
        }

        public async void Disconnect()
        {
            await _subscriptionManager.Dispose();
            _firebase.NotionDatabase.GoOffline();

            Logout();
            _firebase.NotionAuth.Dispose();
            _firebase.NotionApp.Dispose();
        }

        public async Task<IEnumerable<DeviceInfo>> GetDevices()
        {
            return await _user.GetDevices();
        }

        public async Task<DeviceStatus> GetSelectedDevice()
        {
            return await _user.GetSelectedDevice();
        }

        public void Subscribe(IMetricHandler handler)
        {
            _subscriptionManager.Subscribe(handler);
        }

        public void Unsubscribe(IMetricHandler handler)
        {
            _subscriptionManager.Unsubscribe(handler);
        }
    }
}