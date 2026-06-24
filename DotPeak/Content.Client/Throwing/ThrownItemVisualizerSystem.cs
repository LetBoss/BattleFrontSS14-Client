// Decompiled with JetBrains decompiler
// Type: Content.Client.Throwing.ThrownItemVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Throwing;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Analyzers;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Throwing;

public sealed class ThrownItemVisualizerSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _anim;
  [Dependency]
  private SpriteSystem _sprite;
  private const string AnimationKey = "thrown-item";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ThrownItemComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<ThrownItemComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnAutoHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<ThrownItemComponent, ComponentShutdown>(new ComponentEventHandler<ThrownItemComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
  }

  private void OnAutoHandleState(
    EntityUid uid,
    ThrownItemComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent) || !component.Animate)
      return;
    AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(uid);
    if (this._anim.HasRunningAnimation(uid, animationPlayerComponent, "thrown-item"))
      return;
    Animation animation = ThrownItemVisualizerSystem.GetAnimation(Entity<ThrownItemComponent, SpriteComponent>.op_Implicit((uid, component, spriteComponent)));
    if (animation == null)
      return;
    component.OriginalScale = new Vector2?(spriteComponent.Scale);
    this._anim.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), animation, "thrown-item");
  }

  private void OnShutdown(EntityUid uid, ThrownItemComponent component, ComponentShutdown args)
  {
    if (!this._anim.HasRunningAnimation(uid, "thrown-item"))
      return;
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(uid, ref spriteComponent) && component.OriginalScale.HasValue)
      this._sprite.SetScale(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), component.OriginalScale.Value);
    this._anim.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), "thrown-item");
  }

  private static Animation? GetAnimation(Entity<ThrownItemComponent, SpriteComponent> ent)
  {
    TimeSpan? landTime = ent.Comp1.LandTime;
    TimeSpan? thrownTime = ent.Comp1.ThrownTime;
    TimeSpan? nullable = landTime.HasValue & thrownTime.HasValue ? new TimeSpan?(landTime.GetValueOrDefault() - thrownTime.GetValueOrDefault()) : new TimeSpan?();
    if (!nullable.HasValue)
      return (Animation) null;
    TimeSpan valueOrDefault = nullable.GetValueOrDefault();
    if (valueOrDefault <= TimeSpan.Zero)
      return (Animation) null;
    Vector2 scale = ent.Comp2.Scale;
    float totalSeconds = (float) valueOrDefault.TotalSeconds;
    Animation animation = new Animation();
    animation.Length = valueOrDefault;
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Scale";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) scale, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) (scale * 1.4f), totalSeconds * 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) scale, totalSeconds * 0.75f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    animationTracks.Add((AnimationTrack) componentProperty);
    return animation;
  }
}
