using System.Linq;
using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown;
using Robust.Shared.Serialization.Markdown.Sequence;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

[Virtual]
public class PrototypeIdArraySerializer<TPrototype> : ITypeValidator<string[], SequenceDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<string[], SequenceDataNode>, ITypeValidator<string[], ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<string[], ValueDataNode> where TPrototype : class, IPrototype
{
	protected virtual PrototypeIdSerializer<TPrototype> PrototypeSerializer => new PrototypeIdSerializer<TPrototype>();

	public ValidationNode Validate(ISerializationManager serializationManager, SequenceDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return new ValidatedSequenceNode(node.Select((DataNode x) => (!(x is ValueDataNode node2)) ? new ErrorNode(x, $"Cannot cast node {x} to ValueDataNode.") : PrototypeSerializer.Validate(serializationManager, node2, dependencies, context)).ToList());
	}

	public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return PrototypeSerializer.Validate(serializationManager, node, dependencies, context);
	}
}
