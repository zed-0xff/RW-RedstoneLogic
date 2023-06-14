using RimWorld;
using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class Building_Piston : Building {
	private CompPiston pistonComp;

	public override void SpawnSetup(Map map, bool respawningAfterLoad) {
		base.SpawnSetup(map, respawningAfterLoad);
		pistonComp = GetComp<CompPiston>();
	}

    // <IThingHolder>
//    public ThingOwner innerContainer;
//
//    public ThingOwner GetDirectlyHeldThings() {
//        return innerContainer;
//    }
//
//    public void GetChildHolders(List<IThingHolder> outChildren) {
//        ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
//    }
    // </IThingHolder>
}

