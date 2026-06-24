// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.LongSerializer
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
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
public sealed class LongSerializer : 
  ITypeSerializer<long, ValueDataNode>,
  ITypeReader<long, ValueDataNode>,
  ITypeValidator<long, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<long, ValueDataNode>,
  ITypeWriter<long>,
  BaseSerializerInterfaces.ITypeInterface<long>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !Parse.TryInt64(node.Value.AsSpan(), out long _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing long value: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public long Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<long>? instanceProvider = null)
  {
    return Parse.Int64(node.Value.AsSpan());
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    long value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  public long Copy(
    ISerializationManager serializationManager,
    long source,
    long target,
    bool skipHook,
    ISerializationContext? context = null)
  {
    return source;
  }
}
