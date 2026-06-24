// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ResinSurge.SharedXenoResinSurgeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Map;
using Content.Shared._RMC14.Xenonids.Construction;
using Content.Shared._RMC14.Xenonids.Construction.FloorResin;
using Content.Shared._RMC14.Xenonids.Fruit;
using Content.Shared._RMC14.Xenonids.Fruit.Components;
using Content.Shared._RMC14.Xenonids.Hive;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Weeds;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Coordinates;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Doors.Components;
using Content.Shared.Examine;
using Content.Shared.Maps;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ResinSurge;

public sealed class SharedXenoResinSurgeSystem : EntitySystem
{
  [Dependency]
  private IMapManager _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedXenoConstructReinforceSystem _xenoReinforce;
  [Dependency]
  private SharedXenoFruitSystem _xenoFruit;
  [Dependency]
  private XenoPlasmaSystem _xenoPlasma;
  [Dependency]
  private SharedMapSystem _sharedMap;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedXenoHiveSystem _hive;
  [Dependency]
  private RMCMapSystem _rmcMap;
  [Dependency]
  private SharedXenoWeedsSystem _weeds;
  [Dependency]
  private ExamineSystemShared _examine;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoResinSurgeComponent, XenoResinSurgeActionEvent>(new EntityEventRefHandler<XenoResinSurgeComponent, XenoResinSurgeActionEvent>(this.OnXenoResinSurgeAction));
    this.SubscribeLocalEvent<XenoResinSurgeComponent, ResinSurgeStickyResinDoafter>(new EntityEventRefHandler<XenoResinSurgeComponent, ResinSurgeStickyResinDoafter>(this.OnResinSurgeDoAfter));
  }

  private void SurgeUnstableWall(Entity<XenoResinSurgeComponent> xeno, EntityCoordinates target)
  {
    if (!target.IsValid((IEntityManager) this.EntityManager) || !this._net.IsServer)
      return;
    EntityUid dest = this.Spawn((string) xeno.Comp.UnstableWallId, target);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
  }

  private void SurgeStickyResin(Entity<XenoResinSurgeComponent> xeno, EntityCoordinates target)
  {
    if (!target.IsValid((IEntityManager) this.EntityManager) || !this._net.IsServer)
      return;
    EntityUid dest = this.SpawnAtPosition((string) xeno.Comp.StickyResinId, target);
    this._hive.SetSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) dest);
  }

  private void ReduceSurgeCooldown(Entity<XenoResinSurgeComponent> xeno, double? cooldownMult = null)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
    {
      XenoResinSurgeActionComponent comp;
      if (this.TryComp<XenoResinSurgeActionComponent>((EntityUid) action, out comp))
      {
        this._actions.SetCooldown(new Entity<ActionComponent>?(action.AsNullable()), comp.SuccessCooldown * (cooldownMult ?? (double) comp.FailCooldownMult));
        break;
      }
    }
  }

  private void SetSurgeCooldown(Entity<XenoResinSurgeComponent> xeno, TimeSpan? cooldown = null)
  {
    foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) xeno))
    {
      XenoResinSurgeActionComponent comp;
      if (this.TryComp<XenoResinSurgeActionComponent>((EntityUid) action, out comp))
      {
        this._actions.SetCooldown(new Entity<ActionComponent>?(action.AsNullable()), cooldown ?? comp.SuccessCooldown);
        break;
      }
    }
  }

  private void OnXenoResinSurgeAction(
    Entity<XenoResinSurgeComponent> xeno,
    ref XenoResinSurgeActionEvent args)
  {
    if (args.Handled)
      return;
    EntityUid? nullable = this._transform.GetGrid(args.Target);
    if (!nullable.HasValue)
      return;
    EntityUid valueOrDefault1 = nullable.GetValueOrDefault();
    MapGridComponent comp1;
    if (!this.TryComp<MapGridComponent>(valueOrDefault1, out comp1))
      return;
    if (!this._examine.InRangeUnOccluded(xeno.Owner, args.Target, (float) xeno.Comp.Range))
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-resin-surge-see-fail"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    }
    else
    {
      args.Handled = true;
      EntityCoordinates grid = args.Target.SnapToGrid((IEntityManager) this.EntityManager, this._map);
      if (xeno.Comp.ResinDoafter.HasValue || !this._xenoPlasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) (xeno.Owner, (XenoPlasmaComponent) null), args.PlasmaCost))
        return;
      nullable = args.Entity;
      if (nullable.HasValue)
      {
        EntityUid valueOrDefault2 = nullable.GetValueOrDefault();
        if (this.TryComp<ResinSurgeReinforcableComponent>(valueOrDefault2, out ResinSurgeReinforcableComponent _) && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) valueOrDefault2))
        {
          if (this.HasComp<XenoConstructReinforceComponent>(valueOrDefault2))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-resin-surge-shield-fail", ("target", (object) valueOrDefault2)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
            this.ReduceSurgeCooldown(xeno);
            args.Handled = false;
            return;
          }
          this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-resin-surge-shield-self", ("target", (object) valueOrDefault2)), this.Loc.GetString("rmc-xeno-resin-surge-shield-others", (nameof (xeno), (object) xeno), ("target", (object) valueOrDefault2)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
          this._xenoReinforce.Reinforce(valueOrDefault2, xeno.Comp.ReinforceAmount, xeno.Comp.ReinforceDuration);
          if (!this._net.IsServer)
            return;
          if (this.HasComp<DoorComponent>(valueOrDefault2))
          {
            this.SpawnAttachedTo((string) xeno.Comp.SurgeDoorEffect, valueOrDefault2.ToCoordinates(), rotation: new Angle());
            return;
          }
          this.SpawnAttachedTo((string) xeno.Comp.SurgeWallEffect, valueOrDefault2.ToCoordinates(), rotation: new Angle());
          return;
        }
        XenoFruitComponent comp2;
        if (this.TryComp<XenoFruitComponent>(valueOrDefault2, out comp2) && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) valueOrDefault2))
        {
          if (!this._xenoFruit.TrySpeedupGrowth((Entity<XenoFruitComponent>) (valueOrDefault2, comp2), xeno.Comp.FruitGrowth))
          {
            this._popup.PopupClient(this.Loc.GetString("rmc-xeno-resin-surge-fruit-fail", ("target", (object) valueOrDefault2)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
            this.ReduceSurgeCooldown(xeno);
            args.Handled = false;
            return;
          }
          this._popup.PopupClient(this.Loc.GetString("rmc-xeno-resin-surge-fruit", ("target", (object) valueOrDefault2)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
          args.Handled = false;
          double num = (comp2.GrowTime.TotalSeconds - comp2.GrowTime / xeno.Comp.FruitCooldownDivisor) * 0.1;
          this.ReduceSurgeCooldown(xeno, new double?(num));
          return;
        }
        XenoWeedsComponent comp3;
        if (this.TryComp<XenoWeedsComponent>(valueOrDefault2, out comp3) || this._weeds.IsOnFriendlyWeeds((Entity<TransformComponent>) valueOrDefault2))
        {
          EntityUid entityUid = valueOrDefault2;
          if (comp3 == null)
          {
            Entity<XenoWeedsComponent>? weedsOnFloor = this._weeds.GetWeedsOnFloor((Entity<MapGridComponent>) (valueOrDefault1, comp1), valueOrDefault2.ToCoordinates());
            if (weedsOnFloor.HasValue)
            {
              entityUid = (EntityUid) weedsOnFloor.Value;
              this.TryComp<XenoWeedsComponent>(entityUid, out comp3);
            }
          }
          if (comp3 != null && this._hive.FromSameHive((Entity<HiveMemberComponent>) xeno.Owner, (Entity<HiveMemberComponent>) entityUid))
          {
            this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-resin-surge-wall-self"), this.Loc.GetString("rmc-xeno-resin-surge-wall-others", (nameof (xeno), (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
            this.SurgeUnstableWall(xeno, grid);
            return;
          }
        }
      }
      ResinSurgeStickyResinDoafter stickyResinDoafter = new ResinSurgeStickyResinDoafter(this.GetNetCoordinates(grid));
      EntityManager entityManager = this.EntityManager;
      EntityUid user = (EntityUid) xeno;
      TimeSpan resinDoAfterPeriod = xeno.Comp.StickyResinDoAfterPeriod;
      ResinSurgeStickyResinDoafter @event = stickyResinDoafter;
      EntityUid? eventTarget = new EntityUid?((EntityUid) xeno);
      nullable = new EntityUid?();
      EntityUid? target = nullable;
      nullable = new EntityUid?();
      EntityUid? used = nullable;
      DoAfterId? id;
      if (this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) entityManager, user, resinDoAfterPeriod, (DoAfterEvent) @event, eventTarget, target, used)
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent
      }, out id))
        xeno.Comp.ResinDoafter = id;
      else
        this.ReduceSurgeCooldown(xeno);
      args.Handled = false;
    }
  }

  private void OnResinSurgeDoAfter(
    Entity<XenoResinSurgeComponent> xeno,
    ref ResinSurgeStickyResinDoafter args)
  {
    xeno.Comp.ResinDoafter = new DoAfterId?();
    if (args.Cancelled)
      return;
    EntityCoordinates coordinates = this.GetCoordinates(args.Coordinates);
    EntityUid? grid = this._transform.GetGrid(coordinates);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    this._popup.PopupPredicted(this.Loc.GetString("rmc-xeno-resin-surge-sticky-self"), this.Loc.GetString("rmc-xeno-resin-surge-sticky-others", (nameof (xeno), (object) xeno)), (EntityUid) xeno, new EntityUid?((EntityUid) xeno));
    if (this._net.IsServer)
    {
      foreach (TileRef turf in this._sharedMap.GetTilesIntersecting(valueOrDefault, comp, Box2.CenteredAround(coordinates.Position, new Vector2((float) (xeno.Comp.StickyResinRadius * 2), (float) (xeno.Comp.StickyResinRadius * 2))), false))
      {
        if (!this._rmcMap.HasAnchoredEntityEnumerator<StickyResinSurgeBlockerComponent>(this._turf.GetTileCenter(turf), out Entity<StickyResinSurgeBlockerComponent> _, facing: (DirectionFlag) 0))
          this.SurgeStickyResin(xeno, this._turf.GetTileCenter(turf));
      }
    }
    this.SetSurgeCooldown(xeno);
  }
}
