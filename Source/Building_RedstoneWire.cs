using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

class Building_RedstoneWire : Building {
    CompRedstonePower compRedstonePower = null;

    public override void SpawnSetup(Map map, bool respawningAfterLoad) {
        base.SpawnSetup(map, respawningAfterLoad);
        compRedstonePower = GetComp<CompRedstonePower>();
    }

    static readonly Vector3 addDrawPos = new Vector3(0, 0.1f, 0);
    int prevMatId;
    Material matCopy;

    public override void Draw() {
        base.Draw();

        if( compRedstonePower.PowerLevel > 0 ){
            Mesh mesh = Graphic.MeshAt(Rotation);
            var mat = Graphic.MatSingleFor(this);
            if( mat.GetInstanceID() != prevMatId ){
                prevMatId = mat.GetInstanceID();
                matCopy = new Material(mat);
                matCopy.color = Color.red;
            }
            Graphics.DrawMesh(mesh, DrawPos + addDrawPos, Quaternion.identity, matCopy, 0);
        }
    }
}

