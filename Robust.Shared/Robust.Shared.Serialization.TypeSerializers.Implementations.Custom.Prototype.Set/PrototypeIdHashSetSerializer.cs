using System.Collections.Generic;
using System.Collections.Immutable;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

[Virtual]
public class PrototypeIdHashSetSerializer<TPrototype> : ITypeValidator<HashSet<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<HashSet<string>, SequenceDataNode>, ITypeValidator<ImmutableHashSet<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ImmutableHashSet<string>, SequenceDataNode>, ITypeValidator<ISet<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ISet<string>, SequenceDataNode>, ITypeValidator<IReadOnlySet<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlySet<string>, SequenceDataNode> where TPrototype : class, IPrototype
{
	protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer => new PrototypeIdSerializer<TPrototype>();

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		List<ValidationNode> list = new List<ValidationNode>();
		foreach (DataNode item in node.Sequence)
		{
			if (!(item is ValueDataNode node2))
			{
				list.Add(new ErrorNode(item, $"Cannot cast node {item} to ValueDataNode."));
			}
			else
			{
				list.Add(PrototypeSerializer.Validate(serializationManager, node2, dependencies, context));
			}
		}
		return new ValidatedSequenceNode(list);
	}
}
