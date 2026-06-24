// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.ColorSerializer
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

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class ColorSerializer : 
  ITypeSerializer<Color, ValueDataNode>,
  ITypeReader<Color, ValueDataNode>,
  ITypeValidator<Color, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Color, ValueDataNode>,
  ITypeWriter<Color>,
  BaseSerializerInterfaces.ITypeInterface<Color>,
  ITypeCopyCreator<Color>
{
  public Color Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Color>? instanceProvider = null)
  {
    Color color;
    return !Color.TryFromName(node.Value, ref color) ? Color.FromHex(node.Value.AsSpan(), new Color?()) : color;
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    Color color;
    return !Color.TryFromName(node.Value, ref color) && !Color.TryFromHex(node.Value.AsSpan()).HasValue ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing Color.") : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Color value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(((Color) ref value).ToHex());
  }

  public Color CreateCopy(
    ISerializationManager serializationManager,
    Color source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Color(source.R, source.G, source.B, source.A);
  }
}
