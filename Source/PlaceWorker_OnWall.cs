using RimWorld;
using Verse;

namespace RedstoneLogic;

public class PlaceWorker_OnWall : PlaceWorker {
    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map,
            Thing thingToIgnore = null, Thing thing = null)
    {
        foreach (Thing thingHere in map.thingGrid.ThingsListAtFast(loc)){
            if( thingHere == thingToIgnore ) continue;

            if( thingHere.def == checkingDef ){
                return "IdenticalThingExists".Translate();
            }

            if (thingHere.def.entityDefToBuild == checkingDef ){
                if (thingHere is Blueprint) {
                    return new AcceptanceReport("IdenticalBlueprintExists".Translate());
                }
                return new AcceptanceReport("IdenticalThingExists".Translate());
            }
        }

        if (loc.GetThingList(map).Exists(IsWall))
            return true;

        return false;
    }

    static bool IsWall(Thing thing)
    {
        if (thing.def is { building: { isPlaceOverableWall: true } } or { IsSmoothed: true })
            return true;

        if (thing.def.defName.Contains("Wall"))
            return true;

        if (thing.def == VDefOf.Blocky_Block)
            return true;

        return false;
    }
}

