// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.QueueSerializer`1
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
public sealed class QueueSerializer<T> : 
  ITypeSerializer<Queue<T>, SequenceDataNode>,
  ITypeReader<Queue<T>, SequenceDataNode>,
  ITypeValidator<Queue<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Queue<T>, SequenceDataNode>,
  ITypeWriter<Queue<T>>,
  BaseSerializerInterfaces.ITypeInterface<Queue<T>>,
  ITypeCopier<Queue<T>>
{
  Queue<T> ITypeReader<Queue<T>, SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<Queue<T>>? instanceProvider)
  {
    Queue<T> objQueue = instanceProvider != null ? instanceProvider() : new Queue<T>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
      objQueue.Enqueue(serializationManager.Read<T>(node1, hookCtx, context));
    return objQueue;
  }

  ValidationNode ITypeValidator<Queue<T>, SequenceDataNode>.Validate(
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
    Queue<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (T obj in value)
      sequenceDataNode.Add(serializationManager.WriteValue<T>(obj, alwaysWrite, context));
    return (DataNode) sequenceDataNode;
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    Queue<T> source,
    ref Queue<T> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.EnsureCapacity(source.Count);
    foreach (T source1 in source)
      target.Enqueue(serializationManager.CreateCopy<T>(source1, hookCtx, context));
  }
}
