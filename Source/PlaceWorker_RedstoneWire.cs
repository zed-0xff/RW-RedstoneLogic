using RimWorld;
using Verse;

namespace RedstoneLogic;

class PlaceWorker_RedstoneWire : PlaceWorker {

    bool hasComps(ThingDef td){
        return td.comps != null && td.comps.Any(c => typeof(CompRedstonePower).IsAssignableFrom(c.compClass));
    }

    public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
    {
        foreach( Thing t in loc.GetThingList(map) ){
            if( hasComps(t.def) )
                return false;

            if( t.def.entityDefToBuild != null && t.def.entityDefToBuild is ThingDef td && hasComps(td) )
                return false;
        }

        return true;
    }
}

