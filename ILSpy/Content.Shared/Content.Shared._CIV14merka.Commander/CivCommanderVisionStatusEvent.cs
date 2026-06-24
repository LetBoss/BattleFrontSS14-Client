using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderVisionStatusEvent : EntityEventArgs
{
	public bool Active { get; }

	public float Range { get; }

	public CivCommanderVisionStatusEvent(bool active, float range)
	{
		Active = active;
		Range = range;
	}
}
