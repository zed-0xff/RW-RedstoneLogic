using RimWorld;
using System;
using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerTransmitter : CompRedstonePower {
    public virtual void TryPushPower(int amount){
        if( amount >= powerLevel ){
            powerLevel = Math.Max(amount, powerLevel);
            lastPoweredTick = Find.TickManager.TicksGame;
        }
    }
}

