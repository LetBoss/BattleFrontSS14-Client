using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Weapons;

[Serializable]
[NetSerializable]
public sealed class RMCWeaponProfileCatalogChunkEvent : EntityEventArgs
{
	public int Nonce { get; }

	public int Token { get; }

	public int ChunkIndex { get; }

	public int ChunkCount { get; }

	public bool Success { get; }

	public byte[] Payload { get; }

	public RMCWeaponProfileCatalogChunkEvent(int nonce, int token, int chunkIndex, int chunkCount, bool success, byte[]? payload)
	{
		Nonce = nonce;
		Token = token;
		ChunkIndex = chunkIndex;
		ChunkCount = chunkCount;
		Success = success;
		Payload = payload ?? Array.Empty<byte>();
	}
}
