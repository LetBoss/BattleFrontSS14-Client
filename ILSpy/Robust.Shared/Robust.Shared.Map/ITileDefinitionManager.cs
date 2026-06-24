using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;
using Robust.Shared.Random;

namespace Robust.Shared.Map;

[NotContentImplementable]
public interface ITileDefinitionManager : IEnumerable<ITileDefinition>, IEnumerable
{
	ITileDefinition this[string name] { get; }

	ITileDefinition this[int id] { get; }

	int Count { get; }

	Tile GetVariantTile(string name, IRobustRandom random);

	Tile GetVariantTile(string name, System.Random random);

	Tile GetVariantTile(ITileDefinition tileDef, IRobustRandom random);

	Tile GetVariantTile(ITileDefinition tileDef, System.Random random);

	bool TryGetDefinition(string name, [NotNullWhen(true)] out ITileDefinition? definition);

	bool TryGetDefinition(int id, [NotNullWhen(true)] out ITileDefinition? definition);

	void Initialize();

	void Register(ITileDefinition tileDef);
}
