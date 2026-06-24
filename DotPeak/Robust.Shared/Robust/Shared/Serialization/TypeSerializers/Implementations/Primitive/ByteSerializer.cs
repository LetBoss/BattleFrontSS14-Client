// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.ByteSerializer
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
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
public sealed class ByteSerializer : 
  ITypeSerializer<byte, ValueDataNode>,
  ITypeReader<byte, ValueDataNode>,
  ITypeValidator<byte, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<byte, ValueDataNode>,
  ITypeWriter<byte>,
  BaseSerializerInterfaces.ITypeInterface<byte>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !byte.TryParse(node.Value, out byte _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing byte value: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public byte Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<byte>? instanceProvider = null)
  {
    return byte.Parse(node.Value, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    byte value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
