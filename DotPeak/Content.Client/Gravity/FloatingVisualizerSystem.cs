// Decompiled with JetBrains decompiler
// Type: Content.Client.Gravity.FloatingVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Gravity;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Gravity;

public sealed class FloatingVisualizerSystem : SharedFloatingVisualizerSystem
{
  [Dependency]
  private AnimationPlayerSystem AnimationSystem;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FloatingVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<FloatingVisualsComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  public override void FloatAnimation(
    EntityUid uid,
    Vector2 offset,
    string animationKey,
    float animationTime,
    bool stop = false)
  {
    if (stop)
    {
      this.AnimationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), animationKey);
    }
    else
    {
      Animation animation1 = new Animation();
      animation1.Length = TimeSpan.FromSeconds((double) animationTime * 2.0);
      List<AnimationTrack> animationTracks = animation1.AnimationTracks;
      AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
      componentProperty.ComponentType = typeof (SpriteComponent);
      componentProperty.Property = "Offset";
      ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
      ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.0f, (Func<float, float>) null));
      ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, animationTime, (Func<float, float>) null));
      ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, animationTime, (Func<float, float>) null));
      animationTracks.Add((AnimationTrack) componentProperty);
      Animation animation2 = animation1;
      if (this.AnimationSystem.HasRunningAnimation(uid, animationKey))
        return;
      this.AnimationSystem.Play(uid, animation2, animationKey);
    }
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    FloatingVisualsComponent component,
    AnimationCompletedEvent args)
  {
    if (args.Key != component.AnimationKey)
      return;
    this.FloatAnimation(uid, component.Offset, component.AnimationKey, component.AnimationTime, !component.CanFloat);
  }
}
