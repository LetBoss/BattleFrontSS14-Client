// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.EntitySystems.SkatesSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Damage.Systems;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Clothing.EntitySystems;

public sealed class SkatesSystem : EntitySystem
{
  [Dependency]
  private MovementSpeedModifierSystem _move;
  [Dependency]
  private DamageOnHighSpeedImpactSystem _impact;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SkatesComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<SkatesComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SkatesComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<SkatesComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SkatesComponent, InventoryRelayedEvent<RefreshFrictionModifiersEvent>>(new EntityEventRefHandler<SkatesComponent, InventoryRelayedEvent<RefreshFrictionModifiersEvent>>((object) this, __methodptr(OnRefreshFrictionModifiers)), (Type[]) null, (Type[]) null);
  }

  private void OnGotUnequipped(Entity<SkatesComponent> entity, ref ClothingGotUnequippedEvent args)
  {
    this._move.RefreshFrictionModifiers(args.Wearer);
    this._impact.ChangeCollide(args.Wearer, entity.Comp.DefaultMinimumSpeed, entity.Comp.DefaultStunSeconds, entity.Comp.DefaultDamageCooldown, entity.Comp.DefaultSpeedDamage);
  }

  private void OnGotEquipped(Entity<SkatesComponent> entity, ref ClothingGotEquippedEvent args)
  {
    this._move.RefreshFrictionModifiers(args.Wearer);
    this._impact.ChangeCollide(args.Wearer, entity.Comp.MinimumSpeed, entity.Comp.StunSeconds, entity.Comp.DamageCooldown, entity.Comp.SpeedDamage);
  }

  private void OnRefreshFrictionModifiers(
    Entity<SkatesComponent> ent,
    ref InventoryRelayedEvent<RefreshFrictionModifiersEvent> args)
  {
    args.Args.ModifyFriction(ent.Comp.Friction, ent.Comp.FrictionNoInput);
    args.Args.ModifyAcceleration(ent.Comp.Acceleration);
  }
}
