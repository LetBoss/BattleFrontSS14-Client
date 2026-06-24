// Decompiled with JetBrains decompiler
// Type: Content.Client.Power.SMES.SmesVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Power;
using Content.Shared.SMES;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Power.SMES;

public sealed class SmesVisualizerSystem : VisualizerSystem<SmesComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    SmesComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    int num;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<int>(uid, (Enum) SmesVisuals.LastChargeLevel, ref num, args.Component) || num == 0)
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Charge, false);
    }
    else
    {
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Charge, true);
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Charge, RSI.StateId.op_Implicit($"{comp.ChargeOverlayPrefix}{num}"));
    }
    ChargeState chargeState;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<ChargeState>(uid, (Enum) SmesVisuals.LastChargeState, ref chargeState, args.Component))
      chargeState = ChargeState.Still;
    switch (chargeState)
    {
      case ChargeState.Still:
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Input, RSI.StateId.op_Implicit(comp.InputOverlayPrefix + "0"));
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Output, RSI.StateId.op_Implicit(comp.OutputOverlayPrefix + "1"));
        break;
      case ChargeState.Charging:
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Input, RSI.StateId.op_Implicit(comp.InputOverlayPrefix + "1"));
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Output, RSI.StateId.op_Implicit(comp.OutputOverlayPrefix + "1"));
        break;
      case ChargeState.Discharging:
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Input, RSI.StateId.op_Implicit(comp.InputOverlayPrefix + "0"));
        this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) SmesVisualLayers.Output, RSI.StateId.op_Implicit(comp.OutputOverlayPrefix + "2"));
        break;
    }
  }
}
