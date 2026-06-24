// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.CustomArraySerializer`2
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
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

public sealed class CustomArraySerializer<T, TCustomSerializer> : 
  ITypeSerializer<T[], SequenceDataNode>,
  ITypeReader<T[], SequenceDataNode>,
  ITypeValidator<T[], SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<T[], SequenceDataNode>,
  ITypeWriter<T[]>,
  BaseSerializerInterfaces.ITypeInterface<T[]>
  where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
  T[] ITypeReader<T[], SequenceDataNode>.Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<T[]>? instanceProvider)
  {
    T[] objArray = new T[node.Count];
    int num = 0;
    foreach (DataNode node1 in node)
      objArray[num++] = serializationManager.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode) node1, hookCtx, context);
    return objArray;
  }

  ValidationNode ITypeValidator<T[], SequenceDataNode>.Validate(
    ISerializationManager seri,
    SequenceDataNode node,
    IDependencyCollection deps,
    ISerializationContext? ctx)
  {
    List<ValidationNode> sequence = new List<ValidationNode>(node.Count);
    foreach (DataNode node1 in node)
      sequence.Add(seri.ValidateNode<T, ValueDataNode, TCustomSerializer>((ValueDataNode) node1, ctx));
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public DataNode Write(
    ISerializationManager seri,
    T[] value,
    IDependencyCollection deps,
    bool alwaysWrite = false,
    ISerializationContext? ctx = null)
  {
    SequenceDataNode sequenceDataNode = new SequenceDataNode();
    foreach (T obj in value)
      sequenceDataNode.Add(seri.WriteValue<T, TCustomSerializer>(obj, alwaysWrite, ctx));
    return (DataNode) sequenceDataNode;
  }
}
