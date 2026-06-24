// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.SByteSerializer
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
public sealed class SByteSerializer : 
  ITypeSerializer<sbyte, ValueDataNode>,
  ITypeReader<sbyte, ValueDataNode>,
  ITypeValidator<sbyte, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<sbyte, ValueDataNode>,
  ITypeWriter<sbyte>,
  BaseSerializerInterfaces.ITypeInterface<sbyte>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !sbyte.TryParse(node.Value, out sbyte _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing signed byte value: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public sbyte Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<sbyte>? instanceProvider = null)
  {
    return sbyte.Parse(node.Value, (IFormatProvider) CultureInfo.InvariantCulture);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    sbyte value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }
}
