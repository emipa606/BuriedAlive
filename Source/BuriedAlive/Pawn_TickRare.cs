using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BuriedAlive;

[HarmonyPatch(typeof(Pawn), nameof(Pawn.TickRare))]
public static class Pawn_TickRare
{
    public static void Postfix(Pawn __instance)
    {
        if (__instance.Dead)
        {
            return;
        }

        if (__instance.ParentHolder is not { } parentHolder)
        {
            return;
        }

        if (parentHolder is not Building_Grave grave)
        {
            return;
        }

        var part = __instance.health.hediffSet.GetNotMissingParts().Where(record => record.coverageAbs > 0)
            .RandomElement();
        __instance.TakeDamage(new DamageInfo(DamageDefOf.Crush, 15f, 99999f, -1f, grave, part, null,
            DamageInfo.SourceCategory.ThingOrUnknown, null, true, false));
    }
}