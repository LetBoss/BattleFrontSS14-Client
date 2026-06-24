using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Skin;

[Serializable]
[NetSerializable]
public enum SponsorDisplayMode : byte
{
	Auto,
	Manual,
	Hidden
}
