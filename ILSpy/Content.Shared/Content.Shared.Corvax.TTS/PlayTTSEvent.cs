using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Corvax.TTS;

[Serializable]
[NetSerializable]
public sealed class PlayTTSEvent : EntityEventArgs
{
	public byte[] Data { get; }

	public NetEntity? SourceUid { get; }

	public bool IsWhisper { get; }

	public bool IsRadio { get; }

	public bool IsLexiconSound { get; }

	public string LanguageId { get; }

	public PlayTTSEvent(byte[] data, NetEntity? sourceUid = null, bool isWhisper = false, bool isRadio = false, bool isSoundLexicon = false, string languageId = "")
	{
		Data = data;
		SourceUid = sourceUid;
		IsWhisper = isWhisper;
		IsRadio = isRadio;
		IsLexiconSound = isSoundLexicon;
		LanguageId = languageId;
	}
}
