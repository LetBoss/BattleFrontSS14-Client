using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;

namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringButton : Button
{
	public int IndexInTable;

	public NetEntity SuitSensorUid;

	public EntityCoordinates? Coordinates;
}
