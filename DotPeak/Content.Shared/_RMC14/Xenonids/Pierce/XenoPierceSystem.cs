// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Pierce.XenoPierceSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Actions;
using Content.Shared._RMC14.Emote;
using Content.Shared._RMC14.Line;
using Content.Shared._RMC14.Marines;
using Content.Shared._RMC14.Shields;
using Content.Shared._RMC14.Weapons.Melee;
using Content.Shared._RMC14.Xenonids.ScissorCut;
using Content.Shared.Actions;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.Interaction;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Pierce;

public sealed class XenoPierceSystem : EntitySystem
{
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedRMCEmoteSystem _emote;
  [Dependency]
  private DamageableSystem _damage;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private VanguardShieldSystem _vanguard;
  [Dependency]
  private SharedRMCMeleeWeaponSystem _rmcMelee;
  [Dependency]
  private SharedRMCActionsSystem _rmcActions;
  [Dependency]
  private LineSystem _line;
  [Dependency]
  private EntityLookupSystem _lookup;
  private readonly HashSet<Entity<MarineComponent>> _pierceEnts = new HashSet<Entity<MarineComponent>>();
  private readonly HashSet<EntityUid> _hitAlready = new HashSet<EntityUid>();

  public override void Initialize()
  {
    this.SubscribeLocalEvent<XenoPierceComponent, XenoPierceActionEvent>(new EntityEventRefHandler<XenoPierceComponent, XenoPierceActionEvent>(this.OnXenoPierceAction));
  }

  private void OnXenoPierceAction(Entity<XenoPierceComponent> xeno, ref XenoPierceActionEvent args)
  {
    if (args.Handled || !this._rmcActions.TryUseAction((WorldTargetActionEvent) args))
      return;
    EntityUid? blocker = this._transform.GetGrid(args.Target);
    if (!blocker.HasValue || !this.HasComp<MapGridComponent>(blocker.GetValueOrDefault()))
      return;
    EntityCoordinates end = args.Target;
    EntityCoordinates moverCoordinates = this._transform.GetMoverCoordinates((EntityUid) xeno);
    float distance;
    if (!args.Target.TryDistance((IEntityManager) this.EntityManager, moverCoordinates, out distance))
      return;
    if ((FixedPoint2) distance > xeno.Comp.Range)
    {
      Vector2 vector2 = Vector2Helpers.Normalized(args.Target.Position - moverCoordinates.Position) * xeno.Comp.Range.Float();
      end = moverCoordinates.WithPosition(moverCoordinates.Position + vector2);
    }
    List<LineTile> lineTileList = this._line.DrawLine(moverCoordinates, end, TimeSpan.Zero, new float?(xeno.Comp.Range.Float()), out blocker);
    if (lineTileList.Count == 0)
      return;
    args.Handled = true;
    this._hitAlready.Clear();
    int num1 = 0;
    EntityUid? nullable = new EntityUid?();
    foreach (LineTile lineTile in lineTileList)
    {
      this._pierceEnts.Clear();
      EntityUid entityUid = this.Spawn((string) xeno.Comp.Blocker, lineTile.Coordinates, rotation: new Angle());
      this._lookup.GetEntitiesInRange<MarineComponent>(entityUid.ToCoordinates(), 0.5f, this._pierceEnts);
      foreach (Entity<MarineComponent> pierceEnt in this._pierceEnts)
      {
        if (this._interaction.InRangeUnobstructed((Entity<TransformComponent>) entityUid, (Entity<TransformComponent>) pierceEnt.Owner, xeno.Comp.Range.Float()))
        {
          DestroyOnXenoPierceScissorComponent comp;
          if (this.TryComp<DestroyOnXenoPierceScissorComponent>((EntityUid) pierceEnt, out comp))
          {
            if (this._net.IsServer)
            {
              this.SpawnAtPosition((string) comp.SpawnPrototype, pierceEnt.Owner.ToCoordinates());
              this.QueueDel(new EntityUid?((EntityUid) pierceEnt));
            }
            this._audio.PlayPredicted(comp.Sound, (EntityUid) pierceEnt, new EntityUid?((EntityUid) xeno));
          }
          else if (this._xeno.CanAbilityAttackTarget((EntityUid) xeno, (EntityUid) pierceEnt) && this._hitAlready.Add((EntityUid) pierceEnt))
          {
            ++num1;
            DamageableSystem damage1 = this._damage;
            EntityUid? uid = new EntityUid?((EntityUid) pierceEnt);
            DamageSpecifier damage2 = this._xeno.TryApplyXenoSlashDamageMultiplier((EntityUid) pierceEnt, xeno.Comp.Damage);
            EntityUid? origin = new EntityUid?((EntityUid) xeno);
            int ap = xeno.Comp.AP;
            EntityUid? tool = new EntityUid?((EntityUid) xeno);
            int armorPiercing = ap;
            FixedPoint2? total = damage1.TryChangeDamage(uid, damage2, origin: origin, tool: tool, armorPiercing: armorPiercing)?.GetTotal();
            FixedPoint2 zero = FixedPoint2.Zero;
            if ((total.HasValue ? (total.GetValueOrDefault() > zero ? 1 : 0) : 0) != 0)
            {
              Filter filter1 = Filter.Pvs((EntityUid) pierceEnt, entityManager: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == xeno.Owner));
              SharedColorFlashEffectSystem colorFlash = this._colorFlash;
              Color red = Color.Red;
              List<EntityUid> entities = new List<EntityUid>();
              entities.Add((EntityUid) pierceEnt);
              Filter filter2 = filter1;
              colorFlash.RaiseEffect(red, entities, filter2);
            }
            if (this._net.IsServer)
              this.SpawnAttachedTo((string) xeno.Comp.AttackEffect, pierceEnt.Owner.ToCoordinates(), rotation: new Angle());
            if (!nullable.HasValue)
              nullable = new EntityUid?((EntityUid) pierceEnt);
            if (xeno.Comp.MaxTargets.HasValue)
            {
              int num2 = num1;
              int? maxTargets = xeno.Comp.MaxTargets;
              int valueOrDefault = maxTargets.GetValueOrDefault();
              if (num2 >= valueOrDefault & maxTargets.HasValue)
                break;
            }
          }
        }
      }
    }
    this._emote.TryEmoteWithChat((EntityUid) xeno, xeno.Comp.Emote, cooldown: xeno.Comp.EmoteCooldown);
    if (num1 > 0 && nullable.HasValue)
      this._rmcMelee.DoLunge((EntityUid) xeno, nullable.Value);
    if (this._net.IsServer)
      this._audio.PlayPvs(xeno.Comp.Sound, (EntityUid) xeno);
    if (num1 < xeno.Comp.RechargeTargetsRequired)
      return;
    this._vanguard.RegenShield((EntityUid) xeno);
  }
}
