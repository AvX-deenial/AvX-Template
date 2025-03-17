using HarmonyLib;
using UnityEngine;

namespace AVTemp.Patches
{
    [HarmonyPatch(typeof(VRRig), "OnDisable", MethodType.Normal)]
    public static class RigPatch
    {
        public static bool Prefix(VRRig __instance)
        {
            return !(__instance == GorillaTagger.Instance.offlineVRRig);
        }
    }

    [HarmonyPatch(typeof(VRRigJobManager), "DeregisterVRRig")]
    public static class RigPatch2
    {
        public static bool Prefix(VRRigJobManager __instance, VRRig rig)
        {
            return !rig.isOfflineVRRig;
        }
    }
}
