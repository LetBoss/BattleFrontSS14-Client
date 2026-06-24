// Decompiled with JetBrains decompiler
// Type: Content.Client.Rotation.RotationVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Rotation;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Rotation;

public sealed class RotationVisualizerSystem : SharedRotationVisualsSystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private AnimationPlayerSystem _animation;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RotationVisualsComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<RotationVisualsComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChange(
    EntityUid uid,
    RotationVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    RotationState rotationState;
    if (args.Sprite == null || !((SharedAppearanceSystem) this._appearance).TryGetData<RotationState>(uid, (Enum) RotationVisuals.RotationState, ref rotationState, args.Component))
      return;
    if (rotationState != RotationState.Vertical)
    {
      if (rotationState != RotationState.Horizontal)
        return;
      this.AnimateSpriteRotation(uid, args.Sprite, component.HorizontalRotation, component.AnimationTime);
    }
    else
      this.AnimateSpriteRotation(uid, args.Sprite, component.VerticalRotation, component.AnimationTime);
  }

  public void AnimateSpriteRotation(
    EntityUid uid,
    SpriteComponent spriteComp,
    Angle rotation,
    float animationTime)
  {
    Angle rotation1 = spriteComp.Rotation;
    if (((Angle) ref rotation1).Equals(rotation))
      return;
    AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(uid);
    if (this._animation.HasRunningAnimation(animationPlayerComponent, "rotate"))
      this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), "rotate");
    Animation animation1 = new Animation();
    animation1.Length = TimeSpan.FromSeconds((double) animationTime);
    List<AnimationTrack> animationTracks = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Rotation";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) spriteComp.Rotation, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) rotation, animationTime, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    Animation animation2 = animation1;
    this._animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), animation2, "rotate");
  }
}
