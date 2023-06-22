using Blocky.Core;
using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class CompComparator : CompRedstonePowerReceiver {
    enum Mode { Compare, Subtract };

    static Graphic modeOverlayGraphic = GraphicDatabase.Get<Graphic_Single>(
            "Blocky/C/ComparatorModeOverlay", ShaderDatabase.Cutout, Vector2.one, Color.white);

    struct Levels : IExposable {
        public int l,r,c,tick;

        public void ExposeData() {
            Scribe_Values.Look(ref l, "l");
            Scribe_Values.Look(ref r, "r");
            Scribe_Values.Look(ref c, "c");
            Scribe_Values.Look(ref tick, "tick");
        }
    };

    Levels[] values = new Levels[2];
    int pushTick;
    Mode mode;
    IntVec3 posC, posL, posR;

    public override void PostSpawnSetup(bool respawningAfterLoad){
        base.PostSpawnSetup(respawningAfterLoad);
        if( !respawningAfterLoad ){
            values = new Levels[2];
        }
        calcCells();
    }

    void calcCells(){
        posC = parent.Position + IntVec3.South.RotatedBy(parent.Rotation);
        posL = parent.Position + IntVec3.West.RotatedBy(parent.Rotation);
        posR = parent.Position + IntVec3.East.RotatedBy(parent.Rotation);
    }

    public override void Notify_Teleported(){
        base.Notify_Teleported();
        calcCells();
    }

    // 1. push power from queue if not pushed this turn
    void idempotentTick(){
        if( pushTick != Find.TickManager.TicksGame ){
            pushTick = Find.TickManager.TicksGame;

            powerLevel = 0;
            var levels = values[1 - pushTick%2];

            if( levels.tick == pushTick-1 ){
                switch( mode ){
                    case Mode.Compare:
                        if( levels.l <= levels.c && levels.r <= levels.c ){
                            powerLevel = levels.c;
                        }
                        break;
                    case Mode.Subtract:
                        // https://minecraft.fandom.com/wiki/Redstone_Comparator#Subtract_signal_strength
                        powerLevel = Mathf.Max(levels.c - Mathf.Max(levels.l, levels.r), 0);
                        break;
                }
            }

            if( powerLevel > 0 ){
                IntVec3 posOut = parent.Position + IntVec3.North.RotatedBy(parent.Rotation);
                var dst = CompCache<CompRedstonePower>.Get(posOut, parent.Map) as CompRedstonePowerReceiver;
                if( dst != null )
                    dst.TryPushPower(powerLevel, this);
            }
        }
    }

    public override bool TryPushPower(int amount, CompRedstonePower src){
        if( amount <= 0 ) return false;

        ref Levels levels = ref values[Find.TickManager.TicksGame%2];
        if( levels.tick != Find.TickManager.TicksGame ){
            levels.tick = Find.TickManager.TicksGame;
            levels.r = 0;
            levels.l = 0;
            levels.c = 0;
        }

        if( src.parent.Position == posC ){
            levels.c = amount;
            idempotentTick();
            return true;
        }

        if( src.parent.Position == posL ){
            levels.l = amount;
            idempotentTick();
            return true;
        }

        if( src.parent.Position == posR ){
            levels.r = amount;
            idempotentTick();
            return true;
        }

        return false;
    }

    bool isItemFrame(Thing t){
        return t.def.defName.StartsWith("Blocky_Signs_") && t.def.defName.EndsWith("ItemFrame");
    }

    static FieldInfo fi_angle = null;

    int TryMeasure(){
        Thing measureable = null;

        foreach( Thing t in parent.Map.thingGrid.ThingsListAtFast(posC) ){
            var comp = t.TryGetComp<CompRedstonePower>();
            if( comp != null ){
                // can't measure: already has some powered building in input slot
                return 0;
            }
            if( t is Building_Storage || t is Building_FermentingBarrel || isItemFrame(t) ){
                measureable = t;
                // can't yet break because if any CompRedstonePower is found we should return 0
            }
        }

        if( measureable == null ) return 0;

        if( isItemFrame(measureable) ){
            if( measureable is Building_Storage fs && fs.slotGroup.HeldThings.Count() == 0 ){
                return 0;
            }
            if( fi_angle == null )
                fi_angle = AccessTools.Field(measureable.GetType(), "angle");
            if( fi_angle != null ){
                float angle = (float)fi_angle.GetValue(measureable);
                return 1 + (int)(-(angle%360)/45);
            }
            return 0;
        }

        if( measureable is Building_Storage s ){
            int cellCount = s.AllSlotCellsList().Count;
            float fullness = 0;

            foreach( Thing t in s.slotGroup.HeldThings ){
                fullness += 1.0f*t.stackCount / t.def.stackLimit;
            }
            return (int)(MaxPower*fullness/cellCount/s.def.building.maxItemsInCell);
        }

        if( measureable is Building_FermentingBarrel b ){
            return (int)(1.0f*MaxPower*(Building_FermentingBarrel.MaxCapacity-b.SpaceLeftForWort) / Building_FermentingBarrel.MaxCapacity);
        }


        return 0;
    }

    public override void CompTick(){
        if( !TransmitsPower ) return;

        int value = TryMeasure();
        if( value != 0 ){
            ref Levels levels = ref values[Find.TickManager.TicksGame%2];
            if( levels.tick != Find.TickManager.TicksGame ){
                levels.tick = Find.TickManager.TicksGame;
                levels.r = 0;
                levels.l = 0;
                levels.c = 0;
            }
            levels.c = value;
        }

        idempotentTick();
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra(){
        foreach (Gizmo item in base.CompGetGizmosExtra()) {
            yield return item;
        }

        yield return new Command_Action
        {
            icon = ContentFinder<Texture2D>.Get("RedstoneLogic/Settings"),
                 defaultLabel = "Change mode",
                 action = delegate {
                     mode = mode == Mode.Compare ? Mode.Subtract : Mode.Compare;
                 }
        };
    }

	public override void PostDraw(){
        base.PostDraw();
        if( mode == Mode.Subtract ){
			Vector3 drawPos = parent.DrawPos;
			drawPos.y = AltitudeLayer.BuildingOnTop.AltitudeFor() + 0.1f;

			Mesh mesh = modeOverlayGraphic.MeshAt(parent.Rotation);
            Quaternion quat = Quaternion.Euler(Vector3.up * parent.Rotation.AsAngle);
            Material mat = modeOverlayGraphic.MatAt(parent.Rotation);
            var matrix = Matrix4x4.TRS(drawPos, quat, new Vector3(1, 1, 1));
            Graphics.DrawMesh(mesh, matrix, mat, 0);
        }
    }

    public override string CompInspectStringExtra(){
        string s = base.CompInspectStringExtra() + "\n" +
            "mode".Translate() + ": " + mode.ToString().Translate();
        return s;
    }

    public override void PostExposeData() {
        base.PostExposeData();
        Scribe_Values.Look(ref pushTick, "pushTick");
        Scribe_Values.Look(ref mode, "mode");
        Scribe_Deep.Look(ref values[0], "value0");
        Scribe_Deep.Look(ref values[1], "value1");
    }
}

