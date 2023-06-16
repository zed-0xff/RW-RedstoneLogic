using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public class SectionLayer_RedstonePowerGrid : SectionLayer_Things
{
    public SectionLayer_RedstonePowerGrid(Section section)
        : base(section)
    {
        requireAddToMapMesh = false;
        relevantChangeTypes = MapMeshFlag.Buildings;
    }

    private static int lastFrameDraw;
    public virtual bool ShouldDraw => lastFrameDraw + 1 >= Time.frameCount;

    public static void DrawOverlayThisFrame(){
        lastFrameDraw = Time.frameCount;
    }

    public override void DrawLayer()
    {
        if (ShouldDraw)
            base.DrawLayer();
    }

    protected override void TakePrintFrom(Thing t) {
        var comp = t.TryGetComp<CompRedstonePower>();
        if( comp == null ) return;

        comp.CompPrintForOverlay(this);
    }
}
