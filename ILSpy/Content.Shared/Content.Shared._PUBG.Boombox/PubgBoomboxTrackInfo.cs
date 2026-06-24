using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxTrackInfo
{
	public string Id = string.Empty;

	public string Title = string.Empty;

	public int DurationSec;

	public int SizeBytes;
}
