using UnityEngine;
using RimWorld;
using Verse;

namespace RedstoneLogic;

public class CompPoweredGraphic : ThingComp
{
	private CompProperties_PoweredGraphic Props => (CompProperties_PoweredGraphic)props;

	public bool ParentIsPowered {
		get {
            if (!parent.Spawned) {
                return false;
            }
            CompRedstonePower compRedstonePower = parent.TryGetComp<CompRedstonePower>();
            if (compRedstonePower != null && compRedstonePower.PowerLevel > 0) {
                return true;
            }
            return false;
		}
	}

	public override void PostDraw()
	{
		if (ParentIsPowered) {
			Mesh mesh = Props.graphicData.Graphic.MeshAt(parent.Rotation);
			Vector3 drawPos = parent.DrawPos;
			drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor();
			Graphics.DrawMesh(mesh, drawPos + Props.graphicData.drawOffset.RotatedBy(parent.Rotation), Quaternion.identity, Props.graphicData.Graphic.MatAt(parent.Rotation), 0);
		}
	}
}
