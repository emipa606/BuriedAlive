using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch(typeof(Building_Grave), nameof(Building_Grave.GetInspectString))]
public static class Building_Grave_GetInspectString
{
    public static bool Prefix(Building_Grave __instance, ref string __result)
    {
        if (__instance.ContainedThing is not Pawn pawn)
        {
            return true;
        }

        if (pawn.Dead)
        {
            return true;
        }

        __result = "BuriedAlive_Dying".Translate(pawn.LabelCap);
        return false;
    }
}