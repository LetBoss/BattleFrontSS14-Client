using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Corvax.TTS;

[Serializable]
[NetSerializable]
public sealed class RequestPreviewTTSEvent(string voiceId) : EntityEventArgs
{
	public string VoiceId { get; } = voiceId;
}
