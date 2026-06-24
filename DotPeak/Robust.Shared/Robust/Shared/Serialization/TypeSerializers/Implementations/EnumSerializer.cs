// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.EnumSerializer
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

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class EnumSerializer : 
  ITypeSerializer<Enum, ValueDataNode>,
  ITypeReader<Enum, ValueDataNode>,
  ITypeValidator<Enum, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Enum, ValueDataNode>,
  ITypeWriter<Enum>,
  BaseSerializerInterfaces.ITypeInterface<Enum>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return serializationManager.ReflectionManager.TryParseEnumReference(node.Value, out Enum _, false) ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, "Failed to parse enum " + node.Value);
  }

  public Enum Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Enum>? instanceProvider = null)
  {
    Enum @enum;
    if (serializationManager.ReflectionManager.TryParseEnumReference(node.Value, out @enum))
      return @enum;
    throw new ArgumentException("Failed to parse enum " + node.Value);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Enum value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(serializationManager.ReflectionManager.GetEnumReference(value));
  }
}
