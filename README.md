# Notion SDK for Unity
This is the beginnings of a crude C# port of [Neurosity JS SDK](https://github.com/neurosity/notion-js) for use within the Unity3D game engine. This was created to demonstrate how an official Notion Unity SDK might work but more importantly how the NodeJS dependency isn't required when working with Unity and in theory other game engines.

## Dependencies
* [Unity 2020.3.15 LTS or newer](https://unity3d.com/get-unity/download/archive)
* [Firebase for Unity Authentication](https://developers.google.com/unity/packages#firebase_authentication)
* [Firebase for Unity Realtime Database](https://developers.google.com/unity/packages#firebase_realtime_database)
* [Json.NET by jilleJr](https://github.com/jilleJr/Newtonsoft.Json-for-Unity)
* [External Dependency Manager](https://developers.google.com/unity/packages#external_dependency_manager_for_unity)

## Features
This is very much a work in progress. It is feature incomplete, there will be bugs, very little error checking and the architecture isn't sound. The implemented features are not guaranteed to have exact 1-to-1 API parity of the JS SDK but that is the eventual goal.

The following list is what has been implemented:
* [Login](https://docs.neurosity.co/docs/reference/classes/notion#login)
* [Logout](https://docs.neurosity.co/docs/reference/classes/notion#logout)
* [Calm](https://docs.neurosity.co/docs/reference/classes/notion#calm)
* [Focus](https://docs.neurosity.co/docs/reference/classes/notion#focus)
* [GetDevices](https://docs.neurosity.co/docs/reference/classes/notion#getdevices)
* [GetSelectedDevice](https://docs.neurosity.co/docs/reference/classes/notion#getselecteddevice)
* [Status](https://docs.neurosity.co/docs/reference/classes/notion#status)

## How to Use
 This has only been tested in the Unity Editor and Android. In theory this should work on iOS as there is no device specific implementations of the notion functionality.
 
 1. Open in Unity 2020.3.15 LTS or newer
 2. Create a Device ScriptableObject instance. `Create -> Assets -> Device` and fill out the ScriptableObject with your credentials and device ID.
 3. Open `Scripts/Notion-Unity/Example/NotionExample`. Select `NotionTester` in the Hierarchy and select your newly created Device asset in the Notion Tester component.
 4. You should now be able to play the NotionExample scene. The buttons in the UI will print all results to the Console.

## Using in Other Projects
Other apps will require your own Firebase project, you can follow [Firebase Documentation](https://firebase.google.com/docs/unity/setup) for help on that. There is a stub setup for this repo but any app developed using the Notion Unity SDK will eventually require you to setup your own Firebase account. This is currently a requirement as the Neurosity tech is built on top of Firebase and the Unity Firebase SDKs require `google-services.json` and `GoogleService-Into.plist` to be unique for each store app.
