// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.CharSerializer
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
public sealed class CharSerializer : 
  ITypeSerializer<char, ValueDataNode>,
  ITypeReader<char, ValueDataNode>,
  ITypeValidator<char, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<char, ValueDataNode>,
  ITypeWriter<char>,
  BaseSerializerInterfaces.ITypeInterface<char>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !char.TryParse(node.Value, out char _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing char value: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public char Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<char>? instanceProvider = null)
  {
    return char.Parse(node.Value);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    char value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
  }

  public char Copy(
    ISerializationManager serializationManager,
    char source,
    char target,
    bool skipHook,
    ISerializationContext? context = null)
  {
    return source;
  }
}
