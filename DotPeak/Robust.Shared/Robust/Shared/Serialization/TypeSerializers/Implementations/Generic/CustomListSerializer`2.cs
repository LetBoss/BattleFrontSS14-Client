// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.CustomListSerializer`2
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

public sealed class CustomListSerializer<T, TCustomSerializer> : 
  ITypeSerializer<List<T>, SequenceDataNode>,
  ITypeReader<List<T>, SequenceDataNode>,
  ITypeValidator<List<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<List<T>, SequenceDataNode>,
  ITypeWriter<List<T>>,
  BaseSerializerInterfaces.ITypeInterface<List<T>>
  where TCustomSerializer : ITypeSerializer<T, ValueDataNode>
{
  List<T> ITypeReader<List<T>, SequenceDataNode>.Read(
    ISerializationManager seri,
    SequenceDataNode node,
    IDependencyCollection deps,
    SerializationHookContext hookCtx,
    ISerializationContext? ctx,
    ISerializationManager.InstantiationDelegate<List<T>>? instanceProvider)
  {
    List<T> objList = instanceProvider != null ? instanceProvider() : new List<T>(node.Count);
    foreach (DataNode node1 in node)
    {
      T obj = seri.Read<T, ValueDataNode, TCustomSerializer>((ValueDataNode) node1, hookCtx, ctx);
      objList.Add(obj);
    }
    return objList;
  }

  ValidationNode ITypeValidator<List<T>, SequenceDataNode>.Validate(
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
    List<T> value,
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
