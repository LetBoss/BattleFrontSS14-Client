// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.SharedCursedMaskSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing.Components;
using Content.Shared.Damage;
using Content.Shared.Examine;
using Content.Shared.Inventory;
using Content.Shared.Movement.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.Clothing;

public abstract class SharedCursedMaskSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private MovementSpeedModifierSystem _movementSpeedModifier;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursedMaskComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<CursedMaskComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnClothingEquip)), (Type[]) null, (Type[]) null);
    SharedCursedMaskSystem cursedMaskSystem = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<CursedMaskComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<CursedMaskComponent, ClothingGotUnequippedEvent>((object) cursedMaskSystem, __vmethodptr(cursedMaskSystem, OnClothingUnequip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursedMaskComponent, ExaminedEvent>(new EntityEventRefHandler<CursedMaskComponent, ExaminedEvent>((object) this, __methodptr(OnExamine)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursedMaskComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>(new EntityEventRefHandler<CursedMaskComponent, InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent>>((object) this, __methodptr(OnMovementSpeedModifier)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CursedMaskComponent, InventoryRelayedEvent<DamageModifyEvent>>(new EntityEventRefHandler<CursedMaskComponent, InventoryRelayedEvent<DamageModifyEvent>>((object) this, __methodptr(OnModifyDamage)), (Type[]) null, (Type[]) null);
  }

  private void OnClothingEquip(Entity<CursedMaskComponent> ent, ref ClothingGotEquippedEvent args)
  {
    this.RandomizeCursedMask(ent, args.Wearer);
    this.TryTakeover(ent, args.Wearer);
  }

  protected virtual void OnClothingUnequip(
    Entity<CursedMaskComponent> ent,
    ref ClothingGotUnequippedEvent args)
  {
    this.RandomizeCursedMask(ent, args.Wearer);
  }

  private void OnExamine(Entity<CursedMaskComponent> ent, ref ExaminedEvent args)
  {
    args.PushMarkup(this.Loc.GetString("cursed-mask-examine-" + ent.Comp.CurrentState.ToString()));
  }

  private void OnMovementSpeedModifier(
    Entity<CursedMaskComponent> ent,
    ref InventoryRelayedEvent<RefreshMovementSpeedModifiersEvent> args)
  {
    if (ent.Comp.CurrentState != CursedMaskExpression.Joy)
      return;
    args.Args.ModifySpeed(ent.Comp.JoySpeedModifier);
  }

  private void OnModifyDamage(
    Entity<CursedMaskComponent> ent,
    ref InventoryRelayedEvent<DamageModifyEvent> args)
  {
    if (ent.Comp.CurrentState != CursedMaskExpression.Despair)
      return;
    args.Args.Damage = DamageSpecifier.ApplyModifierSet(args.Args.Damage, ent.Comp.DespairDamageModifier);
  }

  protected void RandomizeCursedMask(Entity<CursedMaskComponent> ent, EntityUid wearer)
  {
    Random random = new Random((int) this._timing.CurTick.Value);
    CursedMaskExpression[] values = Enum.GetValues<CursedMaskExpression>();
    ent.Comp.CurrentState = values[random.Next(values.Length)];
    this._appearance.SetData(Entity<CursedMaskComponent>.op_Implicit(ent), (Enum) CursedMaskVisuals.State, (object) ent.Comp.CurrentState, (AppearanceComponent) null);
    this._movementSpeedModifier.RefreshMovementSpeedModifiers(wearer);
  }

  protected virtual void TryTakeover(Entity<CursedMaskComponent> ent, EntityUid wearer)
  {
  }
}
