using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

public sealed class AbstractPrototypeIdSerializer<TPrototype> : PrototypeIdSerializer<TPrototype> where TPrototype : class, IPrototype, IInheritingPrototype
{
	public override ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		IPrototypeManager prototypeManager = dependencies.Resolve<IPrototypeManager>();
		if (!prototypeManager.TryGetKindFrom<TPrototype>(out string _) || !prototypeManager.HasMapping<TPrototype>(node.Value))
		{
			return new ErrorNode(node, $"PrototypeID {node.Value} for type {typeof(TPrototype)} not found");
		}
		return new ValidatedValueNode(node);
	}
}
