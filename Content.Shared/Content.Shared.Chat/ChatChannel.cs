using System;

namespace Content.Shared.Chat;

[Flags]
public enum ChatChannel : uint
{
	None = 0u,
	Local = 1u,
	Whisper = 2u,
	Server = 4u,
	Damage = 8u,
	Radio = 0x10u,
	LOOC = 0x20u,
	OOC = 0x40u,
	Visual = 0x80u,
	Notifications = 0x100u,
	Emotes = 0x200u,
	Dead = 0x400u,
	Admin = 0x800u,
	AdminAlert = 0x1000u,
	AdminChat = 0x2000u,
	Unspecified = 0x4000u,
	MentorChat = 0x8000u,
	Party = 0x10000u,
	MiniGame = 0x20000u,
	Lobby = 0x40000u,
	Killfeed = 0x80000u,
	IC = 0x79Bu,
	AdminRelated = 0xB800u
}
