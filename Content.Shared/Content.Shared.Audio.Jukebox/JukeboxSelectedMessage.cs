using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Audio.Jukebox;

[Serializable]
[NetSerializable]
public sealed class JukeboxSelectedMessage(ProtoId<JukeboxPrototype> songId) : BoundUserInterfaceMessage
{
	public ProtoId<JukeboxPrototype> SongId { get; } = songId;
}
