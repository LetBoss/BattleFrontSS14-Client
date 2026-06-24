// Decompiled with JetBrains decompiler
// Type: Content.Shared.Clothing.SharedMagbootsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Alert;
using Content.Shared.Atmos.Components;
using Content.Shared.Gravity;
using Content.Shared.Inventory;
using Content.Shared.Item.ItemToggle;
using Content.Shared.Item.ItemToggle.Components;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Shared.Clothing;

public sealed class SharedMagbootsSystem : EntitySystem
{
  [Dependency]
  private AlertsSystem _alerts;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private ItemToggleSystem _toggle;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedGravitySystem _gravity;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagbootsComponent, ItemToggledEvent>(new EntityEventRefHandler<MagbootsComponent, ItemToggledEvent>((object) this, __methodptr(OnToggled)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagbootsComponent, ClothingGotEquippedEvent>(new EntityEventRefHandler<MagbootsComponent, ClothingGotEquippedEvent>((object) this, __methodptr(OnGotEquipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagbootsComponent, ClothingGotUnequippedEvent>(new EntityEventRefHandler<MagbootsComponent, ClothingGotUnequippedEvent>((object) this, __methodptr(OnGotUnequipped)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagbootsComponent, IsWeightlessEvent>(new EntityEventRefHandler<MagbootsComponent, IsWeightlessEvent>((object) this, __methodptr(OnIsWeightless)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MagbootsComponent, InventoryRelayedEvent<IsWeightlessEvent>>(new EntityEventRefHandler<MagbootsComponent, InventoryRelayedEvent<IsWeightlessEvent>>((object) this, __methodptr(OnIsWeightless)), (Type[]) null, (Type[]) null);
  }

  private void OnToggled(Entity<MagbootsComponent> ent, ref ItemToggledEvent args)
  {
    EntityUid entityUid1;
    MagbootsComponent magbootsComponent1;
    ent.Deconstruct(ref entityUid1, ref magbootsComponent1);
    EntityUid entityUid2 = entityUid1;
    MagbootsComponent magbootsComponent2 = magbootsComponent1;
    BaseContainer baseContainer;
    EntityUid? entityUid3;
    if (!this._container.TryGetContainingContainer(Entity<TransformComponent, MetaDataComponent>.op_Implicit((entityUid2, (TransformComponent) null, (MetaDataComponent) null)), ref baseContainer) || !this._inventory.TryGetSlotEntity(baseContainer.Owner, magbootsComponent2.Slot, out entityUid3))
      return;
    EntityUid entityUid4 = entityUid2;
    EntityUid? nullable = entityUid3;
    if ((nullable.HasValue ? (EntityUid.op_Equality(entityUid4, nullable.GetValueOrDefault()) ? 1 : 0) : 0) == 0)
      return;
    this.UpdateMagbootEffects(baseContainer.Owner, ent, args.Activated);
  }

  private void OnGotUnequipped(Entity<MagbootsComponent> ent, ref ClothingGotUnequippedEvent args)
  {
    this.UpdateMagbootEffects(args.Wearer, ent, false);
  }

  private void OnGotEquipped(Entity<MagbootsComponent> ent, ref ClothingGotEquippedEvent args)
  {
    this.UpdateMagbootEffects(args.Wearer, ent, this._toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)));
  }

  public void UpdateMagbootEffects(EntityUid user, Entity<MagbootsComponent> ent, bool state)
  {
    MovedByPressureComponent pressureComponent;
    if (this.TryComp<MovedByPressureComponent>(user, ref pressureComponent))
      pressureComponent.Enabled = !state;
    if (state)
      this._alerts.ShowAlert(user, ent.Comp.MagbootsAlert);
    else
      this._alerts.ClearAlert(user, ent.Comp.MagbootsAlert);
  }

  private void OnIsWeightless(Entity<MagbootsComponent> ent, ref IsWeightlessEvent args)
  {
    if (args.Handled || !this._toggle.IsActivated(Entity<ItemToggleComponent>.op_Implicit(ent.Owner)) || ent.Comp.RequiresGrid && !this._gravity.EntityOnGravitySupportingGridOrMap(Entity<TransformComponent>.op_Implicit(ent.Owner)))
      return;
    args.IsWeightless = false;
    args.Handled = true;
  }

  private void OnIsWeightless(
    Entity<MagbootsComponent> ent,
    ref InventoryRelayedEvent<IsWeightlessEvent> args)
  {
    this.OnIsWeightless(ent, ref args.Args);
  }
}
