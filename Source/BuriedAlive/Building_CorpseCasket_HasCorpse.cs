using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch(typeof(Building_CorpseCasket), nameof(Building_CorpseCasket.HasCorpse), MethodType.Getter)]
public static class Building_CorpseCasket_HasCorpse
{
    public static void Postfix(object __instance, ref bool __result)
    {
        if (__instance is not Building_Grave grave)
        {
            return;
        }

        foreach (var thing in grave.GetDirectlyHeldThings())
        {
            if (thing is not Pawn pawn || pawn.Dead)
            {
                continue;
            }

            __result = true;
            return;
        }
    }
}