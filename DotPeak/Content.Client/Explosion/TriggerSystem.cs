// Decompiled with JetBrains decompiler
// Type: Content.Client.Explosion.TriggerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Trigger;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Explosion;

public sealed class TriggerSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _player;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;
  private const string AnimKey = "proximity";
  private static readonly Animation _flasherAnimation;

  public virtual void Initialize()
  {
    base.Initialize();
    this.InitializeProximity();
  }

  private void InitializeProximity()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TriggerOnProximityComponent, ComponentInit>(new ComponentEventHandler<TriggerOnProximityComponent, ComponentInit>((object) this, __methodptr(OnProximityInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TriggerOnProximityComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<TriggerOnProximityComponent, AppearanceChangeEvent>((object) this, __methodptr(OnProxAppChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TriggerOnProximityComponent, AnimationCompletedEvent>(new ComponentEventHandler<TriggerOnProximityComponent, AnimationCompletedEvent>((object) this, __methodptr(OnProxAnimation)), (Type[]) null, (Type[]) null);
  }

  private void OnProxAnimation(
    EntityUid uid,
    TriggerOnProximityComponent component,
    AnimationCompletedEvent args)
  {
    AppearanceComponent appearance;
    if (!this.TryComp<AppearanceComponent>(uid, ref appearance))
      return;
    this._appearance.SetData(uid, (Enum) ProximityTriggerVisualState.State, (object) ProximityTriggerVisuals.Inactive, appearance);
    this.OnChangeData(uid, component, appearance);
  }

  private void OnProximityInit(
    EntityUid uid,
    TriggerOnProximityComponent component,
    ComponentInit args)
  {
    this.EnsureComp<AnimationPlayerComponent>(uid);
  }

  private void OnProxAppChange(
    EntityUid uid,
    TriggerOnProximityComponent component,
    ref AppearanceChangeEvent args)
  {
    this.OnChangeData(uid, component, args.Component, args.Sprite);
  }

  private void OnChangeData(
    EntityUid uid,
    TriggerOnProximityComponent component,
    AppearanceComponent appearance,
    SpriteComponent? spriteComponent = null)
  {
    AnimationPlayerComponent animationPlayerComponent;
    ProximityTriggerVisuals proximityTriggerVisuals;
    int num;
    if (!this.Resolve<SpriteComponent>(uid, ref spriteComponent, true) || !this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent) || !this._appearance.TryGetData<ProximityTriggerVisuals>(uid, (Enum) ProximityTriggerVisualState.State, ref proximityTriggerVisuals, appearance) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), (Enum) TriggerSystem.ProximityTriggerVisualLayers.Base, ref num, false))
      return;
    switch (proximityTriggerVisuals)
    {
      case ProximityTriggerVisuals.Inactive:
        if (this._player.HasRunningAnimation(uid, animationPlayerComponent, "proximity"))
          break;
        this._player.Stop(uid, animationPlayerComponent, "proximity");
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, RSI.StateId.op_Implicit("on"));
        break;
      case ProximityTriggerVisuals.Active:
        if (this._player.HasRunningAnimation(uid, animationPlayerComponent, "proximity"))
          break;
        this._player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), TriggerSystem._flasherAnimation, "proximity");
        break;
      default:
        this._player.Stop(uid, animationPlayerComponent, "proximity");
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), num, RSI.StateId.op_Implicit("off"));
        break;
    }
  }

  static TriggerSystem()
  {
    Animation animation = new Animation();
    animation.Length = TimeSpan.FromSeconds(0.60000002384185791);
    animation.AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) TriggerSystem.ProximityTriggerVisualLayers.Base,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit("flashing"), 0.0f)
      }
    });
    List<AnimationTrack> animationTracks = animation.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (PointLightComponent);
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 2;
    componentProperty.Property = "AnimatedRadius";
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 0.1f, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 3f, 0.1f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 0.1f, 0.5f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    TriggerSystem._flasherAnimation = animation;
  }

  public enum ProximityTriggerVisualLayers : byte
  {
    Base,
  }
}
