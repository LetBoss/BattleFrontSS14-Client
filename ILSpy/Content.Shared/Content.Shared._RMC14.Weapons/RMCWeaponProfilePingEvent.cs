using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfilePingEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public RMCWeaponProfilePingEvent(int nonce, int token)
	{
		Nonce = nonce;
		Token = token;
	}
}
