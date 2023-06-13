using System.Text;
using RimWorld;
using Verse;

namespace RedstoneLogic;

[StaticConstructorOnStartup]
public class Building_PowerSwitch : Building
{
	private bool wantsOnOld;

	private CompRedstonePower redstoneComp;

	public override bool TransmitsPowerNow => redstoneComp != null && redstoneComp.PowerLevel > 0;

	public override void SpawnSetup(Map map, bool respawningAfterLoad)
	{
		base.SpawnSetup(map, respawningAfterLoad);
		redstoneComp = GetComp<CompRedstonePower>();
	}

	public override void ExposeData()
	{
		base.ExposeData();
		if (Scribe.mode == LoadSaveMode.PostLoadInit)
		{
			if (redstoneComp == null)
			{
				redstoneComp = GetComp<CompRedstonePower>();
			}
			UpdatePowerGrid();
		}
	}

	protected override void ReceiveCompSignal(string signal)
	{
		switch (signal)
		{
		case "FlickedOff":
		case "FlickedOn":
        case "PowerTurnedOn":
        case "PowerTurnedOff":
		case "ScheduledOn":
		case "ScheduledOff":
			UpdatePowerGrid();
			break;
		}
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
		if (TransmitsPowerNow)
		{
			stringBuilder.Append("On".Translate().ToLower());
		}
		else
		{
			stringBuilder.Append("Off".Translate().ToLower());
		}
		return stringBuilder.ToString();
	}

	private void UpdatePowerGrid()
	{
		if (TransmitsPowerNow != wantsOnOld)
		{
			if (base.Spawned)
			{
				base.Map.powerNetManager.Notfiy_TransmitterTransmitsPowerNowChanged(base.PowerComp);
			}
			wantsOnOld = TransmitsPowerNow;
		}
	}
}
