using System.Numerics;
using Robust.Shared.GameObjects;

namespace Content.Shared._RMC14.Xenonids.Zoom;

[ByRefEvent]
public record struct XenoGetZoomEvent(Vector2 Zoom)
{
	public void Increase(float zoom)
	{
		if (!(Zoom.X >= zoom))
		{
			Zoom = new Vector2(zoom, zoom);
		}
	}
}
