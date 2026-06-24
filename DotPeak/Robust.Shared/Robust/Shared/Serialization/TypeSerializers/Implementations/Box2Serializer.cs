// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Box2Serializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Maths;
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
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class Box2Serializer : 
  ITypeSerializer<Box2, ValueDataNode>,
  ITypeReader<Box2, ValueDataNode>,
  ITypeValidator<Box2, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Box2, ValueDataNode>,
  ITypeWriter<Box2>,
  BaseSerializerInterfaces.ITypeInterface<Box2>,
  ITypeCopyCreator<Box2>,
  ITypeSerializer<Box2i, ValueDataNode>,
  ITypeReader<Box2i, ValueDataNode>,
  ITypeValidator<Box2i, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Box2i, ValueDataNode>,
  ITypeWriter<Box2i>,
  BaseSerializerInterfaces.ITypeInterface<Box2i>,
  ITypeCopyCreator<Box2i>
{
  public Box2 Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Box2>? instanceProvider = null)
  {
    ReadOnlySpan<char> source = node.Value.AsSpan();
    ReadOnlySpan<char> splitValue;
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    double num1 = (double) Parse.Float(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    float num2 = Parse.Float(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    float num3 = Parse.Float(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    float num4 = Parse.Float(splitValue);
    double num5 = (double) num2;
    double num6 = (double) num3;
    double num7 = (double) num4;
    return new Box2((float) num1, (float) num5, (float) num6, (float) num7);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Box2 value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.Left.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Bottom.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Right.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Top.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  ValidationNode ITypeValidator<Box2, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    string[] strArray = node.Value.Split(',');
    if (strArray.Length != 4)
      return (ValidationNode) new ErrorNode((DataNode) node, "Invalid amount of args for Box2.");
    float result;
    return !Parse.TryFloat(strArray[0].AsSpan(), out result) || !Parse.TryFloat(strArray[1].AsSpan(), out result) || !Parse.TryFloat(strArray[2].AsSpan(), out result) || !Parse.TryFloat(strArray[3].AsSpan(), out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values of Box2.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public Box2 CreateCopy(
    ISerializationManager serializationManager,
    Box2 source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Box2(source.Left, source.Bottom, source.Right, source.Top);
  }

  public Box2i Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Box2i>? instanceProvider = null)
  {
    ReadOnlySpan<char> source = node.Value.AsSpan();
    ReadOnlySpan<char> splitValue;
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    int num1 = Parse.Int32(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    int num2 = Parse.Int32(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    int num3 = Parse.Int32(splitValue);
    Box2Serializer.NextOrThrow(ref source, out splitValue, node.Value);
    int num4 = Parse.Int32(splitValue);
    int num5 = num2;
    int num6 = num3;
    int num7 = num4;
    return new Box2i(num1, num5, num6, num7);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Box2i value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.Left.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Bottom.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Right.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Top.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  ValidationNode ITypeValidator<Box2i, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    string[] strArray = node.Value.Split(',');
    if (strArray.Length != 4)
      return (ValidationNode) new ErrorNode((DataNode) node, "Invalid amount of args for Box2i.");
    int result;
    return !Parse.TryInt32(strArray[0].AsSpan(), out result) || !Parse.TryInt32(strArray[1].AsSpan(), out result) || !Parse.TryInt32(strArray[2].AsSpan(), out result) || !Parse.TryInt32(strArray[3].AsSpan(), out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values of Box2i.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public Box2i CreateCopy(
    ISerializationManager serializationManager,
    Box2i source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Box2i(source.Left, source.Bottom, source.Right, source.Top);
  }

  private static void NextOrThrow(
    ref ReadOnlySpan<char> source,
    out ReadOnlySpan<char> splitValue,
    string errValue)
  {
    if (!SpanSplitExtensions.SplitFindNext<char>(ref source, ',', out splitValue))
      throw new InvalidMappingException($"Could not parse {"Box2"}: '{errValue}'");
  }
}
