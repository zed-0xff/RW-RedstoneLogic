using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_Piston : CompProperties_RedstonePower {
    public GraphicData shaftGraphicData;
    public GraphicData shortGraphicData;
    public GraphicData longGraphicData;
    public float longSize = 0.5f;

	public CompProperties_Piston() {
		compClass = typeof(CompPiston);
	}
}

