// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgStatCard
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

public sealed class PubgStatCard : Control
{
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color OrangeWarning = Color.FromHex((ReadOnlySpan<char>) "#FF9500", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color ProgressBg = Color.FromHex((ReadOnlySpan<char>) "#0d0d15", new Color?());
  private static readonly Color CompletedGlow = Color.FromHex((ReadOnlySpan<char>) "#00FFB3", new Color?());
  [Dependency]
  private IGameTiming _timing;
  private string _title = "";
  private string _value = "";
  private float _progress;
  private bool _showProgress;
  private bool _isPulse;
  private Color _valueColor = Color.White;
  private Label? _titleLabel;
  private Label? _valueLabel;

  public string Title
  {
    get => this._title;
    set
    {
      this._title = value;
      if (this._titleLabel == null)
        return;
      this._titleLabel.Text = value;
    }
  }

  public string Value
  {
    get => this._value;
    set
    {
      this._value = value;
      if (this._valueLabel == null)
        return;
      this._valueLabel.Text = value;
    }
  }

  public float Progress
  {
    get => this._progress;
    set => this._progress = Math.Clamp(value, 0.0f, 1f);
  }

  public bool ShowProgress
  {
    get => this._showProgress;
    set => this._showProgress = value;
  }

  public bool IsPulse
  {
    get => this._isPulse;
    set => this._isPulse = value;
  }

  public Color ValueColor
  {
    get => this._valueColor;
    set
    {
      this._valueColor = value;
      if (this._valueLabel == null)
        return;
      this._valueLabel.FontColorOverride = new Color?(value);
    }
  }

  public PubgStatCard()
  {
    IoCManager.InjectDependencies<PubgStatCard>(this);
    this.MinHeight = 70f;
    this.HorizontalExpand = true;
    this.BuildUI();
  }

  private void BuildUI()
  {
    Label label1 = new Label();
    label1.Text = this._title;
    label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#c0c0c0", new Color?()));
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label1).Margin = new Thickness(12f, 8f, 12f, 0.0f);
    this._titleLabel = label1;
    this.AddChild((Control) this._titleLabel);
    Label label2 = new Label();
    label2.Text = this._value;
    label2.FontColorOverride = new Color?(this._valueColor);
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 1;
    ((Control) label2).Margin = new Thickness(12f, 28f, 12f, 0.0f);
    this._valueLabel = label2;
    ((Control) this._valueLabel).SetOnlyStyleClass("LabelHeadingBigger");
    this.AddChild((Control) this._valueLabel);
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    if (this._titleLabel != null)
      ((Control) this._titleLabel).Arrange(new UIBox2(0.0f, 0.0f, finalSize.X, 20f));
    if (this._valueLabel != null)
      ((Control) this._valueLabel).Arrange(new UIBox2(0.0f, 0.0f, finalSize.X, finalSize.Y - 10f));
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    handle.DrawRect(uiBox2_1, ((Color) ref PubgStatCard.DarkPanel).WithAlpha(0.6f), true);
    ((DrawingHandleBase) handle).DrawLine(uiBox2_1.TopLeft, ((UIBox2) ref uiBox2_1).TopRight, PubgStatCard.CardBorderColor);
    ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_1).TopRight, uiBox2_1.BottomRight, PubgStatCard.CardBorderColor);
    ((DrawingHandleBase) handle).DrawLine(uiBox2_1.BottomRight, ((UIBox2) ref uiBox2_1).BottomLeft, PubgStatCard.CardBorderColor);
    ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_1).BottomLeft, uiBox2_1.TopLeft, PubgStatCard.CardBorderColor);
    if (this._showProgress)
    {
      int num1 = this.PixelSize.Y - 6;
      float num2 = 4f;
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(2f, (float) num1, (float) (this.PixelSize.X - 2), (float) num1 + num2);
      handle.DrawRect(uiBox2_2, PubgStatCard.ProgressBg, true);
      if ((double) this._progress > 0.0)
      {
        float num3 = (float) (this.PixelSize.X - 4) * this._progress;
        UIBox2 uiBox2_3;
        // ISSUE: explicit constructor call
        ((UIBox2) ref uiBox2_3).\u002Ector(2f, (float) num1, 2f + num3, (float) num1 + num2);
        Color color = (double) this._progress >= 0.89999997615814209 ? PubgStatCard.CompletedGlow : ((double) this._progress >= 0.60000002384185791 ? PubgStatCard.LerpColor(PubgStatCard.OrangeWarning, PubgStatCard.GreenSuccess, (float) (((double) this._progress - 0.60000002384185791) / 0.40000000596046448)) : PubgStatCard.OrangeWarning);
        handle.DrawRect(uiBox2_3, color, true);
      }
    }
    if (!this._isPulse)
      return;
    float num = (float) (0.15000000596046448 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2.5f) * 0.10000000149011612);
    UIBox2 uiBox2_4;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_4).\u002Ector(uiBox2_1.Left - 2f, uiBox2_1.Top - 2f, uiBox2_1.Right + 2f, uiBox2_1.Bottom + 2f);
    handle.DrawRect(uiBox2_4, ((Color) ref PubgStatCard.GoldAccent).WithAlpha(num), true);
  }

  private static Color LerpColor(Color a, Color b, float t)
  {
    t = Math.Clamp(t, 0.0f, 1f);
    return new Color(a.R + (b.R - a.R) * t, a.G + (b.G - a.G) * t, a.B + (b.B - a.B) * t, a.A + (b.A - a.A) * t);
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._isPulse)
      return;
    this.InvalidateMeasure();
  }
}
