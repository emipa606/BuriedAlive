using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch]
public static class Building_MassGrave_GetInspectString
{
    public static bool Prepare()
    {
        return BuriedAlive.MassGravesLoaded;
    }

    public static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method("MassGravesContinued.Building_MassGrave:GetInspectString");
    }

    public static void Postfix(object __instance, ref string __result)
    {
        if (__instance is not Building_Grave grave)
        {
            return;
        }

        if (!grave.HasAnyContents)
        {
            return;
        }

        foreach (var thing in grave.GetDirectlyHeldThings())
        {
            if (thing is not Pawn { Dead: false })
            {
                continue;
            }

            __result += Environment.NewLine + "BuriedAlive_Dying".Translate(thing.LabelCap);
        }
    }
}