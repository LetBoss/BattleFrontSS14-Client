using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Actions.Components;

[Serializable]
[DataRecord]
[NetSerializable]
public record struct ActionCooldown
{
	[DataField(null, false, 1, true, false, typeof(TimeOffsetSerializer))]
	public TimeSpan Start;

	[DataField(null, false, 1, true, false, typeof(TimeOffsetSerializer))]
	public TimeSpan End;
}
