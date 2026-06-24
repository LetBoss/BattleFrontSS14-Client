using System.Collections.Generic;
using System.Numerics;
using Robust.Shared.Timing;

namespace Content.Client.Shuttles.UI;

public sealed class GridDrawData
{
	public List<Vector2> Vertices = new List<Vector2>();

	public int EdgeIndex;

	public GameTick LastBuild;
}
