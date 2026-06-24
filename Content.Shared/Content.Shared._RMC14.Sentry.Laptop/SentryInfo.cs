using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Sentry.Laptop;

[Serializable]
[NetSerializable]
public record SentryInfo(NetEntity Id, string Name, SentryMode Mode, float Health, float MaxHealth, int Ammo, int MaxAmmo, string Location, NetEntity? Target, HashSet<string> FriendlyFactions, string? CustomName = null, float VisionRadius = 5f, float MaxDeviation = 75f, HashSet<string>? HumanoidAdded = null);
