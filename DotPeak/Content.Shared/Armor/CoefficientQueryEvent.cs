// Decompiled with JetBrains decompiler
// Type: Content.Shared.Armor.CoefficientQueryEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Damage;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable enable
namespace Content.Shared.Armor;

public sealed class CoefficientQueryEvent : EntityEventArgs, IInventoryRelayEvent
{
  public SlotFlags TargetSlots { get; set; }

  public DamageModifierSet DamageModifiers { get; set; } = new DamageModifierSet();

  public CoefficientQueryEvent(SlotFlags slots) => this.TargetSlots = slots;
}
