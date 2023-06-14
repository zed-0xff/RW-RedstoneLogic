using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public enum DetectType { None, Any, Pawns, Light, Heavy, Auto };

public class CompPressurePlate : CompRedstonePower {
    CompProperties_PressurePlate Props => (CompProperties_PressurePlate)props;

    DetectType type;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        type = Props.type;

        if( type == DetectType.Auto && parent.Stuff != null ){
            if( parent.Stuff == ThingDefOf.Gold )
                type = DetectType.Light;
            else if( parent.Stuff == ThingDefOf.Steel )
                type = DetectType.Heavy;
            else if( parent.Stuff.stuffProps.categories.Contains(StuffCategoryDefOf.Stony) )
                type = DetectType.Pawns;
            else
                type = DetectType.Any;
        }
    }

    public override void CompTick(){
        if( !TransmitsPower ) return;

        var things = parent.Map.thingGrid
            .ThingsListAtFast(parent.Position)
            .Where(t => t != parent);

        switch( type ){
            case DetectType.Any:
                if( things.Any() ){
                    powerLevel = MaxPower;
                    lastPoweredTick = Find.TickManager.TicksGame;
                }
                break;
            case DetectType.Pawns:
                if( things.Any(t => t is Pawn p && !p.Dead) ){
                    powerLevel = MaxPower;
                    lastPoweredTick = Find.TickManager.TicksGame;
                }
                break;
            case DetectType.Light:
            case DetectType.Heavy:
                int count = things.Sum((Thing t) => t.stackCount);
                if( count > 0 ){
                    if( type == DetectType.Heavy )
                        count = (int)Mathf.Ceil(count/10f);
                    powerLevel = Mathf.Min( MaxPower, count );
                    lastPoweredTick = Find.TickManager.TicksGame;
                }
                break;
        }

        PushNext();
        base.CompTick();
    }

    public override string CompInspectStringExtra(){
        return base.CompInspectStringExtra() + "\n" +
            "type: " + type;
    }
}

