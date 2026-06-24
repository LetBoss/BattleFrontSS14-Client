// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.ParaDrop.ParaDropSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Sprite;
using Content.Shared._RMC14.Sprite;
using Content.Shared.ParaDrop;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.ParaDrop;

public sealed class ParaDropSystem : SharedParaDropSystem
{
  [Dependency]
  private AnimationPlayerSystem _animPlayer;
  [Dependency]
  private TransformSystem _transform;
  [Dependency]
  private RMCSpriteSystem _rmcSprite;
  [Dependency]
  private SpriteSystem _sprite;
  private const string DroppingAnimationKey = "dropping-animation";
  private const string SkyFallingAnimationKey = "sky-falling-animation";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SkyFallingComponent, ComponentInit>(new EntityEventRefHandler<SkyFallingComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SkyFallingComponent, ComponentRemove>(new EntityEventRefHandler<SkyFallingComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ParaDroppingComponent, ComponentRemove>(new EntityEventRefHandler<ParaDroppingComponent, ComponentRemove>((object) this, __methodptr(OnParaDroppingRemove)), (Type[]) null, (Type[]) null);
  }

  public Animation ReturnFallAnimation(float fallDuration, float fallHeight)
  {
    Animation animation = new Animation();
    animation.Length = TimeSpan.FromSeconds((double) fallDuration);
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, fallHeight), 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, 0.0f), fallDuration, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return animation;
  }

  private Animation GetFallingDisappearingAnimation(
    float duration,
    Vector2 originalScale,
    Vector2 endScale)
  {
    Animation disappearingAnimation = new Animation();
    disappearingAnimation.Length = TimeSpan.FromSeconds((double) duration);
    List<AnimationTrack> animationTracks1 = disappearingAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty1 = new AnimationTrackComponentProperty();
    componentProperty1.ComponentType = typeof (SpriteComponent);
    componentProperty1.Property = "Scale";
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) originalScale, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) endScale, duration, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).InterpolationMode = (AnimationInterpolationMode) 1;
    animationTracks1.Add((AnimationTrack) componentProperty1);
    List<AnimationTrack> animationTracks2 = disappearingAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty2 = new AnimationTrackComponentProperty();
    componentProperty2.ComponentType = typeof (SpriteComponent);
    componentProperty2.Property = "Offset";
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, 0.0f), 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) new Vector2(0.0f, -1f), duration, (Func<float, float>) null));
    animationTracks2.Add((AnimationTrack) componentProperty2);
    return disappearingAnimation;
  }

  private void OnComponentInit(Entity<SkyFallingComponent> ent, ref ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref spriteComponent) || this.TerminatingOrDeleted(Entity<SkyFallingComponent>.op_Implicit(ent), (MetaDataComponent) null))
      return;
    ent.Comp.OriginalScale = spriteComponent.Scale;
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.TryComp<AnimationPlayerComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref animationPlayerComponent) || this._animPlayer.HasRunningAnimation(animationPlayerComponent, "sky-falling-animation"))
      return;
    this._animPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<SkyFallingComponent>.op_Implicit(ent), animationPlayerComponent)), this.GetFallingDisappearingAnimation(ent.Comp.RemainingTime, ent.Comp.OriginalScale, ent.Comp.AnimationScale), "sky-falling-animation");
  }

  private void OnComponentRemove(Entity<SkyFallingComponent> ent, ref ComponentRemove args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref spriteComponent) || this.TerminatingOrDeleted(Entity<SkyFallingComponent>.op_Implicit(ent), (MetaDataComponent) null))
      return;
    AnimationPlayerComponent animationPlayerComponent;
    if (this.TryComp<AnimationPlayerComponent>(Entity<SkyFallingComponent>.op_Implicit(ent), ref animationPlayerComponent))
      this._animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<SkyFallingComponent>.op_Implicit(ent), animationPlayerComponent)), "sky-falling-animation");
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), ent.Comp.OriginalScale);
  }

  private void OnParaDroppingRemove(Entity<ParaDroppingComponent> ent, ref ComponentRemove args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (this.TerminatingOrDeleted(Entity<ParaDroppingComponent>.op_Implicit(ent), (MetaDataComponent) null) || !this.TryComp<AnimationPlayerComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref animationPlayerComponent))
      return;
    this._animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<ParaDroppingComponent>.op_Implicit(ent), animationPlayerComponent)), "dropping-animation");
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<ParaDroppingComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), new Vector2());
  }

  private void SpawnParachute(
    float fallDuration,
    EntityCoordinates coordinates,
    ParaDroppableComponent paraDroppable,
    float multiplier)
  {
    EntityUid entityUid = this.Spawn(EntProtoId.op_Implicit(paraDroppable.ParachutePrototype), coordinates);
    this.EnsureComp<TimedDespawnComponent>(entityUid).Lifetime = fallDuration;
    this.AddComp<RMCUpdateClientLocationComponent>(entityUid);
    this.EnsureComp<ParaDroppingComponent>(entityUid).RemainingTime = fallDuration;
    this._animPlayer.Play(entityUid, this.ReturnFallAnimation(fallDuration, paraDroppable.FallHeight * multiplier), "dropping-animation");
  }

  public void PlayFallAnimation(
    EntityUid fallingUid,
    float fallDuration,
    float timeRemaining,
    float fallHeight,
    string animationKey,
    ParaDroppableComponent? paraDroppable = null)
  {
    float multiplier = timeRemaining / fallDuration;
    float fallDuration1 = fallDuration * multiplier;
    float fallHeight1 = fallHeight * multiplier;
    if ((double) timeRemaining <= 0.0 || (double) multiplier <= 0.0 || (double) multiplier >= 1.0)
      return;
    this._animPlayer.Play(fallingUid, this.ReturnFallAnimation(fallDuration1, fallHeight1), animationKey);
    if (paraDroppable == null)
      return;
    this.SpawnParachute(fallDuration1, ((SharedTransformSystem) this._transform).GetMoverCoordinates(fallingUid), paraDroppable, multiplier);
  }

  public override void Update(float frameTime)
  {
    base.Update(frameTime);
    EntityQueryEnumerator<ParaDroppableComponent, ParaDroppingComponent> entityQueryEnumerator = this.EntityQueryEnumerator<ParaDroppableComponent, ParaDroppingComponent>();
    EntityUid entityUid;
    ParaDroppableComponent paraDroppable;
    ParaDroppingComponent droppingComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref paraDroppable, ref droppingComponent))
    {
      if (!this.HasComp<SkyFallingComponent>(entityUid))
      {
        if (!this._animPlayer.HasRunningAnimation(entityUid, "dropping-animation") && paraDroppable.LastParaDrop.HasValue && MapId.op_Inequality(this.Transform(entityUid).MapID, MapId.Nullspace))
          this.PlayFallAnimation(entityUid, paraDroppable.DropDuration, droppingComponent.RemainingTime, paraDroppable.FallHeight, "dropping-animation", paraDroppable);
        this._rmcSprite.UpdatePosition(entityUid);
      }
    }
  }
}
