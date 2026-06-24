using System;
using Robust.Shared.Serialization;

namespace Robust.Shared.Audio.Components;

[Serializable]
[NetSerializable]
public enum AudioState : byte
{
	Stopped,
	Playing,
	Paused
}
