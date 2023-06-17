using RimWorld;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public class CompButton : CompRedstonePowerGenerator {
    CompProperties_Button Props => (CompProperties_Button)props;

    CompFlickable flickableComp;
    int flickedOnTick;

    public override bool GeneratesPower => Find.TickManager.TicksGame - flickedOnTick < Delay;
    public virtual int Delay {
        get {
            float speed = parent?.GetStatValue(StatDefOf.DoorOpenSpeed) ?? 0f;
            return speed == 0f ? 0 : Mathf.RoundToInt(Props.baseDelay / speed);
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        flickableComp = parent.GetComp<CompFlickable>();
        if( !respawningAfterLoad ){
            flickableComp.ResetToOn();
        }
    }

    public override void CompTick(){
        base.CompTick();
        if( Find.TickManager.TicksGame - flickedOnTick == Delay ){
            flickableComp.ResetToOn();
        }
    }

    public override void ReceiveCompSignal(string signal) {
        base.ReceiveCompSignal(signal);
        if( signal == "FlickedOff" ){
            flickedOnTick = Find.TickManager.TicksGame;
        }
    }

    public override string CompInspectStringExtra(){
        return base.CompInspectStringExtra() + "\n" +
            "delay: " + string.Format("{0} ticks = {1:F2}s", Delay, Delay.TicksToSeconds());
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref flickedOnTick, "flickedOnTick");
    }
}

