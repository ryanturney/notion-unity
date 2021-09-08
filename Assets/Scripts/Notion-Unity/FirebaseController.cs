using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System;
using UnityEngine;
using System.Threading.Tasks;

namespace Notion.Unity
{
    public class FirebaseController
    {
        public FirebaseApp App { get; private set; }
        public FirebaseApp NotionApp { get; private set; }
        public FirebaseDatabase NotionDatabase { get; private set; }
        public FirebaseAuth NotionAuth { get; private set; }

        public async Task Initialize()
        {
            var status = await FirebaseApp.CheckAndFixDependenciesAsync();

            if (status == DependencyStatus.Available)
            {
                App = FirebaseApp.DefaultInstance;

                var notionOptions = new AppOptions
                {
                    ApiKey = "AIzaSyB0TkZ83Fj0CIzn8AAmE-Osc92s3ER8hy8",
                    DatabaseUrl = new Uri("https://neurosity-device.firebaseio.com"),
                    ProjectId = "neurosity-device",
                    StorageBucket = "neurosity-device.appspot.com",
                    MessageSenderId = "212595049674"
                };
                NotionApp = FirebaseApp.Create(notionOptions, "notion");
                NotionDatabase = FirebaseDatabase.GetInstance(NotionApp);
                NotionAuth = FirebaseAuth.GetAuth(NotionApp);
                Debug.Log("Initialized Firebase");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
            }
        }

        public async Task<FirebaseUser> Login(Device credentials)
        {
            return await NotionAuth.SignInWithEmailAndPasswordAsync(credentials.Email, credentials.Password);
        }

        public void Logout()
        {
            NotionDatabase.GoOffline();
            NotionAuth.SignOut();
            NotionAuth.Dispose();
            NotionApp.Dispose();
            App.Dispose();

            NotionDatabase = null;
            NotionAuth = null;
            NotionApp = null;
            App = null;
        }
    }
}