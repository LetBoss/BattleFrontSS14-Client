// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Xenonids.Animations.XenoAnimationsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Xenonids.Animation;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._RMC14.Xenonids.Animations;

public sealed class XenoAnimationsSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private IPlayerManager _player;
  private const string MeleeLungeKey = "melee-lunge";

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<PlayLungeAnimationEvent>(new EntityEventHandler<PlayLungeAnimationEvent>(this.OnPlayLungeAnimation), (Type[]) null, (Type[]) null);
  }

  private void OnPlayLungeAnimation(PlayLungeAnimationEvent ev)
  {
    EntityUid? nullable1;
    if (!this.TryGetEntity(ev.EntityUid, ref nullable1))
      return;
    if (!ev.Client)
    {
      EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
      EntityUid? nullable2 = nullable1;
      if ((localEntity.HasValue == nullable2.HasValue ? (localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), nullable2.GetValueOrDefault()) ? 1 : 0) : 1) : 0) != 0)
        return;
    }
    Robust.Client.Animations.Animation lungeAnimation = this.GetLungeAnimation(ev.Direction);
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(nullable1.Value), "melee-lunge");
    this._animation.Play(nullable1.Value, lungeAnimation, "melee-lunge");
  }

  private Robust.Client.Animations.Animation GetLungeAnimation(Vector2 direction)
  {
    Robust.Client.Animations.Animation lungeAnimation = new Robust.Client.Animations.Animation();
    lungeAnimation.Length = TimeSpan.FromSeconds(0.40000000596046448);
    List<AnimationTrack> animationTracks = lungeAnimation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) (Vector2Helpers.Normalized(direction) * 0.6f), 0.1f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, 0.3f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    return lungeAnimation;
  }
}
