using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using Verse;
using Building_Grave = RimWorld.Building_Grave;

namespace BuriedAlive;

[HarmonyPatch]
public static class Building_Grave_Accepts
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Building_Grave), nameof(Building_Grave.Accepts));

        if (BuriedAlive.MassGravesLoaded)
        {
            yield return AccessTools.Method("MassGravesContinued.Building_MassGrave:Accepts");
        }
    }

    public static void Postfix(object __instance, Thing thing, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (__instance is not Building_Grave grave)
        {
            return;
        }

        if (thing is not Pawn pawn || pawn.Dead)
        {
            return;
        }

        if (grave.AssignedPawn != null)
        {
            return;
        }

        __result = true;
    }
}