using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Boombox;

[Serializable]
[NetSerializable]
public sealed class PubgBoomboxFileErrorEvent : EntityEventArgs
{
	public string TrackId = string.Empty;

	public PubgBoomboxFileErrorEvent(string trackId)
	{
		TrackId = trackId;
	}
}
