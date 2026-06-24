using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Localization;

[Serializable]
[NetSerializable]
public enum CivAnnouncementKind : byte
{
	TeamAnnounce,
	TeamSystem,
	SquadAnnounce,
	PlayerNotice
}
