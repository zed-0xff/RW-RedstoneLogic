using HarmonyLib;
using System;
using Verse;

namespace RedstoneLogic;

[HarmonyPatch(typeof(Log), nameof(Log.Warning), new Type[]{ typeof(string) } )]
static class Patch_Log_HideChangedPositionOfSpawnedThingUnsupportedMsg {
    static bool Prefix(string text) {
        return text != "Changed position of a spawned thing which affects regions. This is not supported.";
    }
}

