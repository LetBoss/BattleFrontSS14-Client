using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.CrashLand;

[Serializable]
[NetSerializable]
public abstract class FallAnimationEventArgs : EntityEventArgs
{
	public NetEntity Entity;

	public NetCoordinates Coordinates;

	public float FallDuration;
}
