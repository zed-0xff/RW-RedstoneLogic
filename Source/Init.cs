using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class Init
{
    static Init()
    {
        Harmony harmony = new Harmony("RedstoneLogic");
        harmony.PatchAll();
        AddRedstoneToTraders();
    }

    static readonly FieldInfo fi_thingDef = AccessTools.Field(typeof(StockGenerator_SingleDef), "thingDef");

    static void AddRedstoneToTraders(){
        foreach( TraderKindDef trader in DefDatabase<TraderKindDef>.AllDefs ){
            foreach( StockGenerator sg in trader.stockGenerators ){
                if( sg is StockGenerator_SingleDef sd && sd.HandlesThingDef(ThingDefOf.Gold) ){
                    var redstone_sd = new StockGenerator_SingleDef();
                    fi_thingDef.SetValue(redstone_sd, VDefOf.Redstone);
                    redstone_sd.countRange = sd.countRange;
                    trader.stockGenerators.Add(redstone_sd);
                    trader.ResolveReferences();
                    break;
                }
            }
        }
    }
}
