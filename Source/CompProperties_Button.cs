using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompProperties_Button : CompProperties_RedstonePower{
    public int baseDelay = 60; // ticks

	public CompProperties_Button() {
		compClass = typeof(CompButton);
	}
}

