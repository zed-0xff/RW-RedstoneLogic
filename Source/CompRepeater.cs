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
    Queue<bool> queue = new Queue<bool>();

    class Value {
        public bool value;
        public int tickNo;
    }

    const int MaxDelay = 250; // rare tick

    BitArray values = new BitArray(MaxDelay);

    public int Delay {
        get { return delay; }
        set { delay = value; }
    }

    // 1. push power from queue if not pushed this turn
    // 2. enqueue value if not enqueued this turn
    // 3. update enqueued value if enqueued 0, but now got 1
    void idempotentTick(){
        IntVec3 posOut = parent.Position + IntVec3.North.RotatedBy(parent.Rotation);

        var dst = CompCache<CompRedstonePower>.Get(posOut, parent.Map) as CompRedstonePowerTransmitter;

        // queue.Count is O(1)
        if( queue.Count <= delay ){
            powerLevel = 0;
            return;
        }

        while( queue.Count > delay ){
            bool state = queue.Dequeue();
            powerLevel = state ? MaxPower : 0;
            if( dst != null )
                dst.TryPushPower(powerLevel, this);
        }
    }

    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( amount <= 0 ) return false;

        IntVec3 posIn  = parent.Position + IntVec3.South.RotatedBy(parent.Rotation);
        if( src.parent.Position != posIn )
            return false;

        lastPoweredTick = Find.TickManager.TicksGame;
        idempotentTick();

        return true;
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
                 defaultLabel = "Meow",
                 action = delegate {
                     Func<int, string> textGetter = ((int ticks) => string.Format("{0} ticks = {1:F2}s", ticks, ticks.TicksToSeconds()));
                     Find.WindowStack.Add( new Dialog_Slider(textGetter, 1, MaxDelay, delegate(int value)
                         {
                         delay = value;
                         foreach( object obj in Find.Selector.SelectedObjects){
                            if( obj is Thing t && t != parent && CompCache<CompRedstonePower>.Get(t.Position, t.Map) is CompRepeater r2 )
                                r2.Delay = value;
                         }
                         }, delay));
                 }
        };
    }

    public override string CompInspectStringExtra(){
        string s = base.CompInspectStringExtra() + "\n" +
            "delay".Translate() + ": " + delay + " " + "ticks".Translate();
        if( Prefs.DevMode ){
            s += "\nqueue size: " + queue.Count;
        }
        return s;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref delay, "delay", 1);

        switch( Scribe.mode ){
            case LoadSaveMode.LoadingVars:
                string tl = null;
                Scribe_Values.Look(ref tl, "queue");
                if( !tl.NullOrEmpty() ){
                    string2queue(tl);
                }
                break;
            case LoadSaveMode.Saving:
                string ts = queue2string();
                if( !ts.NullOrEmpty() ){
                    Scribe_Values.Look(ref ts, "queue");
                }
                break;
        }
    }

    void string2queue(string s){
        queue.Clear();
        for(int i=0; i<s.Length; i++){
            queue.Enqueue( s[i] == '1' );
        }
    }

    string queue2string(){
        char[] chars = new char[queue.Count];
        int i = 0;
        bool was = false;
        foreach( bool b in queue ){
            was |= b;
            chars[i++] = b ? '1' : '0';
        }
        return was ? new string(chars) : null;
    }
}

