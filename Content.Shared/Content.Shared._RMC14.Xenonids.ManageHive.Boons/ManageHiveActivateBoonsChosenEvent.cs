using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Xenonids.ManageHive.Boons;

[Serializable]
[ByRefEvent]
[NetSerializable]
public sealed record ManageHiveActivateBoonsChosenEvent(EntProtoId<HiveBoonDefinitionComponent> Boon);
