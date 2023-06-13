using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RedstoneLogic;

public class CompTripwire : CompRedstonePowerGenerator {
    bool isTriggered;
    List<CompTripwire> links = null;
    Thing triggeredBy = null;
    OverlayHandle? overlayHandle;

    List<CompTripwire> Links {
        get {
            if( links == null ) UpdateLinks();
            return links;
        }
    }

//    public override void PostSpawnSetup(bool respawningAfterLoad){
//        base.PostSpawnSetup(respawningAfterLoad);
//        UpdateLinks();
//        UpdateStatus();
//    }

    public override void CompTick() {
        if( parent.IsHashIntervalTick(200) ){
            UpdateLinks();
        } else if( parent.IsHashIntervalTick(10) ){
            UpdateStatus();
        }

        if( isTriggered ){
            // generate signal
            base.CompTick();
        }
    }

    public void UpdateStatus(){
        isTriggered = false;
        foreach( CompTripwire otherComp in Links ){
            Thing t = GetTriggeredBy(otherComp);
            if( t != null ){
                isTriggered = true;
                triggeredBy = t;
                break;
            }
        }
        UpdateOverlayHandle();
    }

    void UpdateOverlayHandle() {
        if (parent.Spawned) {
            parent.Map.overlayDrawer.Disable(parent, ref overlayHandle);
            if (parent.Spawned && links.Count == 0) {
                overlayHandle = parent.Map.overlayDrawer.Enable(parent, OverlayTypes.Forbidden);
            }
        }
    }

    public void UpdateLinks(){
        if( links == null ){
            links = new List<CompTripwire>();
        } else {
            links.Clear();
        }

		foreach (Thing item in parent.Map.listerThings.ThingsOfDef(VDefOf.TripwireHook)) {
			if( parent != item && item.Faction == Faction.OfPlayer) {
				CompTripwire otherComp = item.TryGetComp<CompTripwire>();
				if ( CanLinkTo(otherComp) ) {
                    links.Add(otherComp);
                }
            }
        }

        UpdateStatus();
    }

    public override void PostDrawExtraSelectionOverlays() {
		foreach( CompTripwire otherComp in Links ){
            if ( CanLinkTo(otherComp) ) {
                var color = GetTriggeredBy(otherComp) != null ? SimpleColor.Red : SimpleColor.White;
                GenDraw.DrawLineBetween(parent.TrueCenter(), otherComp.parent.TrueCenter(), color);
            }
        }
    }

    Thing GetTriggeredBy(CompTripwire other){
        foreach (IntVec3 pos in CellRect.FromLimits(parent.Position, other.parent.Position)){
            if( pos == parent.Position || pos == other.parent.Position ) continue;

            foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(pos) ){
                if( t is Building && t.def.fillPercent == 0 && t.def.passability == Traversability.Standable )
                    continue;
                if( t is Plant && t.def.fillPercent == 0)
                    continue;
                if( t is Filth )
                    continue;
                if( t.def == VDefOf.TripwireHook )
                    continue;
                if( t is Blueprint b )
                    continue;

                return t;
            }
        }
        return null;
    }

    bool CanLinkTo(CompTripwire other) {
        if (this == other || other == null || !parent.Spawned || !other.parent.Spawned) {
            return false;
        }
        return TripwireUtility.CanLinkTo(parent.Position, other);
    }

    public override string CompInspectStringExtra(){
        return base.CompInspectStringExtra() + "\n" +
            "linked to " + Links.Count + " hooks" + "\n" +
            (isTriggered ? ("triggered by " + triggeredBy?.Label) : "not triggered");
    }
}

