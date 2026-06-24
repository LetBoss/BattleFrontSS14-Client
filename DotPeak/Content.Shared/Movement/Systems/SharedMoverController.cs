// Decompiled with JetBrains decompiler
// Type: Content.Shared.Movement.Systems.SharedMoverController
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared._PUBG.Movement;
using Content.Shared.ActionBlocker;
using Content.Shared.Alert;
using Content.Shared.CCVar;
using Content.Shared.Follower.Components;
using Content.Shared.Friction;
using Content.Shared.Gravity;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Maps;
using Content.Shared.Mobs.Systems;
using Content.Shared.Movement.Components;
using Content.Shared.Movement.Events;
using Content.Shared.Movement.Pulling.Components;
using Content.Shared.Tag;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Configuration;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Input;
using Robust.Shared.Input.Binding;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Map.Enumerators;
using Robust.Shared.Maths;
using Robust.Shared.Physics;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Controllers;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

#nullable enable
namespace Content.Shared.Movement.Systems;

public abstract class SharedMoverController : VirtualController
{
  [Dependency]
  private IConfigurationManager _configManager;
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private ITileDefinitionManager _tileDefinitionManager;
  [Dependency]
  private ActionBlockerSystem _blocker;
  [Dependency]
  private EntityLookupSystem _lookup;
  [Dependency]
  private InventorySystem _inventory;
  [Dependency]
  private MobStateSystem _mobState;
  [Dependency]
  private SharedAudioSystem _audio;
  [Dependency]
  private SharedContainerSystem _container;
  [Dependency]
  private SharedMapSystem _mapSystem;
  [Dependency]
  private SharedGravitySystem _gravity;
  [Dependency]
  private SharedTransformSystem _transform;
  [Dependency]
  private TagSystem _tags;
  [Dependency]
  private FootstepCacheSystem _footstepCache;
  protected Robust.Shared.GameObjects.EntityQuery<CanMoveInAirComponent> CanMoveInAirQuery;
  protected Robust.Shared.GameObjects.EntityQuery<FootstepModifierComponent> FootstepModifierQuery;
  protected Robust.Shared.GameObjects.EntityQuery<FootstepVolumeModifierComponent> FootstepVolumeModifierQuery;
  protected Robust.Shared.GameObjects.EntityQuery<InputMoverComponent> MoverQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MapComponent> MapQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MapGridComponent> MapGridQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MobMoverComponent> MobMoverQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MovementRelayTargetComponent> RelayTargetQuery;
  protected Robust.Shared.GameObjects.EntityQuery<MovementSpeedModifierComponent> ModifierQuery;
  protected Robust.Shared.GameObjects.EntityQuery<NoRotateOnMoveComponent> NoRotateQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PhysicsComponent> PhysicsQuery;
  protected Robust.Shared.GameObjects.EntityQuery<RelayInputMoverComponent> RelayQuery;
  protected Robust.Shared.GameObjects.EntityQuery<PullableComponent> PullableQuery;
  protected Robust.Shared.GameObjects.EntityQuery<TransformComponent> XformQuery;
  private static readonly ProtoId<TagPrototype> FootstepSoundTag = (ProtoId<TagPrototype>) "FootstepSound";
  private bool _relativeMovement;
  private float _minDamping;
  private float _airDamping;
  private float _offGridDamping;
  public Dictionary<EntityUid, bool> UsedMobMovement = new Dictionary<EntityUid, bool>();
  public static ProtoId<AlertPrototype> WalkingAlert = (ProtoId<AlertPrototype>) "Walking";

  public override void Initialize()
  {
    this.UpdatesBefore.Add(typeof (TileFrictionController));
    base.Initialize();
    this.MoverQuery = this.GetEntityQuery<InputMoverComponent>();
    this.MobMoverQuery = this.GetEntityQuery<MobMoverComponent>();
    this.ModifierQuery = this.GetEntityQuery<MovementSpeedModifierComponent>();
    this.RelayTargetQuery = this.GetEntityQuery<MovementRelayTargetComponent>();
    this.PhysicsQuery = this.GetEntityQuery<PhysicsComponent>();
    this.RelayQuery = this.GetEntityQuery<RelayInputMoverComponent>();
    this.PullableQuery = this.GetEntityQuery<PullableComponent>();
    this.XformQuery = this.GetEntityQuery<TransformComponent>();
    this.NoRotateQuery = this.GetEntityQuery<NoRotateOnMoveComponent>();
    this.CanMoveInAirQuery = this.GetEntityQuery<CanMoveInAirComponent>();
    this.FootstepModifierQuery = this.GetEntityQuery<FootstepModifierComponent>();
    this.FootstepVolumeModifierQuery = this.GetEntityQuery<FootstepVolumeModifierComponent>();
    this.MapGridQuery = this.GetEntityQuery<MapGridComponent>();
    this.MapQuery = this.GetEntityQuery<MapComponent>();
    this.SubscribeLocalEvent<MovementSpeedModifierComponent, TileFrictionEvent>(new EntityEventRefHandler<MovementSpeedModifierComponent, TileFrictionEvent>(this.OnTileFriction));
    this.InitializeInput();
    this.InitializeRelay();
    this.Subs.CVar<bool>(this._configManager, CCVars.RelativeMovement, (Action<bool>) (value => this._relativeMovement = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.MinFriction, (Action<float>) (value => this._minDamping = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.AirFriction, (Action<float>) (value => this._airDamping = value), true);
    this.Subs.CVar<float>(this._configManager, CCVars.OffgridFriction, (Action<float>) (value => this._offGridDamping = value), true);
  }

  public override void Shutdown()
  {
    base.Shutdown();
    this.ShutdownInput();
  }

  public override void UpdateAfterSolve(bool prediction, float frameTime)
  {
    base.UpdateAfterSolve(prediction, frameTime);
    this.UsedMobMovement.Clear();
  }

  protected void HandleMobMovement(Entity<InputMoverComponent> entity, float frameTime)
  {
    EntityUid owner = entity.Owner;
    InputMoverComponent comp = entity.Comp;
    RelayInputMoverComponent component1;
    if (this.RelayQuery.TryComp(owner, out component1))
    {
      InputMoverComponent component2;
      if (!this.MoverQuery.TryComp(component1.RelayEntity, out component2))
        return;
      this.LerpRotation(owner, comp, frameTime);
      bool flag = false;
      EntityUid? relativeEntity1 = component2.RelativeEntity;
      EntityUid? relativeEntity2 = comp.RelativeEntity;
      if ((relativeEntity1.HasValue == relativeEntity2.HasValue ? (relativeEntity1.HasValue ? (relativeEntity1.GetValueOrDefault() != relativeEntity2.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        component2.RelativeEntity = comp.RelativeEntity;
        flag = true;
      }
      if (Angle.op_Inequality(component2.RelativeRotation, comp.RelativeRotation))
      {
        component2.RelativeRotation = comp.RelativeRotation;
        flag = true;
      }
      if (Angle.op_Inequality(component2.TargetRelativeRotation, comp.TargetRelativeRotation))
      {
        component2.TargetRelativeRotation = comp.TargetRelativeRotation;
        flag = true;
      }
      if (component2.CanMove != comp.CanMove)
      {
        component2.CanMove = comp.CanMove;
        flag = true;
      }
      if (!flag)
        return;
      this.Dirty(component1.RelayEntity, (IComponent) component2);
    }
    else
    {
      TransformComponent component3;
      if (!this.XformQuery.TryComp(entity.Owner, out component3))
        return;
      MovementRelayTargetComponent component4;
      this.RelayTargetQuery.TryComp(owner, out component4);
      EntityUid? nullable1 = new EntityUid?();
      if (component4 != null && this.EnsureValidRelayTarget(owner, component4))
        nullable1 = new EntityUid?(component4.Source);
      if (!nullable1.HasValue)
      {
        if (comp.LerpTarget < this.Timing.CurTime)
          this.TryUpdateRelative(owner, comp, component3);
        this.LerpRotation(owner, comp, frameTime);
      }
      PhysicsComponent component5;
      PullableComponent component6;
      if (!comp.CanMove || !this.PhysicsQuery.TryComp(owner, out component5) || this.PullableQuery.TryGetComponent(owner, out component6) && component6.BeingPulled)
      {
        this.UsedMobMovement[owner] = false;
      }
      else
      {
        bool weightless = this._gravity.IsWeightless(owner, component5, component3);
        bool flag1 = false;
        if (component5.BodyStatus != BodyStatus.OnGround && !this.CanMoveInAirQuery.HasComponent(owner))
        {
          if (!weightless)
          {
            this.UsedMobMovement[owner] = false;
            return;
          }
          flag1 = true;
        }
        this.UsedMobMovement[owner] = true;
        MovementSpeedModifierComponent modifierComponent = this.ModifierQuery.CompOrNull(owner);
        Vector2 linearVelocity = component5.LinearVelocity;
        ContentTileDefinition tileDef = (ContentTileDefinition) null;
        bool flag2 = false;
        Vector2 velocity;
        float val1;
        float? nullable2;
        float num1;
        if (weightless | flag1)
        {
          float walkSpeed = modifierComponent != null ? modifierComponent.WeightlessWalkSpeed : 2.5f;
          float sprintSpeed = modifierComponent != null ? modifierComponent.WeightlessSprintSpeed : 4.5f;
          velocity = this.AssertValidWish(comp, walkSpeed, sprintSpeed);
          CanWeightlessMoveEvent args = new CanWeightlessMoveEvent(owner);
          this.RaiseLocalEvent<CanWeightlessMoveEvent>(owner, ref args, true);
          flag2 = args.CanMove || component3.GridUid.HasValue || this.MapGridQuery.HasComp(component3.GridUid);
          MobMoverComponent component7;
          if (!flag2 && this.MobMoverQuery.TryComp(owner, out component7))
            flag2 |= this.IsAroundCollider(this._lookup, (Entity<PhysicsComponent, MobMoverComponent, TransformComponent>) (owner, component5, component7, component3));
          if (flag2)
          {
            flag2 = true;
            val1 = !(velocity != Vector2.Zero) ? (modifierComponent != null ? modifierComponent.WeightlessFrictionNoInput : this._airDamping) : (modifierComponent != null ? modifierComponent.WeightlessFriction : this._airDamping);
          }
          else
          {
            nullable2 = (float?) modifierComponent?.OffGridFriction;
            val1 = (float) ((double) nullable2 ?? (double) this._offGridDamping);
          }
          num1 = modifierComponent != null ? modifierComponent.WeightlessAcceleration : 1f;
        }
        else
        {
          MapGridComponent component8;
          TileRef tile;
          if (this.MapGridQuery.TryComp(component3.GridUid, out component8) && this._mapSystem.TryGetTileRef(component3.GridUid.Value, component8, component3.Coordinates, out tile) && component5.BodyStatus == BodyStatus.OnGround)
            tileDef = (ContentTileDefinition) this._tileDefinitionManager[tile.Tile.TypeId];
          float walkSpeed = modifierComponent != null ? modifierComponent.CurrentWalkSpeed : 2.5f;
          float sprintSpeed = modifierComponent != null ? modifierComponent.CurrentSprintSpeed : 4.5f;
          velocity = this.AssertValidWish(comp, walkSpeed, sprintSpeed);
          if (velocity != Vector2.Zero)
          {
            double num2 = modifierComponent != null ? (double) modifierComponent.Friction : 2.5;
            nullable2 = (float?) tileDef?.MobFriction;
            double num3 = (double) nullable2 ?? (tileDef != null ? (double) tileDef.Friction : 1.0);
            val1 = (float) (num2 * num3);
          }
          else
            val1 = (float) ((modifierComponent != null ? (double) modifierComponent.FrictionNoInput : 2.5) * (tileDef != null ? (double) tileDef.Friction : 1.0));
          double num4 = modifierComponent != null ? (double) modifierComponent.Acceleration : 20.0;
          float? nullable3;
          if (tileDef == null)
          {
            nullable2 = new float?();
            nullable3 = nullable2;
          }
          else
            nullable3 = tileDef.MobAcceleration;
          nullable2 = nullable3;
          double num5 = (double) (nullable2 ?? 1f);
          num1 = (float) (num4 * num5);
        }
        if (velocity != Vector2.Zero)
          val1 = Math.Min(val1, num1);
        float friction = Math.Max(val1, this._minDamping);
        this.Friction(modifierComponent != null ? modifierComponent.MinimumFrictionSpeed : 0.005f, frameTime, friction, ref linearVelocity);
        if (!weightless | flag2)
          SharedMoverController.Accelerate(ref linearVelocity, in velocity, num1, frameTime);
        this.SetWishDir((Entity<InputMoverComponent>) (owner, comp), velocity);
        this.PhysicsSystem.SetLinearVelocity(owner, linearVelocity, body: component5);
        this.PhysicsSystem.SetAngularVelocity(owner, 0.0f, body: component5);
        if (!(velocity != Vector2.Zero))
          return;
        if (!this.NoRotateQuery.HasComponent(owner))
        {
          Angle worldRotation = this._transform.GetWorldRotation(component3);
          this._transform.SetLocalRotation(owner, Angle.op_Subtraction(Angle.op_Addition(component3.LocalRotation, DirectionExtensions.ToWorldAngle(velocity)), worldRotation), component3);
        }
        MobMoverComponent component9;
        SoundSpecifier sound;
        if (weightless || !this.MobMoverQuery.TryGetComponent(owner, out component9) || !this.TryGetSound(weightless, owner, comp, component9, component3, out sound, tileDef))
          return;
        float num6 = comp.Sprinting ? 3.5f : 1.5f;
        float volume = sound.Params.Volume + num6;
        float? nullable4 = new float?();
        FootstepVolumeModifierComponent component10;
        if (this.FootstepVolumeModifierQuery.TryGetComponent(owner, out component10))
        {
          volume += comp.Sprinting ? component10.SprintVolumeModifier : component10.WalkVolumeModifier;
          float num7 = comp.Sprinting ? component10.SprintMaxDistance : component10.WalkMaxDistance;
          if ((double) num7 > 0.0)
            nullable4 = new float?(num7);
        }
        AudioParams audioParams1 = sound.Params.WithVolume(volume);
        ref AudioParams local = ref audioParams1;
        nullable2 = sound.Params.Variation;
        float? variation = new float?((float) ((double) nullable2 ?? (double) component9.FootstepVariation));
        AudioParams audioParams2 = local.WithVariation(variation);
        if (nullable4.HasValue)
          audioParams2 = audioParams2.WithMaxDistance(nullable4.Value);
        if (nullable1.HasValue)
          this._audio.PlayPredicted(sound, owner, new EntityUid?(nullable1.Value), new AudioParams?(audioParams2));
        else
          this._audio.PlayPredicted(sound, owner, new EntityUid?(owner), new AudioParams?(audioParams2));
      }
    }
  }

  public Vector2 GetWishDir(Entity<InputMoverComponent?> mover)
  {
    return !this.MoverQuery.Resolve(mover.Owner, ref mover.Comp, false) ? Vector2.Zero : mover.Comp.WishDir;
  }

  public void SetWishDir(Entity<InputMoverComponent> mover, Vector2 wishDir)
  {
    if (mover.Comp.WishDir.Equals(wishDir))
      return;
    mover.Comp.WishDir = wishDir;
    this.Dirty<InputMoverComponent>(mover);
  }

  public void LerpRotation(EntityUid uid, InputMoverComponent mover, float frameTime)
  {
    Angle angle1 = Angle.ShortestDistance(ref mover.RelativeRotation, ref mover.TargetRelativeRotation);
    if (!((Angle) ref angle1).EqualsApprox(Angle.Zero, 0.001))
    {
      double val1 = Angle.op_Implicit(angle1) * 5.0 * (double) frameTime;
      double val2 = 0.01 * (double) frameTime;
      double num = Angle.op_Implicit(angle1) >= 0.0 ? Math.Clamp(Math.Max(val1, val2), Angle.op_Implicit(Angle.op_UnaryNegation(angle1)), Angle.op_Implicit(angle1)) : Math.Clamp(Math.Min(val1, -val2), Angle.op_Implicit(angle1), Angle.op_Implicit(Angle.op_UnaryNegation(angle1)));
      InputMoverComponent inputMoverComponent = mover;
      Angle angle2 = Angle.op_Addition(mover.RelativeRotation, Angle.op_Implicit(num));
      Angle angle3 = ((Angle) ref angle2).FlipPositive();
      inputMoverComponent.RelativeRotation = angle3;
      this.Dirty(uid, (IComponent) mover);
    }
    else
    {
      if (((Angle) ref angle1).Equals(Angle.Zero))
        return;
      mover.RelativeRotation = ((Angle) ref mover.TargetRelativeRotation).FlipPositive();
      this.Dirty(uid, (IComponent) mover);
    }
  }

  public void Friction(
    float minimumFrictionSpeed,
    float frameTime,
    float friction,
    ref Vector2 velocity)
  {
    if ((double) velocity.Length() < (double) minimumFrictionSpeed)
      return;
    velocity *= Math.Clamp((float) (1.0 - (double) frameTime * (double) friction), 0.0f, 1f);
  }

  public void Friction(
    float minimumFrictionSpeed,
    float frameTime,
    float friction,
    ref float velocity)
  {
    if ((double) Math.Abs(velocity) < (double) minimumFrictionSpeed)
      return;
    velocity *= Math.Clamp((float) (1.0 - (double) frameTime * (double) friction), 0.0f, 1f);
  }

  public static void Accelerate(
    ref Vector2 currentVelocity,
    in Vector2 velocity,
    float accel,
    float frameTime)
  {
    Vector2 vector2 = velocity != Vector2.Zero ? Vector2Helpers.Normalized(velocity) : Vector2.Zero;
    float num1 = velocity.Length();
    float num2 = Vector2.Dot(currentVelocity, vector2);
    float y = num1 - num2;
    if ((double) y <= 0.0)
      return;
    float num3 = MathF.Min(accel * frameTime * num1, y);
    currentVelocity += vector2 * num3;
  }

  public bool UseMobMovement(EntityUid uid)
  {
    bool flag;
    return this.UsedMobMovement.TryGetValue(uid, out flag) & flag;
  }

  private bool IsAroundCollider(
    EntityLookupSystem lookupSystem,
    Entity<PhysicsComponent, MobMoverComponent, TransformComponent> entity)
  {
    (EntityUid owner, PhysicsComponent comp1, MobMoverComponent comp2, TransformComponent transformComponent) = entity;
    Box2 worldAabb = this._lookup.GetWorldAABB(entity.Owner, transformComponent);
    Box2 worldAABB = ((Box2) ref worldAabb).Enlarged(comp2.GrabRange);
    foreach (EntityUid uid in lookupSystem.GetEntitiesIntersecting(transformComponent.MapID, worldAABB))
    {
      PhysicsComponent component;
      PullableComponent comp;
      if (!(uid == owner) && this.PhysicsQuery.TryComp(uid, out component) && component.BodyType == BodyType.Static && component.CanCollide && ((comp1.CollisionMask & component.CollisionLayer) != 0 || (component.CollisionMask & comp1.CollisionLayer) != 0) && (!this.TryComp<PullableComponent>(uid, out comp) || !comp.BeingPulled))
        return true;
    }
    return false;
  }

  protected abstract bool CanSound();

  private bool TryGetSound(
    bool weightless,
    EntityUid uid,
    InputMoverComponent mover,
    MobMoverComponent mobMover,
    TransformComponent xform,
    [NotNullWhen(true)] out SoundSpecifier? sound,
    ContentTileDefinition? tileDef = null)
  {
    sound = (SoundSpecifier) null;
    if (!this.CanSound() || !this._tags.HasTag(uid, SharedMoverController.FootstepSoundTag))
      return false;
    EntityCoordinates coordinates = xform.Coordinates;
    float num = mover.Sprinting ? mobMover.StepSoundMoveDistanceRunning : mobMover.StepSoundMoveDistanceWalking;
    if (weightless)
      return false;
    float distance;
    if (!coordinates.TryDistance((IEntityManager) this.EntityManager, mobMover.LastPosition, out distance) || (double) distance > (double) num)
      mobMover.StepSoundDistance = num;
    else
      mobMover.StepSoundDistance += distance;
    mobMover.LastPosition = coordinates;
    if ((double) mobMover.StepSoundDistance < (double) num)
      return false;
    mobMover.StepSoundDistance -= num;
    FootstepModifierComponent component1;
    if (this.FootstepModifierQuery.TryComp(uid, out component1))
    {
      sound = component1.FootstepSoundCollection;
      return sound != null;
    }
    EntityUid? entityUid;
    FootstepModifierComponent component2;
    if (!this._inventory.TryGetSlotEntity(uid, "shoes", out entityUid) || !this.FootstepModifierQuery.TryComp(entityUid, out component2))
      return this.TryGetFootstepSound(uid, xform, entityUid.HasValue, out sound, tileDef);
    sound = component2.FootstepSoundCollection;
    return sound != null;
  }

  private bool TryGetFootstepSound(
    EntityUid uid,
    TransformComponent xform,
    bool haveShoes,
    [NotNullWhen(true)] out SoundSpecifier? sound,
    ContentTileDefinition? tileDef = null)
  {
    sound = (SoundSpecifier) null;
    MapGridComponent component1;
    if (!this.MapGridQuery.TryComp(xform.GridUid, out component1))
    {
      FootstepModifierComponent component2;
      if (this.FootstepModifierQuery.TryComp(xform.MapUid, out component2))
        sound = component2.FootstepSoundCollection;
      return sound != null;
    }
    Vector2i tile1 = this._mapSystem.LocalToTile(xform.GridUid.Value, component1, xform.Coordinates);
    if (this._footstepCache.TryGetCachedSound(uid, xform.GridUid.Value, tile1, out sound))
      return sound != null;
    GetFootstepSoundEvent args = new GetFootstepSoundEvent(uid);
    AnchoredEntitiesEnumerator entitiesEnumerator = this._mapSystem.GetAnchoredEntitiesEnumerator(xform.GridUid.Value, component1, tile1);
    EntityUid? uid1;
    if (!entitiesEnumerator.MoveNext(out uid1))
    {
      TileRef tile2;
      if (tileDef == null && this._mapSystem.TryGetTileRef(xform.GridUid.Value, component1, tile1, out tile2))
        tileDef = (ContentTileDefinition) this._tileDefinitionManager[tile2.Tile.TypeId];
      if (tileDef != null)
      {
        sound = haveShoes ? tileDef.FootstepSounds : tileDef.BarestepSounds;
        this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
      }
      return sound != null;
    }
    this.RaiseLocalEvent<GetFootstepSoundEvent>(uid1.Value, ref args);
    if (args.Sound != null)
    {
      sound = args.Sound;
      this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
      return true;
    }
    FootstepModifierComponent component3;
    if (this.FootstepModifierQuery.TryComp(uid1, out component3))
    {
      sound = component3.FootstepSoundCollection;
      this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
      return sound != null;
    }
    EntityUid? uid2;
    while (entitiesEnumerator.MoveNext(out uid2))
    {
      this.RaiseLocalEvent<GetFootstepSoundEvent>(uid2.Value, ref args);
      if (args.Sound != null)
      {
        sound = args.Sound;
        this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
        return true;
      }
      FootstepModifierComponent component4;
      if (this.FootstepModifierQuery.TryComp(uid2, out component4))
      {
        sound = component4.FootstepSoundCollection;
        this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
        return sound != null;
      }
    }
    TileRef tile3;
    if (tileDef == null && this._mapSystem.TryGetTileRef(xform.GridUid.Value, component1, tile1, out tile3))
      tileDef = (ContentTileDefinition) this._tileDefinitionManager[tile3.Tile.TypeId];
    if (tileDef == null)
      return false;
    sound = haveShoes ? tileDef.FootstepSounds : tileDef.BarestepSounds;
    this._footstepCache.SetCachedSound(uid, xform.GridUid.Value, tile1, sound);
    return sound != null;
  }

  private Vector2 AssertValidWish(InputMoverComponent mover, float walkSpeed, float sprintSpeed)
  {
    (Vector2 Walking, Vector2 Sprinting) = this.GetVelocityInput(mover);
    Vector2 vector2 = Walking * walkSpeed + Sprinting * sprintSpeed;
    Angle parentGridAngle = this.GetParentGridAngle(mover);
    return !this._relativeMovement ? vector2 : ((Angle) ref parentGridAngle).RotateVec(ref vector2);
  }

  private void OnTileFriction(
    Entity<MovementSpeedModifierComponent> ent,
    ref TileFrictionEvent args)
  {
    PhysicsComponent comp;
    TransformComponent component;
    if (!this.TryComp<PhysicsComponent>((EntityUid) ent, out comp) || !this.XformQuery.TryComp((EntityUid) ent, out component))
      return;
    if (comp.BodyStatus != BodyStatus.OnGround || this._gravity.IsWeightless((EntityUid) ent, comp, component))
      args.Modifier *= ent.Comp.BaseWeightlessFriction;
    else
      args.Modifier *= ent.Comp.BaseFriction;
  }

  public bool CameraRotationLocked { get; set; }

  private void InitializeInput()
  {
    SharedMoverController.MoverDirInputCmdHandler command1 = new SharedMoverController.MoverDirInputCmdHandler(this, (Direction) 4);
    SharedMoverController.MoverDirInputCmdHandler command2 = new SharedMoverController.MoverDirInputCmdHandler(this, (Direction) 6);
    SharedMoverController.MoverDirInputCmdHandler command3 = new SharedMoverController.MoverDirInputCmdHandler(this, (Direction) 2);
    SharedMoverController.MoverDirInputCmdHandler command4 = new SharedMoverController.MoverDirInputCmdHandler(this, (Direction) 0);
    CommandBinds.Builder.Bind(EngineKeyFunctions.MoveUp, (InputCmdHandler) command1).Bind(EngineKeyFunctions.MoveLeft, (InputCmdHandler) command2).Bind(EngineKeyFunctions.MoveRight, (InputCmdHandler) command3).Bind(EngineKeyFunctions.MoveDown, (InputCmdHandler) command4).Bind(EngineKeyFunctions.Walk, (InputCmdHandler) new SharedMoverController.WalkInputCmdHandler(this)).Bind(EngineKeyFunctions.CameraRotateLeft, (InputCmdHandler) new SharedMoverController.CameraRotateInputCmdHandler(this, (Direction) 2)).Bind(EngineKeyFunctions.CameraRotateRight, (InputCmdHandler) new SharedMoverController.CameraRotateInputCmdHandler(this, (Direction) 6)).Bind(EngineKeyFunctions.CameraReset, (InputCmdHandler) new SharedMoverController.CameraResetInputCmdHandler(this)).Bind(ContentKeyFunctions.ShuttleStrafeUp, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeUp)).Bind(ContentKeyFunctions.ShuttleStrafeLeft, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeLeft)).Bind(ContentKeyFunctions.ShuttleStrafeRight, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeRight)).Bind(ContentKeyFunctions.ShuttleStrafeDown, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.StrafeDown)).Bind(ContentKeyFunctions.ShuttleRotateLeft, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.RotateLeft)).Bind(ContentKeyFunctions.ShuttleRotateRight, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.RotateRight)).Bind(ContentKeyFunctions.ShuttleBrake, (InputCmdHandler) new SharedMoverController.ShuttleInputCmdHandler(this, ShuttleButtons.Brake)).Register<SharedMoverController>();
    this.SubscribeLocalEvent<InputMoverComponent, ComponentInit>(new EntityEventRefHandler<InputMoverComponent, ComponentInit>(this.OnInputInit));
    this.SubscribeLocalEvent<InputMoverComponent, ComponentGetState>(new EntityEventRefHandler<InputMoverComponent, ComponentGetState>(this.OnMoverGetState));
    this.SubscribeLocalEvent<InputMoverComponent, ComponentHandleState>(new EntityEventRefHandler<InputMoverComponent, ComponentHandleState>(this.OnMoverHandleState));
    this.SubscribeLocalEvent<InputMoverComponent, EntParentChangedMessage>(new EntityEventRefHandler<InputMoverComponent, EntParentChangedMessage>(this.OnInputParentChange));
    this.SubscribeLocalEvent<FollowedComponent, EntParentChangedMessage>(new EntityEventRefHandler<FollowedComponent, EntParentChangedMessage>(this.OnFollowedParentChange));
    this.Subs.CVar<bool>(this._configManager, CCVars.CameraRotationLocked, (Action<bool>) (obj => this.CameraRotationLocked = obj), true);
    this.Subs.CVar<bool>(this._configManager, CCVars.GameDiagonalMovement, (Action<bool>) (value => this.DiagonalMovementEnabled = value), true);
  }

  public static MoveButtons GetNormalizedMovement(MoveButtons buttons)
  {
    MoveButtons normalizedMovement = buttons;
    if ((normalizedMovement & (MoveButtons.Left | MoveButtons.Right)) == (MoveButtons.Left | MoveButtons.Right))
      normalizedMovement = normalizedMovement & ~MoveButtons.Left & ~MoveButtons.Right;
    if ((normalizedMovement & (MoveButtons.Up | MoveButtons.Down)) == (MoveButtons.Up | MoveButtons.Down))
      normalizedMovement = normalizedMovement & ~MoveButtons.Up & ~MoveButtons.Down;
    return normalizedMovement;
  }

  protected void SetMoveInput(Entity<InputMoverComponent> entity, MoveButtons buttons)
  {
    if (entity.Comp.HeldMoveButtons == buttons)
      return;
    MoveInputEvent args1 = new MoveInputEvent(entity, entity.Comp.HeldMoveButtons);
    entity.Comp.HeldMoveButtons = buttons;
    this.RaiseLocalEvent<MoveInputEvent>((EntityUid) entity, ref args1);
    this.Dirty((EntityUid) entity, (IComponent) entity.Comp);
    SpriteMoveEvent args2 = new SpriteMoveEvent(entity.Comp.HasDirectionalMovement);
    this.RaiseLocalEvent<SpriteMoveEvent>((EntityUid) entity, ref args2);
  }

  private void OnMoverHandleState(Entity<InputMoverComponent> entity, ref ComponentHandleState args)
  {
    if (!(args.Current is InputMoverComponentState current))
      return;
    entity.Comp.LerpTarget = current.LerpTarget;
    entity.Comp.RelativeRotation = current.RelativeRotation;
    entity.Comp.TargetRelativeRotation = current.TargetRelativeRotation;
    entity.Comp.CanMove = current.CanMove;
    entity.Comp.RelativeEntity = this.EnsureEntity<InputMoverComponent>(current.RelativeEntity, entity.Owner);
    entity.Comp.LastInputTick = GameTick.Zero;
    entity.Comp.LastInputSubTick = (ushort) 0;
    if (entity.Comp.HeldMoveButtons == current.HeldMoveButtons)
      return;
    MoveInputEvent args1 = new MoveInputEvent(entity, entity.Comp.HeldMoveButtons);
    entity.Comp.HeldMoveButtons = current.HeldMoveButtons;
    this.RaiseLocalEvent<MoveInputEvent>(entity.Owner, ref args1);
    SpriteMoveEvent args2 = new SpriteMoveEvent(entity.Comp.HasDirectionalMovement);
    this.RaiseLocalEvent<SpriteMoveEvent>((EntityUid) entity, ref args2);
  }

  private void OnMoverGetState(Entity<InputMoverComponent> entity, ref ComponentGetState args)
  {
    args.State = (IComponentState) new InputMoverComponentState()
    {
      CanMove = entity.Comp.CanMove,
      RelativeEntity = this.GetNetEntity(entity.Comp.RelativeEntity),
      LerpTarget = entity.Comp.LerpTarget,
      HeldMoveButtons = entity.Comp.HeldMoveButtons,
      RelativeRotation = entity.Comp.RelativeRotation,
      TargetRelativeRotation = entity.Comp.TargetRelativeRotation
    };
  }

  private void ShutdownInput() => CommandBinds.Unregister<SharedMoverController>();

  public bool DiagonalMovementEnabled { get; private set; }

  protected virtual void HandleShuttleInput(
    EntityUid uid,
    ShuttleButtons button,
    ushort subTick,
    bool state)
  {
  }

  public void RotateCamera(EntityUid uid, Angle angle)
  {
    InputMoverComponent component;
    if (this.CameraRotationLocked || !this.MoverQuery.TryGetComponent(uid, out component))
      return;
    InputMoverComponent inputMoverComponent = component;
    inputMoverComponent.TargetRelativeRotation = Angle.op_Addition(inputMoverComponent.TargetRelativeRotation, angle);
    this.Dirty(uid, (IComponent) component);
  }

  public void ResetCamera(EntityUid uid)
  {
    InputMoverComponent component;
    if (this.CameraRotationLocked || !this.MoverQuery.TryGetComponent(uid, out component) || !this.TryUpdateRelative(uid, component, this.XformQuery.GetComponent(uid)) && ((Angle) ref component.TargetRelativeRotation).Equals(Angle.Zero))
      return;
    component.LerpTarget = TimeSpan.Zero;
    component.TargetRelativeRotation = Angle.Zero;
    this.Dirty(uid, (IComponent) component);
  }

  private bool TryUpdateRelative(
    EntityUid uid,
    InputMoverComponent mover,
    TransformComponent xform)
  {
    EntityUid? uid1 = xform.GridUid;
    if (!uid1.HasValue)
      uid1 = xform.MapUid;
    if (mover.RelativeEntity.Equals((object) uid1))
      return false;
    Angle angle1 = Angle.Zero;
    Angle angle2 = Angle.Zero;
    TransformComponent component1;
    if (this.XformQuery.TryGetComponent(mover.RelativeEntity, out component1))
      angle1 = this._transform.GetWorldRotation(component1);
    TransformComponent component2;
    if (this.XformQuery.TryGetComponent(uid1, out component2))
      angle2 = this._transform.GetWorldRotation(component2);
    Angle angle3 = Angle.op_Subtraction(angle2, angle1);
    if (this.MapQuery.HasComp(uid1) && this.MapGridQuery.HasComp(mover.RelativeEntity))
    {
      InputMoverComponent inputMoverComponent = mover;
      inputMoverComponent.TargetRelativeRotation = Angle.op_Subtraction(inputMoverComponent.TargetRelativeRotation, angle3);
    }
    else if (this.MapGridQuery.HasComp(uid1) && (this.MapQuery.HasComp(mover.RelativeEntity) || this.MapGridQuery.HasComp(mover.RelativeEntity)))
    {
      Angle angle4 = Angle.op_Subtraction(mover.TargetRelativeRotation, angle3);
      Angle angle5 = DirectionExtensions.ToAngle(((Angle) ref angle4).GetCardinalDir());
      Angle angle6 = ((Angle) ref angle5).Reduced();
      mover.TargetRelativeRotation = angle6;
    }
    InputMoverComponent inputMoverComponent1 = mover;
    inputMoverComponent1.RelativeRotation = Angle.op_Subtraction(inputMoverComponent1.RelativeRotation, angle3);
    mover.RelativeEntity = uid1;
    this.Dirty(uid, (IComponent) mover);
    return true;
  }

  public Angle GetParentGridAngle(InputMoverComponent mover)
  {
    Angle relativeRotation = mover.RelativeRotation;
    TransformComponent component;
    return this.XformQuery.TryGetComponent(mover.RelativeEntity, out component) ? Angle.op_Addition(this._transform.GetWorldRotation(component), relativeRotation) : relativeRotation;
  }

  private void OnFollowedParentChange(
    Entity<FollowedComponent> entity,
    ref EntParentChangedMessage args)
  {
    foreach (EntityUid entityUid in entity.Comp.Following)
    {
      InputMoverComponent component;
      if (this.MoverQuery.TryGetComponent(entityUid, out component))
      {
        EntParentChangedMessage args1 = new EntParentChangedMessage(entityUid, new EntityUid?(), args.OldMapId, this.XformQuery.GetComponent(entityUid));
        this.OnInputParentChange((Entity<InputMoverComponent>) (entityUid, component), ref args1);
      }
    }
  }

  private void OnInputParentChange(
    Entity<InputMoverComponent> entity,
    ref EntParentChangedMessage args)
  {
    EntityUid? nullable1 = args.Transform.GridUid;
    if (!nullable1.HasValue)
      nullable1 = args.Transform.MapUid;
    if (entity.Comp.LifeStage < ComponentLifeStage.Running)
    {
      entity.Comp.RelativeEntity = nullable1;
      this.Dirty(entity.Owner, (IComponent) entity.Comp);
    }
    else
    {
      EntityUid? oldMapId = args.OldMapId;
      EntityUid? mapUid = args.Transform.MapUid;
      EntityUid? nullable2 = oldMapId;
      EntityUid? nullable3 = mapUid;
      if ((nullable2.HasValue == nullable3.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault() ? 1 : 0) : 0) : 1) != 0)
      {
        entity.Comp.RelativeEntity = nullable1;
        entity.Comp.TargetRelativeRotation = Angle.Zero;
        entity.Comp.RelativeRotation = Angle.Zero;
        entity.Comp.LerpTarget = TimeSpan.Zero;
        this.Dirty(entity.Owner, (IComponent) entity.Comp);
      }
      else
      {
        EntityUid? nullable4 = nullable1;
        nullable2 = entity.Comp.RelativeEntity;
        if ((nullable4.HasValue == nullable2.HasValue ? (nullable4.HasValue ? (nullable4.GetValueOrDefault() == nullable2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
        {
          if (!(entity.Comp.LerpTarget >= this.Timing.CurTime))
            return;
          entity.Comp.LerpTarget = TimeSpan.Zero;
          this.Dirty(entity.Owner, (IComponent) entity.Comp);
        }
        else
        {
          entity.Comp.LerpTarget = TimeSpan.FromSeconds(1.0) + this.Timing.CurTime;
          this.Dirty(entity.Owner, (IComponent) entity.Comp);
        }
      }
    }
  }

  private void HandleDirChange(EntityUid entity, Direction dir, ushort subTick, bool state)
  {
    RelayInputMoverComponent comp1;
    if (this.TryComp<RelayInputMoverComponent>(entity, out comp1))
    {
      InputMoverComponent component;
      if (this.MoverQuery.TryGetComponent(entity, out component))
        this.SetMoveInput((Entity<InputMoverComponent>) (entity, component), MoveButtons.None);
      if (this._mobState.IsIncapacitated(entity))
        return;
      this.HandleDirChange(comp1.RelayEntity, dir, subTick, state);
    }
    else
    {
      InputMoverComponent component;
      if (!this.MoverQuery.TryGetComponent(entity, out component))
        return;
      TransformComponent comp2;
      if (this._container.IsEntityInContainer(entity) && this.TryComp(entity, out comp2) && comp2.ParentUid.IsValid() && this._mobState.IsAlive(entity))
      {
        ContainerRelayMovementEntityEvent args = new ContainerRelayMovementEntityEvent(entity);
        this.RaiseLocalEvent<ContainerRelayMovementEntityEvent>(comp2.ParentUid, ref args);
      }
      this.SetVelocityDirection((Entity<InputMoverComponent>) (entity, component), dir, subTick, state);
    }
  }

  private void OnInputInit(Entity<InputMoverComponent> entity, ref ComponentInit args)
  {
    TransformComponent transformComponent = this.Transform(entity.Owner);
    if (!transformComponent.ParentUid.IsValid())
      return;
    entity.Comp.RelativeEntity = transformComponent.GridUid ?? transformComponent.MapUid;
    entity.Comp.TargetRelativeRotation = Angle.Zero;
  }

  private void HandleRunChange(EntityUid uid, ushort subTick, bool walking)
  {
    InputMoverComponent component;
    this.MoverQuery.TryGetComponent(uid, out component);
    RelayInputMoverComponent comp;
    if (this.TryComp<RelayInputMoverComponent>(uid, out comp))
    {
      if (component != null)
        this.SetMoveInput((Entity<InputMoverComponent>) (uid, component), MoveButtons.None);
      this.HandleRunChange(comp.RelayEntity, subTick, walking);
    }
    else
    {
      if (component == null)
        return;
      this.SetSprinting((Entity<InputMoverComponent>) (uid, component), subTick, walking);
    }
  }

  public (Vector2 Walking, Vector2 Sprinting) GetVelocityInput(InputMoverComponent mover)
  {
    if (!this.Timing.InSimulation)
    {
      Vector2 vector2 = this.DirVecForButtons(mover.HeldMoveButtons);
      return !mover.Sprinting ? (vector2, Vector2.Zero) : (Vector2.Zero, vector2);
    }
    Vector2 vector2_1;
    Vector2 vector2_2;
    float num;
    if (this.Timing.CurTick > mover.LastInputTick)
    {
      vector2_1 = Vector2.Zero;
      vector2_2 = Vector2.Zero;
      num = 1f;
    }
    else
    {
      vector2_1 = mover.CurTickWalkMovement;
      vector2_2 = mover.CurTickSprintMovement;
      num = (float) ((int) ushort.MaxValue - (int) mover.LastInputSubTick) / (float) ushort.MaxValue;
    }
    Vector2 vector2_3 = this.DirVecForButtons(mover.HeldMoveButtons) * num;
    if (mover.Sprinting)
      vector2_2 += vector2_3;
    else
      vector2_1 += vector2_3;
    return (vector2_1, vector2_2);
  }

  public void SetVelocityDirection(
    Entity<InputMoverComponent> entity,
    Direction direction,
    ushort subTick,
    bool enabled)
  {
    MoveButtons moveButtons;
    switch ((int) direction)
    {
      case 0:
        moveButtons = MoveButtons.Down;
        break;
      case 2:
        moveButtons = MoveButtons.Right;
        break;
      case 4:
        moveButtons = MoveButtons.Up;
        break;
      case 6:
        moveButtons = MoveButtons.Left;
        break;
      default:
        throw new ArgumentException(nameof (direction));
    }
    MoveButtons bit = moveButtons;
    this.SetMoveInput(entity, subTick, enabled, bit);
  }

  private void SetMoveInput(
    Entity<InputMoverComponent> entity,
    ushort subTick,
    bool enabled,
    MoveButtons bit)
  {
    this.ResetSubtick(entity.Comp);
    if ((int) subTick >= (int) entity.Comp.LastInputSubTick)
    {
      float num = (float) ((int) subTick - (int) entity.Comp.LastInputSubTick) / (float) ushort.MaxValue;
      // ISSUE: explicit reference operation
      ^ref (entity.Comp.Sprinting ? ref entity.Comp.CurTickSprintMovement : ref entity.Comp.CurTickWalkMovement) += this.DirVecForButtons(entity.Comp.HeldMoveButtons) * num;
      entity.Comp.LastInputSubTick = subTick;
    }
    MoveButtons heldMoveButtons = entity.Comp.HeldMoveButtons;
    MoveButtons buttons = !enabled ? heldMoveButtons & ~bit : heldMoveButtons | bit;
    this.SetMoveInput(entity, buttons);
  }

  private void ResetSubtick(InputMoverComponent component)
  {
    if (this.Timing.CurTick <= component.LastInputTick)
      return;
    component.CurTickWalkMovement = Vector2.Zero;
    component.CurTickSprintMovement = Vector2.Zero;
    component.LastInputTick = this.Timing.CurTick;
    component.LastInputSubTick = (ushort) 0;
  }

  public virtual void SetSprinting(
    Entity<InputMoverComponent> entity,
    ushort subTick,
    bool walking)
  {
    this.SetMoveInput(entity, subTick, walking, MoveButtons.Walk);
  }

  public Vector2 DirVecForButtons(MoveButtons buttons)
  {
    int x = -(SharedMoverController.HasFlag(buttons, MoveButtons.Left) ? 1 : 0) + (SharedMoverController.HasFlag(buttons, MoveButtons.Right) ? 1 : 0);
    int y = 0;
    if (this.DiagonalMovementEnabled || x == 0)
      y = y - (SharedMoverController.HasFlag(buttons, MoveButtons.Down) ? 1 : 0) + (SharedMoverController.HasFlag(buttons, MoveButtons.Up) ? 1 : 0);
    Vector2 vector2 = new Vector2((float) x, (float) y);
    if ((double) vector2.LengthSquared() > 1E-06)
      vector2 = Vector2Helpers.Normalized(vector2);
    return vector2;
  }

  private static bool HasFlag(MoveButtons buttons, MoveButtons flag) => (buttons & flag) == flag;

  private void InitializeRelay()
  {
    this.SubscribeLocalEvent<RelayInputMoverComponent, ComponentShutdown>(new EntityEventRefHandler<RelayInputMoverComponent, ComponentShutdown>(this.OnRelayShutdown));
    this.SubscribeLocalEvent<MovementRelayTargetComponent, ComponentShutdown>(new EntityEventRefHandler<MovementRelayTargetComponent, ComponentShutdown>(this.OnTargetRelayShutdown));
    this.SubscribeLocalEvent<MovementRelayTargetComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MovementRelayTargetComponent, AfterAutoHandleStateEvent>(this.OnAfterRelayTargetState));
    this.SubscribeLocalEvent<RelayInputMoverComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RelayInputMoverComponent, AfterAutoHandleStateEvent>(this.OnAfterRelayState));
  }

  private void OnAfterRelayTargetState(
    Entity<MovementRelayTargetComponent> entity,
    ref AfterAutoHandleStateEvent args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner));
    this.EnsureValidRelayTarget(entity.Owner, entity.Comp);
  }

  private void OnAfterRelayState(
    Entity<RelayInputMoverComponent> entity,
    ref AfterAutoHandleStateEvent args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner));
  }

  public void SetRelay(EntityUid uid, EntityUid relayEntity)
  {
    if (uid == relayEntity)
    {
      this.Log.Error($"An entity attempted to relay movement to itself. Entity:{this.ToPrettyString((Entity<MetaDataComponent>) uid)}");
    }
    else
    {
      RelayInputMoverComponent inputMoverComponent = this.EnsureComp<RelayInputMoverComponent>(uid);
      if (inputMoverComponent.RelayEntity == relayEntity)
        return;
      MovementRelayTargetComponent comp1;
      if (this.TryComp<MovementRelayTargetComponent>(inputMoverComponent.RelayEntity, out comp1))
      {
        comp1.Source = EntityUid.Invalid;
        this.RemComp(inputMoverComponent.RelayEntity, (IComponent) comp1);
        this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(inputMoverComponent.RelayEntity));
      }
      MovementRelayTargetComponent relayTargetComponent = this.EnsureComp<MovementRelayTargetComponent>(relayEntity);
      RelayInputMoverComponent comp2;
      if (this.TryComp<RelayInputMoverComponent>(relayTargetComponent.Source, out comp2))
      {
        comp2.RelayEntity = EntityUid.Invalid;
        this.RemComp(relayTargetComponent.Source, (IComponent) comp2);
        this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(relayTargetComponent.Source));
      }
      this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(uid));
      this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(relayEntity));
      inputMoverComponent.RelayEntity = relayEntity;
      relayTargetComponent.Source = uid;
      this.Dirty(uid, (IComponent) inputMoverComponent);
      this.Dirty(relayEntity, (IComponent) relayTargetComponent);
      this._blocker.UpdateCanMove(uid);
    }
  }

  private void OnRelayShutdown(Entity<RelayInputMoverComponent> entity, ref ComponentShutdown args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner));
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Comp.RelayEntity));
    InputMoverComponent comp1;
    if (this.TryComp<InputMoverComponent>(entity.Comp.RelayEntity, out comp1))
      this.SetMoveInput((Entity<InputMoverComponent>) (entity.Comp.RelayEntity, comp1), MoveButtons.None);
    if (this.Timing.ApplyingState)
      return;
    MovementRelayTargetComponent comp2;
    if (this.TryComp<MovementRelayTargetComponent>(entity.Comp.RelayEntity, out comp2) && comp2.LifeStage <= ComponentLifeStage.Running)
      this.RemComp(entity.Comp.RelayEntity, (IComponent) comp2);
    this._blocker.UpdateCanMove(entity.Owner);
  }

  private void OnTargetRelayShutdown(
    Entity<MovementRelayTargetComponent> entity,
    ref ComponentShutdown args)
  {
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Owner));
    this.PhysicsSystem.UpdateIsPredicted(new EntityUid?(entity.Comp.Source));
    RelayInputMoverComponent comp;
    if (this.Timing.ApplyingState || !this.TryComp<RelayInputMoverComponent>(entity.Comp.Source, out comp) || comp.LifeStage > ComponentLifeStage.Running)
      return;
    this.RemComp(entity.Comp.Source, (IComponent) comp);
  }

  private bool EnsureValidRelayTarget(EntityUid uid, MovementRelayTargetComponent relayTarget)
  {
    EntityUid source = relayTarget.Source;
    RelayInputMoverComponent component1;
    if ((!source.IsValid() || !this.RelayQuery.TryComp(source, out component1) ? 0 : (component1.RelayEntity == uid ? 1 : 0)) != 0)
      return true;
    InputMoverComponent component2;
    if (this.MoverQuery.TryComp(uid, out component2))
      this.SetMoveInput((Entity<InputMoverComponent>) (uid, component2), MoveButtons.None);
    if (!this.Timing.ApplyingState)
      this.RemCompDeferred<MovementRelayTargetComponent>(uid);
    return false;
  }

  private sealed class CameraRotateInputCmdHandler : InputCmdHandler
  {
    private readonly SharedMoverController _controller;
    private readonly Angle _angle;

    public CameraRotateInputCmdHandler(SharedMoverController controller, Direction direction)
    {
      this._controller = controller;
      this._angle = DirectionExtensions.ToAngle(direction);
    }

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      EntityUid? attachedEntity;
      int num;
      if (session == null)
      {
        num = 1;
      }
      else
      {
        attachedEntity = session.AttachedEntity;
        num = !attachedEntity.HasValue ? 1 : 0;
      }
      if (num != 0 || message.State != BoundKeyState.Up)
        return false;
      SharedMoverController controller = this._controller;
      attachedEntity = session.AttachedEntity;
      EntityUid uid = attachedEntity.Value;
      Angle angle = this._angle;
      controller.RotateCamera(uid, angle);
      return false;
    }
  }

  private sealed class CameraResetInputCmdHandler : InputCmdHandler
  {
    private readonly SharedMoverController _controller;

    public CameraResetInputCmdHandler(SharedMoverController controller)
    {
      this._controller = controller;
    }

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      EntityUid? attachedEntity;
      int num;
      if (session == null)
      {
        num = 1;
      }
      else
      {
        attachedEntity = session.AttachedEntity;
        num = !attachedEntity.HasValue ? 1 : 0;
      }
      if (num != 0 || message.State != BoundKeyState.Up)
        return false;
      SharedMoverController controller = this._controller;
      attachedEntity = session.AttachedEntity;
      EntityUid uid = attachedEntity.Value;
      controller.ResetCamera(uid);
      return false;
    }
  }

  private sealed class MoverDirInputCmdHandler : InputCmdHandler
  {
    private readonly SharedMoverController _controller;
    private readonly Direction _dir;

    public MoverDirInputCmdHandler(SharedMoverController controller, Direction dir)
    {
      this._controller = controller;
      this._dir = dir;
    }

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      EntityUid? attachedEntity;
      int num1;
      if (session == null)
      {
        num1 = 1;
      }
      else
      {
        attachedEntity = session.AttachedEntity;
        num1 = !attachedEntity.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        return false;
      SharedMoverController controller = this._controller;
      attachedEntity = session.AttachedEntity;
      EntityUid entity = attachedEntity.Value;
      Direction dir = this._dir;
      int subTick = (int) message.SubTick;
      int num2 = message.State == BoundKeyState.Down ? 1 : 0;
      controller.HandleDirChange(entity, dir, (ushort) subTick, num2 != 0);
      return false;
    }
  }

  private sealed class WalkInputCmdHandler : InputCmdHandler
  {
    private SharedMoverController _controller;

    public WalkInputCmdHandler(SharedMoverController controller) => this._controller = controller;

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      EntityUid? attachedEntity;
      int num1;
      if (session == null)
      {
        num1 = 1;
      }
      else
      {
        attachedEntity = session.AttachedEntity;
        num1 = !attachedEntity.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        return false;
      SharedMoverController controller = this._controller;
      attachedEntity = session.AttachedEntity;
      EntityUid uid = attachedEntity.Value;
      int subTick = (int) message.SubTick;
      int num2 = message.State == BoundKeyState.Down ? 1 : 0;
      controller.HandleRunChange(uid, (ushort) subTick, num2 != 0);
      return false;
    }
  }

  private sealed class ShuttleInputCmdHandler : InputCmdHandler
  {
    private readonly SharedMoverController _controller;
    private readonly ShuttleButtons _button;

    public ShuttleInputCmdHandler(SharedMoverController controller, ShuttleButtons button)
    {
      this._controller = controller;
      this._button = button;
    }

    public override bool HandleCmdMessage(
      IEntityManager entManager,
      ICommonSession? session,
      IFullInputCmdMessage message)
    {
      EntityUid? attachedEntity;
      int num1;
      if (session == null)
      {
        num1 = 1;
      }
      else
      {
        attachedEntity = session.AttachedEntity;
        num1 = !attachedEntity.HasValue ? 1 : 0;
      }
      if (num1 != 0)
        return false;
      SharedMoverController controller = this._controller;
      attachedEntity = session.AttachedEntity;
      EntityUid uid = attachedEntity.Value;
      int button = (int) this._button;
      int subTick = (int) message.SubTick;
      int num2 = message.State == BoundKeyState.Down ? 1 : 0;
      controller.HandleShuttleInput(uid, (ShuttleButtons) button, (ushort) subTick, num2 != 0);
      return false;
    }
  }
}
