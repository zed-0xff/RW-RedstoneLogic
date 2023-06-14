using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class Building_Piston : Building {

//    static readonly Graphic HeadGraphic
//        = GraphicDatabase.Get<Graphic_Multi>("Blocky/P/PistonHead", ShaderDatabase.Cutout, Vector2.one, Color.white);
//
    static readonly Graphic BaseGraphic
        = GraphicDatabase.Get<Graphic_Multi>("Blocky/P/PistonBase", ShaderDatabase.Cutout, Vector2.one, Color.white);

//    static readonly Graphic ShaftGraphic
//        = GraphicDatabase.Get<Graphic_Multi>("Blocky/P/PistonShaft", ShaderDatabase.Transparent, Vector2.one, Color.white);
//
    public bool IsActive => redstoneComp != null && redstoneComp.PowerLevel > 0;

    public override Graphic Graphic => IsActive ? BaseGraphic : base.Graphic;

	private CompRedstonePower redstoneComp;

	public override void SpawnSetup(Map map, bool respawningAfterLoad) {
		base.SpawnSetup(map, respawningAfterLoad);
		redstoneComp = GetComp<CompRedstonePower>();
	}

//    public override void Draw() {
//        base.Draw();
//        if( !IsActive) return;
//
//        const int speed = 50;
//        float OpenPct = (Find.TickManager.TicksGame%speed) / (float)speed;
//
//        Vector3 vector = default(Vector3);
//        vector = new Vector3(1, 0, 0);
//        Rot4 rotation = base.Rotation;
//        rotation.Rotate(RotationDirection.Clockwise);
//        vector = rotation.AsQuat * vector;
//        Vector3 drawPos = DrawPos;
//        drawPos += vector * OpenPct;
//
////        drawPos.y = AltitudeLayer.BuildingBelowTop.AltitudeFor();
////        Graphics.DrawMesh(MeshPool.plane10, drawPos,  Quaternion.identity, HeadGraphic.MatAt(base.Rotation), 0);
////
////        drawPos.y = AltitudeLayer.DoorMoveable.AltitudeFor();
////        Graphics.DrawMesh(MeshPool.plane10, drawPos, Quaternion.identity, ShaftGraphic.MatAt(base.Rotation), 0);
////        ShaftGraphic.Draw(drawPos, Rotation, this);
//
//        //Graphic.ShadowGraphic?.DrawWorker(drawPos, base.Rotation, def, this, 0f);
//    }
}

