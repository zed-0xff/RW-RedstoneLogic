using Verse;
using RimWorld;

namespace RedstoneLogic;

public class CompProperties_PoweredGraphic : CompProperties {
    public GraphicData graphicData;

	public CompProperties_PoweredGraphic() {
		compClass = typeof(CompPoweredGraphic);
	}
}
