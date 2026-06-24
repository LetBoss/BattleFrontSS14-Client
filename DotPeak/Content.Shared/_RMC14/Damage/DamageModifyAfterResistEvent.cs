// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Damage.DamageModifyAfterResistEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared._RMC14.Damage;

public sealed class DamageModifyAfterResistEvent : EntityEventArgs, IInventoryRelayEvent
{
  public readonly DamageSpecifier OriginalDamage;
  public DamageSpecifier Damage;
  public EntityUid? Origin;
  public EntityUid? Tool;

  public SlotFlags TargetSlots { get; } = SlotFlags.WITHOUT_POCKET;

  public DamageModifyAfterResistEvent(DamageSpecifier damage, EntityUid? origin = null, EntityUid? tool = null)
  {
    this.OriginalDamage = damage;
    this.Damage = damage;
    this.Origin = origin;
    this.Tool = tool;
  }
}
