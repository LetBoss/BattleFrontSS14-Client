// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgTaskRowPanel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;
using System;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgTaskRowPanel : PanelContainer
{
  private static readonly Color HoverOverlay = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>) "#00FFB3", new Color?());
  private bool _isHoverable;
  private bool _isCompleted;
  private bool _hovered;

  public bool IsHoverable
  {
    get => this._isHoverable;
    set
    {
      this._isHoverable = value;
      ((Control) this).MouseFilter = value ? (Control.MouseFilterMode) 0 : (Control.MouseFilterMode) 1;
    }
  }

  public bool IsCompleted
  {
    get => this._isCompleted;
    set => this._isCompleted = value;
  }

  public PubgTaskRowPanel() => ((Control) this).MouseFilter = (Control.MouseFilterMode) 1;

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    base.Draw(handle);
    if (this._hovered && this._isHoverable)
    {
      UIBox2 uiBox2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2).\u002Ector(0.0f, 0.0f, (float) ((Control) this).PixelSize.X, (float) ((Control) this).PixelSize.Y);
      handle.DrawRect(uiBox2, ((Color) ref PubgTaskRowPanel.HoverOverlay).WithAlpha(0.4f), true);
    }
    if (!this._isCompleted)
      return;
    UIBox2 uiBox2_1 = new UIBox2(0.0f, 0.0f, (float) ((Control) this).PixelSize.X, (float) ((Control) this).PixelSize.Y);
    UIBox2 uiBox2_2;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_2).\u002Ector(0.0f, 0.0f, 3f, (float) ((Control) this).PixelSize.Y);
    handle.DrawRect(uiBox2_2, ((Color) ref PubgTaskRowPanel.CompletedGlow).WithAlpha(0.6f), true);
  }

  protected virtual void MouseEntered()
  {
    ((Control) this).MouseEntered();
    if (!this._isHoverable)
      return;
    this._hovered = true;
    ((Control) this).UserInterfaceManager.HoverSound();
  }

  protected virtual void MouseExited()
  {
    ((Control) this).MouseExited();
    this._hovered = false;
  }
}
