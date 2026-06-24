// Decompiled with JetBrains decompiler
// Type: Content.Client.Popups.PopupUIController
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Gameplay;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Client.Popups;

public sealed class PopupUIController : 
  UIController,
  IOnStateEntered<GameplayState>,
  IOnStateExited<GameplayState>
{
  [UISystemDependency]
  private readonly PopupSystem? _popup;
  private Font _smallFont;
  private Font _mediumFont;
  private Font _largeFont;
  private PopupUIController.PopupRootControl? _popupControl;

  public virtual void Initialize()
  {
    base.Initialize();
    IResourceCache iresourceCache = IoCManager.Resolve<IResourceCache>();
    this._smallFont = (Font) new VectorFont(iresourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 10);
    this._mediumFont = (Font) new VectorFont(iresourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Italic.ttf", true), 12);
    this._largeFont = (Font) new VectorFont(iresourceCache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-BoldItalic.ttf", true), 14);
  }

  public void OnStateEntered(GameplayState state)
  {
    this._popupControl = new PopupUIController.PopupRootControl(this._popup, this);
    ((Control) this.UIManager.RootControl).AddChild((Control) this._popupControl);
  }

  public void OnStateExited(GameplayState state)
  {
    if (this._popupControl == null)
      return;
    ((Control) this.UIManager.RootControl).RemoveChild((Control) this._popupControl);
    this._popupControl = (PopupUIController.PopupRootControl) null;
  }

  public void DrawPopup(
    PopupSystem.PopupLabel popup,
    DrawingHandleScreen handle,
    Vector2 position,
    float scale)
  {
    float popupLifetime = PopupSystem.GetPopupLifetime(popup);
    float num = MathF.Min(1f, (float) (1.0 - (double) MathF.Max(0.0f, popup.TotalTime - popupLifetime / 2f) * 2.0 / (double) popupLifetime));
    Vector2 vector2 = position - new Vector2(0.0f, MathF.Min(8f, (float) (12.0 * ((double) popup.TotalTime * (double) popup.TotalTime + (double) popup.TotalTime))));
    Font font = this._smallFont;
    Color white = Color.White;
    Color color = ((Color) ref white).WithAlpha(num);
    switch (popup.Type)
    {
      case PopupType.SmallCaution:
        color = Color.Red;
        break;
      case PopupType.Medium:
        font = this._mediumFont;
        color = Color.LightGray;
        break;
      case PopupType.MediumCaution:
        font = this._mediumFont;
        color = Color.Red;
        break;
      case PopupType.Large:
        font = this._largeFont;
        color = Color.LightGray;
        break;
      case PopupType.LargeCaution:
        font = this._largeFont;
        color = Color.Red;
        break;
      case PopupType.MediumXeno:
        font = this._largeFont;
        color = Color.FromHex((ReadOnlySpan<char>) "#C400FF", new Color?());
        break;
    }
    Vector2 dimensions = handle.GetDimensions(font, (ReadOnlySpan<char>) popup.Text, scale);
    handle.DrawString(font, vector2 - dimensions / 2f, (ReadOnlySpan<char>) popup.Text, scale, ((Color) ref color).WithAlpha(num));
  }

  private sealed class PopupRootControl : Control
  {
    private readonly PopupSystem? _popup;
    private readonly PopupUIController _controller;

    public PopupRootControl(PopupSystem? system, PopupUIController controller)
    {
      this._popup = system;
      this._controller = controller;
    }

    protected virtual void Draw(DrawingHandleScreen handle)
    {
      base.Draw(handle);
      if (this._popup == null)
        return;
      WindowId id = ((UIRoot) this.UserInterfaceManager.RootControl).Window.Id;
      foreach (PopupSystem.CursorPopupLabel cursorLabel in (IEnumerable<PopupSystem.CursorPopupLabel>) this._popup.CursorLabels)
      {
        if (!WindowId.op_Inequality(cursorLabel.InitialPos.Window, id))
          this._controller.DrawPopup((PopupSystem.PopupLabel) cursorLabel, handle, cursorLabel.InitialPos.Position, this.UIScale);
      }
    }
  }
}
