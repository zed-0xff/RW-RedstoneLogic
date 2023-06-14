using RimWorld;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace RedstoneLogic;

public class CompPiston : CompRedstonePowerReceiver {
    CompProperties_Piston Props => (CompProperties_Piston)props;

    float openPct;
    public float OpenPct => openPct;

    IntVec3 direction;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        direction = IntVec3.South.RotatedBy(parent.Rotation);
    }

    public override void PostDeSpawn(Map map){
        base.PostDeSpawn(map);
    }

    public Thing GetBlocker(){
        IntVec3 pistonCell = parent.Position + direction;

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
            if( t.def.category != ThingCategory.Item ){
                return t;
            }
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

    void PushThings(){
        IntVec3 pistonCell = parent.Position + direction;
        IntVec3 nextCell = pistonCell + direction;

        List<Thing> things = parent.Map.thingGrid.ThingsListAtFast(pistonCell);
        foreach( Thing t in things ){
            var ext = t.def.GetModExtension<ExtPistonMoveable>();
            if( ext != null ){
                if( ext.breaks ){
                    t.Destroy(DestroyMode.KillFinalize);
                    continue;
                }
                if( ext.moveable ){
                    t.Position = nextCell;
                    var pComp = t.TryGetComp<CompRedstonePower>();
                    if( pComp != null ){
                        pComp.Notify_Teleported();
                    }
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
                p.Position = nextCell;
                p.Notify_Teleported();
                continue;
            }
            if( t.def.category == ThingCategory.Item ){
                t.Position = nextCell;
                continue;
            }
        }
    }

    public override void CompTick(){
        base.CompTick();
        if( !CanExtend() ){
            openPct = 0;
            return;
        }

        // TODO: sound
        if( HasPowerRelaxed ){
            if( openPct < 1f ){
                openPct += 1f / Props.baseSpeed;
                if( openPct > 1f ) openPct = 1f;
            }
        } else {
            if( openPct > 0f ){
                openPct -= 1f / Props.baseSpeed;
                if( openPct < 0f ) openPct = 0f;
            }
        }

        if( openPct > 0.5f ){
            PushThings();
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
        Thing t = GetBlocker();
        if( t != null ){
            r += "\n" + "BlockedBy".Translate(t).CapitalizeFirst();
        }
        return r;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref openPct, "openPct");
    }
}

