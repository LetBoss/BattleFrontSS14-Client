using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Marines.Announce;

[Serializable]
[NetSerializable]
public readonly record struct LandingZone(NetEntity Id, string Name);
