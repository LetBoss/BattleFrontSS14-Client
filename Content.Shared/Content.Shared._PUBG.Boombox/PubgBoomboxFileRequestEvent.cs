using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxFileRequestEvent : EntityEventArgs
{
	public string TrackId = string.Empty;

	public PubgBoomboxFileRequestEvent(string trackId)
	{
		TrackId = trackId;
	}
}
