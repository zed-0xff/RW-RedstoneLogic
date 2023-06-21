using RimWorld;
using Verse;
using Blocky.Core;
using UnityEngine;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class CompRedstonePower : ThingComp {
    public const int MaxPower = 15;

    protected int powerLevel;
    protected int lastPoweredTick;
    bool prevOn;

    static Graphic wireGraphic = GraphicDatabase.Get<Graphic_RedstoneWire>(
            "RedstoneLogic/RedstoneWire_Atlas", ShaderDatabase.MetaOverlay, Vector2.one,
            new Color(1, 0, 0, 0.3f));

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

    // had power in this or previous tick
    public bool HasPowerRelaxed => Find.TickManager.TicksGame - lastPoweredTick < 2;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        CompCache<CompRedstonePower>.Add(this);
    }

    public override void PostDeSpawn(Map map){
        CompCache<CompRedstonePower>.Remove(this);
    }

    public virtual void Notify_Teleported(){
        CompCache<CompRedstonePower>.Remove(this);
        CompCache<CompRedstonePower>.Add(this);
    }

    protected void PushNext(CompRedstonePower src = null){
        foreach (IntVec3 cell in GenAdj.CellsAdjacentCardinal(parent)){
            CompRedstonePower neighbor = CompCache<CompRedstonePower>.Get(cell, parent.Map);
            if( neighbor is CompRedstonePowerReceiver pt && pt != src ){
                pt.TryPushPower(PowerLevel, this);
            }
        }
    }

    public override void CompTick(){
        if( !TransmitsPower ){
            powerLevel = 0;
            return;
        }

        if( Find.TickManager.TicksGame - lastPoweredTick > 0 ){
            powerLevel = 0;
        }

        bool isOn = (powerLevel > 0);
        if( isOn != prevOn ){
            prevOn = isOn;
            // for glowers and power switch
            parent.BroadcastCompSignal(isOn ? "PowerTurnedOn" : "PowerTurnedOff");
        }
    }

    public void CompPrintForOverlay(SectionLayer layer) {
        wireGraphic.Print(layer, parent, 0);
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

