using System.Numerics;

namespace Content.Client.Power;

public struct PowerMonitoringConsoleLine(Vector2 origin, Vector2 terminus, PowerMonitoringConsoleLineGroup group)
{
	public readonly Vector2 Origin = origin;

	public readonly Vector2 Terminus = terminus;

	public readonly PowerMonitoringConsoleLineGroup Group = group;
}
