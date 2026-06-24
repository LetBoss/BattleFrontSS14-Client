// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Matrix3x2Serializer
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
public sealed class Matrix3x2Serializer : 
  ITypeSerializer<Matrix3x2, ValueDataNode>,
  ITypeReader<Matrix3x2, ValueDataNode>,
  ITypeValidator<Matrix3x2, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Matrix3x2, ValueDataNode>,
  ITypeWriter<Matrix3x2>,
  BaseSerializerInterfaces.ITypeInterface<Matrix3x2>,
  ITypeCopyCreator<Matrix3x2>
{
  public Matrix3x2 Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Matrix3x2>? instanceProvider = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 6, out args))
      throw new InvalidMappingException($"Could not parse {"Matrix3x2"}: '{node.Value}'");
    float[] numArray = new float[6];
    for (int index = 0; index < 6; ++index)
      numArray[index] = float.Parse(args[index], (IFormatProvider) CultureInfo.InvariantCulture);
    return new Matrix3x2(numArray[0], numArray[1], numArray[2], numArray[3], numArray[4], numArray[5]);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 6, out args))
      throw new InvalidMappingException($"Could not parse {"Matrix3x2"}: '{node.Value}'");
    float result;
    return !float.TryParse(args[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[2], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[3], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[4], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(args[5], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Matrix3x2.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Matrix3x2 value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.M11.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.M12.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.M21.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.M22.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.M31.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.M32.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public Matrix3x2 CreateCopy(
    ISerializationManager serializationManager,
    Matrix3x2 source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Matrix3x2(source.M11, source.M12, source.M21, source.M22, source.M31, source.M32);
  }
}
