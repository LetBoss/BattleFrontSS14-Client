// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.DateTimeSerializer
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
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class DateTimeSerializer : 
  ITypeSerializer<DateTime, ValueDataNode>,
  ITypeReader<DateTime, ValueDataNode>,
  ITypeValidator<DateTime, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<DateTime, ValueDataNode>,
  ITypeWriter<DateTime>,
  BaseSerializerInterfaces.ITypeInterface<DateTime>,
  ITypeCopyCreator<DateTime>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !DateTime.TryParse(node.Value, (IFormatProvider) null, DateTimeStyles.RoundtripKind, out DateTime _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing DateTime") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DateTime Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<DateTime>? instanceProvider = null)
  {
    return DateTime.Parse(node.Value, (IFormatProvider) null, DateTimeStyles.RoundtripKind);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    DateTime value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString("o"));
  }

  public DateTime CreateCopy(
    ISerializationManager serializationManager,
    DateTime source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
