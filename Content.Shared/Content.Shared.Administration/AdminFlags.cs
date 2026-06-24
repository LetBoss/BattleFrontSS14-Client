using System;

namespace Content.Shared.Administration;

[Flags]
public enum AdminFlags : uint
{
	None = 0u,
	Admin = 1u,
	Ban = 2u,
	Debug = 4u,
	Fun = 8u,
	Permissions = 0x10u,
	Server = 0x20u,
	Spawn = 0x40u,
	VarEdit = 0x80u,
	Mapping = 0x100u,
	Logs = 0x200u,
	Round = 0x400u,
	Query = 0x800u,
	Adminhelp = 0x1000u,
	ViewNotes = 0x2000u,
	EditNotes = 0x4000u,
	MassBan = 0x8000u,
	Stealth = 0x10000u,
	Adminchat = 0x20000u,
	Pii = 0x40000u,
	Moderator = 0x80000u,
	AdminWho = 0x100000u,
	NameColor = 0x200000u,
	Anticheat = 0x400000u,
	MentorHelp = 0x40000000u,
	Host = 0x80000000u
}
