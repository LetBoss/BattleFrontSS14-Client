// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.ValueTupleSerializer`2
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class ValueTupleSerializer<T1, T2> : 
  ITypeReader<(T1, T2), MappingDataNode>,
  ITypeValidator<(T1, T2), MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<(T1, T2), MappingDataNode>,
  ITypeSerializer<(T1, T2), SequenceDataNode>,
  ITypeReader<(T1, T2), SequenceDataNode>,
  ITypeValidator<(T1, T2), SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<(T1, T2), SequenceDataNode>,
  ITypeWriter<(T1, T2)>,
  BaseSerializerInterfaces.ITypeInterface<(T1, T2)>,
  ITypeCopyCreator<(T1, T2)>
{
  public (T1, T2) Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<(T1, T2)>? instanceProvider = null)
  {
    if (node.Children.Count != 1)
      throw new InvalidMappingException("Less than or more than 1 mappings provided to ValueTupleSerializer");
    KeyValuePair<string, DataNode> keyValuePair = node.Children.First<KeyValuePair<string, DataNode>>();
    return (serializationManager.Read<T1>((DataNode) node.GetKeyNode(keyValuePair.Key), hookCtx, context), serializationManager.Read<T2>(keyValuePair.Value, hookCtx, context));
  }

  public (T1, T2) Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<(T1, T2)>? val = null)
  {
    if (node.Count != 2)
      throw new InvalidMappingException("Sequence must contain exactly 2 elements.");
    return (serializationManager.Read<T1>(node[0], hookCtx, context), serializationManager.Read<T2>(node[1], hookCtx, context));
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (node.Children.Count != 1)
      return (ValidationNode) new ErrorNode((DataNode) node, "More or less than 1 Mapping for ValueTuple found.");
    KeyValuePair<string, DataNode> keyValuePair = node.Children.First<KeyValuePair<string, DataNode>>();
    return (ValidationNode) new ValidatedMappingNode(new Dictionary<ValidationNode, ValidationNode>()
    {
      {
        serializationManager.ValidateNode<T1>((DataNode) node.GetKeyNode(keyValuePair.Key), context),
        serializationManager.ValidateNode<T2>(keyValuePair.Value, context)
      }
    });
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (node.Count != 2)
      throw new InvalidMappingException("Sequence must contain exactly 2 elements.");
    return (ValidationNode) new ValidatedSequenceNode(new List<ValidationNode>()
    {
      serializationManager.ValidateNode<T1>(node[0], context),
      serializationManager.ValidateNode<T2>(node[1], context)
    });
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    (T1, T2) value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new SequenceDataNode(new List<DataNode>()
    {
      serializationManager.WriteValue<T1>(value.Item1, alwaysWrite, context),
      serializationManager.WriteValue<T2>(value.Item2, alwaysWrite, context)
    });
  }

  public (T1, T2) CreateCopy(
    ISerializationManager serializationManager,
    (T1, T2) source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return (serializationManager.CreateCopy<T1>(source.Item1, hookCtx, context), serializationManager.CreateCopy<T2>(source.Item2, hookCtx, context));
  }
}
