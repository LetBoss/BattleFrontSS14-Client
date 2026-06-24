// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgSubcategoryTab
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgSubcategoryTab : Control
{
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
  private static readonly Color DisabledColor = Color.FromHex((ReadOnlySpan<char>) "#4a4a5a", new Color?());
  [Dependency]
  private IGameTiming _timing;
  private string _text = "";
  private bool _isActive;
  private bool _hovered;
  private Label? _label;

  public event Action? OnPressed;

  public string Text
  {
    get => this._text;
    set
    {
      this._text = value;
      if (this._label != null)
        this._label.Text = value;
      this.InvalidateMeasure();
    }
  }

  public bool IsActive
  {
    get => this._isActive;
    set
    {
      this._isActive = value;
      this.InvalidateMeasure();
    }
  }

  public PubgSubcategoryTab()
  {
    IoCManager.InjectDependencies<PubgSubcategoryTab>(this);
    this.MouseFilter = (Control.MouseFilterMode) 0;
    this.MinHeight = 45f;
    Label label = new Label();
    label.Text = this._text;
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    label.FontColorOverride = new Color?(Color.White);
    this._label = label;
    ((Control) this._label).SetOnlyStyleClass("LabelHeading");
    this.AddChild((Control) this._label);
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize) => new Vector2(180f, 45f);

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    if (this._label != null)
      ((Control) this._label).Arrange(new UIBox2(0.0f, 0.0f, finalSize.X, finalSize.Y));
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    Color color1 = !this._isActive ? (!this._hovered ? PubgSubcategoryTab.DarkPanel : PubgSubcategoryTab.CardHoverColor) : Color.FromHex((ReadOnlySpan<char>) "#1a2a1a", new Color?());
    handle.DrawRect(uiBox2_1, color1, true);
    if (this._isActive)
    {
      float num = (float) (0.20000000298023224 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2f) * 0.10000000149011612);
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left - 2f, uiBox2_1.Top - 2f, uiBox2_1.Right + 2f, uiBox2_1.Bottom + 2f);
      handle.DrawRect(uiBox2_2, ((Color) ref PubgSubcategoryTab.GreenSuccess).WithAlpha(num), true);
    }
    if (this._hovered && !this._isActive)
    {
      UIBox2 uiBox2_3;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_3).\u002Ector(uiBox2_1.Left - 1f, uiBox2_1.Top - 1f, uiBox2_1.Right + 1f, uiBox2_1.Bottom + 1f);
      handle.DrawRect(uiBox2_3, ((Color) ref PubgSubcategoryTab.CardBorderColor).WithAlpha(0.5f), true);
    }
    Color color2 = this._isActive ? PubgSubcategoryTab.GreenSuccess : PubgSubcategoryTab.CardBorderColor;
    float num1 = this._isActive ? 2f : 1f;
    for (float num2 = 0.0f; (double) num2 < (double) num1; ++num2)
    {
      UIBox2 uiBox2_4;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_4).\u002Ector(uiBox2_1.Left + num2, uiBox2_1.Top + num2, uiBox2_1.Right - num2, uiBox2_1.Bottom - num2);
      handle.DrawRect(uiBox2_4, Color.Transparent, true);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_4.TopLeft, ((UIBox2) ref uiBox2_4).TopRight, color2);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_4).TopRight, uiBox2_4.BottomRight, color2);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_4.BottomRight, ((UIBox2) ref uiBox2_4).BottomLeft, color2);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_4).BottomLeft, uiBox2_4.TopLeft, color2);
    }
    if (this._isActive)
    {
      UIBox2 uiBox2_5;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_5).\u002Ector(uiBox2_1.Left, uiBox2_1.Bottom - 3f, uiBox2_1.Right, uiBox2_1.Bottom);
      handle.DrawRect(uiBox2_5, ((Color) ref PubgSubcategoryTab.GreenSuccess).WithAlpha(0.8f), true);
    }
    if (this._label == null)
      return;
    this._label.FontColorOverride = new Color?(this._isActive ? PubgSubcategoryTab.GreenSuccess : (this._hovered ? Color.White : Color.FromHex((ReadOnlySpan<char>) "#c0c0c0", new Color?())));
  }

  protected virtual void MouseEntered()
  {
    base.MouseEntered();
    this._hovered = true;
    this.UserInterfaceManager.HoverSound();
    this.InvalidateMeasure();
  }

  protected virtual void MouseExited()
  {
    base.MouseExited();
    this._hovered = false;
    this.InvalidateMeasure();
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    base.KeyBindDown(args);
    if (BoundKeyFunction.op_Inequality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
      return;
    this.UserInterfaceManager.ClickSound();
    Action onPressed = this.OnPressed;
    if (onPressed != null)
      onPressed();
    ((BoundKeyEventArgs) args).Handle();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._isActive)
      return;
    this.InvalidateMeasure();
  }
}
