using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Robust.Shared.Maths;

namespace Content.Shared.Procedural;

public sealed record DungeonPath(string Tile, string Wall, HashSet<Vector2i> Tiles)
{
	public string Tile = Tile;

	public string Wall = Wall;

	[CompilerGenerated]
	public void Deconstruct(out string Tile, out string Wall, out HashSet<Vector2i> Tiles)
	{
		Tile = this.Tile;
		Wall = this.Wall;
		Tiles = this.Tiles;
	}
}
