using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;

namespace Content.Shared.Projectiles;

[Serializable]
[NetSerializable]
public sealed class ImpactEffectEvent : EntityEventArgs
{
	public string Prototype;

	public NetCoordinates Coordinates;

	public ImpactEffectEvent(string prototype, NetCoordinates coordinates)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Prototype = prototype;
		Coordinates = coordinates;
	}
}
