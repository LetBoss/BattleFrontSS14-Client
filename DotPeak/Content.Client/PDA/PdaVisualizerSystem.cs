// Decompiled with JetBrains decompiler
// Type: Content.Client.PDA.PdaVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Light;
using Content.Shared.PDA;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.PDA;

public sealed class PdaVisualizerSystem : VisualizerSystem<PdaVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PdaVisualsComponent comp,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    string str;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) PdaVisuals.PdaType, ref str, args.Component))
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PdaVisualizerSystem.PdaVisualLayers.Base, RSI.StateId.op_Implicit(str));
    bool flag1;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) UnpoweredFlashlightVisuals.LightOn, ref flag1, args.Component))
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PdaVisualizerSystem.PdaVisualLayers.Flashlight, flag1);
    bool flag2;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<bool>(uid, (Enum) PdaVisuals.IdCardInserted, ref flag2, args.Component))
      return;
    this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PdaVisualizerSystem.PdaVisualLayers.IdLight, flag2);
  }

  public enum PdaVisualLayers : byte
  {
    Base,
    Flashlight,
    IdLight,
  }
}
