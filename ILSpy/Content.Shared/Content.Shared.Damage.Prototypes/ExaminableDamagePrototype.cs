using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Damage.Prototypes;

[Prototype(null, 1)]
public sealed class ExaminableDamagePrototype : IPrototype
{
	[DataField("messages", false, 1, false, false, null)]
	public string[] Messages = new string[0];

	[IdDataField(1, null)]
	public string ID { get; private set; }
}
