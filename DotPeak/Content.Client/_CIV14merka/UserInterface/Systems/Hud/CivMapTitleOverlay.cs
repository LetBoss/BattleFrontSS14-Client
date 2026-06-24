// Decompiled with JetBrains decompiler
// Type: Content.Client._CIV14merka.UserInterface.Systems.Hud.CivMapTitleOverlay
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Resources;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._CIV14merka.UserInterface.Systems.Hud;

public sealed class CivMapTitleOverlay : Overlay
{
  [Dependency]
  private IResourceCache _resources;
  [Dependency]
  private IGameTiming _timing;
  private readonly Font _font;
  private string? _title;
  private int _index;
  private bool _reverse;
  private TimeSpan _nextUpdate;
  private Vector2? _position;
  private TimeSpan _charInterval = TimeSpan.Zero;

  public virtual OverlaySpace Space => (OverlaySpace) 2;

  public CivMapTitleOverlay()
  {
    IoCManager.InjectDependencies<CivMapTitleOverlay>(this);
    this._font = this._resources.GetFont("/Fonts/NotoSansDisplay/NotoSansDisplay-Bold.ttf", 64 /*0x40*/);
    this.ZIndex = new int?(300);
  }

  public void Show(string title)
  {
    if (string.IsNullOrWhiteSpace(title))
    {
      this.Clear();
    }
    else
    {
      this._title = title.Trim();
      this._index = 0;
      this._reverse = false;
      this._nextUpdate = TimeSpan.Zero;
      this._position = new Vector2?();
      this._charInterval = TimeSpan.FromSeconds(2.0 / (double) Math.Max(1, this._title.Length));
    }
  }

  private void Clear()
  {
    this._title = (string) null;
    this._index = 0;
    this._reverse = false;
    this._nextUpdate = TimeSpan.Zero;
    this._position = new Vector2?();
    this._charInterval = TimeSpan.Zero;
  }

  protected virtual void Draw(in OverlayDrawArgs args)
  {
    if (string.IsNullOrEmpty(this._title))
      return;
    this._position.GetValueOrDefault();
    if (!this._position.HasValue)
      this._position = new Vector2?(this.CalculatePosition(in args));
    int length = Math.Clamp(this._index, 0, this._title.Length);
    if (length > 0)
    {
      string str = this._title.Substring(0, length);
      Vector2 vector2_1 = this._position.Value;
      DrawingHandleScreen screenHandle = ((OverlayDrawArgs) ref args).ScreenHandle;
      Font font = this._font;
      Vector2 vector2_2 = vector2_1 + new Vector2(2f, 2f);
      ReadOnlySpan<char> readOnlySpan = (ReadOnlySpan<char>) str;
      Color black = Color.Black;
      Color color = ((Color) ref black).WithAlpha(0.85f);
      screenHandle.DrawString(font, vector2_2, readOnlySpan, 1f, color);
      ((OverlayDrawArgs) ref args).ScreenHandle.DrawString(this._font, vector2_1, (ReadOnlySpan<char>) str, 1f, Color.White);
    }
    if (this._nextUpdate > this._timing.CurTime)
      return;
    if (!this._reverse && this._index >= this._title.Length)
    {
      this._reverse = true;
      this._nextUpdate = this._timing.CurTime + TimeSpan.FromSeconds(2L);
    }
    else if (this._reverse && this._index <= 0)
    {
      this.Clear();
    }
    else
    {
      this._index = this._reverse ? this._index - 1 : this._index + 1;
      this._nextUpdate = this._timing.CurTime + this._charInterval;
    }
  }

  private Vector2 CalculatePosition(in OverlayDrawArgs args)
  {
    Vector2 dimensions = ((OverlayDrawArgs) ref args).ScreenHandle.GetDimensions(this._font, (ReadOnlySpan<char>) this._title, 1f);
    return new Vector2((float) (((double) ((UIBox2i) ref args.ViewportBounds).Width - (double) dimensions.X) / 2.0), 110f);
  }
}
