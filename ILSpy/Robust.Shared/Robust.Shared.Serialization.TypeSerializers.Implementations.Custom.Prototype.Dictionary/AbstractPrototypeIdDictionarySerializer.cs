using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

public sealed class AbstractPrototypeIdDictionarySerializer<TValue, TPrototype> : PrototypeIdDictionarySerializer<TValue, TPrototype> where TPrototype : class, IPrototype, IInheritingPrototype
{
	protected override PrototypeIdSerializer<TPrototype> PrototypeSerializer => new AbstractPrototypeIdSerializer<TPrototype>();
}
