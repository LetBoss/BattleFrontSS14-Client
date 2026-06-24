// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set.PrototypeIdHashSetSerializer`1
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
namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

[Virtual]
public class PrototypeIdHashSetSerializer<TPrototype> : 
  ITypeValidator<HashSet<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<HashSet<string>, SequenceDataNode>,
  ITypeValidator<ImmutableHashSet<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ImmutableHashSet<string>, SequenceDataNode>,
  ITypeValidator<ISet<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<ISet<string>, SequenceDataNode>,
  ITypeValidator<IReadOnlySet<string>, SequenceDataNode>,
  BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlySet<string>, SequenceDataNode>
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
    List<ValidationNode> sequence = new List<ValidationNode>();
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
