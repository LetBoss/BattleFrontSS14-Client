// Decompiled with JetBrains decompiler
// Type: Content.Shared.Construction.Steps.ConstructionGraphStepTypeSerializer
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
namespace Content.Shared.Construction.Steps;

[TypeSerializer]
public sealed class ConstructionGraphStepTypeSerializer : 
  ITypeReader<ConstructionGraphStep, MappingDataNode>,
  ITypeValidator<ConstructionGraphStep, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ConstructionGraphStep, MappingDataNode>
{
  private Type? GetType(MappingDataNode node)
  {
    if (node.Has("material"))
      return typeof (MaterialConstructionGraphStep);
    if (node.Has("tool"))
      return typeof (ToolConstructionGraphStep);
    if (node.Has("component"))
      return typeof (ComponentConstructionGraphStep);
    if (node.Has("tag"))
      return typeof (TagConstructionGraphStep);
    if (node.Has("allTags") || node.Has("anyTags"))
      return typeof (MultipleTagsConstructionGraphStep);
    if (node.Has("minTemperature") || node.Has("maxTemperature"))
      return typeof (TemperatureConstructionGraphStep);
    return node.Has("assemblyId") || node.Has("guideString") ? typeof (PartAssemblyConstructionGraphStep) : (Type) null;
  }

  public ConstructionGraphStep Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<ConstructionGraphStep>? instanceProvider = null)
  {
    Type type = this.GetType(node) ?? throw new ArgumentException("Tried to convert invalid YAML node mapping to ConstructionGraphStep!");
    return (ConstructionGraphStep) serializationManager.Read(type, (DataNode) node, hookCtx, context, false);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    Type type = this.GetType(node);
    return type == (Type) null ? (ValidationNode) new ErrorNode((DataNode) node, "No construction graph step type found.", true) : serializationManager.ValidateNode(type, (DataNode) node, context);
  }
}
