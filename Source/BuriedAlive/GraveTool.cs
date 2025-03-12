using RimWorld;
using Verse;
using Verse.AI;

namespace BuriedAlive;

public static class GraveTool
{
    public static Building_Grave FindGraveFor(Pawn p, Pawn traveler, bool ignoreOtherReservations = false)
    {
        var position = p.Position;
        var map = p.Map;
        var thingReq = ThingRequest.ForDef(ThingDefOf.Grave);
        var peMode = PathEndMode.ClosestTouch;
        var traverseParams = TraverseParms.For(traveler);
        var maxDistance = 9999f;

        var grave = (Building_Grave)GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams,
            maxDistance, Validator);
        return grave;

        bool Validator(Thing x)
        {
            return !((Building_Grave)x).HasAnyContents &&
                   traveler.CanReserve(x, 1, -1, null, ignoreOtherReservations) &&
                   (((Building_Grave)x).AssignedPawn == null || ((Building_Grave)x).AssignedPawn == p);
        }
    }
}