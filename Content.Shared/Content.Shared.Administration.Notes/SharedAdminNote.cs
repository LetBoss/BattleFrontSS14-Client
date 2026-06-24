using System;
using Content.Shared.Database;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared.Administration.Notes;

[Serializable]
[NetSerializable]
public sealed record SharedAdminNote(int Id, NetUserId Player, int? Round, string? ServerName, TimeSpan PlaytimeAtNote, NoteType NoteType, string Message, NoteSeverity? NoteSeverity, bool Secret, string CreatedByName, string EditedByName, DateTime CreatedAt, DateTime? LastEditedAt, DateTime? ExpiryTime, string[]? BannedRoles, DateTime? UnbannedTime, string? UnbannedByName, bool? Seen);
