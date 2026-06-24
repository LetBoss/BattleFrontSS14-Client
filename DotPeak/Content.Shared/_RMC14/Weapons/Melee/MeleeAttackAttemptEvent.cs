// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Melee.MeleeAttackAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Melee;

[ByRefEvent]
[NetSerializable]
[Serializable]
public record struct MeleeAttackAttemptEvent(
  NetEntity Target,
  AttackEvent Attack,
  NetCoordinates Coordinates,
  List<NetEntity> PotentialTargets,
  NetEntity Weapon)
;
