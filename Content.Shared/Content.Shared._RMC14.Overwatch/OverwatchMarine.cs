using System;
using System.Numerics;
using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Mobs;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Overwatch;

[Serializable]
[NetSerializable]
public readonly record struct OverwatchMarine(NetEntity Id, NetEntity Camera, string Name, MobState State, bool SSD, ProtoId<JobPrototype>? Role, bool Deployed, OverwatchLocation Location, string AreaName, Vector2? LeaderDistance, ProtoId<RankPrototype>? Rank, LocId? RoleOverride);
