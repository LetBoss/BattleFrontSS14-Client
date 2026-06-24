// Decompiled with JetBrains decompiler
// Type: Content.Client.Paper.UI.PaperVisualizerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared.Paper;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client.Paper.UI;

public sealed class PaperVisualizerSystem : VisualizerSystem<PaperVisualsComponent>
{
  protected virtual void OnAppearanceChange(
    EntityUid uid,
    PaperVisualsComponent component,
    ref AppearanceChangeEvent args)
  {
    if (args.Sprite == null)
      return;
    PaperComponent.PaperStatus paperStatus;
    if (((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<PaperComponent.PaperStatus>(uid, (Enum) PaperComponent.PaperVisuals.Status, ref paperStatus, args.Component))
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PaperVisualLayers.Writing, paperStatus == PaperComponent.PaperStatus.Written);
    string str;
    if (!((SharedAppearanceSystem) this.AppearanceSystem).TryGetData<string>(uid, (Enum) PaperComponent.PaperVisuals.Stamp, ref str, args.Component))
      return;
    if (str != string.Empty)
    {
      this.SpriteSystem.LayerSetRsiState(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PaperVisualLayers.Stamp, RSI.StateId.op_Implicit(str));
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PaperVisualLayers.Stamp, true);
    }
    else
      this.SpriteSystem.LayerSetVisible(Entity<SpriteComponent>.op_Implicit((uid, args.Sprite)), (Enum) PaperVisualLayers.Stamp, false);
  }
}
