using RimWorld;
using System.Collections.Generic;
using Verse;
using Blocky.Core;
using UnityEngine;

namespace RedstoneLogic;

public class CompComparator : CompRedstonePowerReceiver {
    enum Mode { Compare, Subtract, Measure };

    struct Levels : IExposable {
        public int l,r,c,tick;

        public void ExposeData() {
            Scribe_Values.Look(ref l, "l");
            Scribe_Values.Look(ref r, "r");
            Scribe_Values.Look(ref c, "c");
            Scribe_Values.Look(ref tick, "tick");
        }
    };

    Levels[] values = new Levels[2];
    int pushTick;
    Mode mode;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        if( !respawningAfterLoad ){
            values = new Levels[2];
        }
    }

    // 1. push power from queue if not pushed this turn
    void idempotentTick(){
        if( pushTick != Find.TickManager.TicksGame ){
            pushTick = Find.TickManager.TicksGame;

            powerLevel = 0;
            var levels = values[1 - pushTick%2];

            switch( mode ){
                case Mode.Compare:
                    if( levels.tick == pushTick-1 && levels.l <= levels.c && levels.r <= levels.c ){
                        powerLevel = levels.c;
                    }
                    break;
                case Mode.Subtract:
                    if( levels.tick == pushTick-1 ){
                        // https://minecraft.fandom.com/wiki/Redstone_Comparator#Subtract_signal_strength
                        powerLevel = Mathf.Max(levels.c - Mathf.Max(levels.l, levels.r), 0);
                    }
                    break;
            }

            if( powerLevel > 0 ){
                IntVec3 posOut = parent.Position + IntVec3.North.RotatedBy(parent.Rotation);
                var dst = CompCache<CompRedstonePower>.Get(posOut, parent.Map) as CompRedstonePowerReceiver;
                if( dst != null )
                    dst.TryPushPower(powerLevel, this);
            }
        }
    }

    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( amount <= 0 ) return false;

        ref Levels levels = ref values[Find.TickManager.TicksGame%2];
        if( levels.tick != Find.TickManager.TicksGame ){
            levels.tick = Find.TickManager.TicksGame;
            levels.r = 0;
            levels.l = 0;
            levels.c = 0;
        }

        IntVec3 posIn = parent.Position + IntVec3.South.RotatedBy(parent.Rotation);
        if( src.parent.Position == posIn ){
            levels.c = amount;
            idempotentTick();
            return true;
        }

        posIn = parent.Position + IntVec3.West.RotatedBy(parent.Rotation);
        if( src.parent.Position == posIn ){
            levels.l = amount;
            idempotentTick();
            return true;
        }

        posIn = parent.Position + IntVec3.East.RotatedBy(parent.Rotation);
        if( src.parent.Position == posIn ){
            levels.r = amount;
            idempotentTick();
            return true;
        }

        return false;
    }

    public override void CompTick(){
        if( !TransmitsPower ) return;

        idempotentTick();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra(){
        foreach (Gizmo item in base.CompGetGizmosExtra()) {
            yield return item;
        }

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("RedstoneLogic/Settings"),
                 defaultLabel = "Change mode",
                 action = delegate {
                     mode = mode == Mode.Compare ? Mode.Subtract : Mode.Compare;
                 }
        };
    }

    public override string CompInspectStringExtra(){
        string s = base.CompInspectStringExtra() + "\n" +
            "mode".Translate() + ": " + mode.ToString().Translate();
        return s;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref pushTick, "pushTick");
        Scribe_Values.Look(ref mode, "mode");
        Scribe_Deep.Look(ref values[0], "value0");
        Scribe_Deep.Look(ref values[1], "value1");
    }
}

