// Decompiled with JetBrains decompiler
// Type: Content.Client.Weapons.Melee.MeleeWeaponSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Movement;
using Content.Client._RMC14.Weapons.Melee;
using Content.Client.Animations;
using Content.Client.Gameplay;
using Content.Client.Weapons.Melee.Components;
using Content.Shared._RMC14.Input;
using Content.Shared.ActionBlocker;
using Content.Shared.Effects;
using Content.Shared.Weapons.Melee;
using Content.Shared.Weapons.Melee.Components;
using Content.Shared.Weapons.Melee.Events;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Input;
using Robust.Client.Player;
using Robust.Client.State;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Weapons.Melee;

public sealed class MeleeWeaponSystem : SharedMeleeWeaponSystem
{
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private IInputManager _inputManager;
  [Dependency]
  private IPlayerManager _player;
  [Dependency]
  private IStateManager _stateManager;
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private InputSystem _inputSystem;
  [Dependency]
  private SharedColorFlashEffectSystem _color;
  [Dependency]
  private MapSystem _map;
  [Dependency]
  private SpriteSystem _sprite;
  private EntityQuery<TransformComponent> _xformQuery;
  private const string MeleeLungeKey = "melee-lunge";
  [Dependency]
  private RMCLagCompensationSystem _rmcLagCompensation;
  [Dependency]
  private RMCMeleeWeaponSystem _rmcMeleeWeapon;
  private const string FadeAnimationKey = "melee-fade";
  private const string SlashAnimationKey = "melee-slash";
  private const string ThrustAnimationKey = "melee-thrust";

  public override void Initialize()
  {
    base.Initialize();
    this._xformQuery = this.GetEntityQuery<TransformComponent>();
    this.SubscribeNetworkEvent<MeleeLungeEvent>(new EntityEventHandler<MeleeLungeEvent>(this.OnMeleeLunge), (Type[]) null, (Type[]) null);
    this.UpdatesOutsidePrediction = true;
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    this.UpdateEffects();
  }

  public virtual void Update(float frameTime)
  {
    base.Update(frameTime);
    if (!this.Timing.IsFirstTimePredicted)
      return;
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    if (!localEntity.HasValue)
      return;
    EntityUid entityUid1 = localEntity.Value;
    EntityUid weaponUid;
    MeleeWeaponComponent melee;
    if (!this.TryGetWeapon(entityUid1, out weaponUid, out melee))
      return;
    if (this.CombatMode.IsInCombatMode(new EntityUid?(entityUid1)))
    {
      ActionBlockerSystem blocker = this.Blocker;
      EntityUid uid = entityUid1;
      Entity<MeleeWeaponComponent>? nullable = new Entity<MeleeWeaponComponent>?(Entity<MeleeWeaponComponent>.op_Implicit((weaponUid, melee)));
      EntityUid? target = new EntityUid?();
      Entity<MeleeWeaponComponent>? weapon = nullable;
      if (blocker.CanAttack(uid, target, weapon))
      {
        BoundKeyState state1 = this._inputSystem.CmdStates.GetState(EngineKeyFunctions.Use);
        BoundKeyState state2 = this._inputSystem.CmdStates.GetState(EngineKeyFunctions.UseSecondary);
        BoundKeyState state3 = this._inputSystem.CmdStates.GetState(CMKeyFunctions.CMXenoWideSwing);
        if ((melee.AutoAttack || state1 != 1 && state2 != 1 && state3 != 1) && melee.Attacking)
          this.RaisePredictiveEvent<StopAttackEvent>(new StopAttackEvent(this.GetNetEntity(weaponUid, (MetaDataComponent) null)));
        if (melee.Attacking || melee.NextAttack > this.Timing.CurTime)
          return;
        MapCoordinates map = this._eyeManager.PixelToMap(this._inputManager.MouseScreenPosition);
        if (MapId.op_Equality(map.MapId, MapId.Nullspace))
          return;
        EntityUid entityUid2;
        MapGridComponent mapGridComponent;
        EntityCoordinates coordinates = !this.MapManager.TryFindGridAt(map, ref entityUid2, ref mapGridComponent) ? this.TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(((SharedMapSystem) this._map).GetMap(map.MapId)), map) : this.TransformSystem.ToCoordinates(Entity<TransformComponent>.op_Implicit(entityUid2), map);
        GunComponent gunComponent;
        if (this.TryComp<GunComponent>(weaponUid, ref gunComponent) && gunComponent.UseKey)
        {
          AltFireMeleeComponent fireMeleeComponent;
          if (!this.TryComp<AltFireMeleeComponent>(weaponUid, ref fireMeleeComponent) || state2 != 1)
            return;
          switch (fireMeleeComponent.AttackType)
          {
            case AltFireAttackType.Light:
              this.ClientLightAttack(entityUid1, map, coordinates, weaponUid, melee);
              return;
            case AltFireAttackType.Heavy:
              this.ClientHeavyAttack(entityUid1, coordinates, weaponUid, melee);
              return;
            case AltFireAttackType.Disarm:
              this.ClientDisarm(entityUid1, map, coordinates, melee);
              return;
            default:
              return;
          }
        }
        else
        {
          if (state2 == 1)
          {
            if (melee.AltDisarm && EntityUid.op_Equality(weaponUid, entityUid1))
            {
              this.ClientDisarm(entityUid1, map, coordinates, melee);
              return;
            }
            this.ClientHeavyAttack(entityUid1, coordinates, weaponUid, melee);
            return;
          }
          if (state1 == 1)
            this.ClientLightAttack(entityUid1, map, coordinates, weaponUid, melee);
          if (state3 != 1)
            return;
          this.ClientHeavyAttack(entityUid1, coordinates, weaponUid, melee);
          return;
        }
      }
    }
    melee.Attacking = false;
  }

  protected override bool InRange(
    EntityUid user,
    EntityUid target,
    float range,
    ICommonSession? session)
  {
    TransformComponent transformComponent = this.Transform(target);
    EntityCoordinates coordinates = transformComponent.Coordinates;
    Angle localRotation = transformComponent.LocalRotation;
    return this.Interaction.InRangeUnobstructed(Entity<TransformComponent>.op_Implicit(user), Entity<TransformComponent>.op_Implicit(target), coordinates, localRotation, range, overlapCheck: false);
  }

  protected override void DoDamageEffect(
    List<EntityUid> targets,
    EntityUid? user,
    TransformComponent targetXform)
  {
    this._color.RaiseEffect(Color.Red, targets, Filter.Local());
  }

  public void ClientHeavyAttack(
    EntityUid user,
    EntityCoordinates coordinates,
    EntityUid meleeUid,
    MeleeWeaponComponent component)
  {
    TransformComponent transformComponent;
    if (!this._xformQuery.TryGetComponent(user, ref transformComponent) || !this.Timing.IsFirstTimePredicted)
      return;
    MapCoordinates mapCoordinates = this.TransformSystem.ToMapCoordinates(coordinates, true);
    if (MapId.op_Inequality(mapCoordinates.MapId, transformComponent.MapID))
      return;
    Vector2 worldPosition = this.TransformSystem.GetWorldPosition(transformComponent);
    Vector2 vector2 = mapCoordinates.Position - worldPosition;
    float range = MathF.Min(component.Range, vector2.Length());
    List<NetEntity> netEntityList = this.GetNetEntityList(this.ArcRayCast(worldPosition, DirectionExtensions.ToWorldAngle(vector2), component.Angle, range, transformComponent.MapID, user).ToList<EntityUid>());
    this._rmcLagCompensation.SendLastRealTick();
    this.RaisePredictiveEvent<HeavyAttackEvent>(new HeavyAttackEvent(this.GetNetEntity(meleeUid, (MetaDataComponent) null), netEntityList.GetRange(0, Math.Min(this.MaxTargets, netEntityList.Count)), this.GetNetCoordinates(coordinates, (MetaDataComponent) null)));
  }

  private void ClientDisarm(
    EntityUid attacker,
    MapCoordinates mousePos,
    EntityCoordinates coordinates,
    MeleeWeaponComponent meleeComponent)
  {
    EntityUid? nullable = new EntityUid?();
    if (this._stateManager.CurrentState is GameplayStateBase currentState)
      nullable = currentState.GetClickedEntity(mousePos);
    MapCoordinates mapCoordinates = this.TransformSystem.GetMapCoordinates(attacker, (TransformComponent) null);
    if (MapId.op_Inequality(mousePos.MapId, mapCoordinates.MapId) || (double) (mapCoordinates.Position - mousePos.Position).Length() > (double) meleeComponent.Range)
      return;
    this._rmcLagCompensation.SendLastRealTick();
    this.RaisePredictiveEvent<DisarmAttackEvent>(new DisarmAttackEvent(this.GetNetEntity(nullable, (MetaDataComponent) null), this.GetNetCoordinates(coordinates, (MetaDataComponent) null)));
  }

  private void ClientLightAttack(
    EntityUid attacker,
    MapCoordinates mousePos,
    EntityCoordinates coordinates,
    EntityUid weaponUid,
    MeleeWeaponComponent meleeComponent)
  {
    MapCoordinates mapCoordinates = this.TransformSystem.GetMapCoordinates(attacker, (TransformComponent) null);
    if (MapId.op_Inequality(mousePos.MapId, mapCoordinates.MapId))
      return;
    EntityUid? target = new EntityUid?();
    if (this._stateManager.CurrentState is GameplayStateBase currentState)
      target = currentState.GetClickedEntity(mousePos);
    if ((double) (mapCoordinates.Position - mousePos.Position).Length() > (double) this._rmcMeleeWeapon.GetUserLightAttackRange(attacker, target, meleeComponent) || this.Interaction.CombatModeCanHandInteract(attacker, target))
      return;
    this._rmcLagCompensation.SendLastRealTick();
    this.RaisePredictiveEvent<LightAttackEvent>(new LightAttackEvent(this.GetNetEntity(target, (MetaDataComponent) null), this.GetNetEntity(weaponUid, (MetaDataComponent) null), this.GetNetCoordinates(coordinates, (MetaDataComponent) null)));
  }

  private void OnMeleeLunge(MeleeLungeEvent ev)
  {
    EntityUid entity1 = this.GetEntity(ev.Entity);
    EntityUid entity2 = this.GetEntity(ev.Weapon);
    if (!this.Exists(entity1) || !this.Exists(entity2))
      return;
    this.DoLunge(entity1, entity2, ev.Angle, ev.LocalPos, ev.Animation, true);
  }

  public override void DoLunge(
    EntityUid user,
    EntityUid weapon,
    Angle angle,
    Vector2 localPos,
    string? animation,
    bool predicted = true)
  {
    if (localPos == Vector2.Zero || !this.Timing.IsFirstTimePredicted)
      return;
    Animation lungeAnimation = this.GetLungeAnimation(localPos);
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(user), "melee-lunge");
    this._animation.Play(user, lungeAnimation, "melee-lunge");
    TransformComponent transformComponent;
    if (localPos == Vector2.Zero || animation == null || !this._xformQuery.TryGetComponent(user, ref transformComponent) || MapId.op_Equality(transformComponent.MapID, MapId.Nullspace))
      return;
    EntityUid entityUid = this.Spawn(animation, transformComponent.Coordinates);
    SpriteComponent sprite;
    WeaponArcVisualsComponent visualsComponent;
    if (!this.TryComp<SpriteComponent>(entityUid, ref sprite) || !this.TryComp<WeaponArcVisualsComponent>(entityUid, ref visualsComponent))
      return;
    Angle spriteRotation = Angle.Zero;
    MeleeWeaponComponent meleeWeaponComponent;
    if (visualsComponent.Animation != WeaponArcAnimation.None && this.TryComp<MeleeWeaponComponent>(weapon, ref meleeWeaponComponent))
    {
      SpriteComponent spriteComponent;
      if (EntityUid.op_Inequality(user, weapon) && this.TryComp<SpriteComponent>(weapon, ref spriteComponent))
        this._sprite.CopySprite(Entity<SpriteComponent>.op_Implicit((weapon, spriteComponent)), Entity<SpriteComponent>.op_Implicit((entityUid, sprite)));
      spriteRotation = meleeWeaponComponent.WideAnimationRotation;
      if (meleeWeaponComponent.SwingLeft)
        angle = Angle.op_Implicit(Angle.op_Implicit(angle) * -1.0);
    }
    this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((entityUid, sprite)), DirectionExtensions.ToWorldAngle(localPos));
    float distance = Math.Clamp(localPos.Length() / 2f, 0.2f, 1f);
    TransformComponent component = this._xformQuery.GetComponent(entityUid);
    switch (visualsComponent.Animation)
    {
      case WeaponArcAnimation.None:
        (Vector2 vector2_1, Angle angle1) = this.TransformSystem.GetWorldPositionRotation(transformComponent);
        Vector2 vector2_2 = vector2_1;
        Angle angle2 = Angle.op_Subtraction(angle1, transformComponent.LocalRotation);
        Vector2 vector2_3 = ((Angle) ref angle2).RotateVec(ref localPos);
        Vector2 vector2_4 = Vector2.Transform(vector2_2 + vector2_3, this.TransformSystem.GetInvWorldMatrix(component.ParentUid));
        this.TransformSystem.SetLocalPositionNoLerp(entityUid, vector2_4, component);
        if (!visualsComponent.Fadeout)
          break;
        this._animation.Play(entityUid, this.GetFadeAnimation(sprite, 0.0f, 0.15f), "melee-fade");
        break;
      case WeaponArcAnimation.Thrust:
        this.EnsureComp<TrackUserComponent>(entityUid).User = new EntityUid?(user);
        this._animation.Play(entityUid, this.GetThrustAnimation(Entity<SpriteComponent>.op_Implicit((entityUid, sprite)), distance, spriteRotation), "melee-thrust");
        if (!visualsComponent.Fadeout)
          break;
        this._animation.Play(entityUid, this.GetFadeAnimation(sprite, 0.05f, 0.15f), "melee-fade");
        break;
      case WeaponArcAnimation.Slash:
        this.EnsureComp<TrackUserComponent>(entityUid).User = new EntityUid?(user);
        this._animation.Play(entityUid, this.GetSlashAnimation(sprite, angle, spriteRotation), "melee-slash");
        if (!visualsComponent.Fadeout)
          break;
        this._animation.Play(entityUid, this.GetFadeAnimation(sprite, 0.065f, 0.114999995f), "melee-fade");
        break;
    }
  }

  private Animation GetSlashAnimation(SpriteComponent sprite, Angle arc, Angle spriteRotation)
  {
    Angle angle1 = Angle.op_Addition(sprite.Rotation, Angle.op_Implicit(Angle.op_Implicit(arc) / 2.0));
    Angle angle2 = Angle.op_Subtraction(sprite.Rotation, Angle.op_Implicit(Angle.op_Implicit(arc) / 2.0));
    ref Angle local1 = ref angle1;
    Vector2 vector2_1 = new Vector2(0.0f, -1f);
    ref Vector2 local2 = ref vector2_1;
    Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
    ref Angle local3 = ref angle2;
    Vector2 vector2_3 = new Vector2(0.0f, -1f);
    ref Vector2 local4 = ref vector2_3;
    Vector2 vector2_4 = ((Angle) ref local3).RotateVec(ref local4);
    Angle angle3 = Angle.op_Addition(angle1, spriteRotation);
    Angle angle4 = Angle.op_Addition(angle2, spriteRotation);
    Animation slashAnimation = new Animation();
    slashAnimation.Length = TimeSpan.FromSeconds(0.11499999463558197);
    List<AnimationTrack> animationTracks1 = slashAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty1 = new AnimationTrackComponentProperty();
    componentProperty1.ComponentType = typeof (SpriteComponent);
    componentProperty1.Property = "Rotation";
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) angle3, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) angle3, 0.03f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) angle4, 0.065f, (Func<float, float>) null));
    animationTracks1.Add((AnimationTrack) componentProperty1);
    List<AnimationTrack> animationTracks2 = slashAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty2 = new AnimationTrackComponentProperty();
    componentProperty2.ComponentType = typeof (SpriteComponent);
    componentProperty2.Property = "Offset";
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_2, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_2, 0.03f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_4, 0.065f, (Func<float, float>) null));
    animationTracks2.Add((AnimationTrack) componentProperty2);
    return slashAnimation;
  }

  private Animation GetThrustAnimation(
    Entity<SpriteComponent> sprite,
    float distance,
    Angle spriteRotation)
  {
    Angle rotation1 = sprite.Comp.Rotation;
    ref Angle local1 = ref rotation1;
    Vector2 vector2_1 = new Vector2(0.0f, (float) (-(double) distance / 5.0));
    ref Vector2 local2 = ref vector2_1;
    Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
    Angle rotation2 = sprite.Comp.Rotation;
    ref Angle local3 = ref rotation2;
    Vector2 vector2_3 = new Vector2(0.0f, -distance);
    ref Vector2 local4 = ref vector2_3;
    Vector2 vector2_4 = ((Angle) ref local3).RotateVec(ref local4);
    this._sprite.SetRotation(sprite.AsNullable(), Angle.op_Addition(sprite.Comp.Rotation, spriteRotation));
    Animation thrustAnimation = new Animation();
    thrustAnimation.Length = TimeSpan.FromSeconds(0.15000000596046448);
    List<AnimationTrack> animationTracks = thrustAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_2, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_4, 0.05f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_4, 0.15f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return thrustAnimation;
  }

  private Animation GetFadeAnimation(SpriteComponent sprite, float start, float end)
  {
    Animation fadeAnimation = new Animation();
    fadeAnimation.Length = TimeSpan.FromSeconds((double) end);
    List<AnimationTrack> animationTracks = fadeAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Color";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) sprite.Color, start, (Func<float, float>) null));
    List<AnimationTrackProperty.KeyFrame> keyFrames = ((AnimationTrackProperty) componentProperty).KeyFrames;
    Color color = sprite.Color;
    AnimationTrackProperty.KeyFrame keyFrame = new AnimationTrackProperty.KeyFrame((object) ((Color) ref color).WithAlpha(0.0f), end, (Func<float, float>) null);
    keyFrames.Add(keyFrame);
    animationTracks.Add((AnimationTrack) componentProperty);
    return fadeAnimation;
  }

  private Animation GetLungeAnimation(Vector2 direction)
  {
    Animation lungeAnimation = new Animation();
    lungeAnimation.Length = TimeSpan.FromSeconds(0.10000000149011612);
    List<AnimationTrack> animationTracks = lungeAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) (Vector2Helpers.Normalized(direction) * 0.15f), 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.1f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return lungeAnimation;
  }

  private void UpdateEffects()
  {
    EntityQueryEnumerator<TrackUserComponent, TransformComponent> entityQueryEnumerator = this.EntityQueryEnumerator<TrackUserComponent, TransformComponent>();
    EntityUid entityUid;
    TrackUserComponent trackUserComponent;
    TransformComponent transformComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref trackUserComponent, ref transformComponent))
    {
      if (trackUserComponent.User.HasValue && !this.TerminatingOrDeleted(trackUserComponent.User, (MetaDataComponent) null))
      {
        Vector2 vector2_1 = this.TransformSystem.GetWorldPosition(trackUserComponent.User.Value);
        if (trackUserComponent.Offset != Vector2.Zero)
        {
          Angle worldRotation = this.TransformSystem.GetWorldRotation(transformComponent);
          vector2_1 += ((Angle) ref worldRotation).RotateVec(ref trackUserComponent.Offset);
        }
        if (trackUserComponent.OriginOffset.HasValue)
        {
          Vector2? originOffset = trackUserComponent.OriginOffset;
          Vector2 zero = Vector2.Zero;
          if ((originOffset.HasValue ? (originOffset.GetValueOrDefault() != zero ? 1 : 0) : 1) != 0)
          {
            Angle worldRotation = this.TransformSystem.GetWorldRotation(trackUserComponent.User.Value);
            Vector2 vector2_2 = vector2_1;
            ref Angle local1 = ref worldRotation;
            zero = trackUserComponent.OriginOffset.Value;
            ref Vector2 local2 = ref zero;
            Vector2 vector2_3 = ((Angle) ref local1).RotateVec(ref local2);
            vector2_1 = vector2_2 + vector2_3;
          }
        }
        this.TransformSystem.SetWorldPosition(entityUid, vector2_1);
      }
    }
  }
}
