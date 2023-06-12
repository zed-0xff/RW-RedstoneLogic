using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public enum DetectType { None, Any, Pawns, Light, Heavy };

public class CompPressurePlate : CompRedstonePower {
    CompProperties_PressurePlate Props => (CompProperties_PressurePlate)props;

    public override void CompTick(){
        if( !TransmitsPower ) return;

        var things = parent.Map.thingGrid
            .ThingsListAtFast(parent.Position)
            .Where(t => t != parent);

        switch( Props.type ){
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
                    if( Props.type == DetectType.Heavy )
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
            "type: " + Props.type;
    }
}

