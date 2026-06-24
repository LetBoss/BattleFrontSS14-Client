using System.Collections.Generic;
using Robust.Shared.Prototypes;

namespace Content.Shared.Parallax.Biomes.Markers;

public interface IBiomeMarkerLayer : IPrototype
{
	Dictionary<EntProtoId, EntProtoId> EntityMask { get; }

	string? Prototype { get; }

	int Size { get; }
}
