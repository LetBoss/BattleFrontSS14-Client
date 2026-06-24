// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.EntProtoIdSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class EntProtoIdSerializer<T> : 
  ITypeSerializer<EntProtoId<T>, ValueDataNode>,
  ITypeReader<EntProtoId<T>, ValueDataNode>,
  ITypeValidator<EntProtoId<T>, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<EntProtoId<T>, ValueDataNode>,
  ITypeWriter<EntProtoId<T>>,
  BaseSerializerInterfaces.ITypeInterface<EntProtoId<T>>,
  ITypeCopyCreator<EntProtoId<T>>
  where T : IComponent, new()
{
  public ValidationNode Validate(
    ISerializationManager serialization,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
    MappingDataNode mappings;
    if (!prototypeManager.TryGetKindFrom<EntityPrototype>(out string _) || !prototypeManager.TryGetMapping(typeof (EntityPrototype), node.Value, out mappings))
      return (ValidationNode) new ErrorNode((DataNode) node, $"No {"EntityPrototype"} found with id {node.Value} that has a {typeof (T).Name}");
    SequenceDataNode node1;
    if (!mappings.TryGet<SequenceDataNode>("components", out node1))
      return (ValidationNode) new ErrorNode((DataNode) node, $"{"EntityPrototype"} {node.Value} doesn't have a {typeof (T).Name}.");
    ComponentRegistration registration = dependencies.Resolve<IComponentFactory>().GetRegistration<T>();
    foreach (DataNode dataNode in node1)
    {
      ValueDataNode node2;
      if (dataNode is MappingDataNode mappingDataNode && mappingDataNode.TryGet<ValueDataNode>("type", out node2) && node2.Value == registration.Name)
        return (ValidationNode) new ValidatedValueNode((DataNode) node);
    }
    return (ValidationNode) new ErrorNode((DataNode) node, $"{"EntityPrototype"} {node.Value} doesn't have a {typeof (T).Name}.");
  }

  public EntProtoId<T> Read(
    ISerializationManager serialization,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<EntProtoId<T>>? instanceProvider = null)
  {
    return new EntProtoId<T>(node.Value);
  }

  public DataNode Write(
    ISerializationManager serialization,
    EntProtoId<T> value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.Id);
  }

  public EntProtoId<T> CreateCopy(
    ISerializationManager serializationManager,
    EntProtoId<T> source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
