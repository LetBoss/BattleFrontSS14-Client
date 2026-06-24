using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared._RMC14.Marines.Roles.Ranks;

[Prototype(null, 1)]
public sealed class RankPrototype : IPrototype, IInheritingPrototype
{
	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<RankPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[ViewVariables]
	[IdDataField(1, null)]
	public string ID { get; private set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, true, false, null)]
	public string Name { get; set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, true, false, null)]
	public string Prefix { get; set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, false, false, null)]
	public string? MalePrefix { get; set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, false, false, null)]
	public string? FemalePrefix { get; set; }

	[AlwaysPushInheritance]
	[DataField(null, false, 1, false, false, null)]
	public string? Paygrade { get; set; }
}
