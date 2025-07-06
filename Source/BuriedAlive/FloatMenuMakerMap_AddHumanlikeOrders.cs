using RimWorld;
using Verse;
using Verse.AI;

namespace BuriedAlive;

public class FloatMenuOptionProvider_BuryAlive : FloatMenuOptionProvider
{
    protected override bool Drafted => true;

    protected override bool Undrafted => true;

    protected override bool Multiselect => false;

    protected override bool RequiresManipulation => true;

    protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
    {
        if (!clickedPawn.Downed)
        {
            return null;
        }

        if (!context.FirstSelectedPawn.CanReserveAndReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly, 1, -1, null,
                true))
        {
            return null;
        }

        if (GraveTool.FindGraveFor(clickedPawn, context.FirstSelectedPawn, true) == null)
        {
            return null;
        }

        var taggedString = "BuriedAlive_BuryAliveNew".Translate(clickedPawn.LabelCap);

        return FloatMenuUtility.DecoratePrioritizedTask(
            new FloatMenuOption(taggedString, action, MenuOptionPriority.Default, null, clickedPawn),
            context.FirstSelectedPawn, clickedPawn);

        void action()
        {
            var grave = GraveTool.FindGraveFor(clickedPawn, context.FirstSelectedPawn) ??
                        GraveTool.FindGraveFor(clickedPawn, context.FirstSelectedPawn, true);

            if (grave == null)
            {
                Messages.Message(
                    "BuriedAlive_CannotBuryAlive".Translate() + ": " + "BuriedAlive_NoGrave".Translate(), clickedPawn,
                    MessageTypeDefOf.RejectInput, false);
                return;
            }

            var job = JobMaker.MakeJob(BuriedDefs.BuryAlive, clickedPawn, grave);
            job.count = 1;
            context.FirstSelectedPawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }
    }
}