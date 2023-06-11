using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_PressurePlate : CompProperties_RedstonePower {
    public DetectType type;

	public CompProperties_PressurePlate() {
		compClass = typeof(CompPressurePlate);
	}
}

