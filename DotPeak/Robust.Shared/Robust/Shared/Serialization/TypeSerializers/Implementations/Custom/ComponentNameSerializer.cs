// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.ComponentNameSerializer
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

public sealed class ComponentNameSerializer : 
  ITypeSerializer<string, ValueDataNode>,
  ITypeReader<string, ValueDataNode>,
  ITypeValidator<string, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<string, ValueDataNode>,
  ITypeWriter<string>,
  BaseSerializerInterfaces.ITypeInterface<string>
{
  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IComponentFactory componentFactory = dependencies.Resolve<IComponentFactory>();
    return !componentFactory.TryGetRegistration(node.Value, out ComponentRegistration _) && componentFactory.GetComponentAvailability(node.Value) != ComponentAvailability.Ignore ? (ValidationNode) new ErrorNode((DataNode) node, "Unknown component kind: " + node.Value) : (ValidationNode) new ValidatedValueNode((DataNode) node);
  }

  public string Read(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null,
    ISerializationManager.InstantiationDelegate<string>? instanceProvider = null)
  {
    return node.Value;
  }

  public DataNode Write(
    ISerializationManager serializationManager,
    string value,
    IDependencyCollection dependencies,
    bool alwaysWrite = false,
    ISerializationContext? context = null)
  {
    return (DataNode) new ValueDataNode(value);
  }
}
