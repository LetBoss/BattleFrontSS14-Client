// Decompiled with JetBrains decompiler
// Type: Content.Client.Turrets.DeployableTurretSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Power;
using Content.Shared.Turrets;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Turrets;

public sealed class DeployableTurretSystem : SharedDeployableTurretSystem
{
  [Dependency]
  private AppearanceSystem _appearance;
  [Dependency]
  private AnimationPlayerSystem _animation;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeployableTurretComponent, ComponentInit>(new EntityEventRefHandler<DeployableTurretComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeployableTurretComponent, AnimationCompletedEvent>(new EntityEventRefHandler<DeployableTurretComponent, AnimationCompletedEvent>((object) this, __methodptr(OnAnimationCompleted)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DeployableTurretComponent, AppearanceChangeEvent>(new EntityEventRefHandler<DeployableTurretComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(Entity<DeployableTurretComponent> ent, ref ComponentInit args)
  {
    ent.Comp.DeploymentAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) ent.Comp.DeploymentLength),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DeployableTurretVisuals.Turret,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(ent.Comp.DeployingState), 0.0f)
          }
        }
      }
    };
    ent.Comp.RetractionAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) ent.Comp.RetractionLength),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DeployableTurretVisuals.Turret,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(ent.Comp.RetractingState), 0.0f)
          }
        }
      }
    };
  }

  private void OnAnimationCompleted(
    Entity<DeployableTurretComponent> ent,
    ref AnimationCompletedEvent args)
  {
    SpriteComponent sprite;
    if (args.Key != "deployable_turret_animation" || !this.TryComp<SpriteComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref sprite))
      return;
    DeployableTurretState visualState;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<DeployableTurretState>(Entity<DeployableTurretComponent>.op_Implicit(ent), (Enum) DeployableTurretVisuals.Turret, ref visualState, (AppearanceComponent) null))
      visualState = ent.Comp.VisualState;
    DeployableTurretState state = visualState & DeployableTurretState.Deployed;
    this.UpdateVisuals(ent, state, sprite, args.AnimationPlayer);
  }

  private void OnAppearanceChange(
    Entity<DeployableTurretComponent> ent,
    ref AppearanceChangeEvent args)
  {
    AnimationPlayerComponent animPlayer;
    if (args.Sprite == null || !this.TryComp<AnimationPlayerComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref animPlayer))
      return;
    DeployableTurretState state;
    if (!((SharedAppearanceSystem) this._appearance).TryGetData<DeployableTurretState>(Entity<DeployableTurretComponent>.op_Implicit(ent), (Enum) DeployableTurretVisuals.Turret, ref state, args.Component))
      state = DeployableTurretState.Retracted;
    this.UpdateVisuals(ent, state, args.Sprite, animPlayer);
  }

  private void UpdateVisuals(
    Entity<DeployableTurretComponent> ent,
    DeployableTurretState state,
    SpriteComponent sprite,
    AnimationPlayerComponent? animPlayer = null)
  {
    if (!this.Resolve<AnimationPlayerComponent>(Entity<DeployableTurretComponent>.op_Implicit(ent), ref animPlayer, true) || this._animation.HasRunningAnimation(Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer, "deployable_turret_animation"))
      return;
    DeployableTurretState deployableTurretState1 = state & DeployableTurretState.Deployed;
    DeployableTurretState deployableTurretState2 = ent.Comp.VisualState & DeployableTurretState.Deployed;
    if (deployableTurretState1 != deployableTurretState2)
      deployableTurretState1 |= DeployableTurretState.Retracting;
    ent.Comp.VisualState = state;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) DeployableTurretVisuals.Weapon, (deployableTurretState1 & DeployableTurretState.Deployed) > DeployableTurretState.Retracted);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) PowerDeviceVisualLayers.Powered, this.HasAmmo(ent) && deployableTurretState1 == DeployableTurretState.Retracted);
    switch (deployableTurretState1)
    {
      case DeployableTurretState.Retracted:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) DeployableTurretVisuals.Turret, RSI.StateId.op_Implicit(ent.Comp.RetractedState));
        break;
      case DeployableTurretState.Deployed:
        this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((ent.Owner, sprite)), (Enum) DeployableTurretVisuals.Turret, RSI.StateId.op_Implicit(ent.Comp.DeployedState));
        break;
      case DeployableTurretState.Retracting:
        this._animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer)), (Animation) ent.Comp.RetractionAnimation, "deployable_turret_animation");
        break;
      case DeployableTurretState.Deploying:
        this._animation.Play(Entity<AnimationPlayerComponent>.op_Implicit((Entity<DeployableTurretComponent>.op_Implicit(ent), animPlayer)), (Animation) ent.Comp.DeploymentAnimation, "deployable_turret_animation");
        break;
    }
  }
}
