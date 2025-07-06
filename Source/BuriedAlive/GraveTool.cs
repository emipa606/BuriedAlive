using RimWorld;
using Verse;
using Verse.AI;

namespace BuriedAlive;

public static class GraveTool
{
    public static Building_Grave FindGraveFor(Pawn victim, Pawn carrier, bool ignoreOtherReservations = false)
    {
        var position = victim.Position;
        var map = victim.Map;
        var thingReq = ThingRequest.ForDef(ThingDefOf.Grave);
        if (BuriedAlive.MassGravesLoaded)
        {
            thingReq = ThingRequest.ForGroup(ThingRequestGroup.Grave);
        }

        const PathEndMode peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(carrier);
        const float maxDistance = 9999f;

        var grave = (Building_Grave)GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams,
            maxDistance, validator);
        return grave;

        bool validator(Thing building)
        {
            if (!carrier.CanReserve(building, 1, -1, null, ignoreOtherReservations))
            {
                return false;
            }

            var buildingGrave = (Building_Grave)building;
            if (buildingGrave.AssignedPawn != null && buildingGrave.AssignedPawn != victim)
            {
                return false;
            }

            return GraveEmpty(building);
        }
    }

    public static bool GraveEmpty(Thing building)
    {
        if (building.def.defName == "Grave")
        {
            if (((Building_Grave)building).HasAnyContents)
            {
                return false;
            }
        }

        if (!BuriedAlive.MassGravesLoaded || !building.def.defName.StartsWith("MassGrave"))
        {
            return true;
        }

        return (bool)BuriedAlive.MassGraveAcceptCorpses.Invoke(building, null);
    }
}