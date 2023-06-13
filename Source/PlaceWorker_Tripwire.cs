using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public class PlaceWorker_Tripwire : PlaceWorker {
    public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null) {
        Map currentMap = Find.CurrentMap;
        if (def.HasComp(typeof(CompTripwire))) {
            TripwireUtility.DrawLinesToNearbyHooks(def, center, rot, currentMap, thing);
        }
    }
}

