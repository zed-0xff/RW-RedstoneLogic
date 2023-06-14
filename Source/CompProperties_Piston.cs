using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_Piston : CompProperties_RedstonePower {
    public GraphicData shaftGraphicData;
    public GraphicData headGraphicData;

	public CompProperties_Piston() {
		compClass = typeof(CompPiston);
	}
}

