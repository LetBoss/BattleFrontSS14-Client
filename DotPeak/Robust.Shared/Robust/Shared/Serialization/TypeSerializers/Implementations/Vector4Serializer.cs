// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Vector4Serializer
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
public sealed class Vector4Serializer : 
  ITypeSerializer<Vector4, ValueDataNode>,
  ITypeReader<Vector4, ValueDataNode>,
  ITypeValidator<Vector4, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Vector4, ValueDataNode>,
  ITypeWriter<Vector4>,
  BaseSerializerInterfaces.ITypeInterface<Vector4>,
  ITypeCopyCreator<Vector4>
{
  public Vector4 Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Vector4>? instanceProvider = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out args))
      throw new InvalidMappingException($"Could not parse {"Vector4"}: '{node.Value}'");
    double x = (double) float.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture);
    float num1 = float.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture);
    float num2 = float.Parse(args[2], (IFormatProvider) CultureInfo.InvariantCulture);
    float num3 = float.Parse(args[3], (IFormatProvider) CultureInfo.InvariantCulture);
    double y = (double) num1;
    double z = (double) num2;
    double w = (double) num3;
    return new Vector4((float) x, (float) y, (float) z, (float) w);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 4, out args))
      return (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector4.");
    float result;
    return !float.TryParse(args[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[2], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[3], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector4.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Vector4 value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Z.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.W.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public Vector4 CreateCopy(
    ISerializationManager serializationManager,
    Vector4 source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
