// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Xenonids.Stab.SharedXenoTailStabSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._RMC14.Barricade;
using Content.Shared._RMC14.CameraShake;
using Content.Shared._RMC14.Stun;
using Content.Shared._RMC14.Xenonids.GasToggle;
using Content.Shared._RMC14.Xenonids.Neurotoxin;
using Content.Shared._RMC14.Xenonids.Rotate;
using Content.Shared.ActionBlocker;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Coordinates;
using Content.Shared.Damage;
using Content.Shared.Effects;
using Content.Shared.FixedPoint;
using Content.Shared.IdentityManagement;
using Content.Shared.Interaction;
using Content.Shared.Popups;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Events;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Xenonids.Stab;

public abstract class SharedXenoTailStabSystem : EntitySystem
{
  [Dependency]
  private ActionBlockerSystem _actionBlocker;
  [Dependency]
  private SharedActionsSystem _actions;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedColorFlashEffectSystem _colorFlash;
  [Dependency]
  private DamageableSystem _damageable;
  [Dependency]
  private SharedInteractionSystem _interaction;
  [Dependency]
  private SharedMeleeWeaponSystem _melee;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private INetManager _net;
  [Dependency]
  private SharedDirectionalAttackBlockSystem _directionBlock;
  [Dependency]
  private SharedSolutionContainerSystem _solutionContainer;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private XenoSystem _xeno;
  [Dependency]
  private XenoRotateSystem _rotate;
  [Dependency]
  private RMCDazedSystem _daze;
  [Dependency]
  private RMCCameraShakeSystem _cameraShake;
  [Dependency]
  private RMCSizeStunSystem _size;
  protected Box2Rotated LastTailAttack;

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<XenoTailStabComponent, XenoTailStabEvent>(new EntityEventRefHandler<XenoTailStabComponent, XenoTailStabEvent>(this.OnXenoTailStab));
    this.SubscribeLocalEvent<XenoTailStabComponent, XenoGasToggleActionEvent>(new EntityEventRefHandler<XenoTailStabComponent, XenoGasToggleActionEvent>(this.OnXenoGasToggle));
  }

  private void OnXenoGasToggle(
    Entity<XenoTailStabComponent> stab,
    ref XenoGasToggleActionEvent args)
  {
    if (!stab.Comp.Toggle)
      return;
    stab.Comp.InjectNeuro = !stab.Comp.InjectNeuro;
  }

  private void OnXenoTailStab(Entity<XenoTailStabComponent> stab, ref XenoTailStabEvent args)
  {
    TransformComponent comp1;
    if (!this._actionBlocker.CanAttack((EntityUid) stab) || !this.TryComp((EntityUid) stab, out comp1))
      return;
    MapCoordinates mapCoordinates1 = this._transform.GetMapCoordinates((EntityUid) stab, comp1);
    if (mapCoordinates1.MapId == MapId.Nullspace)
      return;
    MapCoordinates mapCoordinates2 = this._transform.ToMapCoordinates(args.Target);
    if (mapCoordinates1.MapId != mapCoordinates2.MapId)
      return;
    MeleeWeaponComponent comp2;
    if (this.TryComp<MeleeWeaponComponent>((EntityUid) stab, out comp2))
    {
      if (this._timing.CurTime < comp2.NextAttack)
        return;
      comp2.NextAttack = this._timing.CurTime + TimeSpan.FromSeconds(1L);
      this.Dirty((EntityUid) stab, (IComponent) comp2);
    }
    bool flag = false;
    DamageSpecifier damageSpecifier = new DamageSpecifier(stab.Comp.TailDamage);
    RMCGetTailStabBonusDamageEvent args1 = new RMCGetTailStabBonusDamageEvent(new DamageSpecifier());
    this.RaiseLocalEvent<RMCGetTailStabBonusDamageEvent>((EntityUid) stab, ref args1);
    DamageSpecifier baseDamage1 = damageSpecifier + args1.Damage;
    if (!args.Entity.HasValue || this.TerminatingOrDeleted(args.Entity) || !this._xeno.CanAbilityAttackTarget((EntityUid) stab, args.Entity.Value, true))
    {
      MeleeHitEvent args2 = new MeleeHitEvent(new List<EntityUid>(), (EntityUid) stab, (EntityUid) stab, baseDamage1, new Vector2?());
      this.RaiseLocalEvent<MeleeHitEvent>((EntityUid) stab, args2);
      foreach (Entity<ActionComponent> action in this._actions.GetActions((EntityUid) stab))
      {
        XenoTailStabActionComponent comp3;
        if (this.TryComp<XenoTailStabActionComponent>((EntityUid) action, out comp3))
          this._actions.SetCooldown(new Entity<ActionComponent>?(action.AsNullable()), comp3.MissCooldown);
      }
    }
    else
    {
      args.Handled = true;
      EntityUid entityUid = args.Entity.Value;
      MeleeHitEvent args3 = new MeleeHitEvent(new List<EntityUid>()
      {
        entityUid
      }, (EntityUid) stab, (EntityUid) stab, baseDamage1, new Vector2?());
      this.RaiseLocalEvent<MeleeHitEvent>((EntityUid) stab, args3);
      if (!args3.Handled)
      {
        this._interaction.DoContactInteraction((EntityUid) stab, new EntityUid?((EntityUid) stab));
        this._interaction.DoContactInteraction((EntityUid) stab, new EntityUid?(entityUid));
        Vector2 position1 = this._transform.GetMoverCoordinates(entityUid).Position;
        Vector2 position2 = this._transform.GetMoverCoordinates((EntityUid) stab).Position;
        foreach (NetEntity netEntity in this.GetNetEntityList(this._melee.ArcRayCast(position2, DirectionExtensions.ToWorldAngle(position1 - position2), Angle.op_Implicit(0.0f), stab.Comp.TailRange.Float(), this._transform.GetMapId((Entity<TransformComponent>) stab.Owner), (EntityUid) stab).ToList<EntityUid>()))
        {
          EntityUid entity = this.GetEntity(netEntity);
          if (this._directionBlock.IsAttackBlocked((EntityUid) stab, entity))
          {
            entityUid = entity;
            break;
          }
        }
        Filter filter1 = Filter.Pvs(comp1.Coordinates, entityMan: (IEntityManager) this.EntityManager).RemoveWhereAttachedEntity((Predicate<EntityUid>) (o => o == stab.Owner));
        AttackedEvent args4 = new AttackedEvent((EntityUid) stab, (EntityUid) stab, args.Target);
        this.RaiseLocalEvent<AttackedEvent>(entityUid, args4);
        DamageSpecifier baseDamage2 = DamageSpecifier.ApplyModifierSets(baseDamage1 + args3.BonusDamage + args4.BonusDamage, (IEnumerable<DamageModifierSet>) args3.ModifiersList);
        FixedPoint2? total = this._damageable.TryChangeDamage(new EntityUid?(entityUid), this._xeno.TryApplyXenoSlashDamageMultiplier(entityUid, baseDamage2), origin: new EntityUid?((EntityUid) stab), tool: new EntityUid?((EntityUid) stab))?.GetTotal();
        FixedPoint2 zero3 = FixedPoint2.Zero;
        if ((total.HasValue ? (total.GetValueOrDefault() > zero3 ? 1 : 0) : 0) != 0)
        {
          flag = true;
          SharedColorFlashEffectSystem colorFlash = this._colorFlash;
          Color red = Color.Red;
          List<EntityUid> entities = new List<EntityUid>();
          entities.Add(entityUid);
          Filter filter2 = filter1;
          colorFlash.RaiseEffect(red, entities, filter2);
        }
        if (this._net.IsServer)
        {
          this.SpawnAttachedTo((string) stab.Comp.HitAnimationId, entityUid.ToCoordinates(), rotation: new Angle());
          RMCSizes size;
          if (this._size.TryGetSize((EntityUid) stab, out size))
          {
            if (size >= RMCSizes.Big)
              this._daze.TryDaze(entityUid, stab.Comp.BigDazeTime, true);
            else if (size == RMCSizes.Xeno)
              this._daze.TryDaze(entityUid, stab.Comp.DazeTime, true);
          }
        }
        this._cameraShake.ShakeCamera(entityUid, 2, 1);
        if (!this.HasComp<XenoComponent>(entityUid))
        {
          NeurotoxinInjectorComponent comp4;
          if (stab.Comp.InjectNeuro && this.TryComp<NeurotoxinInjectorComponent>((EntityUid) stab, out comp4))
          {
            NeurotoxinComponent comp5;
            if (!this.EnsureComp<NeurotoxinComponent>(entityUid, out comp5))
            {
              comp5.LastMessage = this._timing.CurTime;
              comp5.LastAccentTime = this._timing.CurTime;
              comp5.LastStumbleTime = this._timing.CurTime;
            }
            comp5.NeurotoxinAmount += comp4.NeuroPerSecond;
            comp5.ToxinDamage = comp4.ToxinDamage;
            comp5.OxygenDamage = comp4.OxygenDamage;
            comp5.CoughDamage = comp4.CoughDamage;
          }
          else
          {
            Entity<SolutionComponent>? soln;
            if (stab.Comp.Inject != null && this._solutionContainer.TryGetInjectableSolution((Entity<InjectableSolutionComponent, SolutionContainerManagerComponent>) entityUid, out soln, out Solution _))
            {
              FixedPoint2 zero2 = FixedPoint2.Zero;
              foreach (FixedPoint2 fixedPoint2 in stab.Comp.Inject.Values)
                zero2 += fixedPoint2;
              FixedPoint2 availableVolume = soln.Value.Comp.Solution.AvailableVolume;
              if (availableVolume < zero2)
                this._solutionContainer.SplitSolution(soln.Value, zero2 - availableVolume);
              ProtoId<ReagentPrototype> protoId2;
              foreach ((protoId2, zero3) in stab.Comp.Inject)
              {
                FixedPoint2 quantity = zero3;
                this._solutionContainer.TryAddReagent(soln.Value, (string) protoId2, quantity);
              }
            }
          }
        }
        string message = this.Loc.GetString("rmc-xeno-tail-stab-self", ("target", (object) Identity.Name(entityUid, (IEntityManager) this.EntityManager, new EntityUid?((EntityUid) stab))));
        if (this._net.IsServer)
          this._popup.PopupEntity(message, (EntityUid) stab, (EntityUid) stab);
        this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-tail-stab-target", ("user", (object) Identity.Name((EntityUid) stab, (IEntityManager) this.EntityManager, new EntityUid?(entityUid)))), (EntityUid) stab, entityUid, PopupType.MediumCaution);
        Filter filter3 = Filter.PvsExcept((EntityUid) stab).RemovePlayerByAttachedEntity(entityUid);
        foreach (ICommonSession recipient in filter3.Recipients)
        {
          EntityUid? attachedEntity = recipient.AttachedEntity;
          if (attachedEntity.HasValue)
          {
            EntityUid valueOrDefault = attachedEntity.GetValueOrDefault();
            this._popup.PopupEntity(this.Loc.GetString("rmc-xeno-tail-stab-others", ("user", (object) Identity.Name((EntityUid) stab, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault))), ("target", (object) Identity.Name(entityUid, (IEntityManager) this.EntityManager, new EntityUid?(valueOrDefault)))), (EntityUid) stab, filter3, true, PopupType.SmallCaution);
          }
        }
      }
    }
    if (this._net.IsServer)
    {
      if (args.Entity.HasValue && !this.TerminatingOrDeleted(args.Entity))
      {
        Angle worldRotation = this._transform.GetWorldRotation((EntityUid) stab);
        Angle angle = Angle.op_Subtraction(DirectionExtensions.ToAngle(((Angle) ref worldRotation).GetDir()), Angle.FromDegrees(180.0));
        this._rotate.RotateXeno((EntityUid) stab, ((Angle) ref angle).GetDir());
      }
      SoundSpecifier sound;
      if (args.Entity.HasValue & flag && !this.TerminatingOrDeleted(args.Entity))
      {
        EntityUid? entity = args.Entity;
        EntityUid entityUid = (EntityUid) stab;
        if ((entity.HasValue ? (entity.GetValueOrDefault() != entityUid ? 1 : 0) : 1) != 0)
        {
          sound = stab.Comp.SoundHit;
          goto label_69;
        }
      }
      sound = stab.Comp.SoundMiss;
label_69:
      this._audio.PlayPvs(sound, (EntityUid) stab);
    }
    MeleeAttackEvent args5 = new MeleeAttackEvent((EntityUid) stab);
    this.RaiseLocalEvent<MeleeAttackEvent>((EntityUid) stab, ref args5);
  }

  protected virtual void DoLunge(
    Entity<XenoTailStabComponent, TransformComponent> user,
    Vector2 localPos,
    EntProtoId animationId)
  {
  }
}
