// Decompiled with JetBrains decompiler
// Type: Content.Client.Disposal.Unit.DisposalUnitSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Disposal.Components;
using Content.Shared.Disposal.Unit;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Analyzers;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Disposal.Unit;

public sealed class DisposalUnitSystem : SharedDisposalUnitSystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private AnimationPlayerSystem _animationSystem;
  [Dependency]
  private SharedAudioSystem _audioSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;
  [Dependency]
  private SpriteSystem _sprite;
  private const string AnimationKey = "disposal_unit_animation";
  private const string DefaultFlushState = "disposal-flush";
  private const string DefaultChargeState = "disposal-charging";

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, AfterAutoHandleStateEvent>(new ComponentEventRefHandler<DisposalUnitComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnHandleState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DisposalUnitComponent, AppearanceChangeEvent>(new EntityEventRefHandler<DisposalUnitComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnHandleState(
    EntityUid uid,
    DisposalUnitComponent component,
    ref AfterAutoHandleStateEvent args)
  {
    this.UpdateUI(Entity<DisposalUnitComponent>.op_Implicit((uid, component)));
  }

  protected override void UpdateUI(Entity<DisposalUnitComponent> entity)
  {
    DisposalUnitBoundUserInterface boundUserInterface;
    if (!this._uiSystem.TryGetOpenUi<DisposalUnitBoundUserInterface>(Entity<UserInterfaceComponent>.op_Implicit(entity.Owner), (Enum) DisposalUnitComponent.DisposalUnitUiKey.Key, ref boundUserInterface))
      return;
    boundUserInterface.Refresh(entity);
  }

  protected override void OnDisposalInit(Entity<DisposalUnitComponent> ent, ref ComponentInit args)
  {
    base.OnDisposalInit(ent, ref args);
    SpriteComponent sprite;
    AppearanceComponent appearance;
    if (!this.TryComp<SpriteComponent>(Entity<DisposalUnitComponent>.op_Implicit(ent), ref sprite) || !this.TryComp<AppearanceComponent>(Entity<DisposalUnitComponent>.op_Implicit(ent), ref appearance))
      return;
    this.UpdateState(ent, sprite, appearance);
  }

  private void OnAppearanceChange(Entity<DisposalUnitComponent> ent, ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    this.UpdateState(ent, args.Sprite, args.Component);
  }

  private void UpdateState(
    Entity<DisposalUnitComponent> ent,
    SpriteComponent sprite,
    AppearanceComponent appearance)
  {
    DisposalUnitComponent.VisualState visualState;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<DisposalUnitComponent.VisualState>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum) DisposalUnitComponent.Visuals.VisualState, ref visualState, appearance))
      return;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.Unanchored, visualState == DisposalUnitComponent.VisualState.UnAnchored);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.Base, visualState == DisposalUnitComponent.VisualState.Anchored);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayFlush, visualState == DisposalUnitComponent.VisualState.OverlayFlushing);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.BaseCharging, visualState == DisposalUnitComponent.VisualState.OverlayCharging);
    int num1;
    if (!this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.BaseCharging, ref num1, false))
    {
      RSI.StateId stateId1 = new RSI.StateId("disposal-charging");
    }
    else
      this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), num1);
    if (visualState == DisposalUnitComponent.VisualState.OverlayFlushing)
    {
      if (!this._animationSystem.HasRunningAnimation(Entity<DisposalUnitComponent>.op_Implicit(ent), "disposal_unit_animation"))
      {
        int num2;
        RSI.StateId stateId2 = this._sprite.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayFlush, ref num2, false) ? this._sprite.LayerGetRsiState(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), num2) : new RSI.StateId("disposal-flush");
        Animation animation = new Animation()
        {
          Length = ent.Comp.FlushDelay,
          AnimationTracks = {
            (AnimationTrack) new AnimationTrackSpriteFlick()
            {
              LayerKey = (object) DisposalUnitVisualLayers.OverlayFlush,
              KeyFrames = {
                new AnimationTrackSpriteFlick.KeyFrame(stateId2, 0.0f)
              }
            }
          }
        };
        if (ent.Comp.FlushSound != null)
          animation.AnimationTracks.Add((AnimationTrack) new AnimationTrackPlaySound()
          {
            KeyFrames = {
              new AnimationTrackPlaySound.KeyFrame(this._audioSystem.ResolveSound(ent.Comp.FlushSound), 0.0f, (Func<AudioParams>) null)
            }
          });
        this._animationSystem.Play(Entity<DisposalUnitComponent>.op_Implicit(ent), animation, "disposal_unit_animation");
      }
    }
    else
      this._animationSystem.Stop(Entity<AnimationPlayerComponent>.op_Implicit(ent.Owner), "disposal_unit_animation");
    DisposalUnitComponent.HandleState handleState;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<DisposalUnitComponent.HandleState>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum) DisposalUnitComponent.Visuals.Handle, ref handleState, appearance))
      handleState = DisposalUnitComponent.HandleState.Normal;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayEngaged, handleState != 0);
    DisposalUnitComponent.LightStates lightStates;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<DisposalUnitComponent.LightStates>(Entity<DisposalUnitComponent>.op_Implicit(ent), (Enum) DisposalUnitComponent.Visuals.Light, ref lightStates, appearance))
      lightStates = DisposalUnitComponent.LightStates.Off;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayCharging, (lightStates & DisposalUnitComponent.LightStates.Charging) != 0);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayReady, (lightStates & DisposalUnitComponent.LightStates.Ready) != 0);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((Entity<DisposalUnitComponent>.op_Implicit(ent), sprite)), (Enum) DisposalUnitVisualLayers.OverlayFull, (lightStates & DisposalUnitComponent.LightStates.Full) != 0);
  }
}
