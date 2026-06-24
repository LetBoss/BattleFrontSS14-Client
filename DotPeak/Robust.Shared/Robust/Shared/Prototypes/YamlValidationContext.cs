// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Prototypes.YamlValidationContext
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Prototypes;

internal sealed class YamlValidationContext : 
  ISerializationContext,
  ITypeSerializer<EntityUid, ValueDataNode>,
  ITypeReader<EntityUid, ValueDataNode>,
  ITypeValidator<EntityUid, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<EntityUid, ValueDataNode>,
  ITypeWriter<EntityUid>,
  BaseSerializerInterfaces.ITypeInterface<EntityUid>,
  ITypeSerializer<NetEntity, ValueDataNode>,
  ITypeReader<NetEntity, ValueDataNode>,
  ITypeValidator<NetEntity, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<NetEntity, ValueDataNode>,
  ITypeWriter<NetEntity>,
  BaseSerializerInterfaces.ITypeInterface<NetEntity>,
  ITypeSerializer<MapId, ValueDataNode>,
  ITypeReader<MapId, ValueDataNode>,
  ITypeValidator<MapId, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<MapId, ValueDataNode>,
  ITypeWriter<MapId>,
  BaseSerializerInterfaces.ITypeInterface<MapId>
{
  public SerializationManager.SerializerProvider SerializerProvider { get; } = new SerializationManager.SerializerProvider();

  public bool WritingReadingPrototypes => true;

  public YamlValidationContext() => this.SerializerProvider.RegisterSerializer((object) this);

  ValidationNode ITypeValidator<EntityUid, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return node.Value == "invalid" ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Prototypes should not contain EntityUids");
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    EntityUid value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return !value.Valid ? (DataNode) new ValueDataNode("invalid") : (DataNode) new ValueDataNode(value.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  EntityUid ITypeReader<EntityUid, ValueDataNode>.Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context,
    ISerializationManager.InstantiationDelegate<EntityUid>? _)
  {
    return node.Value == "invalid" ? EntityUid.Invalid : EntityUid.Parse(node.Value.AsSpan());
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return node.Value == "invalid" ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Prototypes should not contain NetEntities");
  }

  public NetEntity Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<NetEntity>? instanceProvider = null)
  {
    return node.Value == "invalid" ? NetEntity.Invalid : NetEntity.Parse(node.Value.AsSpan());
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    NetEntity value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return !value.Valid ? (DataNode) new ValueDataNode("invalid") : (DataNode) new ValueDataNode(value.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  ValidationNode ITypeValidator<MapId, ValueDataNode>.Validate(
    ISerializationManager seri,
    ValueDataNode node,
    IDependencyCollection deps,
    ISerializationContext? context)
  {
    return node.Value == "invalid" ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Prototypes should not contain map ids");
  }

  MapId ITypeReader<MapId, ValueDataNode>.Read(
    ISerializationManager seri,
    ValueDataNode node,
    IDependencyCollection deps,
    SerializationHookContext hookCtx,
    ISerializationContext? ctx,
    ISerializationManager.InstantiationDelegate<MapId>? instanceProvider)
  {
    return !(node.Value == "invalid") ? new MapId(int.Parse(node.Value)) : MapId.Nullspace;
  }

  DataNode ITypeWriter<MapId>.Write(
    ISerializationManager seri,
    MapId value,
    IDependencyCollection deps,
    bool alwaysWrite,
    ISerializationContext? ctx)
  {
    return value == MapId.Nullspace ? (DataNode) new ValueDataNode("invalid") : (DataNode) new ValueDataNode(value.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
