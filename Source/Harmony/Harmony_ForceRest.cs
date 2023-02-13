using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ForceRest { 
    static partial class HarmonyPatches
    {
        [HarmonyPatch(typeof(Building_Bed))]
        [HarmonyPatch("GetFloatMenuOptions")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPostfix]
        public static IEnumerable<FloatMenuOption> ForceRestMenu(IEnumerable<FloatMenuOption> values, Building_Bed __instance, Pawn myPawn)
        {
            foreach (FloatMenuOption item in values)
            {
                yield return item;
            }
            if (!__instance.Medical)
            {


                FloatMenuOption option = new FloatMenuOption("UseBedForced".Translate(), delegate
                {
                    if (!__instance.ForPrisoners && myPawn.CanReserveAndReach(__instance, PathEndMode.ClosestTouch, Danger.Deadly, __instance.SleepingSlotsCount, -1, null, ignoreOtherReservations: true))
                    {
                        if (myPawn.CurJobDef == JobDefOf.LayDown && myPawn.CurJob.GetTarget(TargetIndex.A).Thing == __instance)
                        {
                            myPawn.CurJob.restUntilHealed = true;
                            myPawn.CurJob.forceSleep = true;
                        }
                        else
                        {
                            Job job = JobMaker.MakeJob(JobDefOf.LayDown, __instance);
                            job.restUntilHealed = true;
                            job.forceSleep = true;
                            myPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                        }
                        myPawn.mindState.ResetLastDisturbanceTick();
                    }
                });
                yield return option;

            }
        }

        [HarmonyPatch(typeof(ThinkNode_ConditionalMustKeepLyingDown))]
        [HarmonyPatch("Satisfied")]
        [HarmonyPatch(new Type[] { typeof(Pawn) })]
        [HarmonyPrefix]
        public static bool KeepRestForHalfCycler(ref bool __result, Pawn pawn)
        {
            if (pawn.needs?.rest == null && (pawn.CurJob?.forceSleep ?? false))
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}