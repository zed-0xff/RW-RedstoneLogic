using RimWorld;
using Verse;

namespace RedstoneLogic;

class CompRedstoneGlower : CompGlower {
    protected override bool ShouldBeLitNow {
        get {
            if (!parent.Spawned) {
                return false;
            }
            CompRedstonePower compRedstonePower = parent.TryGetComp<CompRedstonePower>();
            if (compRedstonePower != null && compRedstonePower.PowerLevel > 0) {
                return true;
            }
            return false;
        }
    }
}

