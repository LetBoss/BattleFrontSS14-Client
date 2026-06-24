// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.UIBox2Serializer
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
using System;
using System.Globalization;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class UIBox2Serializer : 
  ITypeSerializer<UIBox2, ValueDataNode>,
  ITypeReader<UIBox2, ValueDataNode>,
  ITypeValidator<UIBox2, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<UIBox2, ValueDataNode>,
  ITypeWriter<UIBox2>,
  BaseSerializerInterfaces.ITypeInterface<UIBox2>,
  ITypeCopyCreator<UIBox2>
{
  public UIBox2 Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<UIBox2>? instanceProvider = null)
  {
    string[] strArray = node.Value.Split(',');
    float num1 = strArray.Length == 4 ? float.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture) : throw new InvalidMappingException($"Could not parse {"UIBox2"}: '{node.Value}'");
    float num2 = float.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture);
    float num3 = float.Parse(strArray[2], (IFormatProvider) CultureInfo.InvariantCulture);
    float num4 = float.Parse(strArray[3], (IFormatProvider) CultureInfo.InvariantCulture);
    return new UIBox2(num2, num1, num4, num3);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string[] strArray = node.Value.Split(',');
    if (strArray.Length != 4)
      return (ValidationNode) new ErrorNode((DataNode) node, "Invalid amount of arguments for UIBox2.");
    float result;
    return !float.TryParse(strArray[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(strArray[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(strArray[2], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !float.TryParse(strArray[3], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for UIBox2.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    UIBox2 value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.Top.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Left.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Bottom.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Right.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public UIBox2 CreateCopy(
    ISerializationManager serializationManager,
    UIBox2 source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new UIBox2(source.Left, source.Top, source.Right, source.Bottom);
  }
}
