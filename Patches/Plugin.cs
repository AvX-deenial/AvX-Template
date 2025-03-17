using BepInEx;
using GorillaLocomotion.Swimming;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace AVTemp.Patches
{
    [Description(PlugInfo.Description)]
    [BepInPlugin(PlugInfo.GUID, PlugInfo.Name, PlugInfo.Version)]
    public class HarmonyPatches : BaseUnityPlugin
    {
        private void OnEnable()
        {
            Menu.ApplyHarmonyPatches();
        }

        private void OnDisable()
        {
            Menu.RemoveHarmonyPatches();
        }
    }
    /*[System.ComponentModel.Description(PluginInfo.Description)]
    <color=white>[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        private void Start() // To that one dude that uses SMI to inject my menu, it's this method
        {
            Console.Title = "Fluxx Menu // Build " + PluginInfo.Version;

            FluxxMenu.Patches.Menu.ApplyHarmonyPatches();
            GameObject Loading = new GameObject("fluxx");
            Loading.AddComponent<DagonMenu.Menu.Main>();
            Loading.AddComponent<FluxxMenu.Notifications.Notifs>();
            Loading.AddComponent<DagonMenu.Classes.covid>();
            UnityEngine.Object.DontDestroyOnLoad(Loading);
        }
    }*/
}
