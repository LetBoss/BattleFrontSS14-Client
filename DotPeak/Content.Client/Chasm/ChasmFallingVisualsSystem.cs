// Decompiled with JetBrains decompiler
// Type: Content.Client.Chasm.ChasmFallingVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chasm;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Chasm;

public sealed class ChasmFallingVisualsSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _anim;
  [Dependency]
  private SpriteSystem _sprite;
  private readonly string _chasmFallAnimationKey = "chasm_fall";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChasmFallingComponent, ComponentInit>(new ComponentEventHandler<ChasmFallingComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ChasmFallingComponent, ComponentRemove>(new ComponentEventHandler<ChasmFallingComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, ChasmFallingComponent component, ComponentInit args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || this.TerminatingOrDeleted(uid, (MetaDataComponent) null))
      return;
    component.OriginalScale = spriteComponent.Scale;
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || this._anim.HasRunningAnimation(animationPlayerComponent, this._chasmFallAnimationKey))
      return;
    this._anim.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.GetFallingAnimation(component), this._chasmFallAnimationKey);
  }

  private void OnComponentRemove(
    EntityUid uid,
    ChasmFallingComponent component,
    ComponentRemove args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || this.TerminatingOrDeleted(uid, (MetaDataComponent) null))
      return;
    this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.OriginalScale);
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || !this._anim.HasRunningAnimation(animationPlayerComponent, this._chasmFallAnimationKey))
      return;
    this._anim.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this._chasmFallAnimationKey);
  }

  private Animation GetFallingAnimation(ChasmFallingComponent component)
  {
    TimeSpan animationTime = component.AnimationTime;
    Animation fallingAnimation = new Animation();
    fallingAnimation.Length = animationTime;
    List<AnimationTrack> animationTracks = fallingAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Scale";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) component.OriginalScale, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) component.AnimationScale, (float) animationTime.Seconds, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 1;
    animationTracks.Add((AnimationTrack) componentProperty);
    return fallingAnimation;
  }
}
