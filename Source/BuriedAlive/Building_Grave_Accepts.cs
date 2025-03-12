using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch(typeof(Building_Grave), nameof(Building_Grave.Accepts))]
public static class Building_Grave_Accepts
{
    public static void Postfix(ref Thing thing, ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (thing is Pawn)
        {
            __result = true;
        }
    }
}