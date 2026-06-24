// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.AbstractPrototypeIdSerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

public sealed class AbstractPrototypeIdSerializer<TPrototype> : PrototypeIdSerializer<TPrototype> where TPrototype : class, IPrototype, IInheritingPrototype
{
  public override ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
    if (prototypeManager.TryGetKindFrom<TPrototype>(out string _) && prototypeManager.HasMapping<TPrototype>(node.Value))
      return (ValidationNode) new ValidatedValueNode((DataNode) node);
    return (ValidationNode) new ErrorNode((DataNode) node, $"PrototypeID {node.Value} for type {typeof (TPrototype)} not found");
  }
}
