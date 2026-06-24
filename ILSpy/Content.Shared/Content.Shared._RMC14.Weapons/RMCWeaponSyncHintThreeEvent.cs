using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponSyncHintThreeEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Probe { get; }

	public RMCWeaponSyncHintThreeEvent(int nonce, int probe = 0)
	{
		Nonce = nonce;
		Probe = probe;
	}
}
