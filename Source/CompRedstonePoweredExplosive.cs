using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompRedstonePoweredExplosive : CompRedstonePowerReceiver {
    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( amount <= 0 ) return false;

        CompExplosive comp = parent.TryGetComp<CompExplosive>();
        if( comp != null ){
            comp.StartWick();
        }

        return true;
    }
}

