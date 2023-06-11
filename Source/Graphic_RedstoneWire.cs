using Blocky.Core;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace RedstoneLogic;

public class Graphic_RedstoneWire : Graphic_Linked {
    public Graphic_RedstoneWire() {
        subGraphic = new Graphic_Single();
    }

    public Graphic_RedstoneWire(Graphic subGraphic) : base(subGraphic) {
    }

    public override void Init(GraphicRequest req){
        this.data = req.graphicData;
        this.path = req.path;
        this.color = req.color;
        this.colorTwo = req.colorTwo;
        this.drawSize = req.drawSize;
        subGraphic.Init(req);
    }

    public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo) {
        return new Graphic_RedstoneWire(subGraphic.GetColoredVersion(newShader, newColor, newColorTwo))
        {
            data = data
        };
    }

    public override bool ShouldLinkWith(IntVec3 vec, Thing parent) {
        return GenGrid.InBounds(vec, parent.Map) &&
            ( CompCache<CompRedstonePower>.Get(vec, parent.Map) != null
              ||
              parent.Map.thingGrid.ThingsAt(vec).Any((Thing t) => t.def.entityDefToBuild == VDefOf.RedstoneWire)
            );
    }
}

