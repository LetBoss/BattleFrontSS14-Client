// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgEmoteSlot
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgEmoteSlot : Control
{
  [Dependency]
  private IGameTiming _timing;
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color RedLocked = Color.FromHex((ReadOnlySpan<char>) "#AA3333", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color IconBgColor = Color.FromHex((ReadOnlySpan<char>) "#0f0a1e", new Color?());
  private bool _isLocked;
  private bool _isFilled;
  private int _slotNumber;
  private BoxContainer? _contentContainer;

  public bool IsLocked
  {
    get => this._isLocked;
    set
    {
      this._isLocked = value;
      this.InvalidateMeasure();
    }
  }

  public bool IsFilled
  {
    get => this._isFilled;
    set
    {
      this._isFilled = value;
      this.InvalidateMeasure();
    }
  }

  public int SlotNumber
  {
    get => this._slotNumber;
    set => this._slotNumber = value;
  }

  public PubgEmoteSlot()
  {
    IoCManager.InjectDependencies<PubgEmoteSlot>(this);
    this.MinSize = new Vector2(180f, 140f);
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) boxContainer).VerticalAlignment = (Control.VAlignment) 2;
    this._contentContainer = boxContainer;
    this.AddChild((Control) this._contentContainer);
  }

  public void SetContent(Control content)
  {
    ((Control) this._contentContainer)?.RemoveAllChildren();
    ((Control) this._contentContainer)?.AddChild(content);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    if (this._contentContainer != null)
      ((Control) this._contentContainer).Arrange(new UIBox2(0.0f, 0.0f, finalSize.X, finalSize.Y));
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    float num1 = this._isLocked ? 0.3f : (this._isFilled ? 0.7f : 0.5f);
    handle.DrawRect(uiBox2_1, ((Color) ref PubgEmoteSlot.DarkPanel).WithAlpha(num1), true);
    Color color;
    float num2;
    if (this._isLocked)
    {
      color = PubgEmoteSlot.RedLocked;
      num2 = 2f;
    }
    else if (this._isFilled)
    {
      color = PubgEmoteSlot.GreenSuccess;
      num2 = 3f;
    }
    else
    {
      color = PubgEmoteSlot.CardBorderColor;
      num2 = 2f;
    }
    for (float num3 = 0.0f; (double) num3 < (double) num2; ++num3)
    {
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left + num3, uiBox2_1.Top + num3, uiBox2_1.Right - num3, uiBox2_1.Bottom - num3);
      handle.DrawRect(uiBox2_2, Color.Transparent, true);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_2.TopLeft, ((UIBox2) ref uiBox2_2).TopRight, color);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_2).TopRight, uiBox2_2.BottomRight, color);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_2.BottomRight, ((UIBox2) ref uiBox2_2).BottomLeft, color);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_2).BottomLeft, uiBox2_2.TopLeft, color);
    }
    if (!this._isFilled || this._isLocked)
      return;
    float num4 = (float) (0.15000000596046448 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2f) * 0.079999998211860657);
    UIBox2 uiBox2_3;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_3).\u002Ector(uiBox2_1.Left - 2f, uiBox2_1.Top - 2f, uiBox2_1.Right + 2f, uiBox2_1.Bottom + 2f);
    handle.DrawRect(uiBox2_3, ((Color) ref PubgEmoteSlot.GreenSuccess).WithAlpha(num4), true);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._isFilled || this._isLocked)
      return;
    this.InvalidateMeasure();
  }
}
