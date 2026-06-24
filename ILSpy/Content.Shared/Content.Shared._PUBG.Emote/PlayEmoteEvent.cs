using System;
using Robust.Shared.Audio;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Emote;

[Serializable]
[NetSerializable]
public sealed class PlayEmoteEvent : EntityEventArgs
{
	public NetEntity Entity;

	public EmotePlayMode PlayMode;

	public float? Duration;

	public int? RepeatCount;

	public string RsiPath = string.Empty;

	public string StateName = string.Empty;

	public SoundSpecifier? Sound;

	public float Scale = 1f;
}
