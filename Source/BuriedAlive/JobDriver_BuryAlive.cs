using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace BuriedAlive;

public class JobDriver_BuryAlive : JobDriver_HaulToContainer
{
    private Effecter graveDigEffect;
    protected Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;

    protected Building_Grave Grave => (Building_Grave)job.GetTarget(TargetIndex.B).Thing;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(Takee, job, 1, -1, null, errorOnFailed) &&
               pawn.Reserve(Grave, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);
        this.FailOnDestroyedOrNull(TargetIndex.B);
        this.FailOnAggroMentalState(TargetIndex.A);
        this.FailOn(() => !Grave.Accepts(Takee));
        var goToTakee = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.OnCell)
            .FailOnDestroyedNullOrForbidden(TargetIndex.A).FailOnDespawnedNullOrForbidden(TargetIndex.B)
            .FailOn(() => !GraveTool.GraveEmpty(Grave)).FailOn(() => !Takee.Downed)
            .FailOn(() => !pawn.CanReach(Takee, PathEndMode.OnCell, Danger.Deadly))
            .FailOnSomeonePhysicallyInteracting(TargetIndex.A);
        var startCarryingTakee = Toils_Haul.StartCarryThing(TargetIndex.A);
        var goToThing = Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.ClosestTouch);
        yield return Toils_Jump.JumpIf(goToThing, () => pawn.IsCarryingPawn(Takee));
        yield return goToTakee;
        yield return startCarryingTakee;
        yield return goToThing;
        var toil = Toils_General.Wait(500, TargetIndex.B);
        toil.FailOnCannotTouch(TargetIndex.B, PathEndMode.ClosestTouch);
        toil.WithProgressBarToilDelay(TargetIndex.B);
        var workEffecter = WorkEffecter;
        if (workEffecter != null)
        {
            toil.WithEffect(workEffecter, TargetIndex.B);
        }

        var workSustainer = WorkSustainer;
        if (workSustainer != null)
        {
            toil.PlaySustainerOrSound(workSustainer);
        }

        var destThing = job.GetTarget(TargetIndex.B).Thing;
        toil.tickAction = delegate
        {
            if (pawn.IsHashIntervalTick(80) && graveDigEffect == null)
            {
                graveDigEffect = EffecterDefOf.BuryPawn.Spawn();
                graveDigEffect.Trigger(destThing, destThing);
            }

            var effecter = graveDigEffect;
            if (effecter == null)
            {
                return;
            }

            effecter.EffectTick(destThing, destThing);
        };
        ModifyPrepareToil(toil);
        yield return toil;
        yield return new Toil
        {
            initAction = delegate { Grave.TryAcceptThing(Takee); },
            defaultCompleteMode = ToilCompleteMode.Instant
        };
    }
}