using HarmonyLib;
using RimWorld;
using Verse;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class Init
{
    static Init()
    {
        Harmony harmony = new Harmony("RedstoneLogic");
        harmony.PatchAll();
    }
}
