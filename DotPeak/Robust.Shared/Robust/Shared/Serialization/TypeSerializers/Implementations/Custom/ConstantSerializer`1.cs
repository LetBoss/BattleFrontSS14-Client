// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.ConstantSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class ConstantSerializer<TTag> : 
  ITypeSerializer<int, ValueDataNode>,
  ITypeReader<int, ValueDataNode>,
  ITypeValidator<int, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<int, ValueDataNode>,
  ITypeWriter<int>,
  BaseSerializerInterfaces.ITypeInterface<int>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return !Enum.TryParse(serializationManager.GetConstantTypeFromTag(typeof (TTag)), node.Value, out object _) ? (ValidationNode) new ErrorNode((DataNode) node, "Failed parsing constant.", false) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public int Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<int>? instanceProvider = null)
  {
    return (int) Enum.Parse(serializationManager.GetConstantTypeFromTag(typeof (TTag)), node.Value);
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    int value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    Type constantTypeFromTag = serializationManager.GetConstantTypeFromTag(typeof (TTag));
    return (DataNode) new ValueDataNode(Enum.GetName(constantTypeFromTag, (object) value) ?? throw new InvalidOperationException($"No constant corresponding to value {value} in {constantTypeFromTag}."));
  }
}
