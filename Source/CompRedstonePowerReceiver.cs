using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerReceiver : CompRedstonePower {
    public virtual bool TryPushPower(int amount, CompRedstonePower src){
        if( this is CompRedstonePowerTransmitter && src is CompRedstonePowerTransmitter )
            amount--;

        if( amount <= 0 ) return false;
        if( amount > MaxPower )
            amount = MaxPower;

        if( amount > powerLevel || lastPoweredTick != Find.TickManager.TicksGame ){
            lastPoweredTick = Find.TickManager.TicksGame;
            powerLevel = amount;
            return true;
        }

        return false;
    }
}

