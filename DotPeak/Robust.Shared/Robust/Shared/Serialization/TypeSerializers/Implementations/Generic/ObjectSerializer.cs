// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Generic.ObjectSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;

[TypeSerializer]
public sealed class ObjectSerializer : 
  ITypeSerializer<object, ValueDataNode>,
  ITypeReader<object, ValueDataNode>,
  ITypeValidator<object, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<object, ValueDataNode>,
  ITypeWriter<object>,
  BaseSerializerInterfaces.ITypeInterface<object>,
  ITypeCopier<object>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IReflectionManager reflectionManager = dependencies.Resolve<IReflectionManager>();
    if (node.Tag != null)
    {
      string tag = node.Tag;
      string name = tag.Substring(6, tag.Length - 6);
      Type type;
      return !reflectionManager.TryLooseGetType(name, out type) ? (ValidationNode) new ErrorNode((DataNode) node, "Unable to find type for " + name) : serializationManager.ValidateNode(type, (DataNode) node, context);
    }
    return (ValidationNode) new ErrorNode((DataNode) node, $"Unable to find type for {node}");
  }

  public object Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<object>? instanceProvider = null)
  {
    IReflectionManager reflectionManager = dependencies.Resolve<IReflectionManager>();
    object obj = instanceProvider != null ? instanceProvider() : new object();
    if (node.Tag != null)
    {
      string tag = node.Tag;
      string name = tag.Substring(6, tag.Length - 6);
      Type type;
      if (!reflectionManager.TryLooseGetType(name, out type))
        throw new NullReferenceException("Found null type for " + name);
      obj = serializationManager.Read(type, (DataNode) node, hookCtx, context);
      if (obj == null)
        throw new NullReferenceException($"Found null data for {node}, expected {type}");
    }
    return obj;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    object value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return serializationManager.WriteValue(value.GetType(), value) ?? throw new NullReferenceException($"Attempted to write node with type {value.GetType()}, node returned null");
  }

  public void CopyTo(
    ISerializationManager serializationManager,
    object source,
    ref object target,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    target = source;
  }
}
