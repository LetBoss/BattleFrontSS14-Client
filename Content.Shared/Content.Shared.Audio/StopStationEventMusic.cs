using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio;

[Serializable]
[NetSerializable]
public sealed class StopStationEventMusic : EntityEventArgs
{
	public StationEventMusicType Type;

	public StopStationEventMusic(StationEventMusicType type)
	{
		Type = type;
	}
}
