// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array.PrototypeIdArraySerializer`1
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;
using System;
using System.Linq;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

[Virtual]
public class PrototypeIdArraySerializer<TPrototype> : 
  ITypeValidator<string[], SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<string[], SequenceDataNode>,
  ITypeValidator<string[], ValueDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<string[], ValueDataNode>
  where TPrototype : class, IPrototype
{
  protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer
  {
    get => new PrototypeIdSerializer<TPrototype>();
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return (ValidationNode) new ValidatedSequenceNode(node.Select<DataNode, ValidationNode>((Func<DataNode, ValidationNode>) (x =>
    {
      if (x is ValueDataNode node2)
        return this.PrototypeSerializer.Validate(serializationManager, node2, dependencies, context);
      return (ValidationNode) new ErrorNode(x, $"Cannot cast node {x} to ValueDataNode.");
    })).ToList<ValidationNode>());
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    ValueDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return this.PrototypeSerializer.Validate(serializationManager, node, dependencies, context);
  }
}
