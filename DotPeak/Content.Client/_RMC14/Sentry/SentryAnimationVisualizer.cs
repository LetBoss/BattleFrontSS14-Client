// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Sentry.SentryAnimationVisualizer
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Sentry;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Sentry;

public sealed class SentryAnimationVisualizer : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  private const string AnimationKey = "rmc_sentry_deploy";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SentrySpikesComponent, AppearanceChangeEvent>(new EntityEventRefHandler<SentrySpikesComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAfterAutoHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnAfterAutoHandleState(
    Entity<SentrySpikesComponent> spikes,
    ref AppearanceChangeEvent args)
  {
    SentryComponent sentryComponent;
    if (!this.TryComp<SentryComponent>(Entity<SentrySpikesComponent>.op_Implicit(spikes), ref sentryComponent) || sentryComponent.Mode != SentryMode.On || this._animation.HasRunningAnimation(Entity<SentrySpikesComponent>.op_Implicit(spikes), "rmc_sentry_deploy"))
      return;
    this._animation.Play(Entity<SentrySpikesComponent>.op_Implicit(spikes), new Animation()
    {
      Length = spikes.Comp.AnimationTime,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) spikes.Comp.Layer,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(spikes.Comp.AnimationState), 0.0f)
          }
        }
      }
    }, "rmc_sentry_deploy");
  }
}
