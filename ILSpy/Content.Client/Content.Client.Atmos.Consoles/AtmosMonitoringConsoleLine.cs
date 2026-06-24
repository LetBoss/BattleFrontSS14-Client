using System.Numerics;
using Robust.Shared.Maths;

namespace Content.Client.Atmos.Consoles;

public struct AtmosMonitoringConsoleLine(Vector2 origin, Vector2 terminus, Color color)
{
	public readonly Vector2 Origin = origin;

	public readonly Vector2 Terminus = terminus;

	public readonly Color Color = color;
}
