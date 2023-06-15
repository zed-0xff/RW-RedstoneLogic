using HarmonyLib;
using Verse;
using Verse.AI;
using Blocky.Core;

namespace RedstoneLogic;

[HarmonyPatch(typeof(PathGrid), nameof(PathGrid.CalculatedCostAt))]
static class Patch_ImpassablePistonHead {
    static bool Prefix(ref int __result, IntVec3 c, Map ___map ){
        CompPiston comp = CompCache<CompPiston>.Get(c, ___map);
        if( comp != null && !comp.IsWalkable ){
            __result = PathGrid.ImpassableCost;
            return false;
        }
        return true;
    }
}

