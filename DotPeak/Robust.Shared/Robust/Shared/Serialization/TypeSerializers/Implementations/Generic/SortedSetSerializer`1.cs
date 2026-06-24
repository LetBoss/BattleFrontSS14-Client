// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.SortedSetSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class SortedSetSerializer<T> : 
  ITypeSerializer<SortedSet<T>, SequenceDataNode>,
  ITypeReader<SortedSet<T>, SequenceDataNode>,
  ITypeValidator<SortedSet<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SortedSet<T>, SequenceDataNode>,
  ITypeWriter<SortedSet<T>>,
  BaseSerializerInterfaces.ITypeInterface<SortedSet<T>>,
  ITypeCopyCreator<SortedSet<T>>
{
  SortedSet<T> ITypeReader<SortedSet<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<SortedSet<T>>? instanceProvider)
  {
    SortedSet<T> sortedSet = instanceProvider != null ? instanceProvider() : new SortedSet<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      sortedSet.Add(serializationManager.Read<T>(node1, hookCtx, context));
    return sortedSet;
  }

  ValidationNode ITypeValidator<SortedSet<T>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      sequence.Add(serializationManager.ValidateNode<T>(node1, context));
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    SortedSet<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (T obj in value)
      sequenceDataNode.Add(serializationManager.WriteValue<T>(obj, alwaysWrite, context));
    return (DataNode) sequenceDataNode;
  }

  SortedSet<T> ITypeCopyCreator<SortedSet<T>>.CreateCopy(
    ISerializationManager serializationManager,
    SortedSet<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context)
  {
    SortedSet<T> copy = new SortedSet<T>();
    foreach (T source1 in source)
      copy.Add(serializationManager.CreateCopy<T>(source1, hookCtx, context));
    return copy;
  }
}
