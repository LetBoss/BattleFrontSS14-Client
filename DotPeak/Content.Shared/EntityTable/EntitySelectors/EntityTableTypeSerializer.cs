// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityTable.EntitySelectors.EntityTableTypeSerializer
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;

#nullable enable
namespace Content.Shared.EntityTable.EntitySelectors;

[TypeSerializer]
public sealed class EntityTableTypeSerializer : 
  ITypeReader<EntityTableSelector, MappingDataNode>,
  ITypeValidator<EntityTableSelector, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<EntityTableSelector, MappingDataNode>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return node.Has("id") ? serializationManager.ValidateNode<EntSelector>((DataNode) node, context) : (ValidationNode) new ErrorNode((DataNode) node, "Custom validation not supported! Please specify the type manually!");
  }

  public EntityTableSelector Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<EntityTableSelector>? instanceProvider = null)
  {
    Type type = typeof (EntityTableSelector);
    if (node.Has("id"))
      type = typeof (EntSelector);
    return (EntityTableSelector) serializationManager.Read(type, (DataNode) node, context);
  }
}
