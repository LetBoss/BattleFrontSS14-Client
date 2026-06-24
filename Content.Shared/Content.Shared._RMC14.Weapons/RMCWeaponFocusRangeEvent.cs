using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponFocusRangeEvent : EntityEventArgs
{
	public int Nonce { get; }

	public float AimDistance { get; }

	public RMCWeaponFocusRangeEvent(int nonce, float aimDistance)
	{
		Nonce = nonce;
		AimDistance = aimDistance;
	}
}
