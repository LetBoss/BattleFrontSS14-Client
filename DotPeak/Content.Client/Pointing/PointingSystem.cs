// Decompiled with JetBrains decompiler
// Type: Content.Client.Pointing.PointingSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Pointing.Components;
using Content.Shared.Pointing;
using Content.Shared.Verbs;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Pointing;

public sealed class PointingSystem : SharedPointingSystem
{
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private IEyeManager _eyeManager;
  [Dependency]
  private AnimationPlayerSystem _animationPlayer;
  [Dependency]
  private TransformSystem _transformSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    this.SubscribeLocalEvent<GetVerbsEvent<Verb>>(new EntityEventHandler<GetVerbsEvent<Verb>>(this.AddPointingVerb), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PointingArrowComponent, ComponentStartup>(new ComponentEventHandler<PointingArrowComponent, ComponentStartup>((object) this, __methodptr(OnArrowStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RoguePointingArrowComponent, ComponentStartup>(new ComponentEventHandler<RoguePointingArrowComponent, ComponentStartup>((object) this, __methodptr(OnRogueArrowStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PointingArrowComponent, ComponentHandleState>(new EntityEventRefHandler<PointingArrowComponent, ComponentHandleState>((object) this, __methodptr(HandleCompState)), (Type[]) null, (Type[]) null);
    this.InitializeVisualizer();
  }

  private void AddPointingVerb(GetVerbsEvent<Verb> args)
  {
    if (this.IsClientSide(args.Target, (MetaDataComponent) null) || this.HasComp<PointingArrowComponent>(args.Target) || !this.CanPoint(args.User))
      return;
    Verb verb = new Verb()
    {
      Text = this.Loc.GetString("pointing-verb-get-data-text"),
      Icon = (SpriteSpecifier) new SpriteSpecifier.Texture(new ResPath("/Textures/Interface/VerbIcons/point.svg.192dpi.png")),
      ClientExclusive = true,
      Act = (Action) (() => this.RaiseNetworkEvent((EntityEventArgs) new PointingAttemptEvent(this.GetNetEntity(args.Target, (MetaDataComponent) null))))
    };
    args.Verbs.Add(verb);
  }

  private void OnArrowStartup(
    EntityUid uid,
    PointingArrowComponent component,
    ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 13);
    this.BeginPointAnimation(uid, component.StartPosition, component.Offset, component.AnimationKey);
  }

  private void OnRogueArrowStartup(
    EntityUid uid,
    RoguePointingArrowComponent arrow,
    ComponentStartup args)
  {
    SpriteComponent spriteComponent;
    if (!this.TryComp<SpriteComponent>(uid, ref spriteComponent))
      return;
    this._sprite.SetDrawDepth(Entity<SpriteComponent>.op_Implicit((uid, spriteComponent)), 13);
    spriteComponent.NoRotation = false;
  }

  private void HandleCompState(Entity<PointingArrowComponent> entity, ref ComponentHandleState args)
  {
    if (!(((ComponentHandleState) ref args).Current is SharedPointingSystem.SharedPointingArrowComponentState current))
      return;
    entity.Comp.StartPosition = current.StartPosition;
    entity.Comp.EndTime = current.EndTime;
  }

  public void InitializeVisualizer()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<PointingArrowComponent, AnimationCompletedEvent>(new ComponentEventHandler<PointingArrowComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnAnimationCompleted(
    EntityUid uid,
    PointingArrowComponent component,
    AnimationCompletedEvent args)
  {
    if (!(args.Key == component.AnimationKey))
      return;
    this._animationPlayer.Stop(Entity<AnimationPlayerComponent>.op_Implicit(uid), component.AnimationKey);
  }

  private void BeginPointAnimation(
    EntityUid uid,
    Vector2 startPosition,
    Vector2 offset,
    string animationKey)
  {
    if (this._animationPlayer.HasRunningAnimation(uid, animationKey))
      return;
    Angle angle = new Angle(Angle.op_Implicit(Angle.op_Addition(this._eyeManager.CurrentEye.Rotation, ((SharedTransformSystem) this._transformSystem).GetWorldRotation(uid))));
    startPosition = ((Angle) ref angle).RotateVec(ref startPosition);
    Animation animation1 = new Animation();
    animation1.Length = this.PointDuration;
    List<AnimationTrack> animationTracks = animation1.AnimationTracks;
    AnimationTrackComponentProperty componentProperty = new AnimationTrackComponentProperty();
    componentProperty.ComponentType = typeof (SpriteComponent);
    componentProperty.Property = "Offset";
    ((AnimationTrackProperty) componentProperty).InterpolationMode = (AnimationInterpolationMode) 1;
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) startPosition, 0.0f, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Lerp(startPosition, offset, 0.9f), this.PointKeyTimeMove, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, this.PointKeyTimeMove, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, this.PointKeyTimeMove, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) offset, this.PointKeyTimeHover, (Func<float, float>) null));
    ((AnimationTrackProperty) componentProperty).KeyFrames.Add(new AnimationTrackProperty.KeyFrame((object) Vector2.Zero, this.PointKeyTimeHover, (Func<float, float>) null));
    animationTracks.Add((AnimationTrack) componentProperty);
    Animation animation2 = animation1;
    this._animationPlayer.Play(uid, animation2, animationKey);
  }
}
