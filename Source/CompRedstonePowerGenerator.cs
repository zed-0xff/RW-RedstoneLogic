using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerGenerator : CompRedstonePower {

    const int GenAmount = 15;

//    public override int PowerLevel {
//        get { return GenAmount; }
//    }

    public override void CompTick(){
        powerLevel = GenAmount;
        lastPoweredTick = Find.TickManager.TicksGame;

        base.CompTick();
    }
}

