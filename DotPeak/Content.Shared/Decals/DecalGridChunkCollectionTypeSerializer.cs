// Decompiled with JetBrains decompiler
// Type: Content.Shared.Decals.DecalGridChunkCollectionTypeSerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared.Decals;

[TypeSerializer]
public sealed class DecalGridChunkCollectionTypeSerializer : 
  ITypeSerializer<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>,
  ITypeReader<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>,
  ITypeValidator<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<DecalGridComponent.DecalGridChunkCollection, MappingDataNode>,
  ITypeWriter<DecalGridComponent.DecalGridChunkCollection>,
  BaseSerializerInterfaces.ITypeInterface<DecalGridComponent.DecalGridChunkCollection>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return serializationManager.ValidateNode<Dictionary<Vector2i, Dictionary<uint, Decal>>>((DataNode) node, context);
  }

  public DecalGridComponent.DecalGridChunkCollection Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<DecalGridComponent.DecalGridChunkCollection>? _ = null)
  {
    DataNode dataNode1;
    node.TryGetValue("version", ref dataNode1);
    ValueDataNode valueDataNode = (ValueDataNode) dataNode1;
    int num1 = valueDataNode != null ? valueDataNode.AsInt() : 1;
    uint num2 = 0;
    HashSet<uint> uintSet = new HashSet<uint>();
    Dictionary<Vector2i, DecalGridComponent.DecalChunk> ChunkCollection;
    if (num1 > 1)
    {
      SequenceDataNode sequenceDataNode = (SequenceDataNode) node["nodes"];
      ChunkCollection = new Dictionary<Vector2i, DecalGridComponent.DecalChunk>();
      foreach (MappingDataNode mappingDataNode in sequenceDataNode)
      {
        DecalGridChunkCollectionTypeSerializer.DecalData decalData = serializationManager.Read<DecalGridChunkCollectionTypeSerializer.DecalData>(mappingDataNode[nameof (node)], hookCtx, context, (ISerializationManager.InstantiationDelegate<DecalGridChunkCollectionTypeSerializer.DecalData>) null, false);
        foreach ((string str, DataNode dataNode2) in (MappingDataNode) mappingDataNode["decals"])
        {
          CultureInfo invariantCulture = CultureInfo.InvariantCulture;
          uint num3 = uint.Parse(str, (IFormatProvider) invariantCulture);
          Vector2 coordinates = serializationManager.Read<Vector2>(dataNode2, hookCtx, context, (ISerializationManager.InstantiationDelegate<Vector2>) null, false);
          Vector2i chunkIndices = SharedMapSystem.GetChunkIndices(coordinates, 32 /*0x20*/);
          DecalGridComponent.DecalChunk orNew = Extensions.GetOrNew<Vector2i, DecalGridComponent.DecalChunk>(ChunkCollection, chunkIndices);
          Decal decal = new Decal(coordinates, decalData.Id, decalData.Color, decalData.Angle, decalData.ZIndex, decalData.Cleanable);
          num2 = Math.Max(num2, num3);
          if (!uintSet.Add(num3))
          {
            num3 = num2++;
            uintSet.Add(num3);
          }
          orNew.Decals[num3] = decal;
        }
      }
    }
    else
    {
      ChunkCollection = serializationManager.Read<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>((DataNode) node, hookCtx, context, (ISerializationManager.InstantiationDelegate<Dictionary<Vector2i, DecalGridComponent.DecalChunk>>) null, true);
      foreach (DecalGridComponent.DecalChunk decalChunk in ChunkCollection.Values)
      {
        foreach (uint key in decalChunk.Decals.Keys)
          num2 = Math.Max(key, num2);
      }
    }
    uint num4 = num2 + 1U;
    return new DecalGridComponent.DecalGridChunkCollection(ChunkCollection)
    {
      NextDecalId = num4
    };
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    DecalGridComponent.DecalGridChunkCollection value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    Dictionary<DecalGridChunkCollectionTypeSerializer.DecalData, List<uint>> dictionary1 = new Dictionary<DecalGridChunkCollectionTypeSerializer.DecalData, List<uint>>();
    Dictionary<uint, Decal> dictionary2 = new Dictionary<uint, Decal>();
    MappingDataNode mappingDataNode1 = new MappingDataNode();
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (DecalGridComponent.DecalChunk decalChunk in value.ChunkCollection.Values)
    {
      foreach ((uint key, Decal decal) in decalChunk.Decals)
      {
        DecalGridChunkCollectionTypeSerializer.DecalData decalData = new DecalGridChunkCollectionTypeSerializer.DecalData(decal);
        Extensions.GetOrNew<DecalGridChunkCollectionTypeSerializer.DecalData, List<uint>>(dictionary1, decalData).Add(key);
        dictionary2[key] = decal;
      }
    }
    List<DecalGridChunkCollectionTypeSerializer.DecalData> list = dictionary1.Keys.ToList<DecalGridChunkCollectionTypeSerializer.DecalData>();
    list.Sort();
    foreach (DecalGridChunkCollectionTypeSerializer.DecalData key1 in list)
    {
      List<uint> uintList = dictionary1[key1];
      MappingDataNode mappingDataNode2 = new MappingDataNode();
      mappingDataNode2.Add("node", serializationManager.WriteValue<DecalGridChunkCollectionTypeSerializer.DecalData>(key1, alwaysWrite, context, false));
      MappingDataNode mappingDataNode3 = mappingDataNode2;
      MappingDataNode mappingDataNode4 = new MappingDataNode();
      uintList.Sort();
      foreach (uint key2 in uintList)
      {
        Decal decal = dictionary2[key2];
        mappingDataNode4.Add(key2.ToString(), serializationManager.WriteValue<Vector2>(decal.Coordinates, alwaysWrite, context, false));
      }
      mappingDataNode3.Add("decals", (DataNode) mappingDataNode4);
      sequenceDataNode.Add((DataNode) mappingDataNode3);
    }
    MappingDataNodeExtensions.Add(mappingDataNode1, "version", 2.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    mappingDataNode1.Add("nodes", (DataNode) sequenceDataNode);
    return (DataNode) mappingDataNode1;
  }

  [DataDefinition]
  private readonly struct DecalData : 
    IEquatable<DecalGridChunkCollectionTypeSerializer.DecalData>,
    IComparable<DecalGridChunkCollectionTypeSerializer.DecalData>,
    ISerializationGenerated<DecalGridChunkCollectionTypeSerializer.DecalData>,
    ISerializationGenerated
  {
    [DataField("id", false, 1, false, false, null)]
    public string Id { get; init; }

    [DataField("color", false, 1, false, false, null)]
    public Color? Color { get; init; }

    [DataField("angle", false, 1, false, false, null)]
    public Angle Angle { get; init; }

    [DataField("zIndex", false, 1, false, false, null)]
    public int ZIndex { get; init; }

    [DataField("cleanable", false, 1, false, false, null)]
    public bool Cleanable { get; init; }

    public DecalData(string id, Color? color, Angle angle, int zIndex, bool cleanable)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CId\u003Ek__BackingField = string.Empty;
      // ISSUE: reference to a compiler-generated field
      this.\u003CAngle\u003Ek__BackingField = Angle.Zero;
      this.Id = id;
      this.Color = color;
      this.Angle = angle;
      this.ZIndex = zIndex;
      this.Cleanable = cleanable;
    }

    public DecalData(Decal decal)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CId\u003Ek__BackingField = string.Empty;
      // ISSUE: reference to a compiler-generated field
      this.\u003CAngle\u003Ek__BackingField = Angle.Zero;
      this.Id = decal.Id;
      this.Color = decal.Color;
      this.Angle = decal.Angle;
      this.ZIndex = decal.ZIndex;
      this.Cleanable = decal.Cleanable;
    }

    public bool Equals(
      DecalGridChunkCollectionTypeSerializer.DecalData other)
    {
      if (this.Id == other.Id && Nullable.Equals<Color>(this.Color, other.Color))
      {
        Angle angle = this.Angle;
        if (((Angle) ref angle).Equals(other.Angle) && this.ZIndex == other.ZIndex)
          return this.Cleanable == other.Cleanable;
      }
      return false;
    }

    public override bool Equals(object? obj)
    {
      return obj is DecalGridChunkCollectionTypeSerializer.DecalData other && this.Equals(other);
    }

    public override int GetHashCode()
    {
      return HashCode.Combine<string, Color?, Angle, int, bool>(this.Id, this.Color, this.Angle, this.ZIndex, this.Cleanable);
    }

    public int CompareTo(
      DecalGridChunkCollectionTypeSerializer.DecalData other)
    {
      int num1 = string.Compare(this.Id, other.Id, StringComparison.Ordinal);
      if (num1 != 0)
        return num1;
      Color? color1 = this.Color;
      ref Color? local1 = ref color1;
      Color valueOrDefault;
      string strA;
      if (!local1.HasValue)
      {
        strA = (string) null;
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        strA = ((Color) ref valueOrDefault).ToHex();
      }
      Color? color2 = other.Color;
      ref Color? local2 = ref color2;
      string strB;
      if (!local2.HasValue)
      {
        strB = (string) null;
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        strB = ((Color) ref valueOrDefault).ToHex();
      }
      int num2 = string.Compare(strA, strB, StringComparison.Ordinal);
      if (num2 != 0)
        return num2;
      int num3 = this.Angle.Theta.CompareTo(other.Angle.Theta);
      if (num3 != 0)
        return num3;
      int num4 = this.ZIndex.CompareTo(other.ZIndex);
      return num4 != 0 ? num4 : this.Cleanable.CompareTo(other.Cleanable);
    }

    public DecalData()
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003CColor\u003Ek__BackingField = new Color?();
      // ISSUE: reference to a compiler-generated field
      this.\u003CZIndex\u003Ek__BackingField = 0;
      // ISSUE: reference to a compiler-generated field
      this.\u003CCleanable\u003Ek__BackingField = false;
      // ISSUE: reference to a compiler-generated field
      this.\u003CId\u003Ek__BackingField = string.Empty;
      // ISSUE: reference to a compiler-generated field
      this.\u003CAngle\u003Ek__BackingField = Angle.Zero;
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void InternalCopy(
      ref DecalGridChunkCollectionTypeSerializer.DecalData target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      if (serialization.TryCustomCopy<DecalGridChunkCollectionTypeSerializer.DecalData>(this, ref target, hookCtx, false, context))
        return;
      string str = (string) null;
      if (this.Id == null)
        throw new NullNotAllowedException();
      if (!serialization.TryCustomCopy<string>(this.Id, ref str, hookCtx, false, context))
        str = this.Id;
      Color? nullable = new Color?();
      if (!serialization.TryCustomCopy<Color?>(this.Color, ref nullable, hookCtx, false, context))
        nullable = serialization.CreateCopy<Color?>(this.Color, hookCtx, context, false);
      Angle angle = new Angle();
      if (!serialization.TryCustomCopy<Angle>(this.Angle, ref angle, hookCtx, false, context))
        angle = serialization.CreateCopy<Angle>(this.Angle, hookCtx, context, false);
      int num = 0;
      if (!serialization.TryCustomCopy<int>(this.ZIndex, ref num, hookCtx, false, context))
        num = this.ZIndex;
      bool flag = false;
      if (!serialization.TryCustomCopy<bool>(this.Cleanable, ref flag, hookCtx, false, context))
        flag = this.Cleanable;
      target = target with
      {
        Id = str,
        Color = nullable,
        Angle = angle,
        ZIndex = num,
        Cleanable = flag
      };
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref DecalGridChunkCollectionTypeSerializer.DecalData target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      this.InternalCopy(ref target, serialization, hookCtx, context);
    }

    [Obsolete("Use ISerializationManager.CopyTo instead")]
    public void Copy(
      ref object target,
      ISerializationManager serialization,
      SerializationHookContext hookCtx,
      ISerializationContext? context = null)
    {
      DecalGridChunkCollectionTypeSerializer.DecalData target1 = (DecalGridChunkCollectionTypeSerializer.DecalData) target;
      this.Copy(ref target1, serialization, hookCtx, context);
      target = (object) target1;
    }

    [Obsolete("Use ISerializationManager.CreateCopy instead")]
    public DecalGridChunkCollectionTypeSerializer.DecalData Instantiate()
    {
      return new DecalGridChunkCollectionTypeSerializer.DecalData();
    }
  }
}
