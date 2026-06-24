// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Audio.SoundSpecifierTypeSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Mapping;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Audio;

[TypeSerializer]
public sealed class SoundSpecifierTypeSerializer : 
  ITypeReader<SoundSpecifier, MappingDataNode>,
  ITypeValidator<SoundSpecifier, MappingDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SoundSpecifier, MappingDataNode>,
  ITypeReader<SoundSpecifier, ValueDataNode>,
  ITypeValidator<SoundSpecifier, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<SoundSpecifier, ValueDataNode>
{
  private Type GetType(MappingDataNode node)
  {
    bool flag1 = node.Has("path");
    bool flag2 = node.Has("collection");
    if (flag1 || !(flag1 ^ flag2))
      return typeof (SoundPathSpecifier);
    return flag2 ? typeof (SoundCollectionSpecifier) : typeof (SoundPathSpecifier);
  }

  public SoundSpecifier Read(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<SoundSpecifier>? instanceProvider = null)
  {
    Type type = this.GetType(node);
    return (SoundSpecifier) serializationManager.Read(type, (DataNode) node, hookCtx, context);
  }

  public SoundSpecifier Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<SoundSpecifier>? instanceProvider = null)
  {
    return (SoundSpecifier) new SoundPathSpecifier(node.Value);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    MappingDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (node.Has("path") && node.Has("collection"))
      return (ValidationNode) new ErrorNode((DataNode) node, "You can only specify either a sound path or a sound collection!");
    return !node.Has("path") && !node.Has("collection") ? (ValidationNode) new ErrorNode((DataNode) node, "You need to specify either a sound path or a sound collection!") : serializationManager.ValidateNode(this.GetType(node), (DataNode) node, context);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return (object) (serializationManager.ValidateNode<ResPath>((DataNode) node, context) as ErrorNode) == null ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "SoundSpecifier value is not a valid resource path!");
  }
}
