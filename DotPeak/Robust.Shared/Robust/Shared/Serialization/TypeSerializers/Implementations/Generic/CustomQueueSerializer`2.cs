// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.CustomQueueSerializer`2
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
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

public sealed class CustomQueueSerializer<T, TCustomSerializer> : 
  ITypeSerializer<Queue<T>, SequenceDataNode>,
  ITypeReader<Queue<T>, SequenceDataNode>,
  ITypeValidator<Queue<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Queue<T>, SequenceDataNode>,
  ITypeWriter<Queue<T>>,
  BaseSerializerInterfaces.ITypeInterface<Queue<T>>
  where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
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
      objQueue.Enqueue(serializationManager.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode) node1, hookCtx, context) ?? throw new InvalidOperationException("TCustomSerializer returned a null value when reading using a custom hashset serializer."));
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
      sequence.Add(serializationManager.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode) node1, context));
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
      sequenceDataNode.Add(serializationManager.WriteValue<T, TCustomSerializer>(obj, alwaysWrite, context));
    return (DataNode) sequenceDataNode;
  }
}
