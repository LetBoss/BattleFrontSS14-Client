using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.Watch;

[Serializable]
[NetSerializable]
public readonly record struct Xeno(NetEntity Entity, string Name, EntProtoId? Id);
