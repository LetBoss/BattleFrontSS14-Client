using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Procedural;

public sealed record DungeonRoom(HashSet<Vector2i> Tiles, Vector2 Center, Box2i Bounds, HashSet<Vector2i> Exterior)
{
	public readonly List<Vector2i> Entrances = new List<Vector2i>();

	public readonly HashSet<Vector2i> Exterior = Exterior;

	[CompilerGenerated]
	public void Deconstruct(out HashSet<Vector2i> Tiles, out Vector2 Center, out Box2i Bounds, out HashSet<Vector2i> Exterior)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Tiles = this.Tiles;
		Center = this.Center;
		Bounds = this.Bounds;
		Exterior = this.Exterior;
	}
}
