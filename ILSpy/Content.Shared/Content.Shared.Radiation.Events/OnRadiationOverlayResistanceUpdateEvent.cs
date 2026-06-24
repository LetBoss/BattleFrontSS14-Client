using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;

namespace Content.Shared.Radiation.Events;

[Serializable]
[NetSerializable]
public sealed class OnRadiationOverlayResistanceUpdateEvent : EntityEventArgs
{
	public readonly Dictionary<NetEntity, Dictionary<Vector2i, float>> Grids;

	public OnRadiationOverlayResistanceUpdateEvent(Dictionary<NetEntity, Dictionary<Vector2i, float>> grids)
	{
		Grids = grids;
	}
}
