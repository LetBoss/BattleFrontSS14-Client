using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;

public sealed class AbstractPrototypeIdListSerializer<T> : PrototypeIdListSerializer<T> where T : class, IPrototype, IInheritingPrototype
{
	protected override PrototypeIdSerializer<T> PrototypeSerializer => new AbstractPrototypeIdSerializer<T>();
}
