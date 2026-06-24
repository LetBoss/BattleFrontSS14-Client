using Content.Shared.Alert;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.StatusEffect;

[Prototype(null, 1)]
public sealed class StatusEffectPrototype : IPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField("alert", false, 1, false, false, null)]
	public ProtoId<AlertPrototype>? Alert { get; private set; }

	[DataField("alwaysAllowed", false, 1, false, false, null)]
	public bool AlwaysAllowed { get; private set; }
}
