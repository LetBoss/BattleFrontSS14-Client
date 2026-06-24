// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.PrototypeFlagsTypeSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Log;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

[TypeSerializer]
public sealed class PrototypeFlagsTypeSerializer<T> : 
  ITypeSerializer<PrototypeFlags<T>, SequenceDataNode>,
  ITypeReader<PrototypeFlags<T>, SequenceDataNode>,
  ITypeValidator<PrototypeFlags<T>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<PrototypeFlags<T>, SequenceDataNode>,
  ITypeWriter<PrototypeFlags<T>>,
  BaseSerializerInterfaces.ITypeInterface<PrototypeFlags<T>>,
  ITypeSerializer<PrototypeFlags<T>, ValueDataNode>,
  ITypeReader<PrototypeFlags<T>, ValueDataNode>,
  ITypeValidator<PrototypeFlags<T>, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<PrototypeFlags<T>, ValueDataNode>,
  ITypeCopyCreator<PrototypeFlags<T>>,
  ITypeCopier<PrototypeFlags<T>>
  where T : class, IPrototype
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    List<ValidationNode> sequence = new List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(node1 is ValueDataNode node2))
        sequence.Add((ValidationNode) new ErrorNode(node1, $"Cannot cast node {node1} to ValueDataNode."));
      else
        sequence.Add(serializationManager.ValidateNode<string, ValueDataNode, PrototypeIdSerializer<T>>(node2, context));
    }
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public PrototypeFlags<T> Read(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<PrototypeFlags<T>>? instanceProvider = null)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a PrototypeFlags. Ignoring...");
    List<string> flags = new List<string>(node.Sequence.Count);
    foreach (DataNode dataNode in (IEnumerable<DataNode>) node.Sequence)
    {
      if (dataNode is ValueDataNode valueDataNode)
        flags.Add(valueDataNode.Value);
    }
    return new PrototypeFlags<T>((IEnumerable<string>) flags);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    PrototypeFlags<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new SequenceDataNode(value.ToArray<string>());
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return serializationManager.ValidateNode<string, ValueDataNode, PrototypeIdSerializer<T>>(node, context);
  }

  public PrototypeFlags<T> Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<PrototypeFlags<T>>? instanceProvider = null)
  {
    if (instanceProvider != null)
      dependencies.Resolve<ILogManager>().GetSawmill("szr").Warning("Provided value to a Read-call for a PrototypeFlags. Ignoring...");
    return new PrototypeFlags<T>(new string[1]{ node.Value });
  }

  public PrototypeFlags<T> CreateCopy(
    ISerializationManager serializationManager,
    PrototypeFlags<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new PrototypeFlags<T>((IEnumerable<string>) source);
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    PrototypeFlags<T> source,
    ref PrototypeFlags<T> target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target.Clear();
    target.UnionWith(source);
  }
}
