using Robust.Shared.Prototypes;

namespace Content.Shared.Chemistry.Reaction;

[Prototype(null, 1)]
public sealed class ReactiveGroupPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
