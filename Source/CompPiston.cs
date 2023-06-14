using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public class CompPiston : CompRedstonePowerReceiver {
    CompProperties_Piston Props => (CompProperties_Piston)props;

	public override void PostDraw(){
        base.PostDraw();
        if( powerLevel <= 0 || !parent.Spawned ) return;

        const int speed = 50;
        float OpenPct = Mathf.Abs(1f-((Find.TickManager.TicksGame%speed) / (float)speed)*2);

        Vector3 vector = default(Vector3);
        vector = new Vector3(1, 0, 0);
        Rot4 rotation = parent.Rotation;
        rotation.Rotate(RotationDirection.Clockwise);
        vector = rotation.AsQuat * vector;
        Vector3 drawPos = parent.DrawPos;
        drawPos += vector * OpenPct;

        parent.DrawAt(parent.DrawPos);
        Props.headGraphicData.Graphic.Draw(drawPos + Props.headGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
        Props.shaftGraphicData.Graphic.Draw(drawPos + Props.shaftGraphicData.DrawOffsetForRot(parent.Rotation), parent.Rotation, parent);
    }
}

