// Decompiled with JetBrains decompiler
// Type: Content.Shared.Teleportation.Systems.SwapTeleporterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Examine;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Teleportation.Components;
using Content.Shared.Verbs;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Teleportation.Systems;

public sealed class SwapTeleporterSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private EntityWhitelistSystem _whitelistSystem;
  private Robust.Shared.GameObjects.EntityQuery<TransformComponent> _xformQuery;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<SwapTeleporterComponent, AfterInteractEvent>(new EntityEventRefHandler<SwapTeleporterComponent, AfterInteractEvent>(this.OnInteract));
    this.SubscribeLocalEvent<SwapTeleporterComponent, GetVerbsEvent<AlternativeVerb>>(new EntityEventRefHandler<SwapTeleporterComponent, GetVerbsEvent<AlternativeVerb>>(this.OnGetAltVerb));
    this.SubscribeLocalEvent<SwapTeleporterComponent, ActivateInWorldEvent>(new EntityEventRefHandler<SwapTeleporterComponent, ActivateInWorldEvent>(this.OnActivateInWorld));
    this.SubscribeLocalEvent<SwapTeleporterComponent, ExaminedEvent>(new EntityEventRefHandler<SwapTeleporterComponent, ExaminedEvent>(this.OnExamined));
    this.SubscribeLocalEvent<SwapTeleporterComponent, ComponentShutdown>(new EntityEventRefHandler<SwapTeleporterComponent, ComponentShutdown>(this.OnShutdown));
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
  }

  private void OnInteract(Entity<SwapTeleporterComponent> ent, ref AfterInteractEvent args)
  {
    (EntityUid entityUid, SwapTeleporterComponent comp1) = ent;
    if (!args.Target.HasValue || !args.CanReach)
      return;
    EntityUid uid = args.Target.Value;
    SwapTeleporterComponent comp2;
    if (!this.TryComp<SwapTeleporterComponent>(uid, out comp2) || this._whitelistSystem.IsWhitelistFail(comp1.TeleporterWhitelist, uid) || this._whitelistSystem.IsWhitelistFail(comp2.TeleporterWhitelist, entityUid))
      return;
    if (comp1.LinkedEnt.HasValue)
      this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-link-fail-already"), entityUid, new EntityUid?(args.User));
    else if (comp2.LinkedEnt.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-link-fail-already-other"), entityUid, new EntityUid?(args.User));
    }
    else
    {
      comp1.LinkedEnt = new EntityUid?(uid);
      comp2.LinkedEnt = new EntityUid?(entityUid);
      this.Dirty(entityUid, (IComponent) comp1);
      this.Dirty(uid, (IComponent) comp2);
      this._appearance.SetData(entityUid, (Enum) SwapTeleporterVisuals.Linked, (object) true);
      this._appearance.SetData(uid, (Enum) SwapTeleporterVisuals.Linked, (object) true);
      this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-link-create"), entityUid, new EntityUid?(args.User));
    }
  }

  private void OnGetAltVerb(
    Entity<SwapTeleporterComponent> ent,
    ref GetVerbsEvent<AlternativeVerb> args)
  {
    (EntityUid owner, SwapTeleporterComponent comp1) = ent;
    SwapTeleporterComponent comp2;
    if (!args.CanAccess || !args.CanInteract || args.Hands == null || comp1.TeleportTime.HasValue || !this.TryComp<SwapTeleporterComponent>(comp1.LinkedEnt, out comp2) || comp2.TeleportTime.HasValue)
      return;
    EntityUid user = args.User;
    SortedSet<AlternativeVerb> verbs = args.Verbs;
    AlternativeVerb alternativeVerb = new AlternativeVerb();
    alternativeVerb.Text = this.Loc.GetString("swap-teleporter-verb-destroy-link");
    alternativeVerb.Priority = 1;
    alternativeVerb.Act = (Action) (() => this.DestroyLink((Entity<SwapTeleporterComponent>) (owner, comp1), new EntityUid?(user)));
    verbs.Add(alternativeVerb);
  }

  private void OnActivateInWorld(Entity<SwapTeleporterComponent> ent, ref ActivateInWorldEvent args)
  {
    if (args.Handled || !args.Complex)
      return;
    (EntityUid entityUid, SwapTeleporterComponent comp1) = ent;
    EntityUid user = args.User;
    if (comp1.TeleportTime.HasValue)
      return;
    if (!comp1.LinkedEnt.HasValue)
    {
      this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-teleport-cancel-link"), (EntityUid) ent, new EntityUid?(user));
    }
    else
    {
      SwapTeleporterComponent comp2;
      if (!this.TryComp<SwapTeleporterComponent>(comp1.LinkedEnt, out comp2) || comp2.TeleportTime.HasValue)
        return;
      if (this._timing.CurTime < comp1.NextTeleportUse)
      {
        this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-teleport-cancel-time"), (EntityUid) ent, new EntityUid?(user));
      }
      else
      {
        this._audio.PlayPredicted(comp1.TeleportSound, entityUid, new EntityUid?(user));
        this._audio.PlayPredicted(comp2.TeleportSound, comp1.LinkedEnt.Value, new EntityUid?(user));
        comp1.NextTeleportUse = this._timing.CurTime + comp1.Cooldown;
        comp1.TeleportTime = new TimeSpan?(this._timing.CurTime + comp1.TeleportDelay);
        this.Dirty(entityUid, (IComponent) comp1);
        args.Handled = true;
      }
    }
  }

  public void DoTeleport(
    Entity<SwapTeleporterComponent, TransformComponent> ent)
  {
    (EntityUid entityUid, SwapTeleporterComponent comp1, TransformComponent comp2) = ent;
    comp1.TeleportTime = new TimeSpan?();
    this.Dirty(entityUid, (IComponent) comp1);
    if (this._net.IsClient)
      return;
    EntityUid? nullable = comp1.LinkedEnt;
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault = nullable.GetValueOrDefault();
    EntityUid teleportingEntity1 = this.GetTeleportingEntity((Entity<TransformComponent>) (entityUid, comp2));
    EntityUid teleportingEntity2 = this.GetTeleportingEntity((Entity<TransformComponent>) (valueOrDefault, this.Transform(valueOrDefault)));
    TransformComponent transformComponent1 = this.Transform(teleportingEntity1);
    TransformComponent transformComponent2 = this.Transform(teleportingEntity2);
    if (!this.CanSwapTeleport((Entity<TransformComponent>) (teleportingEntity1, transformComponent1), (Entity<TransformComponent>) (teleportingEntity2, transformComponent2)))
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      EntityUid uid1 = valueOrDefault;
      EntityManager entityManager = this.EntityManager;
      nullable = new EntityUid?();
      EntityUid? viewer = nullable;
      (string, object) valueTuple = ("entity", (object) Identity.Entity(uid1, (IEntityManager) entityManager, viewer));
      string message = loc.GetString("swap-teleporter-popup-teleport-fail", valueTuple);
      EntityUid uid2 = teleportingEntity1;
      EntityUid recipient = teleportingEntity1;
      popup.PopupEntity(message, uid2, recipient, PopupType.MediumCaution);
    }
    else
    {
      SharedPopupSystem popup = this._popup;
      ILocalizationManager loc = this.Loc;
      EntityUid uid3 = valueOrDefault;
      EntityManager entityManager = this.EntityManager;
      nullable = new EntityUid?();
      EntityUid? viewer = nullable;
      (string, object) valueTuple = ("entity", (object) Identity.Entity(uid3, (IEntityManager) entityManager, viewer));
      string message = loc.GetString("swap-teleporter-popup-teleport-other", valueTuple);
      EntityUid uid4 = teleportingEntity1;
      EntityUid? recipient = new EntityUid?(teleportingEntity2);
      popup.PopupClient(message, uid4, recipient, PopupType.MediumCaution);
      this._transform.SwapPositions((Entity<TransformComponent>) teleportingEntity1, (Entity<TransformComponent>) teleportingEntity2);
    }
  }

  private bool CanSwapTeleport(
    Entity<TransformComponent> entity1,
    Entity<TransformComponent> entity2)
  {
    BaseContainer container1;
    this._container.TryGetOuterContainer((EntityUid) entity1, (TransformComponent) entity1, out container1);
    BaseContainer container2;
    this._container.TryGetOuterContainer((EntityUid) entity2, (TransformComponent) entity2, out container2);
    return (container2 == null || this._container.CanInsert((EntityUid) entity1, container2)) && (container1 == null || this._container.CanInsert((EntityUid) entity2, container1)) && !this.IsPaused(new EntityUid?((EntityUid) entity1)) && !this.IsPaused(new EntityUid?((EntityUid) entity2));
  }

  public void DestroyLink(Entity<SwapTeleporterComponent?> ent, EntityUid? user)
  {
    if (!this.Resolve<SwapTeleporterComponent>((EntityUid) ent, ref ent.Comp, false))
      return;
    EntityUid? linkedEnt = ent.Comp.LinkedEnt;
    ent.Comp.LinkedEnt = new EntityUid?();
    ent.Comp.TeleportTime = new TimeSpan?();
    this._appearance.SetData((EntityUid) ent, (Enum) SwapTeleporterVisuals.Linked, (object) false);
    this.Dirty((EntityUid) ent, (IComponent) ent.Comp);
    if (user.HasValue)
      this._popup.PopupClient(this.Loc.GetString("swap-teleporter-popup-link-destroyed"), (EntityUid) ent, new EntityUid?(user.Value));
    else
      this._popup.PopupEntity(this.Loc.GetString("swap-teleporter-popup-link-destroyed"), (EntityUid) ent);
    if (!linkedEnt.HasValue)
      return;
    this.DestroyLink((Entity<SwapTeleporterComponent>) linkedEnt.GetValueOrDefault(), user);
  }

  private EntityUid GetTeleportingEntity(Entity<TransformComponent> ent)
  {
    EntityUid parentUid = ent.Comp.ParentUid;
    if (this.HasComp<MapGridComponent>(parentUid) || this.HasComp<MapComponent>(parentUid))
      return (EntityUid) ent;
    TransformComponent component;
    if (!this._xformQuery.TryGetComponent(parentUid, out component) || component.Anchored)
      return (EntityUid) ent;
    PhysicsComponent comp;
    return !this.TryComp<PhysicsComponent>(parentUid, out comp) || comp.BodyType == BodyType.Static ? (EntityUid) ent : this.GetTeleportingEntity((Entity<TransformComponent>) (parentUid, component));
  }

  private void OnExamined(Entity<SwapTeleporterComponent> ent, ref ExaminedEvent args)
  {
    (EntityUid _, SwapTeleporterComponent comp) = ent;
    using (args.PushGroup("SwapTeleporterComponent"))
    {
      string messageId = !comp.LinkedEnt.HasValue ? "swap-teleporter-examine-link-absent" : "swap-teleporter-examine-link-present";
      args.PushMarkup(this.Loc.GetString(messageId));
      if (!(this._timing.CurTime < comp.NextTeleportUse))
        return;
      args.PushMarkup(this.Loc.GetString("swap-teleporter-examine-time-remaining", ("second", (object) (int) ((comp.NextTeleportUse - this._timing.CurTime).TotalSeconds + 0.5))));
    }
  }

  private void OnShutdown(Entity<SwapTeleporterComponent> ent, ref ComponentShutdown args)
  {
    this.DestroyLink((Entity<SwapTeleporterComponent>) ((EntityUid) ent, (SwapTeleporterComponent) ent), new EntityUid?());
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    Robust.Shared.GameObjects.EntityQueryEnumerator<SwapTeleporterComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<SwapTeleporterComponent, TransformComponent>();
    EntityUid uid;
    SwapTeleporterComponent comp1;
    TransformComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (comp1.TeleportTime.HasValue)
      {
        TimeSpan curTime = this._timing.CurTime;
        TimeSpan? teleportTime = comp1.TeleportTime;
        if ((teleportTime.HasValue ? (curTime < teleportTime.GetValueOrDefault() ? 1 : 0) : 0) == 0)
          this.DoTeleport((Entity<SwapTeleporterComponent, TransformComponent>) (uid, comp1, comp2));
      }
    }
  }
}
