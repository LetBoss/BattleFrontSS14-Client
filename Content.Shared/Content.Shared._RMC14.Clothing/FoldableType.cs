using System;
using Robust.Shared.Localization;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared._RMC14.Clothing;

[Serializable]
[DataRecord]
[NetSerializable]
public readonly record struct FoldableType(string Prefix, LocId Name, int Priority, string? BlacklistedPrefix, LocId? BlacklistPopup, bool HideAccessories = false);
