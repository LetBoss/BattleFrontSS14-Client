using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileReadyEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public RMCWeaponProfileReadyEvent(int nonce, int token)
	{
		Nonce = nonce;
		Token = token;
	}
}
