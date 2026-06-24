using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events;

[Serializable]
[NetSerializable]
public sealed class EmergencyShuttlePositionMessage : EntityEventArgs
{
	public NetEntity? StationUid;

	public Box2? Position;
}
