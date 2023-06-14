using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public class CompPiston : CompRedstonePowerReceiver {
    CompProperties_Piston Props => (CompProperties_Piston)props;

    const int speed = 200;
    float OpenPct => Mathf.Abs(1f-((Find.TickManager.TicksGame%speed) / (float)speed)*2);

	public override void PostDraw(){
        base.PostDraw();
        if( powerLevel <= 0 || !parent.Spawned ) return;

        Vector3 vector = default(Vector3);
        vector = new Vector3(1, 0, 0);
        Rot4 rotation = parent.Rotation;
        rotation.Rotate(RotationDirection.Clockwise);
        vector = rotation.AsQuat * vector;
        Vector3 drawPos = parent.DrawPos;
        drawPos += vector * OpenPct;

//        parent.DrawAt(parent.DrawPos);

        if( OpenPct >= Props.longSize ){
            Props.longGraphicData.Graphic.Draw(drawPos + Props.longGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
        } else {
            Props.shortGraphicData.Graphic.Draw(drawPos + Props.shortGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
            if( parent.Rotation == Rot4.North && Props.shaftGraphicData != null ){
                // cuz shaft should go in hole and head should remain on top )
                Props.shaftGraphicData.Graphic.Draw(
                        drawPos + Props.shaftGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
            }
        }
    }

    public override string CompInspectStringExtra(){
        return base.CompInspectStringExtra() + "\n" +
            "OpenPct: " + OpenPct;
    }
}

