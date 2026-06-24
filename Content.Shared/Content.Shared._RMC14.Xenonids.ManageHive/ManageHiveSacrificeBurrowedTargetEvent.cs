using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.ManageHive;

[Serializable]
[ByRefEvent]
[NetSerializable]
public sealed record ManageHiveSacrificeBurrowedTargetEvent(NetEntity Target);
