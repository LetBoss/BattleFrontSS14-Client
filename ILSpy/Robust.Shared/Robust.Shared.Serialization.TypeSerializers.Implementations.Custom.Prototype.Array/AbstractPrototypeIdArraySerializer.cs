using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

public sealed class AbstractPrototypeIdArraySerializer<TPrototype> : PrototypeIdArraySerializer<TPrototype> where TPrototype : class, IPrototype, IInheritingPrototype
{
	protected override PrototypeIdSerializer<TPrototype> PrototypeSerializer => new AbstractPrototypeIdSerializer<TPrototype>();
}
