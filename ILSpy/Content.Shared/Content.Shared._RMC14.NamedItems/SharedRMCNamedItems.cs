using System;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.NamedItems;

[Serializable]
[DataRecord]
[NetSerializable]
public record SharedRMCNamedItems(string? PrimaryGunName = null, string? SidearmName = null, string? HelmetName = null, string? ArmorName = null, string? SentryName = null);
