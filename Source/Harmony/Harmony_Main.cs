using HarmonyLib;

namespace ForceRest
{
    [HarmonyPatch]
    static partial class HarmonyPatches
    {
        public static void Init()
        {
            var harmony = new Harmony("com.forcerest.jesterromut.mod");
            harmony.PatchAll();
        }
    }
}