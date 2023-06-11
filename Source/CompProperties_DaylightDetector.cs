using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_DaylightDetector : CompProperties_RedstonePower {
    public GraphicData moonGraphicData;

	public CompProperties_DaylightDetector() {
		compClass = typeof(CompDaylightDetector);
	}
}

