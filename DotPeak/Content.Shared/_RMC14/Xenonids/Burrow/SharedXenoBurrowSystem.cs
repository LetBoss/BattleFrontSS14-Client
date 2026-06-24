// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Burrow.SharedXenoBurrowSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.Atmos;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Xenonids.Rest;
using Content.Shared.ActionBlocker;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.StatusEffectNew;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Burrow;

public abstract class SharedXenoBurrowSystem : EntitySystem
{
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private EntityManager _entities;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private IGameTiming _time;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private RMCPullingSystem _rmcPulling;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoBurrowComponent, ExamineAttemptEvent>(new EntityEventRefHandler<XenoBurrowComponent, ExamineAttemptEvent>(this.PreventExamine));
    this.SubscribeLocalEvent<XenoBurrowComponent, BeforeStatusEffectAddedEvent>(new EntityEventRefHandler<XenoBurrowComponent, BeforeStatusEffectAddedEvent>(this.PreventEffects));
    this.SubscribeLocalEvent<XenoBurrowComponent, BeforeDamageChangedEvent>(new EntityEventRefHandler<XenoBurrowComponent, BeforeDamageChangedEvent>(this.PreventDamage));
    this.SubscribeLocalEvent<XenoBurrowComponent, PreventCollideEvent>(new EntityEventRefHandler<XenoBurrowComponent, PreventCollideEvent>(this.PreventCollision));
    this.SubscribeLocalEvent<XenoBurrowComponent, InteractionAttemptEvent>(new ComponentEventRefHandler<XenoBurrowComponent, InteractionAttemptEvent>(this.PreventInteraction));
    this.SubscribeLocalEvent<XenoBurrowComponent, RMCIgniteAttemptEvent>(new EntityEventRefHandler<XenoBurrowComponent, RMCIgniteAttemptEvent>(this.OnBurrowedCancel<RMCIgniteAttemptEvent>));
    this.SubscribeLocalEvent<XenoBurrowComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoBurrowComponent, AttackAttemptEvent>(this.OnBurrowedCancel<AttackAttemptEvent>));
    this.SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowActionEvent>(new EntityEventRefHandler<XenoBurrowComponent, XenoBurrowActionEvent>(this.OnBeginBurrow));
    this.SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowDownDoAfter>(new EntityEventRefHandler<XenoBurrowComponent, XenoBurrowDownDoAfter>(this.OnFinishBurrow));
    this.SubscribeLocalEvent<XenoBurrowComponent, BurrowedEvent>(new EntityEventRefHandler<XenoBurrowComponent, BurrowedEvent>(this.SetBurrow));
    this.SubscribeLocalEvent<XenoBurrowComponent, XenoBurrowMoveDoAfter>(new EntityEventRefHandler<XenoBurrowComponent, XenoBurrowMoveDoAfter>(this.OnFinishTunnel));
  }

  private void PreventExamine(Entity<XenoBurrowComponent> burrower, ref ExamineAttemptEvent args)
  {
    if (args.Cancelled || !burrower.Comp.Active || this.HasComp<XenoComponent>(args.Examiner))
      return;
    args.Cancel();
  }

  private void PreventEffects(
    Entity<XenoBurrowComponent> burrower,
    ref BeforeStatusEffectAddedEvent args)
  {
    if (args.Cancelled || !burrower.Comp.Active)
      return;
    args.Cancelled = true;
  }

  private void OnBurrowedCancel<T>(Entity<XenoBurrowComponent> burrower, ref T args) where T : CancellableEntityEventArgs
  {
    if (args.Cancelled || !burrower.Comp.Active)
      return;
    args.Cancel();
  }

  private void PreventDamage(
    Entity<XenoBurrowComponent> burrower,
    ref BeforeDamageChangedEvent args)
  {
    if (args.Cancelled || !burrower.Comp.Active)
      return;
    args.Cancelled = true;
  }

  private void PreventCollision(Entity<XenoBurrowComponent> burrower, ref PreventCollideEvent args)
  {
    if (args.Cancelled || !burrower.Comp.Active)
      return;
    args.Cancelled = true;
  }

  private void PreventInteraction(
    EntityUid ent,
    XenoBurrowComponent comp,
    ref InteractionAttemptEvent args)
  {
    if (args.Cancelled || !comp.Active)
      return;
    args.Cancelled = true;
  }

  private void SetBurrow(Entity<XenoBurrowComponent> burrower, ref BurrowedEvent args)
  {
    if (args.burrowed == burrower.Comp.Active)
      return;
    burrower.Comp.Active = args.burrowed;
    this._actionBlocker.UpdateCanMove((EntityUid) burrower);
    if (args.burrowed)
    {
      this._transform.AnchorEntity((EntityUid) burrower);
    }
    else
    {
      this._transform.Unanchor((EntityUid) burrower);
      PhysicsComponent comp;
      if (this.TryComp<PhysicsComponent>((EntityUid) burrower, out comp))
      {
        this._physics.TrySetBodyType((EntityUid) burrower, BodyType.KinematicController, body: comp);
        this.Dirty((EntityUid) burrower, (IComponent) comp);
      }
      if (this._net.IsServer)
      {
        this._audio.PlayPvs(burrower.Comp.BurrowUpSound, (EntityUid) burrower);
        foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInRange((EntityUid) burrower, burrower.Comp.UnburrowStunRange))
        {
          if (this._xeno.CanAbilityAttackTarget((EntityUid) burrower, entityUid))
            this._stun.TryParalyze(entityUid, burrower.Comp.UnburrowStunLength, false);
        }
      }
      this.Dirty<XenoBurrowComponent>(burrower);
    }
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._time.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoBurrowComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoBurrowComponent>();
    EntityUid uid;
    XenoBurrowComponent comp1;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1))
    {
      TimeSpan? nullable = comp1.NextBurrowAt;
      TimeSpan timeSpan1 = curTime;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() < timeSpan1 ? 1 : 0) : 0) != 0)
      {
        if (!comp1.Active)
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-burrow-cooldown-finish"), uid, uid);
        comp1.NextBurrowAt = new TimeSpan?();
      }
      nullable = comp1.NextTunnelAt;
      TimeSpan timeSpan2 = curTime;
      if ((nullable.HasValue ? (nullable.GetValueOrDefault() < timeSpan2 ? 1 : 0) : 0) != 0)
      {
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-burrow-move-cooldown-finish"), uid, uid);
        comp1.NextTunnelAt = new TimeSpan?();
      }
      if (!comp1.Tunneling)
      {
        nullable = comp1.ForcedUnburrowAt;
        TimeSpan timeSpan3 = curTime;
        if ((nullable.HasValue ? (nullable.GetValueOrDefault() < timeSpan3 ? 1 : 0) : 0) != 0)
        {
          BurrowedEvent args = new BurrowedEvent(false);
          this.RaiseLocalEvent<BurrowedEvent>(uid, ref args);
          comp1.ForcedUnburrowAt = new TimeSpan?();
          this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-burrow-move-forced-unburrow"), uid, uid, PopupType.MediumCaution);
          comp1.NextBurrowAt = new TimeSpan?(curTime + comp1.BurrowCooldown);
        }
      }
    }
  }

  private void OnBeginBurrow(Entity<XenoBurrowComponent> burrower, ref XenoBurrowActionEvent args)
  {
    if (args.Handled)
      return;
    if (burrower.Comp.Active)
    {
      EntityCoordinates target = args.Target;
      float? distance;
      if (!this.CanTunnelPopup(burrower, target, out distance))
        return;
      TimeSpan delay = new TimeSpan(0, 0, (int) distance.Value);
      if (delay < burrower.Comp.MinimumTunnelTime)
        delay = burrower.Comp.MinimumTunnelTime;
      XenoBurrowMoveDoAfter @event = new XenoBurrowMoveDoAfter(this._entities.GetNetCoordinates(target, (MetaDataComponent) null));
      if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this._entities, (EntityUid) burrower, delay, (DoAfterEvent) @event, new EntityUid?((EntityUid) burrower))
      {
        RequireCanInteract = false,
        DuplicateCondition = DuplicateConditions.SameEvent
      }))
      {
        burrower.Comp.Tunneling = true;
        this.Dirty<XenoBurrowComponent>(burrower);
      }
      this.Dirty<XenoBurrowComponent>(burrower);
      if (!this._net.IsServer)
        return;
      this._audio.PlayPvs(burrower.Comp.BurrowDownSound, (EntityUid) burrower);
    }
    else
    {
      DoAfterComponent comp;
      if (this.TryComp<DoAfterComponent>((EntityUid) burrower, out comp))
      {
        foreach (KeyValuePair<ushort, Content.Shared.DoAfter.DoAfter> doAfter in comp.DoAfters)
        {
          if (!doAfter.Value.Cancelled && !doAfter.Value.Completed)
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-doafter-stop"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower), PopupType.SmallCaution);
            return;
          }
        }
      }
      if (!this.CanBurrowPopup(burrower))
        return;
      XenoBurrowDownDoAfter @event = new XenoBurrowDownDoAfter();
      if (!this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this._entities, (EntityUid) burrower, burrower.Comp.BurrowLength, (DoAfterEvent) @event, new EntityUid?((EntityUid) burrower))
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent,
        CancelDuplicate = true
      }))
        return;
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-start"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower));
    }
  }

  private void OnFinishBurrow(Entity<XenoBurrowComponent> burrower, ref XenoBurrowDownDoAfter args)
  {
    if (args.Handled)
      return;
    if (args.Cancelled)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-failure-break"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower));
      burrower.Comp.NextBurrowAt = new TimeSpan?(this._time.CurTime + burrower.Comp.BurrowCooldown);
    }
    else if (this.HasComp<XenoRestingComponent>((EntityUid) burrower))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-failure-rest"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower));
    }
    else
    {
      burrower.Comp.ForcedUnburrowAt = new TimeSpan?(this._time.CurTime + burrower.Comp.BurrowMaxDuration);
      burrower.Comp.NextBurrowAt = new TimeSpan?(this._time.CurTime + burrower.Comp.BurrowCooldown);
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) burrower);
      this.Dirty<XenoBurrowComponent>(burrower);
      BurrowedEvent args1 = new BurrowedEvent(true);
      this.RaiseLocalEvent<BurrowedEvent>((EntityUid) burrower, ref args1);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-finish"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower));
    }
  }

  private bool CanBurrowPopup(Entity<XenoBurrowComponent> ent)
  {
    TimeSpan? nextBurrowAt = ent.Comp.NextBurrowAt;
    TimeSpan curTime = this._time.CurTime;
    if ((nextBurrowAt.HasValue ? (nextBurrowAt.GetValueOrDefault() > curTime ? 1 : 0) : 0) != 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-failure-cooldown"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      return false;
    }
    EntityCoordinates grid1 = this._transform.GetMoverCoordinates(ent.Owner).SnapToGrid();
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea(grid1, out area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-down-failure-bad-area"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      return false;
    }
    EntityUid? grid2 = this._transform.GetGrid((Entity<TransformComponent>) ent.Owner);
    MapGridComponent comp;
    if (this.TryComp<MapGridComponent>(grid2, out comp))
    {
      if (!this._turf.IsSpace(this._map.GetTileRef(grid2.Value, comp, grid1)))
        return true;
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-failure-space"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      return false;
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-failure-space"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    return false;
  }

  private bool CanTunnelPopup(
    Entity<XenoBurrowComponent> ent,
    EntityCoordinates target,
    [NotNullWhen(true)] out float? distance)
  {
    distance = new float?();
    TimeSpan? nextTunnelAt = ent.Comp.NextTunnelAt;
    TimeSpan curTime = this._time.CurTime;
    if ((nextTunnelAt.HasValue ? (nextTunnelAt.GetValueOrDefault() > curTime ? 1 : 0) : 0) != 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-failure-coolown"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      return false;
    }
    Entity<AreaComponent>? area;
    if (!this._area.TryGetArea(target, out area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-failure-bad-area"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      return false;
    }
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) ent.Owner);
    MapGridComponent comp;
    if (this.TryComp<MapGridComponent>(grid, out comp))
    {
      TileRef tileRef = this._map.GetTileRef(grid.Value, comp, target);
      if (this._turf.IsSpace(tileRef))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-failure-space"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
        return false;
      }
      if (this._turf.IsTileBlocked(tileRef, CollisionGroup.Impassable))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-failure-solid"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
        return false;
      }
      float distance1;
      if (!target.TryDistance((IEntityManager) this._entities, ent.Owner.ToCoordinates(), out distance1))
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-failure"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
        return false;
      }
      float? nullable = distance;
      float tunnelingDistance = ent.Comp.MaxTunnelingDistance;
      if ((double) nullable.GetValueOrDefault() > (double) tunnelingDistance & nullable.HasValue)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-failure"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
        return false;
      }
      if (!ent.Comp.Tunneling)
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-start"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      else
        this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-break"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
      distance = new float?(distance1);
      return true;
    }
    this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-failure-space"), (EntityUid) ent, new EntityUid?((EntityUid) ent));
    return false;
  }

  private void OnFinishTunnel(Entity<XenoBurrowComponent> burrower, ref XenoBurrowMoveDoAfter args)
  {
    if (args.Handled || args.Cancelled)
    {
      burrower.Comp.Tunneling = false;
      burrower.Comp.NextTunnelAt = new TimeSpan?(this._time.CurTime + burrower.Comp.TunnelCooldown);
    }
    else
    {
      burrower.Comp.Tunneling = false;
      burrower.Comp.NextTunnelAt = new TimeSpan?();
      burrower.Comp.ForcedUnburrowAt = new TimeSpan?();
      this._rmcPulling.TryStopAllPullsFromAndOn((EntityUid) burrower);
      this.Dirty<XenoBurrowComponent>(burrower);
      if (this._net.IsServer)
        this._transform.SetCoordinates((EntityUid) burrower, this._entities.GetCoordinates(args.TargetCoords));
      BurrowedEvent args1 = new BurrowedEvent(false);
      this.RaiseLocalEvent<BurrowedEvent>((EntityUid) burrower, ref args1);
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-burrow-move-finish"), (EntityUid) burrower, new EntityUid?((EntityUid) burrower));
    }
  }
}
