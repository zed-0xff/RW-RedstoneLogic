using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerGenerator : CompRedstonePower {
    public override void CompTick(){
        powerLevel = MaxPower;
        lastPoweredTick = Find.TickManager.TicksGame;

        base.CompTick();
    }
}

