using System;
using Content.Shared.Database;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Admin.ChatBans;

[Serializable]
[NetSerializable]
public readonly record struct ChatBan(int Id, ChatType Type, DateTime BannedAt, DateTime? ExpiresAt, DateTime? UnbannedAt, string? UnbannedBy, string Reason);
