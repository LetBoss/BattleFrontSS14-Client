// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.AngleSerializer
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
public sealed class AngleSerializer : 
  ITypeSerializer<Angle, ValueDataNode>,
  ITypeReader<Angle, ValueDataNode>,
  ITypeValidator<Angle, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Angle, ValueDataNode>,
  ITypeWriter<Angle>,
  BaseSerializerInterfaces.ITypeInterface<Angle>,
  ITypeCopyCreator<Angle>
{
  public Angle Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Angle>? instanceProvider = null)
  {
    string s = node.Value;
    return !s.EndsWith("rad") ? Angle.FromDegrees(double.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture)) : new Angle(double.Parse(s.Substring(0, s.Length - 3), (IFormatProvider) CultureInfo.InvariantCulture));
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string str = node.Value;
    return !double.TryParse(str.EndsWith("rad") ? str.Substring(0, str.Length - 3) : str, (IFormatProvider) CultureInfo.InvariantCulture, out double _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing angle.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Angle value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.Theta.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " rad");
  }

  public Angle CreateCopy(
    ISerializationManager serializationManager,
    Angle source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Angle(Angle.op_Implicit(source));
  }
}
