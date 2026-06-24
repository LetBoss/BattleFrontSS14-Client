using Robust.Shared.Prototypes;

namespace Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Dictionary;

public sealed class AbstractPrototypeIdValueDictionarySerializer<TValue, TPrototype> : PrototypeIdValueDictionarySerializer<TValue, TPrototype> where TValue : notnull where TPrototype : class, IPrototype, IInheritingPrototype
{
	protected override PrototypeIdSerializer<TPrototype> PrototypeSerializer => new AbstractPrototypeIdSerializer<TPrototype>();
}
