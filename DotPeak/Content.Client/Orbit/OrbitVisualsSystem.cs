// Decompiled with JetBrains decompiler
// Type: Content.Client.Orbit.OrbitVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Follower.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Orbit;

public sealed class OrbitVisualsSystem : EntitySystem
{
  [Dependency]
  private IRobustRandom _robustRandom;
  [Dependency]
  private AnimationPlayerSystem _animations;
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SpriteSystem _sprite;
  private readonly string _orbitStopKey = "orbiting_stop";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OrbitVisualsComponent, ComponentInit>(new ComponentEventHandler<OrbitVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OrbitVisualsComponent, ComponentRemove>(new ComponentEventHandler<OrbitVisualsComponent, ComponentRemove>((object) this, __methodptr(OnComponentRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, OrbitVisualsComponent component, ComponentInit args)
  {
    this._robustRandom.SetSeed((int) this._timing.CurTime.TotalMilliseconds);
    component.OrbitDistance = this._robustRandom.NextFloat(0.75f * component.OrbitDistance, 1.25f * component.OrbitDistance);
    component.OrbitLength = this._robustRandom.NextFloat(0.5f * component.OrbitLength, 1.5f * component.OrbitLength);
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(uid, ref spriteComponent))
    {
      spriteComponent.EnableDirectionOverride = true;
      spriteComponent.DirectionOverride = (Direction) 0;
    }
    AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(uid);
    if (!this._animations.HasRunningAnimation(uid, animationPlayerComponent, this._orbitStopKey))
      return;
    this._animations.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this._orbitStopKey);
  }

  private void OnComponentRemove(
    EntityUid uid,
    OrbitVisualsComponent component,
    ComponentRemove args)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    sprite.EnableDirectionOverride = false;
    AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(uid);
    if (this._animations.HasRunningAnimation(uid, animationPlayerComponent, this._orbitStopKey))
      return;
    this._animations.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.GetStopAnimation(component, sprite), this._orbitStopKey);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQueryEnumerator<OrbitVisualsComponent, SpriteComponent> entityQueryEnumerator = this.EntityQueryEnumerator<OrbitVisualsComponent, SpriteComponent>();
    EntityUid entityUid;
    OrbitVisualsComponent visualsComponent;
    SpriteComponent spriteComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref spriteComponent))
    {
      float num = (float) this._timing.CurTime.TotalSeconds / visualsComponent.OrbitLength % 1f;
      Angle angle;
      // ISSUE: explicit constructor call
      ((Angle) ref angle).\u002Ector(2.0 * Math.PI * (double) num);
      ref Angle local1 = ref angle;
      Vector2 vector2_1 = new Vector2(visualsComponent.OrbitDistance, 0.0f);
      ref Vector2 local2 = ref vector2_1;
      Vector2 vector2_2 = ((Angle) ref local1).RotateVec(ref local2);
      this._sprite.SetRotation(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), angle);
      this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((entityUid, spriteComponent)), vector2_2);
    }
  }

  private Animation GetStopAnimation(OrbitVisualsComponent component, SpriteComponent sprite)
  {
    float orbitStopLength = component.OrbitStopLength;
    Animation stopAnimation = new Animation();
    stopAnimation.Length = TimeSpan.FromSeconds((double) orbitStopLength);
    List<AnimationTrack> animationTracks1 = stopAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty1 = new AnimationTrackComponentProperty();
    componentProperty1.ComponentType = typeof (SpriteComponent);
    componentProperty1.Property = "Offset";
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) sprite.Offset, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, orbitStopLength, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty1).InterpolationMode = (AnimationInterpolationMode) 0;
    animationTracks1.Add((AnimationTrack) componentProperty1);
    List<AnimationTrack> animationTracks2 = stopAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty2 = new AnimationTrackComponentProperty();
    componentProperty2.ComponentType = typeof (SpriteComponent);
    componentProperty2.Property = "Rotation";
    List<AnimationTrackProperty.KeyFrame> keyFrames = ((AnimationTrackProperty) componentProperty2).KeyFrames;
    Angle rotation = sprite.Rotation;
    AnimationTrackProperty.KeyFrame keyFrame = new AnimationTrackProperty.KeyFrame((object) ((Angle) ref rotation).Reduced(), 0.0f, (Func<float, float>) null);
    keyFrames.Add(keyFrame);
    ((AnimationTrackProperty) componentProperty2).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, orbitStopLength, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty2).InterpolationMode = (AnimationInterpolationMode) 0;
    animationTracks2.Add((AnimationTrack) componentProperty2);
    return stopAnimation;
  }
}
