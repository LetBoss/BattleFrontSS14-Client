// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List.PrototypeIdListSerializer`1
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
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

[Virtual]
public class PrototypeIdListSerializer<T> : 
  ITypeValidator<System.Collections.Generic.List<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<System.Collections.Generic.List<string>, SequenceDataNode>,
  ITypeValidator<ImmutableList<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ImmutableList<string>, SequenceDataNode>,
  ITypeValidator<IReadOnlyCollection<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyCollection<string>, SequenceDataNode>,
  ITypeValidator<IReadOnlyList<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyList<string>, SequenceDataNode>
  where T : class, IPrototype
{
  protected virtual PrototypeIdSerializer<T> PrototypeSerializer => new PrototypeIdSerializer<T>();

  private ValidationNode ValidateInternal(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    System.Collections.Generic.List<ValidationNode> sequence = new System.Collections.Generic.List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(node1 is ValueDataNode node2))
        sequence.Add((ValidationNode) new ErrorNode(node1, $"Cannot cast node {node1} to ValueDataNode."));
      else
        sequence.Add(this.PrototypeSerializer.Validate(serializationManager, node2, dependencies, context));
    }
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }

  public ValidationNode Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context = null)
  {
    return this.ValidateInternal(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<IReadOnlyCollection<string>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    return this.ValidateInternal(serializationManager, node, dependencies, context);
  }

  ValidationNode ITypeValidator<IReadOnlyList<string>, SequenceDataNode>.Validate(
    ISerializationManager serializationManager,
    SequenceDataNode node,
    IDependencyCollection dependencies,
    ISerializationContext? context)
  {
    System.Collections.Generic.List<ValidationNode> sequence = new System.Collections.Generic.List<ValidationNode>();
    foreach (DataNode node1 in (IEnumerable<DataNode>) node.Sequence)
    {
      if (!(node1 is ValueDataNode node2))
        sequence.Add((ValidationNode) new ErrorNode(node1, $"Cannot cast node {node1} to ValueDataNode."));
      else
        sequence.Add(this.PrototypeSerializer.Validate(serializationManager, node2, dependencies, context));
    }
    return (ValidationNode) new ValidatedSequenceNode(sequence);
  }
}
