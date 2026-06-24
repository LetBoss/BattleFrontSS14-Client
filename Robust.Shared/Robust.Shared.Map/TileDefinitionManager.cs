using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Analyzers;
using Robust.Shared.Random;

namespace Robust.Shared.Map;

[Virtual]
internal class TileDefinitionManager : ITileDefinitionManager, IEnumerable<ITileDefinition>, IEnumerable
{
	protected readonly List<ITileDefinition> TileDefs;

	private readonly Dictionary<string, ITileDefinition> _tileNames;

	private readonly Dictionary<string, List<string>> _awaitingAliases;

	public ITileDefinition this[string name] => _tileNames[name];

	public ITileDefinition this[int id] => TileDefs[id];

	public int Count => TileDefs.Count;

	public TileDefinitionManager()
	{
		TileDefs = new List<ITileDefinition>();
		_tileNames = new Dictionary<string, ITileDefinition>();
		_awaitingAliases = new Dictionary<string, List<string>>();
	}

	public virtual void Initialize()
	{
	}

	public virtual void Register(ITileDefinition tileDef)
	{
		string iD = tileDef.ID;
		if (_tileNames.ContainsKey(iD))
		{
			throw new ArgumentException("Another tile definition or alias with the same name has already been registered.", "tileDef");
		}
		ushort id = checked((ushort)TileDefs.Count);
		tileDef.AssignTileId(id);
		TileDefs.Add(tileDef);
		_tileNames[iD] = tileDef;
	}

	public Tile GetVariantTile(string name, IRobustRandom random)
	{
		ITileDefinition tileDef = this[name];
		return GetVariantTile(tileDef, random);
	}

	public Tile GetVariantTile(string name, System.Random random)
	{
		ITileDefinition tileDef = this[name];
		return GetVariantTile(tileDef, random);
	}

	public Tile GetVariantTile(ITileDefinition tileDef, IRobustRandom random)
	{
		return new Tile(tileDef.TileId, 0, random.NextByte(tileDef.Variants), 0);
	}

	public Tile GetVariantTile(ITileDefinition tileDef, System.Random random)
	{
		return new Tile(tileDef.TileId, 0, random.NextByte(tileDef.Variants), 0);
	}

	public bool TryGetDefinition(string name, [NotNullWhen(true)] out ITileDefinition? definition)
	{
		return _tileNames.TryGetValue(name, out definition);
	}

	public bool TryGetDefinition(int id, [NotNullWhen(true)] out ITileDefinition? definition)
	{
		if (id >= TileDefs.Count)
		{
			definition = null;
			return false;
		}
		definition = TileDefs[id];
		return true;
	}

	public IEnumerator<ITileDefinition> GetEnumerator()
	{
		return TileDefs.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
