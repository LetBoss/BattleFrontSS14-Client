// Decompiled with JetBrains decompiler
// Type: Content.Client.Trigger.TimerTriggerVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Trigger;

public sealed class TimerTriggerVisualizerSystem : VisualizerSystem<TimerTriggerVisualsComponent>
{
  [Dependency]
  private SharedAudioSystem _audioSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<TimerTriggerVisualsComponent, ComponentInit>(new ComponentEventHandler<TimerTriggerVisualsComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(
    EntityUid uid,
    TimerTriggerVisualsComponent comp,
    ComponentInit args)
  {
    comp.PrimingAnimation = new Animation()
    {
      Length = TimeSpan.MaxValue,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) TriggerVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.PrimingSprite), 0.0f)
          }
        }
      }
    };
    if (comp.PrimingSound == null)
      return;
    comp.PrimingAnimation.AnimationTracks.Add((AnimationTrack) new AnimationTrackPlaySound()
    {
      KeyFrames = {
        new AnimationTrackPlaySound.KeyFrame(this._audioSystem.ResolveSound(comp.PrimingSound), 0.0f, (Func<AudioParams>) null)
      }
    });
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    TimerTriggerVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (args.Sprite == null || !((EntitySystem) this).TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
      return;
    TriggerVisualState triggerVisualState;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<TriggerVisualState>(uid, (Enum) TriggerVisuals.VisualState, ref triggerVisualState, args.Component))
      triggerVisualState = TriggerVisualState.Unprimed;
    if (triggerVisualState != TriggerVisualState.Primed)
    {
      if (triggerVisualState != TriggerVisualState.Unprimed)
        throw new ArgumentOutOfRangeException();
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) TriggerVisualLayers.Base, RSI.StateId.op_Implicit(comp.UnprimedSprite));
    }
    else
    {
      if (this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "priming_animation"))
        return;
      this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), comp.PrimingAnimation, "priming_animation");
    }
  }
}
