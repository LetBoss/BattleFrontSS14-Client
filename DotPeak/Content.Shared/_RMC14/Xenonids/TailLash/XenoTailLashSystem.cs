// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Tail_Lash.XenoTailLashSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Pulling;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.Plasma;
using Content.Shared._RMC14.Xenonids.Sweep;
using Content.Shared._RMC14.Xenonids.TailLash;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.DoAfter;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Content.Shared.Stunnable;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Prototypes;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Tail_Lash;

public sealed class XenoTailLashSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private XenoPlasmaSystem _plasma;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedDoAfterSystem _doAfter;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private RMCSizeStunSystem _size;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedStunSystem _stun;
  [Dependency]
  private RMCPullingSystem _pulling;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoTailLashComponent, XenoTailLashActionEvent>(new EntityEventRefHandler<XenoTailLashComponent, XenoTailLashActionEvent>(this.OnTailLashAction));
    this.SubscribeLocalEvent<XenoTailLashComponent, XenoTailLashDoAfterEvent>(new EntityEventRefHandler<XenoTailLashComponent, XenoTailLashDoAfterEvent>(this.OnTailLashDoAfter));
  }

  private void OnTailLashAction(
    Entity<XenoTailLashComponent> xeno,
    ref XenoTailLashActionEvent args)
  {
    if (args.Handled || !this._plasma.HasPlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.Cost))
      return;
    EntityUid? grid = this._transform.GetGrid(args.Target);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp))
      return;
    Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - this._transform.GetMoverCoordinates((EntityUid) xeno).Position)), Angle.FromDegrees(90.0));
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    Box2 box2_1 = Box2.CenteredAround(moverCoordinates.Position, new Vector2(xeno.Comp.Width, xeno.Comp.Height));
    Box2 box2_2 = ((Box2) ref box2_1).Translated(new Vector2(0.0f, (float) ((double) xeno.Comp.Height / 2.0 + 0.5)));
    Box2Rotated worldArea;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref worldArea).\u002Ector(box2_2, angle, moverCoordinates.Position);
    bool flag = false;
    Box2 box2_3 = ((Box2Rotated) ref worldArea).CalcBoundingBox();
    foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault, comp, worldArea))
    {
      if (this._interaction.InRangeUnobstructed(xeno.Owner, this._turf.GetTileCenter(turf), xeno.Comp.Width * xeno.Comp.Height, CollisionGroup.MobMask))
      {
        flag = true;
        if (!this._net.IsClient)
        {
          EntProtoId prototype = xeno.Comp.Effect;
          ref Box2 local1 = ref box2_3;
          box2_1 = Box2.CenteredAround(this._turf.GetTileCenter(turf).Position, Vector2.One);
          ref Box2 local2 = ref box2_1;
          if (!((Box2) ref local1).Encloses(ref local2))
            prototype = xeno.Comp.EffectEdge;
          this.SpawnAtPosition((string) prototype, this._turf.GetTileCenter(turf));
        }
      }
    }
    if (!flag)
    {
      this._popup.PopupClient(this.Loc.GetString("rmc-xeno-tail-lash-no-room"), (EntityUid) xeno, new EntityUid?((EntityUid) xeno), PopupType.MediumCaution);
    }
    else
    {
      xeno.Comp.Area = new Box2Rotated?(worldArea);
      this._doAfter.TryStartDoAfter(new DoAfterArgs((IEntityManager) this.EntityManager, (EntityUid) xeno, xeno.Comp.Windup, (DoAfterEvent) new XenoTailLashDoAfterEvent(), new EntityUid?((EntityUid) xeno))
      {
        BreakOnMove = true,
        DuplicateCondition = DuplicateConditions.SameEvent
      });
    }
  }

  private void OnTailLashDoAfter(
    Entity<XenoTailLashComponent> xeno,
    ref XenoTailLashDoAfterEvent args)
  {
    if (args.Cancelled || args.Handled || !xeno.Comp.Area.HasValue || !this._plasma.TryRemovePlasmaPopup((Entity<XenoPlasmaComponent>) xeno.Owner, xeno.Comp.Cost))
    {
      xeno.Comp.Area = new Box2Rotated?();
    }
    else
    {
      this.EnsureComp<XenoSweepingComponent>((EntityUid) xeno);
      this.DoCooldown(xeno);
      if (this._net.IsClient)
        return;
      args.Handled = true;
      foreach (Entity<PhysicsComponent> collidingEntity in this._physics.GetCollidingEntities(this.Transform((EntityUid) xeno).MapID, xeno.Comp.Area.Value))
      {
        RMCSizes size;
        if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) collidingEntity) && this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) collidingEntity.Owner, xeno.Comp.Width * xeno.Comp.Height, CollisionGroup.MobMask) && (!this._size.TryGetSize((EntityUid) collidingEntity, out size) || size < RMCSizes.Big))
        {
          this._stun.TryParalyze((EntityUid) collidingEntity, this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) collidingEntity, xeno.Comp.StunTime), true);
          this._slow.TrySlowdown((EntityUid) collidingEntity, this._xeno.TryApplyXenoDebuffMultiplier((EntityUid) collidingEntity, xeno.Comp.SlowTime));
          this._pulling.TryStopAllPullsFromAndOn((EntityUid) collidingEntity);
          MapCoordinates mapCoordinates = this._transform.GetMapCoordinates((EntityUid) xeno);
          this._size.KnockBack((EntityUid) collidingEntity, new MapCoordinates?(mapCoordinates), xeno.Comp.FlingDistance, xeno.Comp.FlingDistance, 10f);
        }
      }
      xeno.Comp.Area = new Box2Rotated?();
      this.Dirty<XenoTailLashComponent>(xeno);
    }
  }

  private void DoCooldown(Entity<XenoTailLashComponent> xeno)
  {
    foreach ((EntityUid owner, ActionComponent _) in this._rmcActions.GetActionsWithEvent<XenoTailLashActionEvent>((EntityUid) xeno))
      this._actions.SetCooldown(new Entity<ActionComponent>?((Entity<ActionComponent>) owner), xeno.Comp.Cooldown);
  }
}
