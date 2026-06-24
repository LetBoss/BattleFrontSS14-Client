// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Destroy.XenoDestroySystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Destroy;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Destroy;

public sealed class XenoDestroySystem : SharedXenoDestroySystem
{
  [Dependency]
  private AnimationPlayerSystem _animPlayer;
  [Dependency]
  private SpriteSystem _sprite;
  private const float JumpHeight = 10f;
  private const string LeapingAnimationKey = "king-leap-animation";

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<XenoDestroyLeapStartEvent>(new EntityEventHandler<XenoDestroyLeapStartEvent>(this.OnXenoLeapStart), (Type[]) null, (Type[]) null);
  }

  public Animation LeapAnimation(XenoDestroyComponent destroy, Vector2 leapOffset)
  {
    Vector2 vector2_1 = leapOffset / 2f;
    Vector2 vector2_2 = -vector2_1;
    Vector2 vector2_3 = vector2_1 + new Vector2(0.0f, 10f);
    Vector2 vector2_4 = vector2_2 + new Vector2(0.0f, 10f);
    float num = (float) (destroy.CrashTime.TotalSeconds / 2.0);
    Animation animation = new Animation();
    animation.Length = destroy.CrashTime;
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_3, num, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) vector2_4, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, num, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return animation;
  }

  private void OnXenoLeapStart(XenoDestroyLeapStartEvent ev)
  {
    EntityUid? nullable;
    XenoDestroyComponent destroy;
    SpriteComponent spriteComponent;
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.TryGetEntity(ev.King, ref nullable) || !this.TryComp<XenoDestroyComponent>(nullable, ref destroy) || !this.TryComp<SpriteComponent>(nullable, ref spriteComponent) || this.TerminatingOrDeleted(nullable, (MetaDataComponent) null) || !this.TryComp<AnimationPlayerComponent>(nullable, ref animationPlayerComponent) || this._animPlayer.HasRunningAnimation(animationPlayerComponent, "king-leap-animation"))
      return;
    this._animPlayer.Play(nullable.Value, this.LeapAnimation(destroy, ev.LeapOffset), "king-leap-animation");
  }

  protected override void OnLeapingRemove(
    Entity<XenoDestroyLeapingComponent> xeno,
    ref ComponentRemove args)
  {
    base.OnLeapingRemove(xeno, ref args);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), ref spriteComponent) || this.TerminatingOrDeleted(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), (MetaDataComponent) null))
      return;
    AnimationPlayerComponent animationPlayerComponent;
    if (this.TryComp<AnimationPlayerComponent>(Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), ref animationPlayerComponent))
      this._animPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit((Entity<XenoDestroyLeapingComponent>.op_Implicit(xeno), animationPlayerComponent)), "king-leap-animation");
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((xeno.Owner, spriteComponent)), Vector2.Zero);
  }
}
