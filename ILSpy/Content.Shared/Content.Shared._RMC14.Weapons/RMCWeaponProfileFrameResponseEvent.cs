using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileFrameResponseEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public RMCWeaponProfileFrameMode Mode { get; }

	public bool Success { get; }

	public byte[] Payload { get; }

	public RMCWeaponProfileFrameResponseEvent(int nonce, int token, RMCWeaponProfileFrameMode mode, bool success, byte[]? payload)
	{
		Nonce = nonce;
		Token = token;
		Mode = mode;
		Success = success;
		Payload = payload ?? Array.Empty<byte>();
	}
}
