// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.TimespanSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using Robust.Shared.Utility;
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class TimespanSerializer : 
  ITypeSerializer<TimeSpan, ValueDataNode>,
  ITypeReader<TimeSpan, ValueDataNode>,
  ITypeValidator<TimeSpan, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<TimeSpan, ValueDataNode>,
  ITypeWriter<TimeSpan>,
  BaseSerializerInterfaces.ITypeInterface<TimeSpan>,
  ITypeCopyCreator<TimeSpan>
{
  public TimeSpan Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<TimeSpan>? instanceProvider = null)
  {
    TimeSpan timeSpan;
    if (TimeSpanExt.TryTimeSpan(node, out timeSpan))
      return timeSpan;
    throw new FormatException($"The input string '{node.Value}' can't be converted to TimeSpan");
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !TimeSpanExt.TryTimeSpan(node, out TimeSpan _) && !double.TryParse(node.Value, NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out double _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing TimeSpan") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    TimeSpan value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.TotalSeconds.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  public TimeSpan CreateCopy(
    ISerializationManager serializationManager,
    TimeSpan source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
