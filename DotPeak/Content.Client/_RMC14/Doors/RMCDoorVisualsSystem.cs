// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Doors.RMCDoorVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Doors;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Doors;

public sealed class RMCDoorVisualsSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private SpriteSystem _sprite;
  private const string ButtonAnimationKey = "rmc_pod_door_button_animation";
  private readonly TimeSpan _buttonAnimationLength = TimeSpan.FromSeconds(1.25);

  public virtual void Initialize()
  {
    this.SubscribeNetworkEvent<RMCPodDoorButtonPressedEvent>(new EntityEventHandler<RMCPodDoorButtonPressedEvent>(this.OnPodDoorButtonPressed), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<RMCDoorButtonComponent, AnimationCompletedEvent>(new EntityEventRefHandler<RMCDoorButtonComponent, AnimationCompletedEvent>((object) this, __methodptr(OnDoorButtonAnimationCompleted)), (Type[]) null, (Type[]) null);
  }

  private void OnPodDoorButtonPressed(RMCPodDoorButtonPressedEvent ev)
  {
    EntityUid? nullable;
    RMCDoorButtonComponent doorButtonComponent;
    if (!this.TryGetEntity(ev.Button, ref nullable) || !this.TryComp<RMCDoorButtonComponent>(nullable, ref doorButtonComponent) || this._animation.HasRunningAnimation(nullable.Value, "rmc_pod_door_button_animation"))
      return;
    string animationState = ev.AnimationState;
    this._animation.Play(nullable.Value, new Animation()
    {
      Length = this._buttonAnimationLength,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) RMCPodDoorButtonLayers.Animation,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(animationState), 0.0f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(animationState), 0.5f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(animationState), 1f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(doorButtonComponent.OffState), 1.25f)
          }
        }
      }
    }, "rmc_pod_door_button_animation");
  }

  private void OnDoorButtonAnimationCompleted(
    Entity<RMCDoorButtonComponent> ent,
    ref AnimationCompletedEvent args)
  {
    SpriteComponent spriteComponent;
    if (args.Key != "rmc_pod_door_button_animation" || !this.TryComp<SpriteComponent>(Entity<RMCDoorButtonComponent>.op_Implicit(ent), ref spriteComponent))
      return;
    this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, spriteComponent)), (Enum) RMCPodDoorButtonLayers.Animation, RSI.StateId.op_Implicit(ent.Comp.OffState));
  }
}
