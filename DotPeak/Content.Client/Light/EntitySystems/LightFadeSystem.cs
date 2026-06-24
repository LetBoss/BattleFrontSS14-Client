// Decompiled with JetBrains decompiler
// Type: Content.Client.Light.EntitySystems.LightFadeSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Light.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client.Light.EntitySystems;

public sealed class LightFadeSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _player;
  private const string FadeTrack = "light-fade";

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<LightFadeComponent, ComponentStartup>(new ComponentEventHandler<LightFadeComponent, ComponentStartup>((object) this, __methodptr(OnFadeStartup)), (Type[]) null, (Type[]) null);
  }

  private void OnFadeStartup(EntityUid uid, LightFadeComponent component, ComponentStartup args)
  {
    PointLightComponent pointLightComponent;
    if (!this.TryComp<PointLightComponent>(uid, ref pointLightComponent))
      return;
    Animation animation1 = new Animation();
    animation1.Length = TimeSpan.FromSeconds((double) component.Duration);
    List<AnimationTrack> animationTracks = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.Property = "Energy";
    componentProperty.ComponentType = typeof (PointLightComponent);
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 1;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) ((SharedPointLightComponent) pointLightComponent).Energy, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) 0.0f, component.Duration, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    Animation animation2 = animation1;
    this._player.Play(uid, animation2, "light-fade");
  }
}
