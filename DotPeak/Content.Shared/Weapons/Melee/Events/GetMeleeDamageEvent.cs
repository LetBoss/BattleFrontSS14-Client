// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Events.GetMeleeDamageEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Robust.Shared.GameObjects;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Melee.Events;

[ByRefEvent]
public record struct GetMeleeDamageEvent(
  EntityUid Weapon,
  DamageSpecifier Damage,
  List<DamageModifierSet> Modifiers,
  EntityUid User,
  bool ResistanceBypass = false)
;
