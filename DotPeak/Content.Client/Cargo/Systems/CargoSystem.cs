// Decompiled with JetBrains decompiler
// Type: Content.Client.Cargo.Systems.CargoSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Cargo;
using Content.Shared.Cargo.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Cargo.Systems;

public sealed class CargoSystem : SharedCargoSystem
{
  [Dependency]
  private AnimationPlayerSystem _player;
  [Dependency]
  private SharedAppearanceSystem _appearance;
  [Dependency]
  private SpriteSystem _sprite;
  private static readonly Animation CargoTelepadBeamAnimation = new Animation()
  {
    Length = TimeSpan.FromSeconds(0.5),
    AnimationTracks = {
      (AnimationTrack) new AnimationTrackSpriteFlick()
      {
        LayerKey = (object) CargoSystem.CargoTelepadLayers.Beam,
        KeyFrames = {
          new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("beam"), 0.0f)
        }
      }
    }
  };
  private static readonly Animation CargoTelepadIdleAnimation = new Animation()
  {
    Length = TimeSpan.FromSeconds(0.8),
    AnimationTracks = {
      (AnimationTrack) new AnimationTrackSpriteFlick()
      {
        LayerKey = (object) CargoSystem.CargoTelepadLayers.Beam,
        KeyFrames = {
          new AnimationTrackSpriteFlick.KeyFrame(new RSI.StateId("idle"), 0.0f)
        }
      }
    }
  };
  private const string TelepadBeamKey = "cargo-telepad-beam";
  private const string TelepadIdleKey = "cargo-telepad-idle";

  public override void Initialize()
  {
    base.Initialize();
    this.InitializeCargoTelepad();
  }

  private void InitializeCargoTelepad()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CargoTelepadComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<CargoTelepadComponent, AppearanceChangeEvent>((object) this, __methodptr(OnCargoAppChange)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CargoTelepadComponent, AnimationCompletedEvent>(new ComponentEventHandler<CargoTelepadComponent, AnimationCompletedEvent>((object) this, __methodptr(OnCargoAnimComplete)), (Type[]) null, (Type[]) null);
  }

  private void OnCargoAppChange(
    EntityUid uid,
    CargoTelepadComponent component,
    ref AppearanceChangeEvent args)
  {
    this.OnChangeData(uid, args.Sprite);
  }

  private void OnCargoAnimComplete(
    EntityUid uid,
    CargoTelepadComponent component,
    AnimationCompletedEvent args)
  {
    this.OnChangeData(uid);
  }

  private void OnChangeData(EntityUid uid, SpriteComponent? sprite = null)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.Resolve<SpriteComponent>(uid, ref sprite, true) || !this.TryComp<AnimationPlayerComponent>(uid, ref animationPlayerComponent))
      return;
    CargoTelepadState? nullable;
    this._appearance.TryGetData<CargoTelepadState?>(uid, (Enum) CargoTelepadVisuals.State, ref nullable, (AppearanceComponent) null);
    if (nullable.HasValue)
    {
      switch (nullable.GetValueOrDefault())
      {
        case CargoTelepadState.Unpowered:
          this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) CargoSystem.CargoTelepadLayers.Beam, false);
          this._player.Stop(uid, animationPlayerComponent, "cargo-telepad-beam");
          this._player.Stop(uid, animationPlayerComponent, "cargo-telepad-idle");
          return;
        case CargoTelepadState.Teleporting:
          this._player.Stop(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), "cargo-telepad-idle");
          if (this._player.HasRunningAnimation(uid, "cargo-telepad-beam"))
            return;
          this._player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), CargoSystem.CargoTelepadBeamAnimation, "cargo-telepad-beam");
          return;
      }
    }
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, sprite)), (Enum) CargoSystem.CargoTelepadLayers.Beam, true);
    if (this._player.HasRunningAnimation(uid, animationPlayerComponent, "cargo-telepad-idle") || this._player.HasRunningAnimation(uid, animationPlayerComponent, "cargo-telepad-beam"))
      return;
    this._player.Play(Entity<AnimationPlayerComponent>.op_Implicit((uid, animationPlayerComponent)), CargoSystem.CargoTelepadIdleAnimation, "cargo-telepad-idle");
  }

  private enum CargoTelepadLayers : byte
  {
    Base,
    Beam,
  }
}
