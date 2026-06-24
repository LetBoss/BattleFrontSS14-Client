using Content.Shared.Alert;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._RMC14.Tracker;

[Prototype(null, 1)]
public sealed class TrackerModePrototype : IPrototype
{
	[DataField(null, false, 1, false, false, typeof(ComponentNameSerializer))]
	public string? Component;

	[IdDataField(1, null)]
	public string ID { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<JobPrototype>? Job { get; private set; }

	[DataField(null, false, 1, false, false, null)]
	public ProtoId<AlertPrototype> Alert { get; private set; } = ProtoId<AlertPrototype>.op_Implicit("SquadTracker");
}
