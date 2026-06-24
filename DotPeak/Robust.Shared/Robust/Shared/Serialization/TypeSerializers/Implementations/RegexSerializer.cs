// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.RegexSerializer
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
using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class RegexSerializer : 
  ITypeSerializer<Regex, ValueDataNode>,
  ITypeReader<Regex, ValueDataNode>,
  ITypeValidator<Regex, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Regex, ValueDataNode>,
  ITypeWriter<Regex>,
  BaseSerializerInterfaces.ITypeInterface<Regex>,
  ITypeCopyCreator<Regex>
{
  public Regex Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Regex>? instanceProvider = null)
  {
    return new Regex(node.Value, RegexOptions.Compiled);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    try
    {
      Regex regex = new Regex(node.Value);
    }
    catch (Exception ex)
    {
      return (ValidationNode) new ErrorNode((DataNode) node, "Failed compiling regex.");
    }
    return (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Regex value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.ToString());
  }

  public Regex CreateCopy(
    ISerializationManager serializationManager,
    Regex source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return new Regex(source.ToString(), source.Options, source.MatchTimeout);
  }
}
