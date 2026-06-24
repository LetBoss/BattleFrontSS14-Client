// Decompiled with JetBrains decompiler
// Type: Content.Client.Doors.FirelockSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Doors.Components;
using Content.Shared.Doors.Systems;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Doors;

public sealed class FirelockSystem : SharedFirelockSystem
{
  [Dependency]
  private SharedAppearanceSystem _appearanceSystem;
  [Dependency]
  private SpriteSystem _sprite;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FirelockComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<FirelockComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChange)), (Type[]) null, (Type[]) null);
  }

  protected override void OnComponentStartup(
    Entity<FirelockComponent> ent,
    ref ComponentStartup args)
  {
    base.OnComponentStartup(ent, ref args);
    DoorComponent doorComponent;
    if (!this.TryComp<DoorComponent>(ent.Owner, ref doorComponent))
      return;
    doorComponent.ClosedSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));
    doorComponent.OpenSpriteStates.Add((DoorVisualLayers.BaseUnlit, ent.Comp.WarningLightSpriteState));
    ((Animation) doorComponent.OpeningAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) DoorVisualLayers.BaseUnlit,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(ent.Comp.OpeningLightSpriteState), 0.0f)
      }
    });
    ((Animation) doorComponent.ClosingAnimation).AnimationTracks.Add((AnimationTrack) new AnimationTrackSpriteFlick()
    {
      LayerKey = (object) DoorVisualLayers.BaseUnlit,
      KeyFrames = {
        new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(ent.Comp.ClosingLightSpriteState), 0.0f)
      }
    });
  }

  private void OnAppearanceChange(
    EntityUid uid,
    FirelockComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    DoorState doorState;
    if (!this._appearanceSystem.TryGetData<DoorState>(uid, (Enum) DoorVisuals.State, ref doorState, args.Component))
      doorState = DoorState.Closed;
    bool flag1;
    bool flag2 = this._appearanceSystem.TryGetData<bool>(uid, (Enum) DoorVisuals.BoltLights, ref flag1, args.Component) & flag1;
    bool flag3;
    bool flag4 = doorState == DoorState.Closing || doorState == DoorState.Opening || doorState == DoorState.Denying || this._appearanceSystem.TryGetData<bool>(uid, (Enum) DoorVisuals.ClosedLights, ref flag3, args.Component) & flag3;
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseUnlit, flag4 && !flag2);
    this._sprite.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) DoorVisualLayers.BaseBolted, flag2);
  }
}
