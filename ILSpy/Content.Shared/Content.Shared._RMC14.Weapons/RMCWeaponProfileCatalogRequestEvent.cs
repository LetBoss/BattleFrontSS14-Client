using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileCatalogRequestEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public int MaxChunkBytes { get; }

	public float ChunkStepSeconds { get; }

	public RMCWeaponProfileCatalogRequestEvent(int nonce, int token, int maxChunkBytes, float chunkStepSeconds)
	{
		Nonce = nonce;
		Token = token;
		MaxChunkBytes = maxChunkBytes;
		ChunkStepSeconds = chunkStepSeconds;
	}
}
