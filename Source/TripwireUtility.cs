using RimWorld;
using Verse;
using UnityEngine;

namespace RedstoneLogic;

public static class TripwireUtility
{
    public static readonly FloatRange Range = new FloatRange(3, 25);

	public static void DrawLinesToNearbyHooks(ThingDef myDef, IntVec3 myPos, Rot4 myRot, Map map, Thing thing = null)
	{
		Vector3 a = GenThing.TrueCenter(myPos, myRot, myDef.size, myDef.Altitude);
		foreach (Thing item in map.listerThings.ThingsOfDef(VDefOf.TripwireHook))
		{
			if ((thing == null || thing != item) && item.Faction == Faction.OfPlayer)
			{
				CompTripwire compTripwire = item.TryGetComp<CompTripwire>();
				if (compTripwire != null && CanLinkTo(myPos, compTripwire) && !GenThing.CloserThingBetween(myDef, myPos, item.Position, map))
				{
					GenDraw.DrawLineBetween(a, item.TrueCenter(), SimpleColor.White);
				}
			}
		}
		float minEdgeDistance = Range.min - 1f;
		float maxEdgeDistance = Range.max - 1f;
		foreach (Thing item2 in map.listerThings.ThingsInGroup(ThingRequestGroup.Blueprint))
		{
			if ((thing == null || thing != item2)
                    && item2.def.entityDefToBuild == myDef
                    && (myPos.x == item2.Position.x || myPos.z == item2.Position.z)
                    && !AlignedDistanceTooShort(myPos, item2.Position, minEdgeDistance)
                    && !AlignedDistanceTooLong(myPos, item2.Position, maxEdgeDistance)
                    && !GenThing.CloserThingBetween(myDef, myPos, item2.Position, map))
			{
				GenDraw.DrawLineBetween(a, item2.TrueCenter(), SimpleColor.White);
			}
		}
	}

	public static bool AlignedDistanceTooShort(IntVec3 position, IntVec3 otherPos, float minEdgeDistance)
	{
		if (position.x == otherPos.x)
		{
			return (float)Mathf.Abs(position.z - otherPos.z) < minEdgeDistance;
		}
		if (position.z == otherPos.z)
		{
			return (float)Mathf.Abs(position.x - otherPos.x) < minEdgeDistance;
		}
		return false;
	}

	private static bool AlignedDistanceTooLong(IntVec3 position, IntVec3 otherPos, float maxEdgeDistance)
	{
		if (position.x == otherPos.x)
		{
			return (float)Mathf.Abs(position.z - otherPos.z) >= maxEdgeDistance;
		}
		if (position.z == otherPos.z)
		{
			return (float)Mathf.Abs(position.x - otherPos.x) >= maxEdgeDistance;
		}
		return false;
	}

    public static bool CheckCell(IntVec3 pos, Map map){
        foreach( Thing t in map.thingGrid.ThingsListAtFast(pos) ){
            if( t is Building && t.def.fillPercent > 0.1 )
                return false;
        }
        return true;
    }

	public static bool CanLinkTo(IntVec3 position, CompTripwire other) {
        IntVec3 pos = new IntVec3(position.x, position.y, position.z);

        if( position.x == other.parent.Position.x && Range.Includes(Mathf.Abs(position.z - other.parent.Position.z) + 1) ){
            int z0 = Mathf.Min(position.z, other.parent.Position.z);
            int z1 = Mathf.Max(position.z, other.parent.Position.z);
            for( int z = z0+1; z<z1; z++ ){
                pos.z = z;
                if( !CheckCell(pos, other.parent.Map) ) return false;
            }
            return true;
        }

		if( position.z == other.parent.Position.z && Range.Includes(Mathf.Abs(position.x - other.parent.Position.x) + 1) ){
            int x0 = Mathf.Min(position.x, other.parent.Position.x);
            int x1 = Mathf.Max(position.x, other.parent.Position.x);
            for( int x = x0+1; x<x1; x++ ){
                pos.x = x;
                if( !CheckCell(pos, other.parent.Map) ) return false;
            }
            return true;
		}

		return false;
	}
}
