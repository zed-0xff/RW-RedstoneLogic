using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompTNT : CompExplosive {
#if RW15
    public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed) {
        base.PostPreApplyDamage(ref dinfo, out absorbed);
#else
    public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed) {
        base.PostPreApplyDamage(dinfo, out absorbed);
#endif
        if( dinfo.Def == DamageDefOf.Bomb && wickStarted && wickTicksLeft >= Props.wickTicks.min ){
            // https://minecraft.fandom.com/wiki/TNT#Behavior
            wickTicksLeft = new IntRange(30, 90).RandomInRange;
        }
    }
}

