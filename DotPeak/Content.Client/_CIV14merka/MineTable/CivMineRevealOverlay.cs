// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.MineTable.CivMineRevealOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._CIV14merka.MineTable;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.MineTable;

public sealed class CivMineRevealOverlay : Overlay
{
  [Dependency]
  private readonly IEyeManager _eye;
  [Dependency]
  private readonly IResourceCache _cache;
  private readonly Font _font;
  public List<CivMineRevealEntry> Entries = new List<CivMineRevealEntry>();

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivMineRevealOverlay()
  {
    IoCManager.InjectDependencies<CivMineRevealOverlay>(this);
    this._font = (Font) new VectorFont(this._cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Bold.ttf", true), 11);
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (this.Entries.Count == 0)
      return;
    MapId mapId = args.MapId;
    if (MapId.op_Equality(mapId, MapId.Nullspace))
      return;
    DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
    string str = Loc.GetString("civ-mine-table-label");
    Color orangeRed = Color.OrangeRed;
    foreach (CivMineRevealEntry entry in this.Entries)
    {
      if (!MapId.op_Inequality(entry.MapId, mapId))
      {
        Vector2 screen = this._eye.WorldToScreen(entry.Position);
        Vector2 dimensions = screenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) str, 1f);
        if ((double) dimensions.X > 0.0)
        {
          float num1 = dimensions.X * 0.5f;
          float num2 = dimensions.Y * 0.5f;
          UIBox2 uiBox2_1;
          // ISSUE: explicit constructor call
          ((UIBox2) ref uiBox2_1).\u002Ector((float) ((double) screen.X - (double) num1 - 3.0), (float) ((double) screen.Y - (double) num2 - 3.0), (float) ((double) screen.X + (double) num1 + 3.0), (float) ((double) screen.Y + (double) num2 + 3.0));
          DrawingHandleScreen drawingHandleScreen = screenHandle;
          UIBox2 uiBox2_2 = uiBox2_1;
          Color black = Color.Black;
          Color color = ((Color) ref black).WithAlpha(0.55f);
          drawingHandleScreen.DrawRect(uiBox2_2, color, true);
          screenHandle.DrawString(this._font, new Vector2(screen.X - num1, screen.Y - num2), (ReadOnlySpan<char>) str, 1f, orangeRed);
        }
      }
    }
  }
}
