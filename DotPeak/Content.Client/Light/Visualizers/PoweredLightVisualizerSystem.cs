// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.Visualizers.PoweredLightVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Random;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Light.Visualizers;

public sealed class PoweredLightVisualizerSystem : VisualizerSystem<PoweredLightVisualsComponent>
{
  [Dependency]
  private IRobustRandom _random;
  [Dependency]
  private SharedAudioSystem _audio;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    ((EntitySystem) this).SubscribeLocalEvent<PoweredLightVisualsComponent, AnimationCompletedEvent>(new ComponentEventHandler<PoweredLightVisualsComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PoweredLightVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    PoweredLightState key;
    if (args.Sprite == null || !((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<PoweredLightState>(uid, (Enum) PoweredLightVisuals.BulbState, ref key, args.Component))
      return;
    string str;
    if (comp.SpriteStateMap.TryGetValue(key, out str))
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PoweredLightLayers.Base, RSI.StateId.op_Implicit(str));
    if (this.SpriteSystem.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PoweredLightLayers.Glow))
    {
      PointLightComponent pointLightComponent;
      if (((EntitySystem) this).TryComp<PointLightComponent>(uid, ref pointLightComponent))
        this.SpriteSystem.LayerSetColor(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PoweredLightLayers.Glow, ((SharedPointLightComponent) pointLightComponent).Color);
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PoweredLightLayers.Glow, key == PoweredLightState.On);
    }
    bool flag;
    this.SetBlinkingAnimation(uid, key == PoweredLightState.On && ((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) PoweredLightVisuals.Blinking, ref flag, args.Component) & flag, comp);
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    PoweredLightVisualsComponent comp,
    AnimationCompletedEvent args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (!((EntitySystem) this).TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || args.Key != "poweredlight_blinking" || !comp.IsBlinking)
      return;
    this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.BlinkingAnimation(comp), "poweredlight_blinking");
  }

  private void SetBlinkingAnimation(
    EntityUid uid,
    bool shouldBeBlinking,
    PoweredLightVisualsComponent comp)
  {
    if (shouldBeBlinking == comp.IsBlinking)
      return;
    comp.IsBlinking = shouldBeBlinking;
    AnimationPlayerComponent animationPlayerComponent = ((EntitySystem) this).EnsureComp<AnimationPlayerComponent>(uid);
    if (shouldBeBlinking)
    {
      this.AnimationSystem.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), this.BlinkingAnimation(comp), "poweredlight_blinking");
    }
    else
    {
      if (!this.AnimationSystem.HasRunningAnimation(uid, animationPlayerComponent, "poweredlight_blinking"))
        return;
      this.AnimationSystem.Stop(uid, animationPlayerComponent, "poweredlight_blinking");
    }
  }

  private Animation BlinkingAnimation(PoweredLightVisualsComponent comp)
  {
    float num = MathHelper.Lerp(comp.MinBlinkingAnimationCycleTime, comp.MaxBlinkingAnimationCycleTime, this._random.NextFloat());
    Animation animation1 = new Animation();
    animation1.Length = TimeSpan.FromSeconds((double) num);
    List<AnimationTrack> animationTracks = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (PointLightComponent);
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 2;
    componentProperty.Property = "AnimatedEnable";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) false, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) true, 1f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    animation1.AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) PoweredLightLayers.Base,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.SpriteStateMap[PoweredLightState.Off]), 0.0f),
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.SpriteStateMap[PoweredLightState.On]), 0.5f)
      }
    });
    Animation animation2 = animation1;
    if (comp.BlinkingSound != null)
    {
      ResolvedSoundSpecifier resolvedSoundSpecifier = this._audio.ResolveSound(comp.BlinkingSound);
      animation2.AnimationTracks.Add((AnimationTrack) new AnimationTrackPlaySound()
      {
        KeyFrames = {
          new AnimationTrackPlaySound.KeyFrame(resolvedSoundSpecifier, 0.5f, (Func<AudioParams>) null)
        }
      });
    }
    return animation2;
  }
}
