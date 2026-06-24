// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.TypeSerializer
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
using System.Collections.Generic;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations;

[TypeSerializer]
public sealed class TypeSerializer : 
  ITypeSerializer<Type, ValueDataNode>,
  ITypeReader<Type, ValueDataNode>,
  ITypeValidator<Type, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<Type, ValueDataNode>,
  ITypeWriter<Type>,
  BaseSerializerInterfaces.ITypeInterface<Type>,
  ITypeCopyCreator<Type>
{
  private static readonly Dictionary<string, Type> Shortcuts = new Dictionary<string, Type>()
  {
    {
      "bool",
      typeof (bool)
    }
  };

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    if (TypeSerializer.Shortcuts.ContainsKey(node.Value))
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return !(dependencies.Resolve<IReflectionManager>().GetType(node.Value) == (Type) null) ? (ValidationNode) new ValidatedValueNode((DataNode) node) : (ValidationNode) new ErrorNode((DataNode) node, $"Type '{node.Value}' not found.");
  }

  public Type Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<Type>? instanceProvider = null)
  {
    Type type1;
    if (TypeSerializer.Shortcuts.TryGetValue(node.Value, out type1))
      return type1;
    Type type2 = dependencies.Resolve<IReflectionManager>().GetType(node.Value);
    return !(type2 == (Type) null) ? type2 : throw new InvalidMappingException($"Type '{node.Value}' not found.");
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    Type value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value.FullName ?? value.Name);
  }

  public Type CreateCopy(
    ISerializationManager serializationManager,
    Type source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
