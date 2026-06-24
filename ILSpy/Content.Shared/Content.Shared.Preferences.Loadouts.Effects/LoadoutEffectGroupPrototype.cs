using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.Preferences.Loadouts.Effects;

[Prototype(null, 1)]
public sealed class LoadoutEffectGroupPrototype : IPrototype
{
	[DataField(null, false, 1, true, false, null)]
	public List<LoadoutEffect> Effects = new List<LoadoutEffect>();

	[IdDataField(1, null)]
	public string ID { get; private set; } = string.Empty;
}
