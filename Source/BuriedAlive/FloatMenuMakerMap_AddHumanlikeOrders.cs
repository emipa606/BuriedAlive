using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace BuriedAlive;

[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
public static class FloatMenuMakerMap_AddHumanlikeOrders
{
    public static void Postfix(ref Vector3 clickPos, ref Pawn pawn, ref List<FloatMenuOption> opts)
    {
        foreach (var localTargetInfo in GenUI.TargetsAt(clickPos, TargetingParameters.ForRescue(pawn), true))
        {
            var victim = (Pawn)localTargetInfo.Thing;
            if (!victim.Downed ||
                !pawn.CanReserveAndReach(victim, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, true) ||
                GraveTool.FindGraveFor(victim, pawn, true) == null)
            {
                continue;
            }

            string text = "BuriedAlive_BuryAlive".Translate(localTargetInfo.Thing.LabelCap, localTargetInfo.Thing);
            var jDef = BuriedDefs.BuryAlive;
            var burier = pawn;

            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(
                new FloatMenuOption(text, Action, MenuOptionPriority.Default, null, victim), pawn, victim));
            continue;

            void Action()
            {
                var grave = GraveTool.FindGraveFor(victim, burier);
                if (grave == null)
                {
                    grave = GraveTool.FindGraveFor(victim, burier, true);
                }

                if (grave == null)
                {
                    Messages.Message(
                        "BuriedAlive_CannotBuryAlive".Translate() + ": " + "BuriedAlive_NoGrave".Translate(), victim,
                        MessageTypeDefOf.RejectInput, false);
                    return;
                }

                var job = JobMaker.MakeJob(jDef, victim, grave);
                job.count = 1;
                burier.jobs.TryTakeOrderedJob(job, JobTag.Misc);
            }
        }
    }
}