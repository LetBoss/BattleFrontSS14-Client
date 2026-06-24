// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Overwatch.OverwatchMarine
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Marines.Roles.Ranks;
using Content.Shared.Mobs;
using Content.Shared.Roles;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Overwatch;

[NetSerializable]
[Serializable]
public readonly record struct OverwatchMarine(
  NetEntity Id,
  NetEntity Camera,
  string Name,
  MobState State,
  bool SSD,
  ProtoId<JobPrototype>? Role,
  bool Deployed,
  OverwatchLocation Location,
  string AreaName,
  Vector2? LeaderDistance,
  ProtoId<RankPrototype>? Rank,
  LocId? RoleOverride)
;
