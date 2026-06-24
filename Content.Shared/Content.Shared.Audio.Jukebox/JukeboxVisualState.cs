using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Jukebox;

[Serializable]
[NetSerializable]
public enum JukeboxVisualState : byte
{
	On,
	Off,
	Select
}
