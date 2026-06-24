using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Jukebox;

[Serializable]
[NetSerializable]
public sealed class JukeboxPlayingMessage : BoundUserInterfaceMessage
{
}
