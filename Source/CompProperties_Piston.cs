using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_Piston : CompProperties_RedstonePower {
    public GraphicData shaftGraphicData;
    public GraphicData shortGraphicData;
    public GraphicData longGraphicData;
    public float longSize = 0.5f;
    public int baseSpeed = 10; // ticks
    public bool sticky;
    public IntRange damageRange = new IntRange(15, 30);

	public CompProperties_Piston() {
		compClass = typeof(CompPiston);
	}
}

