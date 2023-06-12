using RimWorld;
using System;
using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerTransmitter : CompRedstonePowerReceiver {
    public override bool TryPushPower(int amount, CompRedstonePower src){
        bool r = base.TryPushPower(amount, src);
        if( r ) PushNext(src);
        return r;
    }
}

