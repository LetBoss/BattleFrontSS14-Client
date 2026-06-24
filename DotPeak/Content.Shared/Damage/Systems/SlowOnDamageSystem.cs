// Decompiled with JetBrains decompiler
// Type: Content.Shared.Damage.SlowOnDamageSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.Damage.Components;
using Content.Shared.Examine;
using Content.Shared.FixedPoint;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Damage;

public sealed class SlowOnDamageSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifierSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SlowOnDamageComponent, DamageChangedEvent>(new ComponentEventHandler<SlowOnDamageComponent, DamageChangedEvent>((object) this, __methodptr(OnDamageChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>(new ComponentEventHandler<SlowOnDamageComponent, RefreshMovementSpeedModifiersEvent>((object) this, __methodptr(OnRefreshMovespeed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent>>(new EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent>>((object) this, __methodptr(OnModifySpeed)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ExaminedEvent>(new EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ExaminedEvent>((object) this, __methodptr(OnExamined)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ClothingSlowOnDamageModifierComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<ClothingSlowOnDamageModifierComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ComponentStartup>(new EntityEventRefHandler<IgnoreSlowOnDamageComponent, ComponentStartup>((object) this, __methodptr(OnIgnoreStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ComponentShutdown>(new EntityEventRefHandler<IgnoreSlowOnDamageComponent, ComponentShutdown>((object) this, __methodptr(OnIgnoreShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<IgnoreSlowOnDamageComponent, ModifySlowOnDamageSpeedEvent>(new EntityEventRefHandler<IgnoreSlowOnDamageComponent, ModifySlowOnDamageSpeedEvent>((object) this, __methodptr(OnIgnoreModifySpeed)), (Type[]) null, (Type[]) null);
  }

  private void OnRefreshMovespeed(
    EntityUid uid,
    SlowOnDamageComponent component,
    RefreshMovementSpeedModifiersEvent args)
  {
    DamageableComponent damageableComponent;
    if (!this.TryComp<DamageableComponent>(uid, ref damageableComponent) || damageableComponent.TotalDamage == FixedPoint2.Zero)
      return;
    FixedPoint2 key = FixedPoint2.Zero;
    FixedPoint2 totalDamage = damageableComponent.TotalDamage;
    foreach (KeyValuePair<FixedPoint2, float> modifierThreshold in component.SpeedModifierThresholds)
    {
      if (totalDamage >= modifierThreshold.Key && modifierThreshold.Key > key)
        key = modifierThreshold.Key;
    }
    if (!(key != FixedPoint2.Zero))
      return;
    ModifySlowOnDamageSpeedEvent damageSpeedEvent = new ModifySlowOnDamageSpeedEvent(component.SpeedModifierThresholds[key]);
    this.RaiseLocalEvent<ModifySlowOnDamageSpeedEvent>(uid, ref damageSpeedEvent, false);
    args.ModifySpeed(damageSpeedEvent.Speed, damageSpeedEvent.Speed);
  }

  private void OnDamageChanged(
    EntityUid uid,
    SlowOnDamageComponent component,
    DamageChangedEvent args)
  {
    this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(uid);
  }

  private void OnModifySpeed(
    Entity<ClothingSlowOnDamageModifierComponent> ent,
    ref InventoryRelayedEvent<ModifySlowOnDamageSpeedEvent> args)
  {
    float num = 1f - args.Args.Speed;
    if ((double) num <= 0.0)
      return;
    args.Args.Speed += num * ent.Comp.Modifier;
  }

  private void OnExamined(Entity<ClothingSlowOnDamageModifierComponent> ent, ref ExaminedEvent args)
  {
    string markup = this.Loc.GetString("slow-on-damage-modifier-examine", ("mod", (object) (float) ((1.0 - (double) ent.Comp.Modifier) * 100.0)));
    args.PushMarkup(markup);
  }

  private void OnGotEquipped(
    Entity<ClothingSlowOnDamageModifierComponent> ent,
    ref ClothingGotEquippedEvent args)
  {
    this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(args.Wearer);
  }

  private void OnGotUnequipped(
    Entity<ClothingSlowOnDamageModifierComponent> ent,
    ref ClothingGotUnequippedEvent args)
  {
    this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(args.Wearer);
  }

  private void OnIgnoreStartup(Entity<IgnoreSlowOnDamageComponent> ent, ref ComponentStartup args)
  {
    this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(Entity<IgnoreSlowOnDamageComponent>.op_Implicit(ent));
  }

  private void OnIgnoreShutdown(Entity<IgnoreSlowOnDamageComponent> ent, ref ComponentShutdown args)
  {
    this._movementSpeedModifierSystem.RefreshMovementSpeedModifiers(Entity<IgnoreSlowOnDamageComponent>.op_Implicit(ent));
  }

  private void OnIgnoreModifySpeed(
    Entity<IgnoreSlowOnDamageComponent> ent,
    ref ModifySlowOnDamageSpeedEvent args)
  {
    args.Speed = 1f;
  }
}
