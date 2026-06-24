// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.EntProtoIdSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class EntProtoIdSerializer : 
  ITypeSerializer<EntProtoId, ValueDataNode>,
  ITypeReader<EntProtoId, ValueDataNode>,
  ITypeValidator<EntProtoId, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<EntProtoId, ValueDataNode>,
  ITypeWriter<EntProtoId>,
  BaseSerializerInterfaces.ITypeInterface<EntProtoId>,
  ITypeCopyCreator<EntProtoId>
{
  public ValidationNode Validate(
    ISerializationManager serialization,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
    return prototypeManager.TryGetKindFrom<EntityPrototype>(out string _) && prototypeManager.HasMapping<EntityPrototype>(node.Value) ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "No EntityPrototype found with id " + node.Value);
  }

  public EntProtoId Read(
    ISerializationManager serialization,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<EntProtoId>? instanceProvider = null)
  {
    return new EntProtoId(node.Value);
  }

  public DataNode Write(
    ISerializationManager serialization,
    EntProtoId value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.Id);
  }

  public EntProtoId CreateCopy(
    ISerializationManager serializationManager,
    EntProtoId source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
