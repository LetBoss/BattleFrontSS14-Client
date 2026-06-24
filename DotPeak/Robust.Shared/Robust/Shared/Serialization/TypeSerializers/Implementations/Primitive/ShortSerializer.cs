// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.ShortSerializer
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
public sealed class ShortSerializer : 
  ITypeSerializer<short, ValueDataNode>,
  ITypeReader<short, ValueDataNode>,
  ITypeValidator<short, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<short, ValueDataNode>,
  ITypeWriter<short>,
  BaseSerializerInterfaces.ITypeInterface<short>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !Parse.TryInt16(node.Value.AsSpan(), out short _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing short value: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public short Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<short>? instanceProvider = null)
  {
    return Parse.Int16(node.Value.AsSpan());
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    short value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
