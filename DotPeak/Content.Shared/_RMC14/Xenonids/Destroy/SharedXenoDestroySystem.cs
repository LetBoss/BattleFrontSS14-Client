// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Destroy.SharedXenoDestroySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Areas;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Gibbing;
using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Construction.Nest;
using Content.Shared._RMC14.Xenonids.Devour;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Damage;
using Content.Shared.DoAfter;
using Content.Shared.Explosion;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Item;
using Content.Shared.Jittering;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Events;
using Content.Shared.Movement.Systems;
using Content.Shared.Popups;
using Content.Shared.Throwing;
using Content.Shared.Whitelist;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Destroy;

public abstract class SharedXenoDestroySystem : EntitySystem
{
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private AreaSystem _area;
  [Dependency]
  private SharedJitteringSystem _jitter;
  [Dependency]
  private SharedDoAfterSystem _doafter;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private RotateToFaceSystem _rotateToFace;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  protected IGameTiming _timing;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private EntityLookupSystem _entityLookup;
  [Dependency]
  private EntityWhitelistSystem _whitelist;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private SharedBodySystem _body;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private RMCGibSystem _rmcGib;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCCameraShakeSystem _cameraShake;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCPullingSystem _rmcPull;
  [Dependency]
  private ActionBlockerSystem _blocker;
  private readonly HashSet<Entity<MobStateComponent>> _mobs = new HashSet<Entity<MobStateComponent>>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoDestroyComponent, XenoDestroyActionEvent>(new EntityEventRefHandler<XenoDestroyComponent, XenoDestroyActionEvent>(this.OnXenoDestroyAction));
    this.SubscribeLocalEvent<XenoDestroyComponent, XenoDestroyLeapDoafter>(new EntityEventRefHandler<XenoDestroyComponent, XenoDestroyLeapDoafter>(this.OnXenoDestroyDoafter));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, AttemptMobCollideEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, AttemptMobCollideEvent>(this.OnLeapCollide));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, AttemptMobTargetCollideEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, AttemptMobTargetCollideEvent>(this.OnLeapTargetCollide));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentInit>(new EntityEventRefHandler<XenoDestroyLeapingComponent, ComponentInit>(this.OnLeapingInit));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ComponentRemove>(new EntityEventRefHandler<XenoDestroyLeapingComponent, ComponentRemove>(this.OnLeapingRemove));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, DropAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, DropAttemptEvent>(this.OnLeapingCancel<DropAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, UseAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, UseAttemptEvent>(this.OnLeapingCancel<UseAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, PickupAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, PickupAttemptEvent>(this.OnLeapingCancel<PickupAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, AttackAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, AttackAttemptEvent>(this.OnLeapingCancel<AttackAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ThrowAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, ThrowAttemptEvent>(this.OnLeapingCancel<ThrowAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, ChangeDirectionAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, ChangeDirectionAttemptEvent>(this.OnLeapingCancel<ChangeDirectionAttemptEvent>));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, InteractionAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, InteractionAttemptEvent>(this.OnLeapingCancelInteract));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, PullAttemptEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, PullAttemptEvent>(this.OnLeapingCancelPull));
    this.SubscribeLocalEvent<XenoDestroyLeapingComponent, UpdateCanMoveEvent>(new EntityEventRefHandler<XenoDestroyLeapingComponent, UpdateCanMoveEvent>(this.OnLeapingCancel<UpdateCanMoveEvent>));
  }

  private void OnXenoDestroyAction(
    Entity<XenoDestroyComponent> xeno,
    ref XenoDestroyActionEvent args)
  {
    TileRef? tile;
    if (args.Handled || !this._turf.TryGetTileRef(args.Target, out tile))
      return;
    EntityCoordinates tileCenter = this._turf.GetTileCenter(tile.Value);
    if (!this._interaction.InRangeUnobstructed((EntityUid) xeno, tileCenter, xeno.Comp.Range) || this._rmcMap.IsTileBlocked(tileCenter))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-destroy-cant-reach"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      Entity<AreaComponent>? area;
      if (!this._area.TryGetArea(tileCenter, out area, out EntityPrototype _) || area.Value.Comp.NoTunnel)
      {
        this._popup.PopupClient(this.Loc.GetString("rmc-destroy-cant-area"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
      }
      else
      {
        this._jitter.DoJitter((EntityUid) xeno, xeno.Comp.JumpTime, true, 80f, 8f, true);
        this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.JumpTime, (DoAfterEvent) new XenoDestroyLeapDoafter(this.GetNetCoordinates(tileCenter)), new EntityUid?((EntityUid) xeno))
        {
          BreakOnMove = true,
          BreakOnRest = true
        });
        this.Dirty<XenoDestroyComponent>(xeno);
      }
    }
  }

  private void OnXenoDestroyDoafter(
    Entity<XenoDestroyComponent> xeno,
    ref XenoDestroyLeapDoafter args)
  {
    if (args.Handled || args.Cancelled || this._net.IsClient)
      return;
    args.Handled = true;
    EntityCoordinates coordinates = this.GetCoordinates(args.TargetCoords);
    if (!this._interaction.InRangeUnobstructed((EntityUid) xeno, coordinates, xeno.Comp.Range) || this._rmcMap.IsTileBlocked(coordinates))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-destroy-cant-reach"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      this._rotateToFace.TryFaceCoordinates((EntityUid) xeno, this._transform.ToMapCoordinates(args.TargetCoords).Position);
      this._rmcPull.TryStopAllPullsFromAndOn((EntityUid) xeno);
      if (this._net.IsServer)
      {
        XenoDestroyLeapingComponent leapingComponent = this.EnsureComp<XenoDestroyLeapingComponent>((EntityUid) xeno);
        leapingComponent.Target = new EntityCoordinates?(coordinates);
        leapingComponent.LeapMoveAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.CrashTime / 2.0);
        leapingComponent.LeapEndAt = new TimeSpan?(this._timing.CurTime + xeno.Comp.CrashTime);
        this.Dirty(xeno.Owner, (IComponent) leapingComponent);
        Filter filter = Filter.Pvs((EntityUid) xeno);
        Vector2 leapOffset = this._transform.ToMapCoordinates(coordinates).Position - this._transform.GetMapCoordinates((EntityUid) xeno).Position;
        this.RaiseNetworkEvent((EntityEventArgs) new XenoDestroyLeapStartEvent(this.GetNetEntity((EntityUid) xeno), leapOffset), filter);
      }
      this.PredictedSpawnAtPosition((string) xeno.Comp.Telegraph, coordinates);
      this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote);
    }
  }

  private void OnLeapCollide(
    Entity<XenoDestroyLeapingComponent> xeno,
    ref AttemptMobCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnLeapTargetCollide(
    Entity<XenoDestroyLeapingComponent> xeno,
    ref AttemptMobTargetCollideEvent args)
  {
    args.Cancelled = true;
  }

  private void OnLeapingCancel<T>(Entity<XenoDestroyLeapingComponent> ent, ref T args) where T : CancellableEntityEventArgs
  {
    args.Cancel();
  }

  private void OnLeapingCancelInteract(
    Entity<XenoDestroyLeapingComponent> ent,
    ref InteractionAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void OnLeapingCancelPull(
    Entity<XenoDestroyLeapingComponent> ent,
    ref PullAttemptEvent args)
  {
    args.Cancelled = true;
  }

  private void CrashDown(Entity<XenoDestroyComponent> xeno)
  {
    this.RemCompDeferred<XenoDestroyLeapingComponent>((EntityUid) xeno);
    EntityUid? grid = this._transform.GetGrid((Entity<TransformComponent>) xeno.Owner);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp1;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp1))
      return;
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault, comp1, Box2.CenteredAround(this._transform.GetMoverCoordinates((EntityUid) xeno).Position, new Vector2(2f, 2f))))
    {
      foreach (EntityUid entityUid in this._entityLookup.GetEntitiesInTile(turf, LookupFlags.All))
      {
        if (this.CanGib((EntityUid) xeno, entityUid))
        {
          BodyComponent comp2;
          if (!xeno.Comp.Gibs || !this.TryComp<BodyComponent>(entityUid, out comp2))
            this._damage.TryChangeDamage(new EntityUid?(entityUid), xeno.Comp.MobDamage, true, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
          else if (this._net.IsServer)
          {
            this._rmcGib.ScatterInventoryItems(entityUid);
            this._body.GibBody(entityUid, true, comp2, splatCone: new Angle());
          }
        }
        else if (this.HasComp<ItemComponent>(entityUid) && !this.Transform(entityUid).Anchored)
          this._size.KnockBack(entityUid, new MapCoordinates?(this._transform.GetMapCoordinates((EntityUid) xeno)), xeno.Comp.Knockback, xeno.Comp.Knockback, 15f, true);
        else if (this._whitelist.IsWhitelistPass(xeno.Comp.Structures, entityUid))
        {
          GetExplosionResistanceEvent args = new GetExplosionResistanceEvent(xeno.Comp.ExplosionType.Id);
          this.RaiseLocalEvent<GetExplosionResistanceEvent>(entityUid, ref args);
          this._damage.TryChangeDamage(new EntityUid?(entityUid), xeno.Comp.StructureDamage * args.DamageCoefficient, true, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno));
        }
      }
      this.PredictedSpawnAtPosition((string) xeno.Comp.SmokeEffect, this._turf.GetTileCenter(turf));
    }
    this._mobs.Clear();
    this._entityLookup.GetEntitiesInRange<MobStateComponent>(this.Transform((EntityUid) xeno).Coordinates, xeno.Comp.ShakeCameraRange, this._mobs);
    foreach (Entity<MobStateComponent> mob in this._mobs)
    {
      if (mob.Owner == xeno.Owner)
        this._cameraShake.ShakeCamera((EntityUid) mob, 5, 1);
      else
        this._cameraShake.ShakeCamera((EntityUid) mob, 15, 1);
    }
    this.SetCooldown(xeno);
  }

  private bool CanGib(EntityUid king, EntityUid target)
  {
    return !(king == target) && !this._hive.FromSameHive((Entity<HiveMemberComponent>) king, (Entity<HiveMemberComponent>) target) && !this.HasComp<DevouredComponent>(target) && !this.HasComp<XenoNestedComponent>(target) && this.HasComp<MobStateComponent>(target);
  }

  public override void Update(float frameTime)
  {
    if (this._net.IsClient)
      return;
    TimeSpan curTime = this._timing.CurTime;
    Robust.Shared.GameObjects.EntityQueryEnumerator<XenoDestroyLeapingComponent, XenoDestroyComponent> entityQueryEnumerator = this.EntityQueryEnumerator<XenoDestroyLeapingComponent, XenoDestroyComponent>();
    EntityUid uid;
    XenoDestroyLeapingComponent comp1;
    XenoDestroyComponent comp2;
    while (entityQueryEnumerator.MoveNext(out uid, out comp1, out comp2))
    {
      if (this._mob.IsDead(uid))
      {
        this.RemCompDeferred<XenoDestroyLeapingComponent>(uid);
      }
      else
      {
        TimeSpan? nullable;
        if (comp1.LeapMoveAt.HasValue)
        {
          TimeSpan timeSpan = curTime;
          nullable = comp1.LeapMoveAt;
          if ((nullable.HasValue ? (timeSpan > nullable.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          {
            if (comp1.Target.HasValue)
              this._transform.SetCoordinates(uid, comp1.Target.Value);
            comp1.LeapMoveAt = new TimeSpan?();
            this.Dirty(uid, (IComponent) comp1);
          }
        }
        if (comp1.LeapEndAt.HasValue)
        {
          TimeSpan timeSpan = curTime;
          nullable = comp1.LeapEndAt;
          if ((nullable.HasValue ? (timeSpan < nullable.GetValueOrDefault() ? 1 : 0) : 0) == 0)
            this.CrashDown((Entity<XenoDestroyComponent>) (uid, comp2));
        }
      }
    }
  }

  private void SetCooldown(Entity<XenoDestroyComponent> xeno)
  {
    using (IEnumerator<Entity<ActionComponent>> enumerator = this._rmcActions.GetActionsWithEvent<XenoDestroyActionEvent>((EntityUid) xeno).GetEnumerator())
    {
      if (!enumerator.MoveNext())
        return;
      (EntityUid owner, ActionComponent _) = enumerator.Current;
      this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) owner), xeno.Comp.Cooldown);
    }
  }

  private void OnLeapingInit(Entity<XenoDestroyLeapingComponent> xeno, ref ComponentInit args)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
      this._actions.SetEnabled(new Entity<ActionComponent>?(action.AsNullable()), false);
    this._blocker.UpdateCanMove((EntityUid) xeno);
  }

  protected virtual void OnLeapingRemove(
    Entity<XenoDestroyLeapingComponent> xeno,
    ref ComponentRemove args)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
      this._actions.SetEnabled(new Entity<ActionComponent>?(action.AsNullable()), true);
    this._blocker.UpdateCanMove((EntityUid) xeno);
  }
}
