// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.Chamber.RMCGunChamberSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared._RMC14.Weapons.Common;
using Content.Shared._RMC14.Weapons.Ranged.AimedShot;
using Content.Shared._RMC14.Weapons.Ranged.Flamer;
using Content.Shared._RMC14.Weapons.Ranged.Foldable;
using Content.Shared.Weapons.Ranged;
using Content.Shared.Weapons.Ranged.Events;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.Chamber;

public sealed class RMCGunChamberSystem : EntitySystem
{
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedGunSystem _gun;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<RMCGunChamberComponent, EntRemovedFromContainerMessage>(new EntityEventRefHandler<RMCGunChamberComponent, EntRemovedFromContainerMessage>(this.OnEntRemovedFromContainer), after: new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<RMCGunChamberComponent, TakeAmmoEvent>(new EntityEventRefHandler<RMCGunChamberComponent, TakeAmmoEvent>(this.OnTakeAmmo), new Type[1]
    {
      typeof (SharedGunSystem)
    });
    this.SubscribeLocalEvent<RMCGunChamberComponent, UniqueActionEvent>(new EntityEventRefHandler<RMCGunChamberComponent, UniqueActionEvent>(this.OnUniqueAction), after: new Type[8]
    {
      typeof (SharedRMCAimedShotSystem),
      typeof (AttachableHolderSystem),
      typeof (SharedRMCFlamerSystem),
      typeof (RMCFoldableGunSystem),
      typeof (BreechLoadedSystem),
      typeof (CMGunSystem),
      typeof (RMCAirShotSystem),
      typeof (SharedPumpActionSystem)
    });
  }

  private void OnEntRemovedFromContainer(
    Entity<RMCGunChamberComponent> ent,
    ref EntRemovedFromContainerMessage args)
  {
    if (!ent.Comp.Enabled || args.Container.ID != "gun_magazine")
      return;
    this.LoadChamber(ent, args.Entity);
  }

  private void OnTakeAmmo(Entity<RMCGunChamberComponent> ent, ref TakeAmmoEvent args)
  {
    BaseContainer container;
    if (!ent.Comp.Enabled || !this._container.TryGetContainer((EntityUid) ent, ent.Comp.ContainerId, out container))
      return;
    int shots = args.Shots;
    for (int index = 0; index < shots; ++index)
    {
      EntityUid? nullable = container.ContainedEntities.FirstOrNull<EntityUid>();
      if (!nullable.HasValue)
        break;
      EntityUid valueOrDefault = nullable.GetValueOrDefault();
      --args.Shots;
      if (!this._container.Remove((Entity<TransformComponent, MetaDataComponent>) valueOrDefault, container))
        break;
      args.Ammo.Add((new EntityUid?(valueOrDefault), this._gun.EnsureShootable(valueOrDefault)));
    }
  }

  private void LoadChamber(Entity<RMCGunChamberComponent> gun, EntityUid magazine)
  {
    TransformComponent comp;
    if (this.TerminatingOrDeleted((EntityUid) gun) || !this.TryComp((EntityUid) gun, out comp))
      return;
    ContainerSlot container = this._container.EnsureContainer<ContainerSlot>((EntityUid) gun, gun.Comp.ContainerId);
    EntityUid? nullable1 = container.ContainedEntity;
    if (nullable1.HasValue || this._net.IsClient)
      return;
    List<(EntityUid?, IShootable)> ammo = new List<(EntityUid?, IShootable)>();
    EntityCoordinates coordinates = comp.Coordinates;
    nullable1 = new EntityUid?();
    EntityUid? user = nullable1;
    TakeAmmoEvent args = new TakeAmmoEvent(1, ammo, coordinates, user);
    this.RaiseLocalEvent<TakeAmmoEvent>(magazine, args);
    (EntityUid? Entity, IShootable Shootable)? nullable2 = args.Ammo.FirstOrNull<(EntityUid?, IShootable)>();
    if (!nullable2.HasValue)
      return;
    nullable1 = nullable2.GetValueOrDefault().Entity;
    if (!nullable1.HasValue)
      return;
    this._container.Insert((Entity<TransformComponent, MetaDataComponent, PhysicsComponent>) nullable1.GetValueOrDefault(), (BaseContainer) container);
  }

  private void OnUniqueAction(Entity<RMCGunChamberComponent> ent, ref UniqueActionEvent args)
  {
    TransformComponent comp;
    if (!ent.Comp.Enabled || args.Handled || !this.TryComp((EntityUid) ent, out comp))
      return;
    args.Handled = true;
    TimeSpan curTime = this._timing.CurTime;
    TimeSpan? lastChamberedAt = ent.Comp.LastChamberedAt;
    if (lastChamberedAt.HasValue)
    {
      TimeSpan valueOrDefault = lastChamberedAt.GetValueOrDefault();
      if (curTime < valueOrDefault + ent.Comp.ChamberCooldown)
        return;
    }
    TakeAmmoEvent args1 = new TakeAmmoEvent(1, new List<(EntityUid?, IShootable)>(), comp.Coordinates, new EntityUid?());
    this.RaiseLocalEvent<TakeAmmoEvent>((EntityUid) ent, args1);
    if (args1.Ammo.Count == 0)
      return;
    foreach ((EntityUid? Entity, IShootable _) in args1.Ammo)
    {
      if (Entity.HasValue)
      {
        EntityUid valueOrDefault = Entity.GetValueOrDefault();
        this._transform.SetCoordinates(valueOrDefault, this._transform.GetMoverCoordinates(valueOrDefault));
        if (this.IsClientSide(valueOrDefault))
          this.QueueDel(new EntityUid?(valueOrDefault));
      }
    }
    ent.Comp.LastChamberedAt = new TimeSpan?(curTime);
    this.Dirty<RMCGunChamberComponent>(ent);
    this._audio.PlayPredicted(ent.Comp.Sound, (EntityUid) ent, new EntityUid?(args.UserUid));
    this._gun.UpdateAmmoCount((EntityUid) ent);
  }
}
