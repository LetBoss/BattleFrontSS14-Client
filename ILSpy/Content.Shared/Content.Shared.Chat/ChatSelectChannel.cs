using System;

namespace Content.Shared.Chat;

[Flags]
public enum ChatSelectChannel : uint
{
	None = 0u,
	Local = 1u,
	Whisper = 2u,
	Radio = 0x10u,
	LOOC = 0x20u,
	OOC = 0x40u,
	Emotes = 0x200u,
	Dead = 0x400u,
	Admin = 0x2000u,
	Mentor = 0x8000u,
	Party = 0x10000u,
	MiniGame = 0x20000u,
	Lobby = 0x40000u,
	Console = 0x4000u
}
