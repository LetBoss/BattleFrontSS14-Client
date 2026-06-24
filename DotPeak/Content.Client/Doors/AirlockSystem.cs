// Decompiled with JetBrains decompiler
// Type: Content.Client.Doors.AirlockSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Wires.Visualizers;
using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Content.Shared.Power;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Doors;

public sealed class AirlockSystem : SharedAirlockSystem
{
  [Dependency]
  private AppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AirlockComponent, ComponentStartup>(new ComponentEventHandler<AirlockComponent, ComponentStartup>((object) this, __methodptr(OnComponentStartup)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<AirlockComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<AirlockComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentStartup(EntityUid uid, AirlockComponent comp, ComponentStartup args)
  {
    DoorComponent doorComponent;
    if (!this.TryComp<DoorComponent>(uid, ref doorComponent))
      return;
    if (comp.OpenUnlitVisible)
    {
      doorComponent.OpenSpriteStates.Add((DoorVisualLayers.BaseUnlit, comp.OpenSpriteState));
      doorComponent.ClosedSpriteStates.Add((DoorVisualLayers.BaseUnlit, comp.ClosedSpriteState));
    }
    ((Animation) doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) DoorVisualLayers.BaseUnlit,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.OpeningSpriteState), 0.0f)
      }
    });
    ((Animation) doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) DoorVisualLayers.BaseUnlit,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.ClosingSpriteState), 0.0f)
      }
    });
    doorComponent.DenyingAnimation = (object) new Animation()
    {
      Length = TimeSpan.FromSeconds((double) comp.DenyAnimationTime),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) DoorVisualLayers.BaseUnlit,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.DenySpriteState), 0.0f)
          }
        }
      }
    };
    if (!comp.AnimatePanel)
      return;
    ((Animation) doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) WiresVisualLayers.MaintenancePanel,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.OpeningPanelSpriteState), 0.0f)
      }
    });
    ((Animation) doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) WiresVisualLayers.MaintenancePanel,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(comp.ClosingPanelSpriteState), 0.0f)
      }
    });
  }

  private void OnAppearanceChange(
    EntityUid uid,
    AirlockComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    DoorState doorState;
    if (!((SharedAppearanceSystem) this._appearanceSystem).TryGetData<DoorState>(uid, (Enum) DoorVisuals.State, ref doorState, args.Component))
      doorState = DoorState.Closed;
    bool flag4;
    if (((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) PowerDeviceVisuals.Powered, ref flag4, args.Component) & flag4)
    {
      bool flag5;
      flag1 = ((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) DoorVisuals.BoltLights, ref flag5, args.Component) & flag5 && (doorState == DoorState.Closed || doorState == DoorState.Welded);
      bool flag6;
      flag2 = ((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) DoorVisuals.EmergencyLights, ref flag6, args.Component) & flag6;
      int num;
      switch (doorState)
      {
        case DoorState.Closing:
        case DoorState.Opening:
        case DoorState.Denying:
          if (!flag1)
          {
            num = !flag2 ? 1 : 0;
            goto label_11;
          }
          break;
        case DoorState.Open:
          if (comp.OpenUnlitVisible)
            goto case DoorState.Closing;
          goto default;
        default:
          bool flag7;
          if (!(((SharedAppearanceSystem) this._appearanceSystem).TryGetData<bool>(uid, (Enum) DoorVisuals.ClosedLights, ref flag7, args.Component) & flag7))
            break;
          goto case DoorState.Closing;
      }
      num = 0;
label_11:
      flag3 = num != 0;
    }
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, flag3);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseBolted, flag1);
    if (comp.EmergencyAccessLayer)
      this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseEmergencyAccess, flag2 && doorState != DoorState.Open && doorState != DoorState.Opening && doorState != DoorState.Closing && !flag1);
    if (doorState != DoorState.Closed)
    {
      if (doorState != DoorState.Open)
        return;
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, RSI.StateId.op_Implicit(comp.ClosingSpriteState));
      this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, 0.0f);
    }
    else
    {
      this._sprite.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, RSI.StateId.op_Implicit(comp.OpeningSpriteState));
      this._sprite.LayerSetAnimationTime(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, 0.0f);
    }
  }
}
