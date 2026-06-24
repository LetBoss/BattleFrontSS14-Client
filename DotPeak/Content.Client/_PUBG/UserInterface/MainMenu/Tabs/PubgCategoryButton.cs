// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgCategoryButton
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

public sealed class PubgCategoryButton : Control
{
  [Dependency]
  private IGameTiming _timing;
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private string _text = "";
  private string _icon = "";
  private bool _isActive;
  private bool _hovered;
  private Label? _label;
  private Label? _iconLabel;

  public event Action? OnPressed;

  public string Text
  {
    get => this._text;
    set
    {
      this._text = value;
      if (this._label == null)
        return;
      this._label.Text = value;
    }
  }

  public string Icon
  {
    get => this._icon;
    set
    {
      this._icon = value;
      if (this._iconLabel == null)
        return;
      this._iconLabel.Text = value;
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

  public PubgCategoryButton()
  {
    IoCManager.InjectDependencies<PubgCategoryButton>(this);
    this.MouseFilter = (Control.MouseFilterMode) 0;
    this.MinHeight = 50f;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) boxContainer1).Margin = new Thickness(12f, 0.0f, 12f, 0.0f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = this._icon;
    label1.FontColorOverride = new Color?(Color.White);
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 10f, 0.0f);
    ((Control) label1).MinWidth = 30f;
    this._iconLabel = label1;
    ((Control) this._iconLabel).SetOnlyStyleClass("LabelHeading");
    Label label2 = new Label();
    label2.Text = this._text;
    label2.FontColorOverride = new Color?(Color.White);
    ((Control) label2).HorizontalExpand = true;
    this._label = label2;
    ((Control) this._label).SetOnlyStyleClass("LabelHeading");
    ((Control) boxContainer2).AddChild((Control) this._iconLabel);
    ((Control) boxContainer2).AddChild((Control) this._label);
    this.AddChild((Control) boxContainer2);
  }

  protected virtual Vector2 MeasureOverride(Vector2 availableSize)
  {
    return new Vector2(Math.Min(200f, availableSize.X), 50f);
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    Color color1 = !this._isActive ? (!this._hovered ? ((Color) ref PubgCategoryButton.DarkPanel).WithAlpha(0.6f) : PubgCategoryButton.CardHoverColor) : Color.FromHex((ReadOnlySpan<char>) "#1a2a1a", new Color?());
    handle.DrawRect(uiBox2_1, color1, true);
    if (this._isActive)
    {
      float num = (float) (0.15000000596046448 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2f) * 0.079999998211860657);
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left - 2f, uiBox2_1.Top - 2f, uiBox2_1.Right + 2f, uiBox2_1.Bottom + 2f);
      handle.DrawRect(uiBox2_2, ((Color) ref PubgCategoryButton.GreenSuccess).WithAlpha(num), true);
    }
    Color color2 = this._isActive ? PubgCategoryButton.GreenSuccess : (this._hovered ? PubgCategoryButton.GoldAccent : PubgCategoryButton.CardBorderColor);
    UIBox2 uiBox2_3;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_3).\u002Ector(0.0f, uiBox2_1.Top, 4f, uiBox2_1.Bottom);
    handle.DrawRect(uiBox2_3, color2, true);
    Color color3 = this._isActive ? PubgCategoryButton.GreenSuccess : PubgCategoryButton.CardBorderColor;
    ((DrawingHandleBase) handle).DrawLine(uiBox2_1.TopLeft, ((UIBox2) ref uiBox2_1).TopRight, color3);
    ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_1).TopRight, uiBox2_1.BottomRight, color3);
    ((DrawingHandleBase) handle).DrawLine(uiBox2_1.BottomRight, ((UIBox2) ref uiBox2_1).BottomLeft, color3);
    ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_1).BottomLeft, uiBox2_1.TopLeft, color3);
    if (this._label != null)
      this._label.FontColorOverride = new Color?(this._isActive ? PubgCategoryButton.GreenSuccess : (this._hovered ? Color.White : Color.FromHex((ReadOnlySpan<char>) "#c0c0c0", new Color?())));
    if (this._iconLabel == null)
      return;
    this._iconLabel.FontColorOverride = new Color?(this._isActive ? PubgCategoryButton.GreenSuccess : (this._hovered ? PubgCategoryButton.GoldAccent : Color.FromHex((ReadOnlySpan<char>) "#888888", new Color?())));
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
    if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
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
