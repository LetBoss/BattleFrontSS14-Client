using Content.Shared.Power;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;

namespace Content.Client.Power;

public abstract class PowerMonitoringWindowBaseEntry : BoxContainer
{
	public NetEntity NetEntity;

	public PowerMonitoringConsoleEntry Entry;

	public PowerMonitoringButton Button;

	public PowerMonitoringWindowBaseEntry(PowerMonitoringConsoleEntry entry)
	{
		Entry = entry;
		Button = new PowerMonitoringButton();
	}
}
