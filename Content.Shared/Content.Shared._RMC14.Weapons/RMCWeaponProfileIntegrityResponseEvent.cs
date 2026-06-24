using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileIntegrityResponseEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public bool Success { get; }

	public List<byte> ProbeIds { get; }

	public List<string> ProbeDigests { get; }

	public uint FlowToken { get; }

	public RMCWeaponProfileIntegrityResponseEvent(int nonce, int token, bool success, List<byte>? probeIds, List<string>? probeDigests, uint flowToken = 0u)
	{
		Nonce = nonce;
		Token = token;
		Success = success;
		ProbeIds = probeIds ?? new List<byte>();
		ProbeDigests = probeDigests ?? new List<string>();
		FlowToken = flowToken;
	}
}
