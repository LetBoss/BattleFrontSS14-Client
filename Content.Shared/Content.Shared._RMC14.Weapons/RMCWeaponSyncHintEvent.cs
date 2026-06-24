using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponSyncHintEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Probe { get; }

	public RMCWeaponSyncHintEvent(int nonce, int probe = 0)
	{
		Nonce = nonce;
		Probe = probe;
	}
}
