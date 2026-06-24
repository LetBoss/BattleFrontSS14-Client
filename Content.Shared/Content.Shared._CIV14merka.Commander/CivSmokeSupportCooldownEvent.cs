using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivSmokeSupportCooldownEvent : EntityEventArgs
{
	public float Seconds { get; }

	public CivSmokeSupportCooldownEvent(float seconds)
	{
		Seconds = seconds;
	}
}
