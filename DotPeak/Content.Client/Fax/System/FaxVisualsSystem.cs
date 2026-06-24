// Decompiled with JetBrains decompiler
// Type: Content.Client.Fax.System.FaxVisualsSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Fax;
using Content.Shared.Fax.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client.Fax.System;

public sealed class FaxVisualsSystem : EntitySystem
{
  [Dependency]
  private AnimationPlayerSystem _player;
  [Dependency]
  private SharedAppearanceSystem _appearance;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FaxMachineComponent, AppearanceChangeEvent>(new ComponentEventRefHandler<FaxMachineComponent, AppearanceChangeEvent>((object) this, __methodptr(OnAppearanceChanged)), (Type[]) null, (Type[]) null);
  }

  private void OnAppearanceChanged(
    EntityUid uid,
    FaxMachineComponent component,
    ref AppearanceChangeEvent args)
  {
    FaxMachineVisualState machineVisualState;
    if (args.Sprite == null || this._player.HasRunningAnimation(uid, "faxecute") || !this._appearance.TryGetData<FaxMachineVisualState>(uid, (Enum) FaxMachineVisuals.VisualState, ref machineVisualState, (AppearanceComponent) null) || machineVisualState != FaxMachineVisualState.Inserting)
      return;
    this._player.Play(uid, new Animation()
    {
      Length = TimeSpan.FromSeconds(2.4),
      AnimationTracks = {
        (AnimationTrack) new AnimationTrackSpriteFlick()
        {
          LayerKey = (object) FaxMachineVisuals.VisualState,
          KeyFrames = {
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit(component.InsertingState), 0.0f),
            new AnimationTrackSpriteFlick.KeyFrame(RSI.StateId.op_Implicit("icon"), 2.4f)
          }
        }
      }
    }, "faxecute");
  }
}
