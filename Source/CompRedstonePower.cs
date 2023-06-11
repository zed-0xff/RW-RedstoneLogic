using RimWorld;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

public class CompRedstonePower : ThingComp {
    public const int MaxPower = 15;

    protected int powerLevel;
    protected int lastPoweredTick;
    bool prevOn;

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

        int loss = this is CompRedstonePowerTransmitter ? 1 : 0;
        if( PowerLevel <= loss ) return;

        foreach (IntVec3 cell in GenAdj.CellsAdjacentCardinal(parent)){
            CompRedstonePower neighbor = CompCache<CompRedstonePower>.Get(cell, parent.Map);
            if( neighbor is CompRedstonePowerTransmitter pt ){
                pt.TryPushPower(PowerLevel - loss);
            }
        }
    }

    public override string CompInspectStringExtra(){
        return "power level: " + PowerLevel;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref powerLevel, "powerLevel", 0);
        Scribe_Values.Look(ref lastPoweredTick, "lastPoweredTick", 0);
    }
}

