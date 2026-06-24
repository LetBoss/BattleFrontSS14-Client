using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Content.Shared.Maps;
using Content.Shared.Parallax.Biomes.Layers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Noise;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared.Parallax.Biomes;

public abstract class SharedBiomeSystem : EntitySystem
{
	[Dependency]
	protected IPrototypeManager ProtoManager;

	[Dependency]
	private ISerializationManager _serManager;

	[Dependency]
	protected ITileDefinitionManager TileDefManager;

	[Dependency]
	private TileSystem _tile;

	[Dependency]
	private SharedMapSystem _map;

	protected const byte ChunkSize = 8;

	private T Pick<T>(List<T> collection, float value)
	{
		value %= 1f;
		value = Math.Clamp(value, 0f, 1f);
		if (collection.Count == 1)
		{
			return collection[0];
		}
		float randValue = value * (float)collection.Count;
		foreach (T item in collection)
		{
			randValue -= 1f;
			if (randValue <= 0f)
			{
				return item;
			}
		}
		throw new ArgumentOutOfRangeException();
	}

	private int Pick(int count, float value)
	{
		value %= 1f;
		value = Math.Clamp(value, 0f, 1f);
		if (count == 1)
		{
			return 0;
		}
		value *= (float)count;
		for (int i = 0; i < count; i++)
		{
			value -= 1f;
			if (value <= 0f)
			{
				return i;
			}
		}
		throw new ArgumentOutOfRangeException();
	}

	public bool TryGetBiomeTile(EntityUid uid, MapGridComponent grid, Vector2i indices, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		TileRef tileRef = default(TileRef);
		if (_map.TryGetTileRef(uid, grid, indices, ref tileRef) && !((Tile)(ref tileRef.Tile)).IsEmpty)
		{
			tile = tileRef.Tile;
			return true;
		}
		BiomeComponent biome = default(BiomeComponent);
		if (!((EntitySystem)this).TryComp<BiomeComponent>(uid, ref biome))
		{
			tile = null;
			return false;
		}
		return TryGetBiomeTile(indices, biome.Layers, biome.Seed, (Entity<MapGridComponent>?)Entity<MapGridComponent>.op_Implicit((uid, grid)), out tile);
	}

	public bool TryGetBiomeTile(Vector2i indices, List<IBiomeLayer> layers, int seed, Entity<MapGridComponent>? grid, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (grid.HasValue)
		{
			Entity<MapGridComponent> gridEnt = grid.GetValueOrDefault();
			TileRef tileRef = default(TileRef);
			if (_map.TryGetTileRef(Entity<MapGridComponent>.op_Implicit(gridEnt), gridEnt.Comp, indices, ref tileRef) && !((Tile)(ref tileRef.Tile)).IsEmpty)
			{
				tile = tileRef.Tile;
				return true;
			}
		}
		return TryGetTile(indices, layers, seed, grid, out tile);
	}

	[Obsolete("Use the Entity<MapGridComponent>? overload")]
	public bool TryGetBiomeTile(Vector2i indices, List<IBiomeLayer> layers, int seed, MapGridComponent? grid, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return TryGetBiomeTile(indices, layers, seed, (grid == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid))), out tile);
	}

	public bool TryGetTile(Vector2i indices, List<IBiomeLayer> layers, int seed, Entity<MapGridComponent>? grid, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		for (int i = layers.Count - 1; i >= 0; i--)
		{
			IBiomeLayer layer = layers[i];
			FastNoiseLite noiseCopy = GetNoise(layer.Noise, seed);
			bool invert = layer.Invert;
			float value = noiseCopy.GetNoise((float)indices.X, (float)indices.Y);
			value = (invert ? (value * -1f) : value);
			if (!(value < layer.Threshold))
			{
				if (layer is BiomeMetaLayer meta)
				{
					if (TryGetBiomeTile(indices, ProtoManager.Index<BiomeTemplatePrototype>(meta.Template).Layers, seed, grid, out tile))
					{
						return true;
					}
				}
				else if (layer is BiomeTileLayer tileLayer && TryGetTile(indices, noiseCopy, tileLayer.Invert, tileLayer.Threshold, ProtoManager.Index<ContentTileDefinition>(tileLayer.Tile), tileLayer.Variants, out tile))
				{
					return true;
				}
			}
		}
		tile = null;
		return false;
	}

	[Obsolete("Use the Entity<MapGridComponent>? overload")]
	public bool TryGetTile(Vector2i indices, List<IBiomeLayer> layers, int seed, MapGridComponent? grid, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTile(indices, layers, seed, (grid == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid))), out tile);
	}

	private bool TryGetTile(Vector2i indices, FastNoiseLite noise, bool invert, float threshold, ContentTileDefinition tileDef, List<byte>? variants, [NotNullWhen(true)] out Tile? tile)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		float found = noise.GetNoise((float)indices.X, (float)indices.Y);
		found = (invert ? (found * -1f) : found);
		if (found < threshold)
		{
			tile = null;
			return false;
		}
		byte variant = 0;
		int variantCount = variants?.Count ?? tileDef.Variants;
		if (variantCount > 1)
		{
			float variantValue = (noise.GetNoise((float)(indices.X * 8), (float)(indices.Y * 8), (float)variantCount) + 1f) * 100f;
			variant = _tile.PickVariant(tileDef, (int)variantValue);
		}
		tile = new Tile((int)tileDef.TileId, variant, (byte)0, (byte)0);
		return true;
	}

	public bool TryGetEntity(Vector2i indices, BiomeComponent component, Entity<MapGridComponent>? grid, [NotNullWhen(true)] out string? entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetBiomeTile(indices, component.Layers, component.Seed, grid, out var tile))
		{
			entity = null;
			return false;
		}
		return TryGetEntity(indices, component.Layers, tile.Value, component.Seed, grid, out entity);
	}

	[Obsolete("Use the Entity<MapGridComponent>? overload")]
	public bool TryGetEntity(Vector2i indices, BiomeComponent component, MapGridComponent grid, [NotNullWhen(true)] out string? entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return TryGetEntity(indices, component, (grid == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid))), out entity);
	}

	public bool TryGetEntity(Vector2i indices, List<IBiomeLayer> layers, Tile tileRef, int seed, Entity<MapGridComponent>? grid, [NotNullWhen(true)] out string? entity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		string tileId = ((IPrototype)TileDefManager[tileRef.TypeId]).ID;
		for (int i = layers.Count - 1; i >= 0; i--)
		{
			IBiomeLayer layer = layers[i];
			if (layer is BiomeDummyLayer)
			{
				continue;
			}
			if (!(layer is IBiomeWorldLayer worldLayer))
			{
				if (!(layer is BiomeMetaLayer))
				{
					continue;
				}
			}
			else if (!worldLayer.AllowedTiles.Contains(tileId))
			{
				continue;
			}
			FastNoiseLite noiseCopy = GetNoise(layer.Noise, seed);
			bool invert = layer.Invert;
			float value = noiseCopy.GetNoise((float)indices.X, (float)indices.Y);
			value = (invert ? (value * -1f) : value);
			if (value < layer.Threshold)
			{
				continue;
			}
			if (layer is BiomeMetaLayer meta)
			{
				if (TryGetEntity(indices, ProtoManager.Index<BiomeTemplatePrototype>(meta.Template).Layers, tileRef, seed, grid, out entity))
				{
					return true;
				}
				continue;
			}
			if (!(layer is BiomeEntityLayer biomeLayer))
			{
				entity = null;
				return false;
			}
			float noiseValue = noiseCopy.GetNoise((float)indices.X, (float)indices.Y, (float)i);
			entity = Pick(biomeLayer.Entities, (noiseValue + 1f) / 2f);
			return true;
		}
		entity = null;
		return false;
	}

	[Obsolete("Use the Entity<MapGridComponent>? overload")]
	public bool TryGetEntity(Vector2i indices, List<IBiomeLayer> layers, Tile tileRef, int seed, MapGridComponent grid, [NotNullWhen(true)] out string? entity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		return TryGetEntity(indices, layers, tileRef, seed, (grid == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid))), out entity);
	}

	public bool TryGetDecals(Vector2i indices, List<IBiomeLayer> layers, int seed, Entity<MapGridComponent>? grid, [NotNullWhen(true)] out List<(string ID, Vector2 Position)>? decals)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		if (!TryGetBiomeTile(indices, layers, seed, grid, out var tileRef))
		{
			decals = null;
			return false;
		}
		string tileId = ((IPrototype)TileDefManager[tileRef.Value.TypeId]).ID;
		for (int i = layers.Count - 1; i >= 0; i--)
		{
			IBiomeLayer layer = layers[i];
			if (layer is BiomeDummyLayer)
			{
				continue;
			}
			if (!(layer is IBiomeWorldLayer worldLayer))
			{
				if (!(layer is BiomeMetaLayer))
				{
					continue;
				}
			}
			else if (!worldLayer.AllowedTiles.Contains(tileId))
			{
				continue;
			}
			bool invert = layer.Invert;
			FastNoiseLite noiseCopy = GetNoise(layer.Noise, seed);
			float value = noiseCopy.GetNoise((float)indices.X, (float)indices.Y);
			value = (invert ? (value * -1f) : value);
			if (value < layer.Threshold)
			{
				continue;
			}
			if (layer is BiomeMetaLayer meta)
			{
				if (TryGetDecals(indices, ProtoManager.Index<BiomeTemplatePrototype>(meta.Template).Layers, seed, grid, out decals))
				{
					return true;
				}
				continue;
			}
			if (!(layer is BiomeDecalLayer decalLayer))
			{
				decals = null;
				return false;
			}
			decals = new List<(string, Vector2)>();
			for (int x = 0; (float)x < decalLayer.Divisions; x++)
			{
				for (int y = 0; (float)y < decalLayer.Divisions; y++)
				{
					Vector2 index = new Vector2((float)indices.X + (float)x * 1f / decalLayer.Divisions, (float)indices.Y + (float)y * 1f / decalLayer.Divisions);
					float decalValue = noiseCopy.GetNoise(index.X, index.Y);
					decalValue = (invert ? (decalValue * -1f) : decalValue);
					if (!(decalValue < decalLayer.Threshold))
					{
						decals.Add((Pick(decalLayer.Decals, (noiseCopy.GetNoise((float)indices.X, (float)indices.Y, (float)x + (float)y * decalLayer.Divisions) + 1f) / 2f), index));
					}
				}
			}
			if (decals.Count != 0)
			{
				return true;
			}
		}
		decals = null;
		return false;
	}

	[Obsolete("Use the Entity<MapGridComponent>? overload")]
	public bool TryGetDecals(Vector2i indices, List<IBiomeLayer> layers, int seed, MapGridComponent grid, [NotNullWhen(true)] out List<(string ID, Vector2 Position)>? decals)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return TryGetDecals(indices, layers, seed, (grid == null) ? ((Entity<MapGridComponent>?)null) : new Entity<MapGridComponent>?(Entity<MapGridComponent>.op_Implicit((((Component)grid).Owner, grid))), out decals);
	}

	private FastNoiseLite GetNoise(FastNoiseLite seedNoise, int seed)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		FastNoiseLite noiseCopy = new FastNoiseLite();
		_serManager.CopyTo<FastNoiseLite>(seedNoise, ref noiseCopy, (ISerializationContext)null, false, true);
		noiseCopy.SetSeed(noiseCopy.GetSeed() + seed);
		noiseCopy.SetFractalOctaves(noiseCopy.GetFractalOctaves());
		return noiseCopy;
	}
}
