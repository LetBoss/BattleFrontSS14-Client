// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.RotatingLightSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light;
using Content.Shared.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class RotatingLightSystem : SharedRotatingLightSystem
{
  [Dependency]
  private AnimationPlayerSystem _animations;
  private const string AnimKey = "rotating_light";

  private Animation GetAnimation(float speed)
  {
    float num = 120f / speed;
    Animation animation = new Animation();
    animation.Length = TimeSpan.FromSeconds(360.0 / (double) speed);
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (PointLightComponent);
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    componentProperty.Property = "Rotation";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(120.0), num, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(240.0), num, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(360.0), num, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return animation;
  }

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RotatingLightComponent, ComponentStartup>(new ComponentEventHandler<RotatingLightComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RotatingLightComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<RotatingLightComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RotatingLightComponent, AnimationCompletedEvent>(new ComponentEventHandler<RotatingLightComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationComplete)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, RotatingLightComponent comp, ComponentStartup args)
  {
    AnimationPlayerComponent player = this.EnsureComp<AnimationPlayerComponent>(uid);
    this.PlayAnimation(uid, comp, player);
  }

  private void OnAfterAutoHandleState(
    EntityUid uid,
    RotatingLightComponent comp,
    ref AfterAutoHandleStateEvent args)
  {
    AnimationPlayerComponent player;
    if (!this.TryComp<AnimationPlayerComponent>(uid, ref player))
      return;
    if (comp.Enabled)
      this.PlayAnimation(uid, comp, player);
    else
      this._animations.Stop(uid, player, "rotating_light");
  }

  private void OnAnimationComplete(
    EntityUid uid,
    RotatingLightComponent comp,
    AnimationCompletedEvent args)
  {
    if (!args.Finished)
      return;
    this.PlayAnimation(uid, comp);
  }

  public void PlayAnimation(
    EntityUid uid,
    RotatingLightComponent? comp = null,
    AnimationPlayerComponent? player = null)
  {
    if (!this.Resolve<RotatingLightComponent, AnimationPlayerComponent>(uid, ref comp, ref player, true) || !comp.Enabled || this._animations.HasRunningAnimation(uid, player, "rotating_light"))
      return;
    this._animations.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, player)), this.GetAnimation(comp.Speed), "rotating_light");
  }
}
