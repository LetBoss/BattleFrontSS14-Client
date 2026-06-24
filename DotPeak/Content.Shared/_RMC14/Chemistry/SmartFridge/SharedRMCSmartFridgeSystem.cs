// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Chemistry.SmartFridge.SharedRMCSmartFridgeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
using Robust.Shared.Utility;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Chemistry.SmartFridge;

public abstract class SharedRMCSmartFridgeSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  private readonly HashSet<Entity<RMCSmartFridgeComponent>> _smartFridges = new HashSet<Entity<RMCSmartFridgeComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCSmartFridgeComponent, InteractUsingEvent>(new EntityEventRefHandler<RMCSmartFridgeComponent, InteractUsingEvent>(this.OnInteractUsing));
    this.Subs.BuiEvents<RMCSmartFridgeComponent>((object) RMCSmartFridgeUI.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<RMCSmartFridgeComponent>) (subs => subs.Event<RMCSmartFridgeVendMsg>(new EntityEventRefHandler<RMCSmartFridgeComponent, RMCSmartFridgeVendMsg>(this.OnVend))));
  }

  private void OnInteractUsing(Entity<RMCSmartFridgeComponent> ent, ref InteractUsingEvent args)
  {
    if (!this.HasComp<RMCSmartFridgeInsertableComponent>(args.Used))
      return;
    Container container = this._container.EnsureContainer<Container>((EntityUid) ent, ent.Comp.ContainerId);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) args.Used, (BaseContainer) container);
    this.Dirty<RMCSmartFridgeComponent>(ent);
  }

  public void TransferToNearby(EntityCoordinates coords, float range, EntityUid transfer)
  {
    this._smartFridges.Clear();
    this._entityLookup.GetEntitiesInRange<RMCSmartFridgeComponent>(coords, range, this._smartFridges);
    Entity<RMCSmartFridgeComponent>? element;
    if (!this._smartFridges.TryFirstOrNull<Entity<RMCSmartFridgeComponent>>(out element))
      return;
    Container container = this._container.EnsureContainer<Container>((EntityUid) element.Value, element.Value.Comp.ContainerId);
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) transfer, (BaseContainer) container);
    this.Dirty<RMCSmartFridgeComponent>(element.Value);
  }

  private void OnVend(Entity<RMCSmartFridgeComponent> ent, ref RMCSmartFridgeVendMsg args)
  {
    EntityUid? entity;
    BaseContainer container;
    if (!this.TryGetEntity(args.Vend, out entity) || !this._container.TryGetContainingContainer((Entity<TransformComponent, MetaDataComponent>) (entity.Value, (TransformComponent) null), out container) || container.Owner != ent.Owner || container.ID != ent.Comp.ContainerId || !this._container.Remove((Entity<TransformComponent, MetaDataComponent>) entity.Value, container))
      return;
    this._hands.TryPickupAnyHand(args.Actor, entity.Value);
  }
}
