using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Shuttles.Events;

[Serializable]
[NetSerializable]
public sealed class ShuttleConsoleFTLPositionMessage : BoundUserInterfaceMessage
{
	public MapCoordinates Coordinates;

	public Angle Angle;
}
