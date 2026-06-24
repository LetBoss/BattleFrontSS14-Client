// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.TimeOffsetSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.EntitySerialization;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Timing;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class TimeOffsetSerializer : 
  ITypeSerializer<TimeSpan, ValueDataNode>,
  ITypeReader<TimeSpan, ValueDataNode>,
  ITypeValidator<TimeSpan, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<TimeSpan, ValueDataNode>,
  ITypeWriter<TimeSpan>,
  BaseSerializerInterfaces.ITypeInterface<TimeSpan>
{
  public TimeSpan Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<TimeSpan>? instanceProvider = null)
  {
    if (context != null && context.WritingReadingPrototypes || !(context is EntityDeserializer entityDeserializer))
      return TimeSpan.Zero;
    EntityDeserializer.EntData? currentReadingEntity = entityDeserializer.CurrentReadingEntity;
    if (!currentReadingEntity.HasValue || !currentReadingEntity.GetValueOrDefault().PostInit)
      return TimeSpan.Zero;
    IGameTiming timing = entityDeserializer.Timing;
    TimeSpan timeSpan = TimeSpan.FromSeconds(double.Parse(node.Value, (IFormatProvider) CultureInfo.InvariantCulture));
    return timeSpan > TimeSpan.MaxValue - timing.CurTime ? TimeSpan.MaxValue : timeSpan + timing.CurTime;
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !double.TryParse(node.Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out double _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing TimeSpan") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    TimeSpan value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    if (context is EntitySerializer entitySerializer && !entitySerializer.WritingReadingPrototypes)
    {
      EntityManager entMan = entitySerializer.EntMan;
      Entity<MetaDataComponent>? currentEntity = entitySerializer.CurrentEntity;
      EntityUid? uid = currentEntity.HasValue ? new EntityUid?((EntityUid) currentEntity.GetValueOrDefault()) : new EntityUid?();
      MetaDataComponent metaDataComponent;
      ref MetaDataComponent local = ref metaDataComponent;
      if (entMan.TryGetComponent<MetaDataComponent>(uid, out local) && metaDataComponent.EntityLifeStage >= EntityLifeStage.MapInitialized)
      {
        if (metaDataComponent.PauseTime.HasValue)
          value -= metaDataComponent.PauseTime.Value;
        else
          value -= entitySerializer.Timing.CurTime;
        return (DataNode) new ValueDataNode(value.TotalSeconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
    }
    return (DataNode) new ValueDataNode("0");
  }
}
