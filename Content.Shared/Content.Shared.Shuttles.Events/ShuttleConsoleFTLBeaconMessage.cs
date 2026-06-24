using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events;

[Serializable]
[NetSerializable]
public sealed class ShuttleConsoleFTLBeaconMessage : BoundUserInterfaceMessage
{
	public NetEntity Beacon;

	public Angle Angle;
}
