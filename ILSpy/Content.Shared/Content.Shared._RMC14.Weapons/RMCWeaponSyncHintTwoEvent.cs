using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponSyncHintTwoEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Probe { get; }

	public RMCWeaponSyncHintTwoEvent(int nonce, int probe = 0)
	{
		Nonce = nonce;
		Probe = probe;
	}
}
