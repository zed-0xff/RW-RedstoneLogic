using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse.AI;
using Verse.Sound;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

public class CompPiston : CompRedstonePowerReceiver {
    CompProperties_Piston Props => (CompProperties_Piston)props;

    float openPct;
    protected float OpenPct => openPct;
    public bool IsWalkable => openPct < 0.5f;

    IntVec3 direction, pistonCell;
    bool pushedThisCycle, pulledThisCycle;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        direction = IntVec3.South.RotatedBy(parent.Rotation);
        pistonCell = parent.Position + direction;
        CompCache<CompPiston>.Add(this, parent.Map, pistonCell);
    }

    public override void PostDeSpawn(Map map){
        CompCache<CompPiston>.Remove(this);
        base.PostDeSpawn(map);
    }

    public override void Notify_Teleported(){
        base.Notify_Teleported();
        pistonCell = parent.Position + direction;
        CompCache<CompPiston>.Remove(this);
        CompCache<CompPiston>.Add(this, parent.Map, pistonCell);
    }

    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( src.parent.Position == pistonCell )
            return false;

        return base.TryPushPower(amount, src);
    }

    Thing GetBlocker_Storage(Building_Storage bs){
        IntVec3 nextCell = pistonCell + direction;

        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(pistonCell)){
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.breaks )
                    continue;
                else
                    return t;
            }

            if( !t.def.selectable )
                continue;

            if( t is Building b ){
                if( t.def.fillPercent == 0 && t.def.passability == Traversability.Standable ){
                    // wires
                    continue;
                }
                return b;
            }

            if( t is Plant ){
                continue;
            }
            if( t is Filth )
                continue;
            if( t is Blueprint )
                continue;
            if( t is Pawn )
                continue;
            if( t is Frame )
                return t;

            if( t.def.EverStorable(willMinifyIfPossible: false) && bs.Accepts(t)
                    && nextCell.GetItemCount(parent.Map) < nextCell.GetMaxItemsAllowedInCell(parent.Map) )
                continue;

            return t;
        }
        return null;
    }

    public Thing GetBlocker(){
        IntVec3 nextCell = pistonCell + direction;

        bool toWall = false;
        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(nextCell) ){
            if( t is Building_Storage bs ){
                return GetBlocker_Storage(bs);
            }
            if( t.def.passability == Traversability.Impassable )
                toWall = true;
        }

        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(pistonCell)){
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.moveable || ext.breaks )
                    continue;
                else
                    return t;
            }

            if( !t.def.selectable )
                continue;

            if( t is Building b ){
                if( t.def.fillPercent == 0 && t.def.passability == Traversability.Standable ){
                    // wires
                    continue;
                }
                var pComp = b.TryGetComp<CompPiston>();
                if( pComp != null ){
                    if( pComp.OpenPct > 0 )
                        return b;
                    else
                        continue;
                }
            }

            if( t is Plant ){
                continue;
            }
            if( t is Filth )
                continue;
            if( t is Blueprint )
                continue;
            if( t is Pawn )
                continue;
            if( t is Frame )
                return t;

            if( t.def.EverStorable(willMinifyIfPossible: false) ){
                if( toWall )
                    return t;
                else
                    continue;
            }

            return t;
        }

        return null;
    }

    public bool CanExtend(){
        if( !TransmitsPower ) return false;
        return GetBlocker() == null;
    }

    void TryHarvestPlant(Plant plant){
        float statValue = 5;
        int num = plant.YieldNow();
        num = GenMath.RoundRandom((float)num * statValue);
        if (num > 0) {
            Thing thing = ThingMaker.MakeThing(plant.def.plant.harvestedThingDef);
            thing.stackCount = num;
            GenPlace.TryPlaceThing(thing, plant.Position, parent.Map, ThingPlaceMode.Near);
            plant.def.plant.soundHarvestFinish.PlayOneShot(new TargetInfo(plant));
        }
    }

    bool hasWallAt(IntVec3 pos){
        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(pos) ){
            if( t.def.passability == Traversability.Impassable )
                return true;
        }
        return false;
    }

    void MoveThing(Thing t, IntVec3 newPos, HashSet<Thing> processedThings, ExtPistonMoveable ext = null){
        if( processedThings.Contains(t) ) return;
        processedThings.Add(t);

        bool toWall = hasWallAt(newPos);

        if( t is Pawn pawn ){
            if( toWall ){
                var dt = new BattleLogEntry_DamageTaken(pawn, VDefOf.DamageEvent_Piston);
                Find.BattleLog.Add(dt);
                float dmg = Props.damageRange.RandomInRange;
                //Log.Warning("[d] [" + Find.TickManager.TicksGame + "] " + this + " damaging " + pawn + " on " + dmg + " dmg");
                var dinfo = new DamageInfo(DamageDefOf.Blunt, GenMath.RoundRandom(dmg), 0f, -1f, null, null, null, DamageInfo.SourceCategory.Collapse);
                dinfo.SetBodyRegion(BodyPartHeight.Middle, BodyPartDepth.Outside);
                pawn.TakeDamage(dinfo).AssociateWithLog(dt);
                dinfo.SetAmount(dmg/4);
                parent.TakeDamage(dinfo);
            } else {
                pawn.Position = newPos;
                pawn.Notify_Teleported();
            }
            return;
        }

        // prevent flicking when 2 pistons push against each other
        CompPiston piston2 = CompCache<CompPiston>.Get(newPos, parent.Map);
        if( piston2 != null && piston2 != this && !piston2.IsWalkable )
            return;

        if( t is Building_Storage bs ){
            // e.g. Blocky.Frame, or chest
            IntVec3 dir = newPos - bs.Position;
            List<Thing> things = bs.GetSlotGroup().HeldThings.ToList();
            bs.DeSpawn(); // need to update private cachedOccupiedCells
            bs.Position = newPos;
            bs.SpawnSetup(parent.Map, false);
            for( int i=0; i<things.Count; i++){
                if( processedThings.Contains(things[i]) ) continue;

                // piston will already push all things, but sticky piston will not pull items from ground
                // so this is needed for sticky piston to pull whole containers with all the items within
                processedThings.Add(things[i]);

                if( bs.Accepts(things[i]) )
                    things[i].Position += dir;
            }
            bs.Notify_SettingsChanged();
            return;
        }

        if( ext != null && ext.respawn ){
            t.DeSpawn();
            t.Position = newPos;
            t.SpawnSetup(parent.Map, false);
        } else {
            if( toWall )
                return;
            t.Position = newPos;
        }

        var pComp = t.TryGetComp<CompRedstonePower>();
        if( pComp != null ){
            pComp.Notify_Teleported();
        }
    }

    void PushThingsToStorage(Building_Storage bs){
        IntVec3 nextCell = pistonCell + direction;
        HashSet<Thing> processedThings = new HashSet<Thing>();

        List<Thing> things = parent.Map.thingGrid.ThingsListAtFast(pistonCell);
        for(int i=0; i<things.Count; i++){ // 'for' cycle prevents 'Collection was modified' exception
            Thing t = things[i];
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.breaks ){
                    t.Destroy(DestroyMode.KillFinalize);
                }
                continue;
            }

            if( !t.def.selectable )
                continue;

            if( t is Plant plant ){
                if( t.def.selectable ){
                    TryHarvestPlant(plant);
                    plant.Destroy(DestroyMode.KillFinalize);
                } else
                    continue;
            }
            if( t is Filth )
                continue;
            if( t is Blueprint )
                continue;
            if( t is Pawn p ){
                MoveThing(t, nextCell, processedThings);
                continue;
            }

            if( t.def.EverStorable(willMinifyIfPossible: false) ){
                if( bs.Accepts(t) && nextCell.GetItemCount(parent.Map) < nextCell.GetMaxItemsAllowedInCell(parent.Map) )
                    MoveThing(t, nextCell, processedThings);
                continue;
            }
        }
    }

    void PushThings(){
        if( pushedThisCycle ) return;
        pushedThisCycle = true;

        IntVec3 nextCell = pistonCell + direction;
        HashSet<Thing> processedThings = new HashSet<Thing>();

        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(nextCell) ){
            if( t is Building_Storage bs ){
                PushThingsToStorage(bs);
                return;
            }
        }

        List<Thing> things = parent.Map.thingGrid.ThingsListAtFast(pistonCell);
        for(int i=0; i<things.Count; i++){ // 'for' cycle prevents 'Collection was modified' exception
            Thing t = things[i];
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.breaks ){
                    t.Destroy(DestroyMode.KillFinalize);
                    continue;
                }
                if( ext.moveable ){
                    MoveThing(t, nextCell, processedThings, ext);
                    continue;
                }
            }

            if( !t.def.selectable )
                continue;

            if( t is Building b ){
                if( t.def.fillPercent == 0 && t.def.passability == Traversability.Standable ){
                    // wires
                    continue;
                }
                var pComp = b.TryGetComp<CompPiston>();
                if( pComp != null && pComp.OpenPct == 0 ){
                    b.Position = nextCell;
                    pComp.Notify_Teleported();
                    continue;
                }
            }

            if( t is Plant plant ){
                if( t.def.selectable ){
                    TryHarvestPlant(plant);
                    plant.Destroy(DestroyMode.KillFinalize);
                } else
                    continue;
            }
            if( t is Filth )
                continue;
            if( t is Blueprint )
                continue;
            if( t is Pawn p ){
                MoveThing(t, nextCell, processedThings);
                continue;
            }

            if( t.def.EverStorable(willMinifyIfPossible: false) ){
                // intentionally not checking GetMaxItemsAllowedInCell here
                MoveThing(t, nextCell, processedThings);
                continue;
            }
        }
    }

    void PullThings(){
        if( pulledThisCycle ) return;
        pulledThisCycle = true;

        IntVec3 nextCell = pistonCell + direction;
        HashSet<Thing> processedThings = new HashSet<Thing>();

        List<Thing> things = parent.Map.thingGrid.ThingsListAtFast(nextCell);
        for(int i=0; i<things.Count; i++){ // 'for' cycle prevents 'Collection was modified' exception
            Thing t = things[i];
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.moveable ){
                    MoveThing(t, pistonCell, processedThings, ext);
                    continue;
                }
            }

            if( t is Building b ){
                var pComp = b.TryGetComp<CompPiston>();
                if( pComp != null && pComp.OpenPct == 0 ){
                    b.Position = pistonCell;
                    pComp.Notify_Teleported();
                    continue;
                }
            }
        }
    }

    void TickWorker(){
        if( !CanExtend() ){
            openPct = 0;
            return;
        }

        // TODO: sound
        if( HasPowerRelaxed ){
            pulledThisCycle = false;
            if( openPct < 1f ){
                openPct += 1f / Props.baseSpeed;
                if( openPct > 1f ) openPct = 1f;
                if( openPct > 0.5f ){
                    PushThings();
                }
            }
        } else {
            pushedThisCycle = false;
            if( openPct > 0f ){
                openPct -= 1f / Props.baseSpeed;
                if( openPct < 0f ) openPct = 0f;
                if( openPct < 0.5f && Props.sticky ){
                    PullThings();
                }
            }
        }
    }

    public override void CompTick(){
        base.CompTick();
        bool prevWalkable = IsWalkable;
        TickWorker();
        bool curWalkable = IsWalkable;

        if( prevWalkable != curWalkable ){
            // XXX maybe expensive with many pistons
            // TODO: optimize, maybe check only every 4th tick
            parent.Map.pathing.RecalculatePerceivedPathCostAt(pistonCell);
        }
    }

	public override void PostDraw(){
        base.PostDraw();
        if( !parent.Spawned ) return;

        Vector3 vector = new Vector3(1, 0, 0);
        Rot4 rotation = parent.Rotation;
        rotation.Rotate(RotationDirection.Clockwise);
        vector = rotation.AsQuat * vector;
        Vector3 drawPos = parent.DrawPos;
        drawPos += vector * OpenPct;

//        parent.DrawAt(parent.DrawPos);

        if( OpenPct >= Props.longSize ){
            Props.longGraphicData.Graphic.Draw(drawPos + Props.longGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
        } else {
            Props.shortGraphicData.Graphic.Draw(drawPos + Props.shortGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
            if( parent.Rotation == Rot4.North && Props.shaftGraphicData != null && openPct > 0 ){
                // cuz shaft should go in hole and head should remain on top )
                Props.shaftGraphicData.Graphic.Draw(
                        drawPos + Props.shaftGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
            }
        }
    }

    public override string CompInspectStringExtra(){
        string r = base.CompInspectStringExtra();
        if( Prefs.DevMode ){
            r += "\nOpenPct: " + OpenPct + ", CanExtend: " + CanExtend() + ", speed: " + Props.baseSpeed;
        }
        if( TransmitsPower ){
            Thing t = GetBlocker();
            if( t != null ){
                r += "\n" + "BlockedBy".Translate(t).CapitalizeFirst();
            }
        }
        return r;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref openPct, "openPct");
        Scribe_Values.Look(ref pushedThisCycle, "pushedThisCycle");
        Scribe_Values.Look(ref pulledThisCycle, "pulledThisCycle");
    }
}

