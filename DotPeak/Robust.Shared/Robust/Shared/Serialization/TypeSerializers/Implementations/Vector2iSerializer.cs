// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Vector2iSerializer
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
public sealed class Vector2iSerializer : 
  ITypeSerializer<Vector2i, ValueDataNode>,
  ITypeReader<Vector2i, ValueDataNode>,
  ITypeValidator<Vector2i, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Vector2i, ValueDataNode>,
  ITypeWriter<Vector2i>,
  BaseSerializerInterfaces.ITypeInterface<Vector2i>,
  ITypeCopyCreator<Vector2i>
{
  public Vector2i Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Vector2i>? instanceProvider = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out args))
      throw new InvalidMappingException($"Could not parse {"Vector2i"}: '{node.Value}'");
    return new Vector2i(int.Parse(args[0], (IFormatProvider) CultureInfo.InvariantCulture), int.Parse(args[1], (IFormatProvider) CultureInfo.InvariantCulture));
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    string[] args;
    if (!VectorSerializerUtility.TryParseArgs(node.Value, 2, out args))
      return (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector2i.");
    int result;
    return !int.TryParse(args[0], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) || !int.TryParse(args[1], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing values for Vector2i.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Vector2i value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode($"{value.X.ToString((IFormatProvider) CultureInfo.InvariantCulture)},{value.Y.ToString((IFormatProvider) CultureInfo.InvariantCulture)}");
  }

  public Vector2i CreateCopy(
    ISerializationManager serializationManager,
    Vector2i source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Vector2i(source.X, source.Y);
  }
}
