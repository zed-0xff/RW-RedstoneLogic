using RimWorld;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

public class CompRepeater : CompRedstonePowerReceiver {
    int delay = 1;

    const int MaxDelay = 250; // rare tick
    BitArray values = new BitArray(MaxDelay); // ring buffer
    int ptr;
    int pushTick;
    int saveTick;

    public int Delay {
        get { return delay; }
        set {
            delay = Mathf.Max(1, Mathf.Min(value, MaxDelay));
            for( int i=delay; i<MaxDelay; i++)
                values[i] = false;
        }
    }

    // 1. push power from queue if not pushed this turn
    // 2. enqueue value if not enqueued this turn
    // 3. update enqueued value if enqueued 0, but now got 1
    void idempotentTick(bool value){
        if( pushTick != Find.TickManager.TicksGame ){
            pushTick = Find.TickManager.TicksGame;
            if( ++ptr >= delay ) ptr = 0;

            if( values[ptr] ){
                powerLevel = MaxPower;
                IntVec3 posOut = parent.Position + IntVec3.North.RotatedBy(parent.Rotation);
                var dst = CompCache<CompRedstonePower>.Get(posOut, parent.Map) as CompRedstonePowerReceiver;
                if( dst != null )
                    dst.TryPushPower(MaxPower, this);
            } else {
                powerLevel = 0;
            }
        }

        if( saveTick == Find.TickManager.TicksGame ){
            values[ptr] = values[ptr] | value;
        } else {
            saveTick = Find.TickManager.TicksGame;
            values[ptr] = value;
        }
    }

    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( amount <= 0 ) return false;

        IntVec3 posIn = parent.Position + IntVec3.South.RotatedBy(parent.Rotation);
        if( src.parent.Position != posIn )
            return false;

        lastPoweredTick = Find.TickManager.TicksGame;
        idempotentTick(amount > 0);

        return true;
    }

    public override void CompTick(){
        if( !TransmitsPower ) return;

        idempotentTick(false);
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra(){
        foreach (Gizmo item in base.CompGetGizmosExtra()) {
            yield return item;
        }

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("RedstoneLogic/Settings"),
                 defaultLabel = "Set delay",
                 action = delegate {
                     Func<int, string> textGetter = ((int ticks) => string.Format("{0} ticks = {1:F2}s", ticks, ticks.TicksToSeconds()));
                     Find.WindowStack.Add( new Dialog_Slider(textGetter, 1, MaxDelay, delegate(int value)
                         {
                         foreach( object obj in Find.Selector.SelectedObjects){
                            if( obj is Thing t && CompCache<CompRedstonePower>.Get(t.Position, t.Map) is CompRepeater r2 )
                                r2.Delay = value;
                         }
                         }, delay));
                 }
        };

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/Minus"),
                 defaultLabel = "-10",
                 action = delegate {
                     foreach( object obj in Find.Selector.SelectedObjects){
                        if( obj is Thing t && CompCache<CompRedstonePower>.Get(t.Position, t.Map) is CompRepeater r ){
                            r.Delay -= 10;
                        }
                     }
                 }
        };

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("UI/Buttons/Plus"),
                 defaultLabel = "+10",
                 action = delegate {
                     foreach( object obj in Find.Selector.SelectedObjects){
                        if( obj is Thing t && CompCache<CompRedstonePower>.Get(t.Position, t.Map) is CompRepeater r ){
                            r.Delay += 10;
                            if( r.Delay == 11 ) r.Delay = 10;
                        }
                     }
                 }
        };
    }

    public override string CompInspectStringExtra(){
        string s = base.CompInspectStringExtra() + "\n" +
            "delay".Translate() + ": " + delay + " " + "ticks".Translate();
        return s;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref delay, "delay", 1);
        Scribe_Values.Look(ref ptr, "ptr");
        Scribe_Values.Look(ref saveTick, "saveTick");
        Scribe_Values.Look(ref pushTick, "pushTick");

        switch( Scribe.mode ){
            case LoadSaveMode.LoadingVars:
                string tl = null;
                Scribe_Values.Look(ref tl, "values");
                if( !tl.NullOrEmpty() ){
                    string2values(tl);
                }
                break;
            case LoadSaveMode.Saving:
                string ts = values2string();
                if( !ts.NullOrEmpty() ){
                    Scribe_Values.Look(ref ts, "values");
                }
                break;
        }
    }

    void string2values(string s){
        for(int i=0; i<s.Length; i++){
            values[i] = ( s[i] == '1' );
        }
    }

    string values2string(){
        char[] chars = new char[values.Count];
        int i = 0;
        bool was = false;
        foreach( bool b in values ){
            was |= b;
            chars[i++] = b ? '1' : '0';
        }
        return was ? new string(chars) : null;
    }
}

