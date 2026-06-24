using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Admin.Reputation;

[Serializable]
[NetSerializable]
public readonly record struct PubgReputationHistoryEntry(int Id, string ChangedByName, int Delta, int OldValue, int NewValue, string? Reason, string Source, DateTime CreatedAt);
