using System;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Stats;

[Serializable]
[NetSerializable]
public readonly record struct CivKillDetail(string Victim, string VictimUserId, string Weapon, int Distance, int AtSeconds, bool Teamkill);
