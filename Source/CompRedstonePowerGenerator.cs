using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompRedstonePowerGenerator : CompRedstonePower {
    CompFlickable flickableComp;

    public virtual bool GeneratesPower => flickableComp == null || flickableComp.SwitchIsOn;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        flickableComp = parent.GetComp<CompFlickable>();
    }

    public override void CompTick(){
        if( GeneratesPower ){
            powerLevel = MaxPower;
            lastPoweredTick = Find.TickManager.TicksGame;
            PushNext();
        } else {
            powerLevel = 0;
        }
        base.CompTick();
    }
}

