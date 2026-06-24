// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Serialization.GraphNodeEntitySerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Construction.NodeEntities;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

#nullable enable
namespace Content.Shared.Construction.Serialization;

public sealed class GraphNodeEntitySerializer : 
  ITypeSerializer<IGraphNodeEntity, ValueDataNode>,
  ITypeReader<IGraphNodeEntity, ValueDataNode>,
  ITypeValidator<IGraphNodeEntity, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IGraphNodeEntity, ValueDataNode>,
  ITypeWriter<IGraphNodeEntity>,
  BaseSerializerInterfaces.ITypeInterface<IGraphNodeEntity>,
  ITypeSerializer<IGraphNodeEntity, MappingDataNode>,
  ITypeReader<IGraphNodeEntity, MappingDataNode>,
  ITypeValidator<IGraphNodeEntity, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IGraphNodeEntity, MappingDataNode>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string str = node.Value;
    return !dependencies.Resolve<IPrototypeManager>().HasIndex<EntityPrototype>(str) ? (ValidationNode) new ErrorNode((DataNode) node, $"Entity Prototype {str} was not found!", true) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public IGraphNodeEntity Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<IGraphNodeEntity>? instanceProvider = null)
  {
    return (IGraphNodeEntity) new StaticNodeEntity(node.Value);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return serializationManager.ValidateNode<IGraphNodeEntity>((DataNode) node, context);
  }

  public IGraphNodeEntity Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<IGraphNodeEntity>? instanceProvider = null)
  {
    return serializationManager.Read<IGraphNodeEntity>((DataNode) node, hookCtx, context, instanceProvider, false);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    IGraphNodeEntity value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return serializationManager.WriteValue<IGraphNodeEntity>(value, alwaysWrite, context, false);
  }
}
