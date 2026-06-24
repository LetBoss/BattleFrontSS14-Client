using System;
using System.Collections.Generic;
using System.IO;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.EntitySerialization;

[TypeSerializer]
internal sealed class MapChunkSerializer : ITypeSerializer<MapChunk, MappingDataNode>, ITypeReader<MapChunk, MappingDataNode>, ITypeValidator<MapChunk, MappingDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<MapChunk, MappingDataNode>, ITypeWriter<MapChunk>, BaseSerializerInterfaces.ITypeInterface<MapChunk>, ITypeCopyCreator<MapChunk>
{
	public ValidationNode Validate(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		throw new NotImplementedException();
	}

	public MapChunk Read(ISerializationManager serializationManager, MappingDataNode node, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null, ISerializationManager.InstantiationDelegate<MapChunk>? instantiationDelegate = null)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Vector2i val = (Vector2i)serializationManager.Read(typeof(Vector2i), node["ind"], hookCtx, context);
		using MemoryStream input = new MemoryStream(Convert.FromBase64String(((ValueDataNode)node["tiles"]).Value));
		using BinaryReader binaryReader = new BinaryReader(input);
		IMapManager mapManager = dependencies.Resolve<IMapManager>();
		mapManager.SuppressOnTileChanged = true;
		ushort chunkSize = 16;
		if (node.TryGet("size", out ValueDataNode node2))
		{
			chunkSize = (ushort)serializationManager.Read(typeof(ushort), node2, context);
		}
		MapChunk mapChunk = ((instantiationDelegate != null) ? instantiationDelegate() : new MapChunk(val.X, val.Y, chunkSize));
		IReadOnlyDictionary<int, string> readOnlyDictionary = null;
		if (context is EntityDeserializer entityDeserializer)
		{
			readOnlyDictionary = entityDeserializer.TileMap;
		}
		if (readOnlyDictionary == null)
		{
			throw new InvalidOperationException("Someone tried deserializing a gridchunk before deserializing the tileMap.");
		}
		mapChunk.SuppressCollisionRegeneration = true;
		ITileDefinitionManager tileDefinitionManager = dependencies.Resolve<ITileDefinitionManager>();
		node.TryGetValue("version", out DataNode value);
		int num = ((ValueDataNode)value)?.AsInt() ?? 1;
		Tile oldTile;
		bool shapeChanged;
		if (num >= 7)
		{
			for (ushort num2 = 0; num2 < mapChunk.ChunkSize; num2++)
			{
				for (ushort num3 = 0; num3 < mapChunk.ChunkSize; num3++)
				{
					int key = binaryReader.ReadInt32();
					byte flags = binaryReader.ReadByte();
					byte variant = binaryReader.ReadByte();
					byte rotationMirroring = binaryReader.ReadByte();
					string name = readOnlyDictionary[key];
					key = tileDefinitionManager[name].TileId;
					Tile tile = new Tile(key, flags, variant, rotationMirroring);
					mapChunk.TrySetTile(num3, num2, tile, out oldTile, out shapeChanged);
				}
			}
		}
		else
		{
			for (ushort num4 = 0; num4 < mapChunk.ChunkSize; num4++)
			{
				for (ushort num5 = 0; num5 < mapChunk.ChunkSize; num5++)
				{
					int key2 = ((num < 6) ? binaryReader.ReadUInt16() : binaryReader.ReadInt32());
					byte flags2 = binaryReader.ReadByte();
					byte variant2 = binaryReader.ReadByte();
					string name2 = readOnlyDictionary[key2];
					key2 = tileDefinitionManager[name2].TileId;
					Tile tile2 = new Tile(key2, flags2, variant2, 0);
					mapChunk.TrySetTile(num5, num4, tile2, out oldTile, out shapeChanged);
				}
			}
		}
		mapChunk.SuppressCollisionRegeneration = false;
		mapManager.SuppressOnTileChanged = false;
		return mapChunk;
	}

	public DataNode Write(ISerializationManager serializationManager, MapChunk value, IDependencyCollection dependencies, bool alwaysWrite = false, ISerializationContext? context = null)
	{
		MappingDataNode mappingDataNode = new MappingDataNode();
		ValueDataNode node = new ValueDataNode($"{value.X},{value.Y}");
		mappingDataNode.Add("ind", node);
		ValueDataNode valueDataNode = new ValueDataNode();
		mappingDataNode.Add("tiles", valueDataNode);
		mappingDataNode.Add("version", new ValueDataNode("7"));
		valueDataNode.Value = SerializeTiles(value, context as EntitySerializer);
		return mappingDataNode;
	}

	private static string SerializeTiles(MapChunk chunk, EntitySerializer? serializer)
	{
		byte[] array = new byte[chunk.ChunkSize * chunk.ChunkSize * 7];
		using (MemoryStream output = new MemoryStream(array))
		{
			using BinaryWriter binaryWriter = new BinaryWriter(output);
			if (serializer == null)
			{
				for (ushort num = 0; num < chunk.ChunkSize; num++)
				{
					for (ushort num2 = 0; num2 < chunk.ChunkSize; num2++)
					{
						Tile tile = chunk.GetTile(num2, num);
						binaryWriter.Write(tile.TypeId);
						binaryWriter.Write(tile.Flags);
						binaryWriter.Write(tile.Variant);
						binaryWriter.Write(tile.RotationMirroring);
					}
				}
				return Convert.ToBase64String(array);
			}
			int num3 = -1;
			int value = -1;
			for (ushort num4 = 0; num4 < chunk.ChunkSize; num4++)
			{
				for (ushort num5 = 0; num5 < chunk.ChunkSize; num5++)
				{
					Tile tile2 = chunk.GetTile(num5, num4);
					if (tile2.TypeId != num3)
					{
						value = serializer.GetYamlTileId(tile2.TypeId);
					}
					num3 = tile2.TypeId;
					binaryWriter.Write(value);
					binaryWriter.Write(tile2.Flags);
					binaryWriter.Write(tile2.Variant);
					binaryWriter.Write(tile2.RotationMirroring);
				}
			}
		}
		return Convert.ToBase64String(array);
	}

	public MapChunk CreateCopy(ISerializationManager serializationManager, MapChunk source, IDependencyCollection dependencies, SerializationHookContext hookCtx, ISerializationContext? context = null)
	{
		IMapManager mapManager = dependencies.Resolve<IMapManager>();
		mapManager.SuppressOnTileChanged = true;
		MapChunk mapChunk = new MapChunk(source.X, source.Y, source.ChunkSize)
		{
			SuppressCollisionRegeneration = true
		};
		for (ushort num = 0; num < mapChunk.ChunkSize; num++)
		{
			for (ushort num2 = 0; num2 < mapChunk.ChunkSize; num2++)
			{
				mapChunk.TrySetTile(num2, num, source.GetTile(num2, num), out var _, out var _);
			}
		}
		mapManager.SuppressOnTileChanged = false;
		mapChunk.SuppressCollisionRegeneration = false;
		return mapChunk;
	}
}
