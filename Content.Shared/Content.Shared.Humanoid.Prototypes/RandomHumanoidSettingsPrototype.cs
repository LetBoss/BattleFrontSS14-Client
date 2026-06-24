using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.Humanoid.Prototypes;

[Prototype(null, 1)]
public sealed class RandomHumanoidSettingsPrototype : IPrototype, IInheritingPrototype
{
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[ParentDataField(typeof(PrototypeIdArraySerializer<RandomHumanoidSettingsPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[AbstractDataField(1)]
	[NeverPushInheritance]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public bool RandomizeName { get; private set; } = true;

	[DataField("speciesBlacklist", false, 1, false, false, null)]
	public HashSet<string> SpeciesBlacklist { get; private set; } = new HashSet<string>();

	[DataField(null, false, 1, false, false, null)]
	[AlwaysPushInheritance]
	public ComponentRegistry? Components { get; private set; }
}
