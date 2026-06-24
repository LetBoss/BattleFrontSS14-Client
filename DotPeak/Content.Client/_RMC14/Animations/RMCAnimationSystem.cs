// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Animations.RMCAnimationSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Animations;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Animations;

public sealed class RMCAnimationSystem : SharedRMCAnimationSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private SpriteSystem _sprite;
  private const string FlickId = "rmc_flick_animation";

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeNetworkEvent<RMCPlayAnimationEvent>(new EntityEventHandler<RMCPlayAnimationEvent>(this.OnPlayAnimation), (Type[]) null, (Type[]) null);
    this.SubscribeNetworkEvent<RMCFlickEvent>(new EntityEventHandler<RMCFlickEvent>(this.OnFlick), (Type[]) null, (Type[]) null);
  }

  private void OnPlayAnimation(RMCPlayAnimationEvent ev)
  {
    EntityUid entity = this.GetEntity(ev.Entity);
    RMCAnimationComponent animationComponent;
    RMCAnimation rmcAnimation;
    if (!((EntityUid) ref entity).Valid || this._animation.HasRunningAnimation(entity, ev.Animation.Id) || !this.TryComp<RMCAnimationComponent>(entity, ref animationComponent) || !animationComponent.Animations.TryGetValue(ev.Animation, out rmcAnimation))
      return;
    List<AnimationTrack> collection1 = new List<AnimationTrack>();
    foreach (RMCAnimationTrack animationTrack in rmcAnimation.AnimationTracks)
    {
      List<AnimationTrackSpriteFlick.KeyFrame> collection2 = new List<AnimationTrackSpriteFlick.KeyFrame>();
      foreach (RMCKeyFrame keyFrame in animationTrack.KeyFrames)
        collection2.Add(new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(keyFrame.State), keyFrame.KeyTime));
      AnimationTrackSpriteFlick trackSpriteFlick = new AnimationTrackSpriteFlick()
      {
        LayerKey = animationTrack.LayerKey
      };
      trackSpriteFlick.KeyFrames.AddRange((IEnumerable<AnimationTrackSpriteFlick.KeyFrame>) collection2);
      collection1.Add((AnimationTrack) trackSpriteFlick);
    }
    Animation animation = new Animation()
    {
      Length = rmcAnimation.Length
    };
    animation.AnimationTracks.AddRange((IEnumerable<AnimationTrack>) collection1);
    this._animation.Play(entity, animation, ev.Animation.Id);
  }

  private void OnFlick(RMCFlickEvent ev)
  {
    EntityUid entity = this.GetEntity(ev.Entity);
    if (!((EntityUid) ref entity).Valid || this._animation.HasRunningAnimation(entity, "rmc_flick_animation"))
      return;
    string str = ev.Layer ?? "rmc_flick_animation";
    if (!this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit(entity), str))
      this._sprite.LayerMapSet(Entity<SpriteComponent>.op_Implicit(entity), "rmc_flick_animation", 0);
    float animationLength = this._sprite.GetState(ev.AnimationState).AnimationLength;
    Animation animation = new Animation()
    {
      Length = TimeSpan.FromSeconds((double) animationLength),
      AnimationTracks = {
        (AnimationTrack) new RMCAnimationTrackSpriteFlick()
        {
          LayerKey = "rmc_flick_animation",
          KeyFrames = new List<RMCAnimationTrackSpriteFlick.KeyFrame>()
          {
            new RMCAnimationTrackSpriteFlick.KeyFrame(ev.AnimationState, 0.0f),
            new RMCAnimationTrackSpriteFlick.KeyFrame(ev.DefaultState, animationLength)
          }
        }
      }
    };
    this._animation.Play(entity, animation, "rmc_flick_animation");
  }
}
