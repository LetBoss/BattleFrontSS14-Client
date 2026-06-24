// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.ScissorCut.XenoScissorCutSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Slow;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Empower;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Content.Shared.Maps;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.ScissorCut;

public sealed class XenoScissorCutSystem : EntitySystem
{
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedPhysicsSystem _physics;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private RMCSlowSystem _slow;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private TurfSystem _turf;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoScissorCutComponent, XenoScissorCutActionEvent>(new EntityEventRefHandler<XenoScissorCutComponent, XenoScissorCutActionEvent>(this.OnXenoScissorCutAction));
  }

  private void OnXenoScissorCutAction(
    Entity<XenoScissorCutComponent> xeno,
    ref XenoScissorCutActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    bool flag = this.HasComp<XenoSuperEmpoweredComponent>((EntityUid) xeno);
    args.Handled = true;
    EntityUid? grid = this._transform.GetGrid(args.Target);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault = grid.GetValueOrDefault();
    MapGridComponent comp1;
    if (!this.TryComp<MapGridComponent>(valueOrDefault, out comp1))
      return;
    Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - this._transform.GetMoverCoordinates((EntityUid) xeno).Position)), Angle.FromDegrees(90.0));
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    Box2 box2_1 = Box2.CenteredAround(moverCoordinates.Position, new Vector2(1f, xeno.Comp.Range));
    Box2 box2_2 = ((Box2) ref box2_1).Translated(new Vector2(0.0f, (float) ((double) xeno.Comp.Range / 2.0 + 0.5)));
    Box2Rotated worldBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref worldBounds).\u002Ector(box2_2, angle, moverCoordinates.Position);
    List<EntityUid> source = new List<EntityUid>();
    List<EntityUid> entityUidList = new List<EntityUid>();
    if (this._net.IsClient)
      return;
    foreach (Entity<PhysicsComponent> collidingEntity in this._physics.GetCollidingEntities(this.Transform((EntityUid) xeno).MapID, in worldBounds))
    {
      if (this.HasComp<DamageOnXenoScissorsComponent>((EntityUid) collidingEntity) || this.HasComp<DestroyOnXenoPierceScissorComponent>((EntityUid) collidingEntity))
        source.Add((EntityUid) collidingEntity);
      else if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) collidingEntity, canAttackWindows: true))
        entityUidList.Add((EntityUid) collidingEntity);
    }
    EntityCoordinates selfCoords = this._transform.GetMoverCoordinates((EntityUid) xeno);
    float distance;
    foreach (EntityUid entityUid in source.OrderBy<EntityUid, float>((Func<EntityUid, float>) (a => !selfCoords.TryDistance((IEntityManager) this.EntityManager, a.ToCoordinates(), out distance) ? 10f : distance)).ToList<EntityUid>())
    {
      if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entityUid, xeno.Comp.Range + 0.5f))
      {
        DamageOnXenoScissorsComponent comp2;
        if (this.TryComp<DamageOnXenoScissorsComponent>(entityUid, out comp2))
        {
          FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?(entityUid), comp2.Damage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
          FixedPoint2 zero = FixedPoint2.Zero;
          if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
          {
            Filter filter1 = Filter.Pvs(entityUid, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
            SharedColorFlashEffectSystem colorFlash = this._colorFlash;
            Color red = Color.Red;
            List<EntityUid> entities = new List<EntityUid>();
            entities.Add(entityUid);
            Filter filter2 = filter1;
            colorFlash.RaiseEffect(red, entities, filter2);
          }
        }
        else
        {
          DestroyOnXenoPierceScissorComponent comp3;
          if (this.TryComp<DestroyOnXenoPierceScissorComponent>(entityUid, out comp3))
          {
            this.SpawnAtPosition((string) comp3.SpawnPrototype, entityUid.ToCoordinates());
            this.QueueDel(new EntityUid?(entityUid));
            this._audio.PlayEntity(comp3.Sound, entityUid, (EntityUid) xeno);
          }
        }
      }
    }
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote);
    EntityUid? nullable = new EntityUid?();
    foreach (EntityUid entityUid in entityUidList)
    {
      if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entityUid, xeno.Comp.Range + 0.5f))
      {
        if (!nullable.HasValue)
          nullable = new EntityUid?(entityUid);
        FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?(entityUid), xeno.Comp.Damage, origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
        FixedPoint2 zero = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
        {
          Filter filter3 = Filter.Pvs(entityUid, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add(entityUid);
          Filter filter4 = filter3;
          colorFlash.RaiseEffect(red, entities, filter4);
        }
        this.SpawnAttachedTo((string) xeno.Comp.AttackEffect, entityUid.ToCoordinates(), rotation: new Angle());
        this._audio.PlayEntity(xeno.Comp.SlashSound, (EntityUid) xeno, entityUid);
        if (flag)
          this._slow.TrySuperSlowdown(entityUid, xeno.Comp.SuperSlowDuration, ignoreDurationModifier: true);
      }
    }
    if (nullable.HasValue)
      this._rmcMelee.DoLunge((EntityUid) xeno, nullable.Value);
    Box2 box2_3 = ((Box2Rotated) ref worldBounds).CalcBoundingBox();
    foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault, comp1, worldBounds))
    {
      if (this._interaction.InRangeUnobstructed(xeno.Owner, this._turf.GetTileCenter(turf), xeno.Comp.Range + 0.5f))
      {
        EntProtoId prototype = xeno.Comp.TelegraphEffect;
        ref Box2 local1 = ref box2_3;
        Box2 box2_4 = Box2.CenteredAround(this._turf.GetTileCenter(turf).Position, Vector2.One);
        ref Box2 local2 = ref box2_4;
        if (!((Box2) ref local1).Encloses(ref local2))
          prototype = xeno.Comp.TelegraphEffectEdge;
        this.SpawnAtPosition((string) prototype, this._turf.GetTileCenter(turf));
      }
    }
  }
}
