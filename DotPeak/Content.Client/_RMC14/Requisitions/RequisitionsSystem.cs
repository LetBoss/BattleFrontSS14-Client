// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Requisitions.RequisitionsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Requisitions;
using Content.Shared._RMC14.Requisitions.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Requisitions;

public sealed class RequisitionsSystem : SharedRequisitionsSystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private SpriteSystem _sprite;
  private const string AnimationKey = "cm_requisitions_animation";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequisitionsElevatorComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RequisitionsElevatorComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnElevatorHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequisitionsGearComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RequisitionsGearComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnGearHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RequisitionsRailingComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<RequisitionsRailingComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnRailingHandleState)), (Type[]) null, (Type[]) null);
  }

  private void OnElevatorHandleState(
    Entity<RequisitionsElevatorComponent> elevator,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((elevator.Owner, spriteComponent)), (Enum) RequisitionsElevatorLayers.Base, ref num, false))
      return;
    if (elevator.Comp.Mode != RequisitionsElevatorMode.Preparing)
      this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(elevator.Owner), "cm_requisitions_animation");
    switch (elevator.Comp.Mode)
    {
      case RequisitionsElevatorMode.Lowered:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((elevator.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(elevator.Comp.LoweredState));
        break;
      case RequisitionsElevatorMode.Raised:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((elevator.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(elevator.Comp.RaisedState));
        break;
      case RequisitionsElevatorMode.Lowering:
        RequisitionsElevatorComponent comp1 = elevator.Comp;
        if (comp1.LoweringAnimation == null)
          comp1.LoweringAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(2.0999999046325684),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) RequisitionsElevatorLayers.Base,
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(elevator.Comp.LoweringState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), (Animation) elevator.Comp.LoweringAnimation, "cm_requisitions_animation");
        break;
      case RequisitionsElevatorMode.Raising:
        RequisitionsElevatorComponent comp2 = elevator.Comp;
        if (comp2.RaisingAnimation == null)
          comp2.RaisingAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(2.0999999046325684),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) RequisitionsElevatorLayers.Base,
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(elevator.Comp.RaisingState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<RequisitionsElevatorComponent>.op_Implicit(elevator), (Animation) elevator.Comp.RaisingAnimation, "cm_requisitions_animation");
        break;
    }
  }

  private void OnGearHandleState(
    Entity<RequisitionsGearComponent> gear,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<RequisitionsGearComponent>.op_Implicit(gear), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((gear.Owner, spriteComponent)), (Enum) RequisitionsGearLayers.Base, ref num, false))
      return;
    string str1;
    switch (gear.Comp.Mode)
    {
      case RequisitionsGearMode.Static:
        str1 = gear.Comp.StaticState;
        break;
      case RequisitionsGearMode.Moving:
        str1 = gear.Comp.MovingState;
        break;
      default:
        str1 = gear.Comp.StaticState;
        break;
    }
    string str2 = str1;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((gear.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(str2));
  }

  private void OnRailingHandleState(
    Entity<RequisitionsRailingComponent> railing,
    ref AfterAutoHandleStateEvent args)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<RequisitionsRailingComponent>.op_Implicit(railing), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((railing.Owner, spriteComponent)), (Enum) RequisitionsRailingLayers.Base, ref num, false))
      return;
    this._animation.Stop(Entity<AnimationPlayerComponent>.op_Implicit(railing.Owner), "cm_requisitions_animation");
    switch (railing.Comp.Mode)
    {
      case RequisitionsRailingMode.Lowered:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((railing.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(railing.Comp.LoweredState));
        break;
      case RequisitionsRailingMode.Raised:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((railing.Owner, spriteComponent)), num, RSI.StateId.op_Implicit(railing.Comp.RaisedState));
        break;
      case RequisitionsRailingMode.Lowering:
        RequisitionsRailingComponent comp1 = railing.Comp;
        if (comp1.LowerAnimation == null)
          comp1.LowerAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(1.2000000476837158),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) RequisitionsRailingLayers.Base,
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(railing.Comp.LoweringState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<RequisitionsRailingComponent>.op_Implicit(railing), (Animation) railing.Comp.LowerAnimation, "cm_requisitions_animation");
        break;
      case RequisitionsRailingMode.Raising:
        RequisitionsRailingComponent comp2 = railing.Comp;
        if (comp2.RaiseAnimation == null)
          comp2.RaiseAnimation = (object) new Animation()
          {
            Length = TimeSpan.FromSeconds(1.2000000476837158),
            AnimationTracks = {
              (AnimationTrack) new AnimationTrackSpriteFlick()
              {
                LayerKey = (object) RequisitionsRailingLayers.Base,
                KeyFrames = {
                  new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(railing.Comp.RaisingState), 0.0f)
                }
              }
            }
          };
        this._animation.Play(Entity<RequisitionsRailingComponent>.op_Implicit(railing), (Animation) railing.Comp.RaiseAnimation, "cm_requisitions_animation");
        break;
    }
  }

  private void Animate(EntityUid uid, SpriteComponent sprite, Enum layerKey)
  {
    if (!this._sprite.LayerExists(Entity<SpriteComponent>.op_Implicit((uid, sprite)), layerKey) || !(sprite[(object) layerKey] is SpriteComponent.Layer layer))
      return;
    int? delayCount = layer.ActualState?.DelayCount;
    if (!delayCount.HasValue)
      return;
    int valueOrDefault = delayCount.GetValueOrDefault();
    if (layer.AnimationFrame < valueOrDefault - 1)
      return;
    this._sprite.LayerSetAutoAnimated(layer, false);
  }

  public virtual void FrameUpdate(float frameTime)
  {
    base.FrameUpdate(frameTime);
    EntityQueryEnumerator<RequisitionsElevatorComponent, SpriteComponent> entityQueryEnumerator1 = this.EntityQueryEnumerator<RequisitionsElevatorComponent, SpriteComponent>();
    EntityUid uid1;
    RequisitionsElevatorComponent elevatorComponent;
    SpriteComponent sprite1;
    while (entityQueryEnumerator1.MoveNext(ref uid1, ref elevatorComponent, ref sprite1))
      this.Animate(uid1, sprite1, (Enum) elevatorComponent.Mode);
    EntityQueryEnumerator<RequisitionsRailingComponent, SpriteComponent> entityQueryEnumerator2 = this.EntityQueryEnumerator<RequisitionsRailingComponent, SpriteComponent>();
    EntityUid uid2;
    RequisitionsRailingComponent railingComponent;
    SpriteComponent sprite2;
    while (entityQueryEnumerator2.MoveNext(ref uid2, ref railingComponent, ref sprite2))
      this.Animate(uid2, sprite2, (Enum) railingComponent.Mode);
  }
}
