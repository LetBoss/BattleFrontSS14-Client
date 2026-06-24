// Decompiled with JetBrains decompiler
// Type: Content.Shared.IdentityManagement.SharedIdentitySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Clothing;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Shared.IdentityManagement;

public abstract class SharedIdentitySystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  private static string SlotName = "identity";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<IdentityComponent, ComponentInit>(new ComponentEventHandler<IdentityComponent, ComponentInit>(this.OnComponentInit));
    this.SubscribeLocalEvent<IdentityBlockerComponent, SeeIdentityAttemptEvent>(new ComponentEventHandler<IdentityBlockerComponent, SeeIdentityAttemptEvent>(this.OnSeeIdentity));
    this.SubscribeLocalEvent<IdentityBlockerComponent, InventoryRelayedEvent<SeeIdentityAttemptEvent>>((ComponentEventHandler<IdentityBlockerComponent, InventoryRelayedEvent<SeeIdentityAttemptEvent>>) ((e, c, ev) => this.OnSeeIdentity(e, c, ev.Args)));
    this.SubscribeLocalEvent<IdentityBlockerComponent, ItemMaskToggledEvent>(new EntityEventRefHandler<IdentityBlockerComponent, ItemMaskToggledEvent>(this.OnMaskToggled));
  }

  private void OnSeeIdentity(
    EntityUid uid,
    IdentityBlockerComponent component,
    SeeIdentityAttemptEvent args)
  {
    if (!component.Enabled)
      return;
    args.TotalCoverage |= component.Coverage;
    if (args.TotalCoverage != IdentityBlockerCoverage.FULL)
      return;
    args.Cancel();
  }

  protected virtual void OnComponentInit(
    EntityUid uid,
    IdentityComponent component,
    ComponentInit args)
  {
    component.IdentityEntitySlot = this._container.EnsureContainer<ContainerSlot>(uid, SharedIdentitySystem.SlotName);
  }

  private void OnMaskToggled(Entity<IdentityBlockerComponent> ent, ref ItemMaskToggledEvent args)
  {
    ent.Comp.Enabled = !args.Mask.Comp.IsToggled;
  }

  public virtual void QueueIdentityUpdate(EntityUid uid)
  {
  }
}
