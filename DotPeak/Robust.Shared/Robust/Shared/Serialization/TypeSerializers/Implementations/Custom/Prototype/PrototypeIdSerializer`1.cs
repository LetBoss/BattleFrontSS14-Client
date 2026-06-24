// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.PrototypeIdSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

[Virtual]
public class PrototypeIdSerializer<TPrototype> : 
  ITypeValidator<string, ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<string, ValueDataNode>
  where TPrototype : class, IPrototype
{
  public virtual ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return ProtoIdSerializer<TPrototype>.Validate(dependencies, node);
  }
}
