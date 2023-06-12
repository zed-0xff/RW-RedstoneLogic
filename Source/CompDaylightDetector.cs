using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace RedstoneLogic;

public class CompDaylightDetector : CompRedstonePower {
    bool moonlight;

    CompProperties_DaylightDetector Props => (CompProperties_DaylightDetector)props;

    const int MaxMoonPower = 11;

    public override void CompTick(){
        if( parent == null || parent.Map == null ) return;

        lastPoweredTick = Find.TickManager.TicksGame;
        if( moonlight ){
            powerLevel = Mathf.RoundToInt(MaxMoonPower*(1f-GenCelestial.CurCelestialSunGlow(parent.Map)));
        } else {
            powerLevel = Mathf.RoundToInt(MaxPower*GenCelestial.CurCelestialSunGlow(parent.Map));
        }

        PushNext();
        base.CompTick();
    }

    static readonly Vector3 addDrawPos = new Vector3(0, 0.1f, 0);

	public override void PostDraw() {
		if (moonlight) {
			Mesh mesh = Props.moonGraphicData.Graphic.MeshAt(parent.Rotation);
			Graphics.DrawMesh(mesh, parent.DrawPos + addDrawPos, Quaternion.identity, Props.moonGraphicData.Graphic.MatAt(parent.Rotation), 0);
		}
	}

    public override IEnumerable<Gizmo> CompGetGizmosExtra(){
        foreach (Gizmo item in base.CompGetGizmosExtra()) {
            yield return item;
        }

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get(moonlight ? "UI/Commands/SetNormalLight" : "UI/Commands/SetDarklight"),
                 defaultLabel = (moonlight ? "ToggleDarklightOff" : "ToggleMoonlightOn").Translate(),
                 action = delegate {
                     SoundDefOf.Tick_High.PlayOneShotOnCamera();
                     moonlight = !moonlight;
                 }
        };
    }

    public override string CompInspectStringExtra(){
        return base.CompInspectStringExtra() + "\n"
            + "mode".Translate() + ": " + (moonlight ? "moonlight" : "daylight").Translate();
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref moonlight, "moonlight", false);
    }
}

