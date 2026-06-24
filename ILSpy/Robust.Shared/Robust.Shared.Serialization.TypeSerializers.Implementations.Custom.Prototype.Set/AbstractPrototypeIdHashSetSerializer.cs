using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

public sealed class AbstractPrototypeIdHashSetSerializer<TPrototype> : PrototypeIdHashSetSerializer<TPrototype> where TPrototype : class, IPrototype, IInheritingPrototype
{
	protected override PrototypeIdSerializer<TPrototype> PrototypeSerializer => new AbstractPrototypeIdSerializer<TPrototype>();
}
