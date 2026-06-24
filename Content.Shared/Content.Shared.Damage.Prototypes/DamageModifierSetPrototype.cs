using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Shared.Damage.Prototypes;

[Prototype(null, 1)]
public sealed class DamageModifierSetPrototype : DamageModifierSet, IPrototype
{
	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }
}
