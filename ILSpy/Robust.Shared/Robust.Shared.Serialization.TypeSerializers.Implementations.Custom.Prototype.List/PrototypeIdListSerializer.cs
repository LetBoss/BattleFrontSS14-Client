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

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

[Virtual]
public class PrototypeIdListSerializer<T> : ITypeValidator<List<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<List<string>, SequenceDataNode>, ITypeValidator<ImmutableList<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<ImmutableList<string>, SequenceDataNode>, ITypeValidator<IReadOnlyCollection<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyCollection<string>, SequenceDataNode>, ITypeValidator<IReadOnlyList<string>, SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<IReadOnlyList<string>, SequenceDataNode> where T : class, IPrototype
{
	protected virtual PrototypeIdSerializer<T> PrototypeSerializer => new PrototypeIdSerializer<T>();

	private ValidationNode ValidateInternal(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
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

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return ValidateInternal(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<IReadOnlyCollection<string>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
	{
		return ValidateInternal(serializationManager, node, dependencies, context);
	}

	ValidationNode ITypeValidator<IReadOnlyList<string>, SequenceDataNode>.Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context)
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
