using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompTNT : CompExplosive {
    public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed) {
        base.PostPreApplyDamage(dinfo, out absorbed);
        if( dinfo.Def == DamageDefOf.Bomb && wickStarted && wickTicksLeft >= Props.wickTicks.min ){
            // https://minecraft.fandom.com/wiki/TNT#Behavior
            wickTicksLeft = new IntRange(30, 90).RandomInRange;
        }
    }
}

