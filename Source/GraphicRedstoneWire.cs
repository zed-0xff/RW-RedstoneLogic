using RimWorld;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

public class GraphicRedstoneWire : Graphic_Linked {
    public GraphicRedstoneWire() {
        subGraphic = new Graphic_Single();
    }

    public GraphicRedstoneWire(Graphic subGraphic) : base(subGraphic) {
    }

    public override void Init(GraphicRequest req){
        this.data = req.graphicData;
        this.path = req.path;
        this.color = req.color;
        this.colorTwo = req.colorTwo;
        this.drawSize = req.drawSize;
        subGraphic.Init(req);
    }

    public override bool ShouldLinkWith(IntVec3 vec, Thing parent) {
        return GenGrid.InBounds(vec, parent.Map) && CompCache<CompRedstonePower>.Get(vec, parent.Map) != null;
    }
}

