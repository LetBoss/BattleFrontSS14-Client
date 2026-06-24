// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive.UnsafeFloatSerializer
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

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Primitive;

[TypeSerializer]
internal sealed class UnsafeFloatSerializer : 
  ITypeSerializer<UnsafeFloat, ValueDataNode>,
  ITypeReader<UnsafeFloat, ValueDataNode>,
  ITypeValidator<UnsafeFloat, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<UnsafeFloat, ValueDataNode>,
  ITypeWriter<UnsafeFloat>,
  BaseSerializerInterfaces.ITypeInterface<UnsafeFloat>,
  ITypeCopyCreator<UnsafeFloat>,
  ITypeSerializer<UnsafeDouble, ValueDataNode>,
  ITypeReader<UnsafeDouble, ValueDataNode>,
  ITypeValidator<UnsafeDouble, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<UnsafeDouble, ValueDataNode>,
  ITypeWriter<UnsafeDouble>,
  BaseSerializerInterfaces.ITypeInterface<UnsafeDouble>,
  ITypeCopyCreator<UnsafeDouble>
{
  ValidationNode ITypeValidator<UnsafeFloat, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return serializationManager.ValidateNode<float>((DataNode) node, context);
  }

  public UnsafeFloat Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<UnsafeFloat>? instanceProvider = null)
  {
    return UnsafeFloat.op_Implicit(serializationManager.Read<float>((DataNode) node, hookCtx, context));
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    UnsafeFloat value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return serializationManager.WriteValue<float>(((UnsafeFloat) ref value).Value, alwaysWrite, context);
  }

  ValidationNode ITypeValidator<UnsafeDouble, ValueDataNode>.Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return serializationManager.ValidateNode<double>((DataNode) node, context);
  }

  public UnsafeDouble Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<UnsafeDouble>? instanceProvider = null)
  {
    return UnsafeDouble.op_Implicit(serializationManager.Read<double>((DataNode) node, hookCtx, context));
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    UnsafeDouble value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return serializationManager.WriteValue<double>(((UnsafeDouble) ref value).Value, alwaysWrite, context);
  }

  public UnsafeFloat CreateCopy(
    ISerializationManager serializationManager,
    UnsafeFloat source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }

  public UnsafeDouble CreateCopy(
    ISerializationManager serializationManager,
    UnsafeDouble source,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    return source;
  }
}
