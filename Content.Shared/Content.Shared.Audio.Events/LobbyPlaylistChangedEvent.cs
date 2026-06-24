using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Events;

[Serializable]
[NetSerializable]
public sealed class LobbyPlaylistChangedEvent : EntityEventArgs
{
	public string[] Playlist;

	public LobbyPlaylistChangedEvent(string[] playlist)
	{
		Playlist = playlist;
	}
}
