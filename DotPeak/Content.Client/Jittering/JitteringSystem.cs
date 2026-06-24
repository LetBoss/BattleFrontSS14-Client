// Decompiled with JetBrains decompiler
// Type: Content.Client.Jittering.JitteringSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Jittering;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Jittering;

public sealed class JitteringSystem : SharedJitteringSystem
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private AnimationPlayerSystem _animationPlayer;
  [Dependency]
  private SpriteSystem _sprite;
  private readonly float[] _sign = new float[2]{ -1f, 1f };
  private readonly string _jitterAnimationKey = "jittering";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JitteringComponent, ComponentStartup>(new ComponentEventHandler<JitteringComponent, ComponentStartup>((object) this, __methodptr(OnStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JitteringComponent, ComponentShutdown>(new ComponentEventHandler<JitteringComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<JitteringComponent, AnimationCompletedEvent>(new ComponentEventHandler<JitteringComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnStartup(EntityUid uid, JitteringComponent jittering, ComponentStartup args)
  {
    SpriteComponent sprite;
    if (!this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    AnimationPlayerComponent animationPlayerComponent = this.EnsureComp<AnimationPlayerComponent>(uid);
    jittering.StartOffset = sprite.Offset;
    this._animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.GetAnimation(jittering, sprite), this._jitterAnimationKey);
  }

  private void OnShutdown(EntityUid uid, JitteringComponent jittering, ComponentShutdown args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
      this._animationPlayer.Stop(uid, animationPlayerComponent, this._jitterAnimationKey);
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetOffset(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), jittering.StartOffset);
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    JitteringComponent jittering,
    AnimationCompletedEvent args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    SpriteComponent sprite;
    if (args.Key != this._jitterAnimationKey || !args.Finished || !this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || !this.TryComp<SpriteComponent>(uid, ref sprite))
      return;
    this._animationPlayer.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.GetAnimation(jittering, sprite), this._jitterAnimationKey);
  }

  private Animation GetAnimation(JitteringComponent jittering, SpriteComponent sprite)
  {
    float num1 = MathF.Min(4f, (float) ((double) jittering.Amplitude / 100.0 + 1.0)) / 10f;
    Vector2 vector2 = new Vector2(this._random.NextFloat(num1 / 4f, num1), this._random.NextFloat(num1 / 4f, num1 / 3f));
    vector2.X *= RandomExtensions.Pick<float>(this._random, (IReadOnlyList<float>) this._sign);
    vector2.Y *= RandomExtensions.Pick<float>(this._random, (IReadOnlyList<float>) this._sign);
    if (Math.Sign(vector2.X) == Math.Sign(jittering.LastJitter.X) || Math.Sign(vector2.Y) == Math.Sign(jittering.LastJitter.Y))
    {
      if (RandomExtensions.Prob(this._random, 0.5f))
        vector2.X *= -1f;
      else
        vector2.Y *= -1f;
    }
    float num2 = 0.0f;
    if ((double) jittering.Frequency > 0.0)
      num2 = 1f / jittering.Frequency;
    jittering.LastJitter = vector2;
    Animation animation = new Animation();
    animation.Length = TimeSpan.FromSeconds((double) num2);
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) sprite.Offset, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) (jittering.StartOffset + vector2), num2, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return animation;
  }
}
