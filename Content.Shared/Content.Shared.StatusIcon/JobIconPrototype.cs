using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Array;
using Robust.Shared.ViewVariables;

namespace Content.Shared.StatusIcon;

[Prototype(null, 1)]
public sealed class JobIconPrototype : StatusIconPrototype, IInheritingPrototype
{
	[DataField(null, false, 1, false, false, null)]
	public bool AllowSelection = true;

	[ParentDataField(typeof(AbstractPrototypeIdArraySerializer<JobIconPrototype>), 1)]
	public string[]? Parents { get; private set; }

	[NeverPushInheritance]
	[AbstractDataField(1)]
	public bool Abstract { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public string JobName { get; private set; } = string.Empty;

	[ViewVariables(/*Could not decode attribute arguments.*/)]
	public string LocalizedJobName => Loc.GetString(JobName);
}
