using Robust.Shared.Analyzers;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Markdown.Validation;
using Robust.Shared.Serialization.Markdown.Value;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Generic;
using Robust.Shared.Serialization.TypeSerializers.Interfaces;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

[Virtual]
public class PrototypeIdSerializer<TPrototype> : ITypeValidator<string, ValueDataNode>, BaseSerializerInterfaces.ITypeNodeInterface<string, ValueDataNode> where TPrototype : class, IPrototype
{
	public virtual ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node, IDependencyCollection dependencies, ISerializationContext? context = null)
	{
		return ProtoIdSerializer<TPrototype>.Validate(dependencies, node);
	}
}
