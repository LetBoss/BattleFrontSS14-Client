// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Atmos.RMCFlammableSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Atmos;
using Content.Shared.Mobs;
using Content.Shared.Standing;
using Content.Shared.StatusEffect;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Client._RMC14.Atmos;

public sealed class RMCFlammableSystem : SharedRMCFlammableSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  private const string RollKey = "StopDropRollAnimation";
  private static readonly ProtoId<StatusEffectPrototype> KnockdownedKey = ProtoId<StatusEffectPrototype>.op_Implicit("KnockedDown");

  public override void Initialize()
  {
    base.Initialize();
    this.SubscribeAllEvent<RMCStopDropRollVisualsNetworkEvent>(new EntityEventHandler<RMCStopDropRollVisualsNetworkEvent>(this.OnResist), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCStopDropRollVisualsComponent, MobStateChangedEvent>(new EntityEventRefHandler<RMCStopDropRollVisualsComponent, MobStateChangedEvent>((object) this, __methodptr(OnMobStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCStopDropRollVisualsComponent, StatusEffectEndedEvent>(new EntityEventRefHandler<RMCStopDropRollVisualsComponent, StatusEffectEndedEvent>((object) this, __methodptr(OnStatusEffectEnded)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCStopDropRollVisualsComponent, StoodEvent>(new EntityEventRefHandler<RMCStopDropRollVisualsComponent, StoodEvent>((object) this, __methodptr(OnStood)), (Type[]) null, (Type[]) null);
  }

  private void OnResist(RMCStopDropRollVisualsNetworkEvent ev)
  {
    EntityUid? nullable;
    if (!this.TryGetEntity(ev.User, ref nullable) || !this.HasComp<RMCStopDropRollVisualsComponent>(nullable) || this._animation.HasRunningAnimation(nullable.Value, "StopDropRollAnimation"))
      return;
    Animation animation1 = new Animation();
    animation1.Length = TimeSpan.FromSeconds(5L);
    List<AnimationTrack> animationTracks = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (TransformComponent);
    componentProperty.Property = "LocalRotation";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 0;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(90.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(180.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.FromDegrees(270.0), 0.25f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Angle.Zero, 0.25f, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    Animation animation2 = animation1;
    this._animation.Play(nullable.Value, animation2, "StopDropRollAnimation");
  }

  private void OnMobStateChanged(
    Entity<RMCStopDropRollVisualsComponent> ent,
    ref MobStateChangedEvent args)
  {
    if (args.NewMobState == MobState.Alive)
      return;
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
  }

  private void OnStatusEffectEnded(
    Entity<RMCStopDropRollVisualsComponent> ent,
    ref StatusEffectEndedEvent args)
  {
    if (ProtoId<StatusEffectPrototype>.op_Inequality(ProtoId<StatusEffectPrototype>.op_Implicit(args.Key), RMCFlammableSystem.KnockdownedKey))
      return;
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
  }

  private void OnStood(Entity<RMCStopDropRollVisualsComponent> ent, ref StoodEvent args)
  {
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "StopDropRollAnimation");
  }
}
