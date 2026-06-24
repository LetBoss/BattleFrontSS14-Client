// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.IFF.GunIFFSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Attachable.Systems;
using Content.Shared.Examine;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Physics.Events;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged.IFF;

public sealed class GunIFFSystem : EntitySystem
{
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedHandsSystem _hands;
  [Dependency]
  private InventorySystem _inventory;
  private Robust.Shared.GameObjects.EntityQuery<UserIFFComponent> _userIFFQuery;
  private readonly HashSet<EntProtoId<IFFFactionComponent>> _factionBuffer = new HashSet<EntProtoId<IFFFactionComponent>>();

  public override void Initialize()
  {
    this._userIFFQuery = this.GetEntityQuery<UserIFFComponent>();
    this.SubscribeLocalEvent<UserIFFComponent, GetIFFFactionEvent>(new EntityEventRefHandler<UserIFFComponent, GetIFFFactionEvent>(this.OnUserIFFGetFaction));
    this.SubscribeLocalEvent<InventoryComponent, GetIFFFactionEvent>(new EntityEventRefHandler<InventoryComponent, GetIFFFactionEvent>(this.OnInventoryIFFGetFaction));
    this.SubscribeLocalEvent<HandsComponent, GetIFFFactionEvent>(new EntityEventRefHandler<HandsComponent, GetIFFFactionEvent>(this.OnHandsIFFGetFaction));
    this.SubscribeLocalEvent<ItemIFFComponent, InventoryRelayedEvent<GetIFFFactionEvent>>(new EntityEventRefHandler<ItemIFFComponent, InventoryRelayedEvent<GetIFFFactionEvent>>(this.OnItemIFFGetFaction));
    this.SubscribeLocalEvent<GunIFFComponent, AmmoShotEvent>(new EntityEventRefHandler<GunIFFComponent, AmmoShotEvent>(this.OnGunIFFAmmoShot), new Type[1]
    {
      typeof (AttachableIFFSystem)
    });
    this.SubscribeLocalEvent<GunIFFComponent, ExaminedEvent>(new EntityEventRefHandler<GunIFFComponent, ExaminedEvent>(this.OnGunIFFExamined));
    this.SubscribeLocalEvent<ProjectileIFFComponent, PreventCollideEvent>(new EntityEventRefHandler<ProjectileIFFComponent, PreventCollideEvent>(this.OnProjectileIFFPreventCollide));
  }

  private void OnUserIFFGetFaction(Entity<UserIFFComponent> ent, ref GetIFFFactionEvent args)
  {
    args.Factions.UnionWith((IEnumerable<EntProtoId<IFFFactionComponent>>) ent.Comp.Factions);
  }

  private void OnInventoryIFFGetFaction(Entity<InventoryComponent> ent, ref GetIFFFactionEvent args)
  {
    this._inventory.RelayEvent<GetIFFFactionEvent>(ent, ref args);
  }

  private void OnHandsIFFGetFaction(Entity<HandsComponent> ent, ref GetIFFFactionEvent args)
  {
    foreach (EntityUid uid in this._hands.EnumerateHeld((Entity<HandsComponent>) ((EntityUid) ent, (HandsComponent) ent)))
      this.RaiseLocalEvent<GetIFFFactionEvent>(uid, ref args);
  }

  private void OnItemIFFGetFaction(
    Entity<ItemIFFComponent> ent,
    ref InventoryRelayedEvent<GetIFFFactionEvent> args)
  {
    if (ent.Comp.Factions.Count <= 0)
      return;
    args.Args.Factions.UnionWith((IEnumerable<EntProtoId<IFFFactionComponent>>) ent.Comp.Factions);
  }

  private void OnGunIFFAmmoShot(Entity<GunIFFComponent> ent, ref AmmoShotEvent args)
  {
    this.GiveAmmoIFF(ent.Owner, ref args, ent.Comp.Intrinsic, ent.Comp.Enabled);
  }

  private void OnGunIFFExamined(Entity<GunIFFComponent> ent, ref ExaminedEvent args)
  {
    if (!ent.Comp.Enabled)
      return;
    using (args.PushGroup("GunIFFComponent"))
      args.PushMarkup(this.Loc.GetString("rmc-examine-text-iff"));
  }

  private void OnProjectileIFFPreventCollide(
    Entity<ProjectileIFFComponent> ent,
    ref PreventCollideEvent args)
  {
    if (args.Cancelled)
      return;
    foreach (EntProtoId<IFFFactionComponent> faction in ent.Comp.Factions)
    {
      if (this.HasComp<EntityIFFComponent>(args.OtherEntity) && this.IsInFaction(args.OtherEntity, faction))
      {
        args.Cancelled = true;
        break;
      }
      if (ent.Comp.Enabled && this.IsInFaction(args.OtherEntity, faction))
      {
        args.Cancelled = true;
        break;
      }
    }
  }

  public bool TryGetUserFaction(
    Entity<UserIFFComponent?> user,
    out EntProtoId<IFFFactionComponent> faction)
  {
    faction = new EntProtoId<IFFFactionComponent>();
    if (!this._userIFFQuery.Resolve((EntityUid) user, ref user.Comp, false) || user.Comp.Factions.Count == 0)
      return false;
    faction = user.Comp.Factions.First<EntProtoId<IFFFactionComponent>>();
    return true;
  }

  public bool TryGetFaction(
    Entity<UserIFFComponent?> user,
    out EntProtoId<IFFFactionComponent> faction,
    SlotFlags slots = SlotFlags.IDCARD)
  {
    faction = new EntProtoId<IFFFactionComponent>();
    HashSet<EntProtoId<IFFFactionComponent>> entProtoIdSet = new HashSet<EntProtoId<IFFFactionComponent>>();
    if (!this.TryGetFactions(user, entProtoIdSet, slots))
      return false;
    faction = entProtoIdSet.First<EntProtoId<IFFFactionComponent>>();
    return true;
  }

  public bool TryGetFactions(
    Entity<UserIFFComponent?> user,
    HashSet<EntProtoId<IFFFactionComponent>> factions,
    SlotFlags slots = SlotFlags.IDCARD)
  {
    factions.Clear();
    if (!this._userIFFQuery.Resolve((EntityUid) user, ref user.Comp, false))
      return false;
    factions.UnionWith((IEnumerable<EntProtoId<IFFFactionComponent>>) user.Comp.Factions);
    GetIFFFactionEvent args = new GetIFFFactionEvent(slots, new HashSet<EntProtoId<IFFFactionComponent>>());
    this.RaiseLocalEvent<GetIFFFactionEvent>((EntityUid) user, ref args);
    factions.UnionWith((IEnumerable<EntProtoId<IFFFactionComponent>>) args.Factions);
    return factions.Count != 0;
  }

  public bool IsInFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
  {
    EntityUid owner = user.Owner;
    if (this._userIFFQuery.Resolve((EntityUid) user, ref user.Comp, false) && user.Comp.Factions.Count > 0 && user.Comp.Factions.Contains(faction))
      return true;
    GetIFFFactionEvent args = new GetIFFFactionEvent(SlotFlags.IDCARD, new HashSet<EntProtoId<IFFFactionComponent>>());
    this.RaiseLocalEvent<GetIFFFactionEvent>(owner, ref args);
    return args.Factions.Count > 0 && args.Factions.Contains(faction);
  }

  public bool IsInFaction(EntityUid uid, EntProtoId<IFFFactionComponent> faction)
  {
    return this.IsInFaction((Entity<UserIFFComponent>) (uid, (UserIFFComponent) null), faction);
  }

  public void SetIdFaction(Entity<ItemIFFComponent> card, EntProtoId<IFFFactionComponent> faction)
  {
    card.Comp.Factions.Clear();
    card.Comp.Factions.Add(faction);
    this.Dirty<ItemIFFComponent>(card);
  }

  public void SetUserFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
  {
    user.Comp = this.EnsureComp<UserIFFComponent>((EntityUid) user);
    user.Comp.Factions.Clear();
    user.Comp.Factions.Add(faction);
    this.Dirty<UserIFFComponent>(user);
  }

  public void AddUserFaction(Entity<UserIFFComponent?> user, EntProtoId<IFFFactionComponent> faction)
  {
    user.Comp = this.EnsureComp<UserIFFComponent>((EntityUid) user);
    user.Comp.Factions.Add(faction);
    this.Dirty<UserIFFComponent>(user);
  }

  public void RemoveUserFaction(
    Entity<UserIFFComponent?> user,
    EntProtoId<IFFFactionComponent> faction)
  {
    if (!this._userIFFQuery.Resolve((EntityUid) user, ref user.Comp, false))
      return;
    user.Comp.Factions.Remove(faction);
    this.Dirty<UserIFFComponent>(user);
  }

  public void ClearUserFactions(Entity<UserIFFComponent?> user)
  {
    user.Comp = this.EnsureComp<UserIFFComponent>((EntityUid) user);
    user.Comp.Factions.Clear();
    this.Dirty<UserIFFComponent>(user);
  }

  public void SetIFFState(EntityUid ent, bool enabled)
  {
    GunIFFComponent comp;
    if (!this.TryComp<GunIFFComponent>(ent, out comp))
      return;
    comp.Enabled = enabled;
    this.Dirty(ent, (IComponent) comp);
  }

  public void EnableIntrinsicIFF(EntityUid ent)
  {
    GunIFFComponent gunIffComponent = this.EnsureComp<GunIFFComponent>(ent);
    gunIffComponent.Intrinsic = true;
    gunIffComponent.Enabled = true;
    this.Dirty(ent, (IComponent) gunIffComponent);
  }

  public void GiveAmmoIFF(EntityUid gun, ref AmmoShotEvent args, bool intrinsic, bool enabled)
  {
    EntityUid uid;
    if (intrinsic)
    {
      uid = gun;
    }
    else
    {
      BaseContainer container;
      if (!this._container.TryGetOuterContainer(gun, this.Transform(gun), out container))
        return;
      uid = container.Owner;
      GetIFFGunUserEvent args1 = new GetIFFGunUserEvent();
      this.RaiseLocalEvent<GetIFFGunUserEvent>(container.Owner, ref args1);
      if (args1.GunUser.HasValue)
        uid = args1.GunUser.Value;
    }
    UserIFFComponent component;
    if (!this._userIFFQuery.TryComp(uid, out component))
      return;
    this._factionBuffer.Clear();
    if (!this.TryGetFactions((Entity<UserIFFComponent>) (uid, component), this._factionBuffer))
      return;
    bool flag = enabled && this._factionBuffer.Count > 0;
    foreach (EntityUid firedProjectile in args.FiredProjectiles)
    {
      ProjectileIFFComponent projectileIffComponent = this.EnsureComp<ProjectileIFFComponent>(firedProjectile);
      projectileIffComponent.Factions.Clear();
      foreach (EntProtoId<IFFFactionComponent> entProtoId in this._factionBuffer)
        projectileIffComponent.Factions.Add(entProtoId);
      projectileIffComponent.Enabled = flag;
      this.Dirty(firedProjectile, (IComponent) projectileIffComponent);
    }
  }

  public void GiveAmmoIFF(EntityUid uid, EntProtoId<IFFFactionComponent>? faction, bool enabled)
  {
    ProjectileIFFComponent projectileIffComponent = this.EnsureComp<ProjectileIFFComponent>(uid);
    projectileIffComponent.Factions.Clear();
    if (faction.HasValue)
    {
      EntProtoId<IFFFactionComponent> valueOrDefault = faction.GetValueOrDefault();
      projectileIffComponent.Factions.Add(valueOrDefault);
    }
    projectileIffComponent.Enabled = enabled && projectileIffComponent.Factions.Count > 0;
    this.Dirty(uid, (IComponent) projectileIffComponent);
  }

  public void GiveAmmoMultiFactionIFF(
    EntityUid uid,
    HashSet<EntProtoId<IFFFactionComponent>> factions,
    bool enabled)
  {
    ProjectileIFFComponent projectileIffComponent = this.EnsureComp<ProjectileIFFComponent>(uid);
    projectileIffComponent.Factions.Clear();
    foreach (EntProtoId<IFFFactionComponent> faction in factions)
      projectileIffComponent.Factions.Add(faction);
    projectileIffComponent.Enabled = enabled && projectileIffComponent.Factions.Count > 0;
    this.Dirty(uid, (IComponent) projectileIffComponent);
  }
}
