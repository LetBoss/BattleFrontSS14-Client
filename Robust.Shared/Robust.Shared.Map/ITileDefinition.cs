using System.Collections.Generic;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Robust.Shared.Map;

public interface ITileDefinition : IPrototype
{
	ushort TileId { get; }

	string Name { get; }

	ResPath? Sprite { get; }

	Dictionary<Direction, ResPath> EdgeSprites { get; }

	int EdgeSpritePriority { get; }

	float Friction { get; }

	byte Variants { get; }

	bool AllowRotationMirror => false;

	bool EditorHidden => false;

	void AssignTileId(ushort id);
}
