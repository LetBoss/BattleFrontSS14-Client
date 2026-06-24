using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;

namespace Content.Shared.StatusIcon;

[Prototype(null, 1)]
public sealed class HealthIconPrototype : StatusIconPrototype, IInheritingPrototype
{
	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<HealthIconPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }
}
