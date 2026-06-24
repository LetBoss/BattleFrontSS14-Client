// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Melee.Events.HeavyAttackEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Weapons.Melee.Events;

[NetSerializable]
[Serializable]
public sealed class HeavyAttackEvent : AttackEvent
{
  public readonly NetEntity Weapon;
  public List<NetEntity> Entities;

  public HeavyAttackEvent(NetEntity weapon, List<NetEntity> entities, NetCoordinates coordinates)
    : base(coordinates)
  {
    this.Weapon = weapon;
    this.Entities = entities;
  }
}
