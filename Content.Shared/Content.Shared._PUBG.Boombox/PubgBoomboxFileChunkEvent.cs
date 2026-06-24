using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxFileChunkEvent : EntityEventArgs
{
	public string TrackId = string.Empty;

	public int Offset;

	public int TotalSize;

	public byte[] Data = Array.Empty<byte>();
}
