using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch(typeof(Building_Casket), nameof(Building_Casket.TickRare))]
public static class Building_Casket_TickRare
{
    public static void Postfix(object __instance)
    {
        if (__instance is not Building_Grave grave)
        {
            return;
        }

        if (grave.ContainedThing is not Pawn pawn)
        {
            return;
        }

        if (pawn.Dead)
        {
            return;
        }

        var part = pawn.health.hediffSet.GetNotMissingParts().Where(record => record.coverageAbs > 0).RandomElement();
        pawn.TakeDamage(new DamageInfo(DamageDefOf.Crush, 15f, 99999f, -1f, grave, part, null,
            DamageInfo.SourceCategory.ThingOrUnknown, null, true, false));
    }
}