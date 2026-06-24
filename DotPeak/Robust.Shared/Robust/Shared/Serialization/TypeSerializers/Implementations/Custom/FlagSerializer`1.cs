// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.FlagSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class FlagSerializer<TTag> : 
  ITypeSerializer<int, ValueDataNode>,
  ITypeReader<int, ValueDataNode>,
  ITypeValidator<int, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<int, ValueDataNode>,
  ITypeWriter<int>,
  BaseSerializerInterfaces.ITypeInterface<int>,
  ITypeReader<int, SequenceDataNode>,
  ITypeValidator<int, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<int, SequenceDataNode>,
  ITypeCopyCreator<int>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !Enum.TryParse(serializationManager.GetFlagTypeFromTag(typeof (TTag)), node.Value, out object _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing flag.", false) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public int Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
  {
    return (int) Enum.Parse(serializationManager.GetFlagTypeFromTag(typeof (TTag)), node.Value);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    int value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof (TTag));
    if (value == -1)
    {
      string name = Enum.GetName(flagTypeFromTag, (object) -1);
      if (name != null)
      {
        sequenceDataNode.Add((DataNode) new ValueDataNode(name));
        return (DataNode) sequenceDataNode;
      }
    }
    int flagHighestBit = serializationManager.GetFlagHighestBit(typeof (TTag));
    for (int index = 1; index <= flagHighestBit; ++index)
    {
      int num = 1 << index;
      if ((num & value) == num)
        sequenceDataNode.Add((DataNode) new ValueDataNode(Enum.GetName(flagTypeFromTag, (object) num) ?? throw new InvalidOperationException($"No bitflag corresponding to bit {index} in {flagTypeFromTag}, but it was set anyways.")));
    }
    return (DataNode) sequenceDataNode;
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof (TTag));
    foreach (DataNode dataNode in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(dataNode is ValueDataNode valueDataNode))
        return (ValidationNode) new ErrorNode((DataNode) node, "Invalid flagtype in flag-sequence.");
      if (!Enum.TryParse(flagTypeFromTag, valueDataNode.Value, out object _))
        return (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing flag in flag-sequence", false);
    }
    return (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public int Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
  {
    Type flagTypeFromTag = serializationManager.GetFlagTypeFromTag(typeof (TTag));
    int num = 0;
    foreach (DataNode dataNode in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(dataNode is ValueDataNode valueDataNode))
        throw new InvalidNodeTypeException();
      num |= (int) Enum.Parse(flagTypeFromTag, valueDataNode.Value);
    }
    return num;
  }

  public int CreateCopy(
    ISerializationManager serializationManager,
    int source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
