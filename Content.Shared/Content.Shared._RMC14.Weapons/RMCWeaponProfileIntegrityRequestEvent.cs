using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileIntegrityRequestEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public int ChallengeSalt { get; }

	public List<byte> ProbeIds { get; }

	public RMCWeaponProfileIntegrityRequestEvent(int nonce, int token, int challengeSalt, List<byte>? probeIds)
	{
		Nonce = nonce;
		Token = token;
		ChallengeSalt = challengeSalt;
		ProbeIds = probeIds ?? new List<byte>();
	}
}
