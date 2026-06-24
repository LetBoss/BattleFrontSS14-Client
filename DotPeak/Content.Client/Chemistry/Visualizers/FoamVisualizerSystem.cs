// Decompiled with JetBrains decompiler
// Type: Content.Client.Chemistry.Visualizers.FoamVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Chemistry.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Client.Chemistry.Visualizers;

public sealed class FoamVisualizerSystem : VisualizerSystem<FoamVisualsComponent>
{
  [Dependency]
  private IGameTiming _timing;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FoamVisualsComponent, ComponentInit>(new ComponentEventHandler<FoamVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<FoamVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<FoamVisualsComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationComplete)), (Type[]) null, (Type[]) null);
  }

  public virtual void Update(float frameTime)
  {
    ((EntitySystem) this).Update(frameTime);
    if (!this._timing.IsFirstTimePredicted)
      return;
    EntityQueryEnumerator<FoamVisualsComponent, SmokeComponent> entityQueryEnumerator = ((EntitySystem) this).EntityQueryEnumerator<FoamVisualsComponent, SmokeComponent>();
    EntityUid entityUid;
    FoamVisualsComponent visualsComponent;
    SmokeComponent smokeComponent;
    while (entityQueryEnumerator.MoveNext(ref entityUid, ref visualsComponent, ref smokeComponent))
    {
      AnimationPlayerComponent animationPlayerComponent;
      if (!(this._timing.CurTime < visualsComponent.StartTime + TimeSpan.FromSeconds((double) smokeComponent.Duration) - TimeSpan.FromSeconds((double) visualsComponent.AnimationTime)) && ((EntitySystem) this).TryComp<AnimationPlayerComponent>(entityUid, ref animationPlayerComponent) && !this.AnimationSystem.HasRunningAnimation(entityUid, animationPlayerComponent, "foamdissolve_animation"))
        this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((entityUid, animationPlayerComponent)), visualsComponent.Animation, "foamdissolve_animation");
    }
  }

  private void OnComponentInit(EntityUid uid, FoamVisualsComponent comp, ComponentInit args)
  {
    comp.StartTime = this._timing.CurTime;
    comp.Animation = new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.AnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) FoamVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.AnimationState), 0.0f)
          }
        }
      }
    };
  }

  private void OnAnimationComplete(
    EntityUid uid,
    FoamVisualsComponent component,
    AnimationCompletedEvent args)
  {
    SpriteComponent spriteComponent;
    if (args.Key != "foamdissolve_animation" || !((EntitySystem) this).TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this.SpriteSystem.SetVisible(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), false);
  }
}
