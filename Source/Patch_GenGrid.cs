using HarmonyLib;
using Verse;
using Blocky.Core;

namespace RedstoneLogic;

static class Patch_GenGrid {
//    [HarmonyPatch(typeof(GenGrid), nameof(GenGrid.Impassable))]
//    static class Patch_PistonHead_Impassable {
//        static void Postfix(ref bool __result, IntVec3 c, Map map) {
//            if( __result ) return; // already impassable
//
//            CompPiston comp = CompCache<CompPiston>.Get(c, map);
//            if( comp != null && comp.OpenPct > 0.5f )
//                __result = true;
//        }
//    }
//
//    [HarmonyPatch(typeof(GenGrid), nameof(GenGrid.Walkable))]
//    static class Patch_PistonHead_NotWalkable {
//        static void Postfix(ref bool __result, IntVec3 c, Map map) {
//            if( !__result ) return; // already not walkable
//
//            CompPiston comp = CompCache<CompPiston>.Get(c, map);
//            if( comp != null && comp.OpenPct > 0.5f )
//                __result = false;
//        }
//    }
}
