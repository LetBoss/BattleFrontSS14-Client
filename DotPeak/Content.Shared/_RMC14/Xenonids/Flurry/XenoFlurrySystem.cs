// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Flurry.XenoFlurrySystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.Heal;
using Content.Shared._RMC14.Xenonids.Stab;
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
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Flurry;

public sealed class XenoFlurrySystem : EntitySystem
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
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedMapSystem _map;
  [Dependency]
  private TurfSystem _turf;
  [Dependency]
  private SharedXenoHealSystem _xenoHeal;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoFlurryComponent, XenoFlurryActionEvent>(new EntityEventRefHandler<XenoFlurryComponent, XenoFlurryActionEvent>(this.OnXenoFlurryAction));
  }

  private void OnXenoFlurryAction(Entity<XenoFlurryComponent> xeno, ref XenoFlurryActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    args.Handled = true;
    EntityUid? grid = this._transform.GetGrid(args.Target);
    if (!grid.HasValue)
      return;
    EntityUid valueOrDefault1 = grid.GetValueOrDefault();
    MapGridComponent comp;
    if (!this.TryComp<MapGridComponent>(valueOrDefault1, out comp))
      return;
    Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(Vector2Helpers.Normalized(args.Target.Position - this._transform.GetMoverCoordinates((EntityUid) xeno).Position)), Angle.FromDegrees(90.0));
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    Box2 box2_1 = Box2.CenteredAround(moverCoordinates.Position, new Vector2(1f, xeno.Comp.Range));
    Box2 box2_2 = ((Box2) ref box2_1).Translated(new Vector2(0.0f, (float) ((double) xeno.Comp.Range / 2.0 + 0.5)));
    Box2Rotated worldBounds;
    // ISSUE: explicit constructor call
    ((Box2Rotated) ref worldBounds).\u002Ector(box2_2, angle, moverCoordinates.Position);
    List<EntityUid> entityUidList = new List<EntityUid>();
    if (this._net.IsClient)
      return;
    foreach (Entity<PhysicsComponent> collidingEntity in this._physics.GetCollidingEntities(this.Transform((EntityUid) xeno).MapID, in worldBounds))
    {
      if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) collidingEntity))
        entityUidList.Add((EntityUid) collidingEntity);
    }
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote, cooldown: new TimeSpan?(xeno.Comp.EmoteDelay));
    DamageSpecifier damageSpecifier = new DamageSpecifier(xeno.Comp.Damage);
    RMCGetTailStabBonusDamageEvent args1 = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
    this.RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>((EntityUid) xeno, ref args1);
    DamageSpecifier baseDamage = damageSpecifier + args1.Damage;
    int num1 = 0;
    EntityUid? nullable = new EntityUid?();
    foreach (EntityUid entityUid in entityUidList)
    {
      if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) xeno.Owner, (Entity<TransformComponent>) entityUid, xeno.Comp.Range + 0.5f))
      {
        if (!nullable.HasValue)
          nullable = new EntityUid?(entityUid);
        ++num1;
        FixedPoint2? total = this._damage.TryChangeDamage(new EntityUid?(entityUid), this._xeno.TryApplyXenoSlashDamageMultiplier(entityUid, baseDamage), origin: new EntityUid?((EntityUid) xeno), tool: new EntityUid?((EntityUid) xeno))?.GetTotal();
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
        this.SpawnAttachedTo((string) xeno.Comp.AttackEffect, entityUid.ToCoordinates(), rotation: new Angle());
        this._audio.PlayEntity(xeno.Comp.SlashSound, (EntityUid) xeno, entityUid);
        this.SpawnAttachedTo((string) xeno.Comp.HealEffect, xeno.Owner.ToCoordinates(), rotation: new Angle());
        this._xenoHeal.CreateHealStacks((EntityUid) xeno, (FixedPoint2) xeno.Comp.HealAmount, xeno.Comp.HealDelay, xeno.Comp.HealCharges, xeno.Comp.HealDelay);
        if (xeno.Comp.MaxTargets.HasValue)
        {
          int num2 = num1;
          int? maxTargets = xeno.Comp.MaxTargets;
          int valueOrDefault2 = maxTargets.GetValueOrDefault();
          if (num2 >= valueOrDefault2 & maxTargets.HasValue)
            break;
        }
      }
    }
    if (nullable.HasValue)
      this._rmcMelee.DoLunge((EntityUid) xeno, nullable.Value);
    ((Box2Rotated) ref worldBounds).CalcBoundingBox();
    foreach (TileRef turf in this._map.GetTilesIntersecting(valueOrDefault1, comp, worldBounds))
    {
      if (this._interaction.InRangeUnobstructed(xeno.Owner, this._turf.GetTileCenter(turf), xeno.Comp.Range + 0.5f))
        this.SpawnAtPosition((string) xeno.Comp.TelegraphEffect, this._turf.GetTileCenter(turf));
    }
  }
}
