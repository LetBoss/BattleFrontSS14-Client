using System;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Loadout;

[Serializable]
[NetSerializable]
public readonly record struct PubgWeaponStats(float SpreadMultiplier, float HipfireSpreadMultiplier, float RecoilMultiplier, float ReloadTimeMultiplier, int MagazineCapacityBonus, float RangeMultiplier, float FocusBonusTiles);
