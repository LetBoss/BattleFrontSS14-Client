// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Abduct.XenoAbductSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Damage.ObstacleSlamming;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Hook;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chat.Prototypes;
using Content.Shared.Coordinates;
using Content.Shared.DoAfter;
using Content.Shared.Mobs.Systems;
using Content.Shared.Popups;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Abduct;

public sealed class XenoAbductSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCDazedSystem _dazed;
  [Dependency]
  private SharedDoAfterSystem _doafter;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private XenoHookSystem _hook;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private MobStateSystem _mob;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private RMCPullingSystem _pulling;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCObstacleSlammingSystem _rmcObstacleSlamming;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private StatusEffectsSystem _status;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  private readonly HashSet<EntityUid> _abductEnts = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoAbductComponent, XenoAbductActionEvent>(new EntityEventRefHandler<XenoAbductComponent, XenoAbductActionEvent>(this.OnXenoAbduct));
    this.SubscribeLocalEvent<XenoAbductComponent, XenoAbductDoAfterEvent>(new EntityEventRefHandler<XenoAbductComponent, XenoAbductDoAfterEvent>(this.OnXenoAbductDoafter));
  }

  private void OnXenoAbduct(Entity<XenoAbductComponent> xeno, ref XenoAbductActionEvent args)
  {
    if (args.Handled || !this._plasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.Cost))
      return;
    this.CleanUpTiles(xeno);
    NetCoordinates netEntity = this.GetNetCoordinates(args.Target);
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    if ((double) (netEntity.Position - moverCoordinates.Position).Length() > (double) xeno.Comp.Range)
    {
      Vector2 vector2 = Vector2Helpers.Normalized(netEntity.Position - moverCoordinates.Position) * (float) xeno.Comp.Range;
      netEntity = new NetCoordinates(this.GetNetEntity(args.Target.EntityId), moverCoordinates.Position + vector2);
    }
    List<LineTile> lineTileList = this._line.DrawLine(moverCoordinates, this.GetCoordinates(netEntity), TimeSpan.Zero, new float?((float) xeno.Comp.Range), out EntityUid? _);
    if (lineTileList.Count == 0)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-abduct-no-room"), new EntityUid?((EntityUid) xeno), PopupType.SmallCaution);
    }
    else
    {
      XenoAbductDoAfterEvent @event = new XenoAbductDoAfterEvent();
      if (!this._doafter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.DoafterTime, (DoAfterEvent) @event, new EntityUid?((EntityUid) xeno))
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent,
        BlockDuplicate = true
      }))
        return;
      this._stun.TrySlowdown((EntityUid) xeno, xeno.Comp.DoafterTime, false, 0.0f, 0.0f);
      if (this._net.IsClient)
        return;
      foreach (LineTile lineTile in lineTileList)
        xeno.Comp.Tiles.Add(this.Spawn((string) xeno.Comp.Telegraph, lineTile.Coordinates, rotation: new Angle()));
      ProtoId<EmotePrototype>? emote = xeno.Comp.Emote;
      if (!emote.HasValue)
        return;
      ProtoId<EmotePrototype> valueOrDefault = emote.GetValueOrDefault();
      this._emote.TryEmoteWithChat((EntityUid) xeno, valueOrDefault);
    }
  }

  private void OnXenoAbductDoafter(
    Entity<XenoAbductComponent> xeno,
    ref XenoAbductDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-abduct-cancel"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.Medium);
      this.CleanUpTiles(xeno);
      this.DoCooldown(xeno);
      this._status.TryRemoveStatusEffect((EntityUid) xeno, "SlowedDown");
    }
    else
    {
      args.Handled = true;
      XenoHookComponent comp;
      if (!this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.Cost) || this._net.IsClient || !this.TryComp<XenoHookComponent>((EntityUid) xeno, out comp))
        return;
      (EntityUid, XenoHookComponent) xeno1 = (xeno.Owner, comp);
      List<EntityUid> entityUidList = new List<EntityUid>();
      foreach (EntityUid tile in xeno.Comp.Tiles)
      {
        this._abductEnts.Clear();
        this._lookup.GetEntitiesInRange(tile.ToCoordinates(), xeno.Comp.TileRadius, this._abductEnts);
        foreach (EntityUid abductEnt in this._abductEnts)
        {
          RMCSizes size;
          if (!this.HasComp<StunnedComponent>(abductEnt) && this._xeno.CanAbilityAttackTarget((EntityUid) xeno, abductEnt) && !this._mob.IsCritical(abductEnt) && (!this._size.TryGetSize(abductEnt, out size) || size < RMCSizes.Big) && !entityUidList.Contains(abductEnt))
            entityUidList.Add(abductEnt);
        }
      }
      this.CleanUpTiles(xeno);
      string message = this.Loc.GetString("rmc-xeno-abduct-none");
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
      TimeSpan duration = TimeSpan.Zero;
      TimeSpan baseDuration1 = TimeSpan.Zero;
      TimeSpan time = TimeSpan.Zero;
      TimeSpan baseDuration2 = TimeSpan.FromSeconds(0.4);
      if (entityUidList.Count > 2)
      {
        message = this.Loc.GetString("rmc-xeno-abduct-more", ("targets", (object) entityUidList.Count));
        baseDuration2 = xeno.Comp.StunTime;
      }
      else if (entityUidList.Count == 2)
      {
        message = this.Loc.GetString("rmc-xeno-abduct-two");
        baseDuration1 = xeno.Comp.RootTime;
        time = xeno.Comp.DazeTime;
      }
      else if (entityUidList.Count == 1)
      {
        message = this.Loc.GetString("rmc-xeno-abduct-one");
        duration = xeno.Comp.SlowTime;
      }
      this.DoCooldown(xeno);
      this._popup.PopupEntity(message, (EntityUid) xeno, (EntityUid) xeno, PopupType.Medium);
      for (int index = 0; index < entityUidList.Count && index < xeno.Comp.MaxTargets; ++index)
      {
        EntityUid entityUid = entityUidList[index];
        if (this._hook.TryHookTarget((Entity<XenoHookComponent>) xeno1, entityUid))
        {
          this._pulling.TryStopAllPullsFromAndOn(entityUid);
          EntityCoordinates moverCoordinates1 = this._transform.GetMoverCoordinates((EntityUid) xeno);
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
          EntityCoordinates moverCoordinates2 = this._transform.GetMoverCoordinates(entityUid);
          float distance;
          if (!moverCoordinates1.TryDistance((IEntityManager) this.EntityManager, moverCoordinates2, out distance))
            break;
          this._slow.TrySlowdown(entityUid, duration, ignoreDurationModifier: true);
          this._slow.TryRoot(entityUid, this._xeno.TryApplyXenoDebuffMultiplier(entityUid, baseDuration1));
          this._dazed.TryDaze(entityUid, time, true, stutter: true);
          this._stun.TryParalyze(entityUid, this._xeno.TryApplyXenoDebuffMultiplier(entityUid, baseDuration2), true);
          float num = -Math.Max(distance - 2f, 0.5f);
          this._rmcObstacleSlamming.MakeImmune(entityUid);
          this._size.KnockBack(entityUid, new MapCoordinates?(mapCoordinates), num, num, 10f);
        }
      }
    }
  }

  private void CleanUpTiles(Entity<XenoAbductComponent> xeno)
  {
    if (this._net.IsClient)
      return;
    foreach (EntityUid tile in xeno.Comp.Tiles)
      this.QueueDel(new EntityUid?(tile));
    xeno.Comp.Tiles.Clear();
    this.Dirty<XenoAbductComponent>(xeno);
  }

  private void DoCooldown(Entity<XenoAbductComponent> xeno)
  {
    foreach (Entity<ActionComponent> entity in this._rmcActions.GetActionsWithEvent<XenoAbductActionEvent>((EntityUid) xeno))
      this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) ((EntityUid) entity, (ActionComponent) entity)), xeno.Comp.Cooldown);
  }
}
