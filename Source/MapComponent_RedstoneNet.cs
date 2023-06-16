using RimWorld;
using Verse;

namespace RedstoneLogic;

public class MapComponent_RedstoneNet : MapComponent {
    public MapComponent_RedstoneNet(Map map) : base(map) {
    }

    public override void MapComponentUpdate() {
        base.MapComponentUpdate();

        if (((Find.MainTabsRoot.OpenTab?.TabWindow as MainTabWindow_Architect)?.selectedDesPanel?.def == VDefOf.RedstoneLogic )
                || Find.Selector.FirstSelectedObject is Building_RedstoneWire
           ){
            SectionLayer_RedstonePowerGrid.DrawOverlayThisFrame();
        }
    }
}

