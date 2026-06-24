// Decompiled with JetBrains decompiler
// Type: Content.Client.Overlays.EquipmentHudSystem`1
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.GameTicking;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client.Overlays;

public abstract class EquipmentHudSystem<T> : EntitySystem where T : IComponent
{
  [Dependency]
  private IPlayerManager _player;
  [Robust.Shared.ViewVariables.ViewVariables]
  protected bool IsActive;

  protected virtual SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<T, ComponentStartup>(new EntityEventRefHandler<T, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<T, ComponentRemove>(new EntityEventRefHandler<T, ComponentRemove>((object) this, __methodptr(OnRemove)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerAttachedEvent>(new EntityEventHandler<LocalPlayerAttachedEvent>(this.OnPlayerAttached), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<LocalPlayerDetachedEvent>(new EntityEventHandler<LocalPlayerDetachedEvent>(this.OnPlayerDetached), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<T, GotEquippedEvent>(new EntityEventRefHandler<T, GotEquippedEvent>((object) this, __methodptr(OnCompEquip)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<T, GotUnequippedEvent>(new EntityEventRefHandler<T, GotUnequippedEvent>((object) this, __methodptr(OnCompUnequip)), (Type[]) null, (Type[]) null);
    EquipmentHudSystem<T> equipmentHudSystem1 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<T, RefreshEquipmentHudEvent<T>>(new EntityEventRefHandler<T, RefreshEquipmentHudEvent<T>>((object) equipmentHudSystem1, __vmethodptr(equipmentHudSystem1, OnRefreshComponentHud)), (Type[]) null, (Type[]) null);
    EquipmentHudSystem<T> equipmentHudSystem2 = this;
    // ISSUE: virtual method pointer
    this.SubscribeLocalEvent<T, InventoryRelayedEvent<RefreshEquipmentHudEvent<T>>>(new EntityEventRefHandler<T, InventoryRelayedEvent<RefreshEquipmentHudEvent<T>>>((object) equipmentHudSystem2, __vmethodptr(equipmentHudSystem2, OnRefreshEquipmentHud)), (Type[]) null, (Type[]) null);
    this.SubscribeLocalEvent<RoundRestartCleanupEvent>(new EntityEventHandler<RoundRestartCleanupEvent>(this.OnRoundRestart), (Type[]) null, (Type[]) null);
  }

  private void Update(RefreshEquipmentHudEvent<T> ev)
  {
    this.IsActive = true;
    this.UpdateInternal(ev);
  }

  public void Deactivate()
  {
    if (!this.IsActive)
      return;
    this.IsActive = false;
    this.DeactivateInternal();
  }

  protected virtual void UpdateInternal(RefreshEquipmentHudEvent<T> args)
  {
  }

  protected virtual void DeactivateInternal()
  {
  }

  private void OnStartup(Entity<T> ent, ref ComponentStartup args) => this.RefreshOverlay();

  private void OnRemove(Entity<T> ent, ref ComponentRemove args) => this.RefreshOverlay();

  private void OnPlayerAttached(LocalPlayerAttachedEvent args) => this.RefreshOverlay();

  private void OnPlayerDetached(LocalPlayerDetachedEvent args)
  {
    if (((EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity).HasValue)
      return;
    this.Deactivate();
  }

  private void OnCompEquip(Entity<T> ent, ref GotEquippedEvent args) => this.RefreshOverlay();

  private void OnCompUnequip(Entity<T> ent, ref GotUnequippedEvent args) => this.RefreshOverlay();

  private void OnRoundRestart(RoundRestartCleanupEvent args) => this.Deactivate();

  protected virtual void OnRefreshEquipmentHud(
    Entity<T> ent,
    ref InventoryRelayedEvent<RefreshEquipmentHudEvent<T>> args)
  {
    this.OnRefreshComponentHud(ent, ref args.Args);
  }

  protected virtual void OnRefreshComponentHud(Entity<T> ent, ref RefreshEquipmentHudEvent<T> args)
  {
    args.Active = true;
    args.Components.Add(ent.Comp);
  }

  protected void RefreshOverlay()
  {
    EntityUid? attachedEntity = (EntityUid?) ((ISharedPlayerManager) this._player).LocalSession?.AttachedEntity;
    if (!attachedEntity.HasValue)
      return;
    EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
    RefreshEquipmentHudEvent<T> ev = new RefreshEquipmentHudEvent<T>(this.TargetSlots);
    this.RaiseLocalEvent<RefreshEquipmentHudEvent<T>>(valueOrDefault, ref ev, false);
    if (ev.Active)
      this.Update(ev);
    else
      this.Deactivate();
  }
}
