using System;
using Robust.Shared.Serialization;

namespace Content.Shared.Telephone;

[Serializable]
[NetSerializable]
public enum TelephoneVolume : byte
{
	Whisper,
	Speak
}
