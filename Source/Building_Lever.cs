using RimWorld;
using System.Text;
using Verse;

namespace RedstoneLogic;

public class Building_Lever : Building {
    CompFlickable flickableComp;

    public override Graphic Graphic => flickableComp.CurrentGraphic;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        flickableComp = GetComp<CompFlickable>();
    }

    public override string GetInspectString()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(base.GetInspectString());
        if (stringBuilder.Length != 0)
        {
            stringBuilder.AppendLine();
        }
        stringBuilder.Append("PowerSwitch_Power".Translate() + ": ");
        if (FlickUtility.WantsToBeOn(this))
        {
            stringBuilder.Append("On".Translate().ToLower());
        }
        else
        {
            stringBuilder.Append("Off".Translate().ToLower());
        }
        return stringBuilder.ToString();
    }
}

