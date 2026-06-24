// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Vector2Serializer
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
using System.Numerics;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class Vector2Serializer : 
  ITypeSerializer<Vector2, ValueDataNode>,
  ITypeReader<Vector2, ValueDataNode>,
  ITypeValidator<Vector2, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Vector2, ValueDataNode>,
  ITypeWriter<Vector2>,
  BaseSerializerInterfaces.ITypeInterface<Vector2>,
  ITypeCopyCreator<Vector2>
{
  public Vector2 Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Vector2>? instanceProvider = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out args))
      throw new InvalidMappingException($"Could not parse {"Vector2"}: '{node.Value}'");
    return new Vector2(float.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture), float.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture));
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out args))
      return (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector2.");
    float result;
    return !float.TryParse(args[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector2.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Vector2 value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public Vector2 CreateCopy(
    ISerializationManager serializationManager,
    Vector2 source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Vector2(source.X, source.Y);
  }
}
