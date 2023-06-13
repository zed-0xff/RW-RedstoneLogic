using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_Tripwire : CompProperties_RedstonePower {
    public FloatRange edgeLengthRange;

    public CompProperties_Tripwire() {
        compClass = typeof(CompTripwire);
    }
}

