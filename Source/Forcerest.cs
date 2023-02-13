using System;
using UnityEngine;
using Verse;

namespace ForceRest
{
    [StaticConstructorOnStartup]
    public class Startup
    {
        static Startup()
        {
            HarmonyPatches.Init();
        }
    }
}