// Decompiled with JetBrains decompiler
// Type: Robust.Shared.EntitySerialization.MapChunkSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

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
using System;
using System.Collections.Generic;
using System.IO;

#nullable enable
namespace Robust.Shared.EntitySerialization;

[TypeSerializer]
internal sealed class MapChunkSerializer : 
  ITypeSerializer<MapChunk, MappingDataNode>,
  ITypeReader<MapChunk, MappingDataNode>,
  ITypeValidator<MapChunk, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<MapChunk, MappingDataNode>,
  ITypeWriter<MapChunk>,
  BaseSerializerInterfaces.ITypeInterface<MapChunk>,
  ITypeCopyCreator<MapChunk>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    throw new NotImplementedException();
  }

  public MapChunk Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<MapChunk>? instantiationDelegate = null)
  {
    Vector2i vector2i = (Vector2i) serializationManager.Read(typeof (Vector2i), node["ind"], hookCtx, context);
    using (MemoryStream input = new MemoryStream(Convert.FromBase64String(((ValueDataNode) node["tiles"]).Value)))
    {
      using (BinaryReader binaryReader = new BinaryReader((Stream) input))
      {
        IMapManager mapManager = dependencies.Resolve<IMapManager>();
        mapManager.SuppressOnTileChanged = true;
        ushort chunkSize = 16 /*0x10*/;
        ValueDataNode node1;
        if (node.TryGet<ValueDataNode>("size", out node1))
          chunkSize = (ushort) serializationManager.Read(typeof (ushort), (DataNode) node1, context);
        MapChunk mapChunk = instantiationDelegate != null ? instantiationDelegate() : new MapChunk(vector2i.X, vector2i.Y, chunkSize);
        IReadOnlyDictionary<int, string> readOnlyDictionary = (IReadOnlyDictionary<int, string>) null;
        if (context is EntityDeserializer entityDeserializer)
          readOnlyDictionary = (IReadOnlyDictionary<int, string>) entityDeserializer.TileMap;
        if (readOnlyDictionary == null)
          throw new InvalidOperationException("Someone tried deserializing a gridchunk before deserializing the tileMap.");
        mapChunk.SuppressCollisionRegeneration = true;
        ITileDefinitionManager definitionManager = dependencies.Resolve<ITileDefinitionManager>();
        DataNode dataNode;
        node.TryGetValue("version", out dataNode);
        ValueDataNode valueDataNode = (ValueDataNode) dataNode;
        int num = valueDataNode != null ? valueDataNode.AsInt() : 1;
        Tile oldTile;
        bool shapeChanged;
        if (num >= 7)
        {
          for (ushort yIndex = 0; (int) yIndex < (int) mapChunk.ChunkSize; ++yIndex)
          {
            for (ushort xIndex = 0; (int) xIndex < (int) mapChunk.ChunkSize; ++xIndex)
            {
              int key = binaryReader.ReadInt32();
              byte flags = binaryReader.ReadByte();
              byte variant = binaryReader.ReadByte();
              byte rotationMirroring = binaryReader.ReadByte();
              string name = readOnlyDictionary[key];
              Tile tile = new Tile((int) definitionManager[name].TileId, flags, variant, rotationMirroring);
              mapChunk.TrySetTile(xIndex, yIndex, tile, out oldTile, out shapeChanged);
            }
          }
        }
        else
        {
          for (ushort yIndex = 0; (int) yIndex < (int) mapChunk.ChunkSize; ++yIndex)
          {
            for (ushort xIndex = 0; (int) xIndex < (int) mapChunk.ChunkSize; ++xIndex)
            {
              int key = num < 6 ? (int) binaryReader.ReadUInt16() : binaryReader.ReadInt32();
              byte flags = binaryReader.ReadByte();
              byte variant = binaryReader.ReadByte();
              string name = readOnlyDictionary[key];
              Tile tile = new Tile((int) definitionManager[name].TileId, flags, variant);
              mapChunk.TrySetTile(xIndex, yIndex, tile, out oldTile, out shapeChanged);
            }
          }
        }
        mapChunk.SuppressCollisionRegeneration = false;
        mapManager.SuppressOnTileChanged = false;
        return mapChunk;
      }
    }
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    MapChunk value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    MappingDataNode mappingDataNode = new MappingDataNode();
    mappingDataNode.Add("ind", (DataNode) new ValueDataNode($"{value.X},{value.Y}"));
    ValueDataNode node = new ValueDataNode();
    mappingDataNode.Add("tiles", (DataNode) node);
    mappingDataNode.Add("version", (DataNode) new ValueDataNode("7"));
    node.Value = MapChunkSerializer.SerializeTiles(value, context as EntitySerializer);
    return (DataNode) mappingDataNode;
  }

  private static string SerializeTiles(MapChunk chunk, EntitySerializer? serializer)
  {
    byte[] numArray = new byte[(int) chunk.ChunkSize * (int) chunk.ChunkSize * 7];
    using (MemoryStream output = new MemoryStream(numArray))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
      {
        if (serializer == null)
        {
          for (ushort yIndex = 0; (int) yIndex < (int) chunk.ChunkSize; ++yIndex)
          {
            for (ushort xIndex = 0; (int) xIndex < (int) chunk.ChunkSize; ++xIndex)
            {
              Tile tile = chunk.GetTile(xIndex, yIndex);
              binaryWriter.Write(tile.TypeId);
              binaryWriter.Write(tile.Flags);
              binaryWriter.Write(tile.Variant);
              binaryWriter.Write(tile.RotationMirroring);
            }
          }
          return Convert.ToBase64String(numArray);
        }
        int num1 = -1;
        int num2 = -1;
        for (ushort yIndex = 0; (int) yIndex < (int) chunk.ChunkSize; ++yIndex)
        {
          for (ushort xIndex = 0; (int) xIndex < (int) chunk.ChunkSize; ++xIndex)
          {
            Tile tile = chunk.GetTile(xIndex, yIndex);
            if (tile.TypeId != num1)
              num2 = serializer.GetYamlTileId(tile.TypeId);
            num1 = tile.TypeId;
            binaryWriter.Write(num2);
            binaryWriter.Write(tile.Flags);
            binaryWriter.Write(tile.Variant);
            binaryWriter.Write(tile.RotationMirroring);
          }
        }
      }
    }
    return Convert.ToBase64String(numArray);
  }

  public MapChunk CreateCopy(
    ISerializationManager serializationManager,
    MapChunk source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    IMapManager mapManager = dependencies.Resolve<IMapManager>();
    mapManager.SuppressOnTileChanged = true;
    MapChunk copy = new MapChunk(source.X, source.Y, source.ChunkSize)
    {
      SuppressCollisionRegeneration = true
    };
    for (ushort yIndex = 0; (int) yIndex < (int) copy.ChunkSize; ++yIndex)
    {
      for (ushort xIndex = 0; (int) xIndex < (int) copy.ChunkSize; ++xIndex)
        copy.TrySetTile(xIndex, yIndex, source.GetTile(xIndex, yIndex), out Tile _, out bool _);
    }
    mapManager.SuppressOnTileChanged = false;
    copy.SuppressCollisionRegeneration = false;
    return copy;
  }
}
