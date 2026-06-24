// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.AegisCrate.AegisCrateSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.AegisCrate;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.AegisCrate;

public sealed class AegisCrateSystem : SharedAegisCrateSystem
{
  private const string AnimationKey = "AegisCrateOpenAnim";
  private Animation? _openingAnimation;
  [Dependency]
  private SpriteSystem _sprite;
  [Dependency]
  private AnimationPlayerSystem _animation;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AegisCrateComponent, AegisCrateStateChangedEvent>(new EntityEventRefHandler<AegisCrateComponent, AegisCrateStateChangedEvent>((object) this, __methodptr(OnStateChanged)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AegisCrateComponent, AnimationCompletedEvent>(new EntityEventRefHandler<AegisCrateComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAegisAnimationFinished)), (Type[]) null, (Type[]) null);
    this._openingAnimation = new Animation()
    {
      Length = this.OpeningSpeed,
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) AegisCrateVisualLayers.Base,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit("aegis_crate_opening"), 0.0f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit("aegis_crate_open"), 1.46f)
          }
        }
      }
    };
  }

  protected override void OnStartup(Entity<AegisCrateComponent> crate, ref ComponentStartup args)
  {
    base.OnStartup(crate, ref args);
    this.SetVisuals(crate);
  }

  private void SetVisuals(Entity<AegisCrateComponent> ent)
  {
    SpriteComponent spriteComponent;
    int num;
    if (!this.TryComp<SpriteComponent>(Entity<AegisCrateComponent>.op_Implicit(ent), ref spriteComponent) || !this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), spriteComponent)), (Enum) AegisCrateVisualLayers.Base, ref num, true))
      return;
    switch (ent.Comp.State)
    {
      case AegisCrateState.Closed:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), spriteComponent)), num, RSI.StateId.op_Implicit("aegis_crate"));
        break;
      case AegisCrateState.Opening:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), spriteComponent)), num, RSI.StateId.op_Implicit("aegis_crate_opening"));
        if (this._animation.HasRunningAnimation(Entity<AegisCrateComponent>.op_Implicit(ent), "AegisCrateOpenAnim"))
          break;
        this._animation.Play(Entity<AegisCrateComponent>.op_Implicit(ent), this._openingAnimation, "AegisCrateOpenAnim");
        break;
      case AegisCrateState.Open:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<AegisCrateComponent>.op_Implicit(ent), spriteComponent)), num, RSI.StateId.op_Implicit("aegis_crate_open"));
        break;
    }
  }

  private void OnStateChanged(Entity<AegisCrateComponent> ent, ref AegisCrateStateChangedEvent args)
  {
    this.SetVisuals(ent);
  }

  private void OnAegisAnimationFinished(
    Entity<AegisCrateComponent> ent,
    ref AnimationCompletedEvent args)
  {
    AnimationPlayerComponent animationPlayerComponent;
    if (!this.TryComp<AnimationPlayerComponent>(Entity<AegisCrateComponent>.op_Implicit(ent), ref animationPlayerComponent))
      return;
    this._animation.Stop(Entity<AegisCrateComponent>.op_Implicit(ent), animationPlayerComponent, "AegisCrateOpenAnim");
  }
}
