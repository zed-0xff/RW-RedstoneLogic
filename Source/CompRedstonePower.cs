using RimWorld;
using System;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

public class CompRedstonePower : ThingComp {
    protected int powerLevel;
    protected int lastPoweredTick;
    bool prevOn;

    CompProperties_RedstonePower Props => (CompProperties_RedstonePower)props;

    public virtual int PowerLevel {
        get { return powerLevel < 0 ? 0 : powerLevel; }
    }

    public virtual bool TransmitsPower {
        get {
            if (!ThingUtility.DestroyedOrNull(parent)) {
                return parent.Spawned;
            }
            return false;
        }
    }

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        CompCache<CompRedstonePower>.Add(this);
    }

    public override void PostDeSpawn(Map map){
        CompCache<CompRedstonePower>.Remove(this);
    }

    public virtual void TryPushPower(int amount){
        if( amount >= powerLevel ){
            powerLevel = Math.Max(amount, powerLevel);
            lastPoweredTick = Find.TickManager.TicksGame;
        }
    }

    public override void CompTick(){
        base.CompTick();
        if( !TransmitsPower ) return;

        if( Find.TickManager.TicksGame - lastPoweredTick > 1 && powerLevel > 0 ){
            powerLevel--;
        }

        bool isOn = (powerLevel > 0);
        if( isOn != prevOn ){
            prevOn = isOn;
            // only for glower
            parent.BroadcastCompSignal(isOn ? "PowerTurnedOn" : "PowerTurnedOff");
        }

        if( PowerLevel <= 1 ) return;

        foreach (IntVec3 cell in GenAdj.CellsAdjacentCardinal(parent)){
            CompRedstonePower neighbor = CompCache<CompRedstonePower>.Get(cell, parent.Map);
            if( neighbor == null ) continue;

            neighbor.TryPushPower(PowerLevel-1);
        }
    }

    public override string CompInspectStringExtra(){
        return "power level: " + PowerLevel;
    }

    // FIXME: scribe
}

