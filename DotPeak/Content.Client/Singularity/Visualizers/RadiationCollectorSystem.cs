// Decompiled with JetBrains decompiler
// Type: Content.Client.Singularity.Visualizers.RadiationCollectorSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Singularity.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Singularity.Visualizers;

public sealed class RadiationCollectorSystem : VisualizerSystem<RadiationCollectorComponent>
{
  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RadiationCollectorComponent, ComponentInit>(new ComponentEventHandler<RadiationCollectorComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<RadiationCollectorComponent, AnimationCompletedEvent>(new ComponentEventHandler<RadiationCollectorComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, RadiationCollectorComponent comp, ComponentInit args)
  {
    comp.ActivateAnimation = new Animation()
    {
      Length = TimeSpan.FromSeconds(0.800000011920929),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) RadiationCollectorVisualLayers.Main,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.ActivatingState), 0.0f)
          }
        }
      }
    };
    comp.DeactiveAnimation = new Animation()
    {
      Length = TimeSpan.FromSeconds(0.800000011920929),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) RadiationCollectorVisualLayers.Main,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.DeactivatingState), 0.0f)
          }
        }
      }
    };
  }

  private void UpdateVisuals(
    EntityUid uid,
    RadiationCollectorVisualState state,
    RadiationCollectorComponent comp,
    SpriteComponent sprite,
    AnimationPlayerComponent? animPlayer = null)
  {
    if (state == comp.CurrentState || !((EntitySystem) this).Resolve<AnimationPlayerComponent>(uid, ref animPlayer, true) || this.AnimationSystem.HasRunningAnimation(uid, animPlayer, "radiationcollector_animation"))
      return;
    RadiationCollectorVisualState collectorVisualState1 = state & RadiationCollectorVisualState.Active;
    RadiationCollectorVisualState collectorVisualState2 = comp.CurrentState & RadiationCollectorVisualState.Active;
    if (collectorVisualState1 != collectorVisualState2)
      collectorVisualState1 |= RadiationCollectorVisualState.Deactivating;
    comp.CurrentState = state;
    switch (collectorVisualState1)
    {
      case RadiationCollectorVisualState.Deactive:
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) RadiationCollectorVisualLayers.Main, RSI.StateId.op_Implicit(comp.InactiveState));
        break;
      case RadiationCollectorVisualState.Active:
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) RadiationCollectorVisualLayers.Main, RSI.StateId.op_Implicit(comp.ActiveState));
        break;
      case RadiationCollectorVisualState.Deactivating:
        this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animPlayer)), comp.DeactiveAnimation, "radiationcollector_animation");
        break;
      case RadiationCollectorVisualState.Activating:
        this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animPlayer)), comp.ActivateAnimation, "radiationcollector_animation");
        break;
    }
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    RadiationCollectorComponent comp,
    AnimationCompletedEvent args)
  {
    SpriteComponent sprite;
    AnimationPlayerComponent animPlayer;
    if (args.Key != "radiationcollector_animation" || !((EntitySystem) this).TryComp<SpriteComponent>(uid, ref sprite) || !((EntitySystem) this).TryComp<AnimationPlayerComponent>(uid, ref animPlayer))
      return;
    RadiationCollectorVisualState currentState;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<RadiationCollectorVisualState>(uid, (Enum) RadiationCollectorVisuals.VisualState, ref currentState, (AppearanceComponent) null))
      currentState = comp.CurrentState;
    RadiationCollectorVisualState state = currentState & RadiationCollectorVisualState.Active;
    this.UpdateVisuals(uid, state, comp, sprite, animPlayer);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    RadiationCollectorComponent comp,
    ref AppearanceChangeEvent args)
  {
    AnimationPlayerComponent animPlayer;
    if (args.Sprite == null || !((EntitySystem) this).TryComp<AnimationPlayerComponent>(uid, ref animPlayer))
      return;
    RadiationCollectorVisualState state;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<RadiationCollectorVisualState>(uid, (Enum) RadiationCollectorVisuals.VisualState, ref state, args.Component))
      state = RadiationCollectorVisualState.Deactive;
    this.UpdateVisuals(uid, state, comp, args.Sprite, animPlayer);
  }
}
