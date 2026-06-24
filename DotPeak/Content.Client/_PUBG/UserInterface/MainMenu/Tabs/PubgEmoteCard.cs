// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgEmoteCard
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

public sealed class PubgEmoteCard : Control
{
  [Dependency]
  private IGameTiming _timing;
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private static readonly Color IconBgColor = Color.FromHex((ReadOnlySpan<char>) "#0f0a1e", new Color?());
  private string _emoteName = "";
  private string _emoteId = "";
  private Texture? _emoteIcon;
  private bool _isEquipped;
  private bool _hovered;
  private PanelContainer? _iconContainer;
  private TextureRect? _iconView;
  private Label? _nameLabel;
  private Label? _statusLabel;

  public event Action<string>? OnCardClicked;

  public string EmoteName
  {
    get => this._emoteName;
    set
    {
      this._emoteName = value;
      if (this._nameLabel == null)
        return;
      this._nameLabel.Text = value;
    }
  }

  public string EmoteId
  {
    get => this._emoteId;
    set => this._emoteId = value;
  }

  public Texture? EmoteIcon
  {
    get => this._emoteIcon;
    set
    {
      this._emoteIcon = value;
      this.UpdateIcon();
    }
  }

  public bool IsEquipped
  {
    get => this._isEquipped;
    set
    {
      this._isEquipped = value;
      this.UpdateStatus();
    }
  }

  public PubgEmoteCard()
  {
    IoCManager.InjectDependencies<PubgEmoteCard>(this);
    this.MouseFilter = (Control.MouseFilterMode) 0;
    this.MinSize = new Vector2(140f, 180f);
    this.BuildUI();
  }

  private void BuildUI()
  {
    PanelContainer panelContainer = new PanelContainer();
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = ((Color) ref PubgEmoteCard.IconBgColor).WithAlpha(0.6f)
    };
    ((Control) panelContainer).Margin = new Thickness(4f, 4f, 4f, 0.0f);
    ((Control) panelContainer).MinSize = new Vector2(130f, 100f);
    this._iconContainer = panelContainer;
    TextureRect textureRect = new TextureRect();
    ((Control) textureRect).HorizontalExpand = true;
    ((Control) textureRect).VerticalExpand = true;
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    this._iconView = textureRect;
    ((Control) this._iconContainer).AddChild((Control) this._iconView);
    this.AddChild((Control) this._iconContainer);
    Label label1 = new Label();
    label1.Text = this._emoteName;
    label1.FontColorOverride = new Color?(Color.White);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(4f, 110f, 4f, 0.0f);
    this._nameLabel = label1;
    this.AddChild((Control) this._nameLabel);
    Label label2 = new Label();
    label2.Text = "";
    label2.FontColorOverride = new Color?(Color.Gray);
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label2).Margin = new Thickness(4f, 130f, 4f, 0.0f);
    this._statusLabel = label2;
    this.AddChild((Control) this._statusLabel);
  }

  private void UpdateIcon()
  {
    if (this._iconView == null || this._emoteIcon == null)
      return;
    this._iconView.Texture = this._emoteIcon;
  }

  private void UpdateStatus()
  {
    if (this._statusLabel == null)
      return;
    if (this._isEquipped)
    {
      this._statusLabel.Text = "✓ ВЫБРАНО";
      this._statusLabel.FontColorOverride = new Color?(PubgEmoteCard.GreenSuccess);
    }
    else
      this._statusLabel.Text = "";
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    if (this._iconContainer != null)
      ((Control) this._iconContainer).Arrange(new UIBox2(4f, 4f, finalSize.X - 4f, 105f));
    if (this._nameLabel != null)
      ((Control) this._nameLabel).Arrange(new UIBox2(0.0f, 110f, finalSize.X, 130f));
    if (this._statusLabel != null)
      ((Control) this._statusLabel).Arrange(new UIBox2(0.0f, 130f, finalSize.X, 150f));
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    handle.DrawRect(uiBox2_1, ((Color) ref PubgEmoteCard.DarkPanel).WithAlpha(0.8f), true);
    if (this._hovered)
    {
      float num = 2f;
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left - num, uiBox2_1.Top - num, uiBox2_1.Right + num, uiBox2_1.Bottom + num);
      handle.DrawRect(uiBox2_2, ((Color) ref PubgEmoteCard.CardHoverColor).WithAlpha(0.5f), true);
    }
    Color color = this._isEquipped ? PubgEmoteCard.GreenSuccess : PubgEmoteCard.CardBorderColor;
    float num1 = this._isEquipped ? 3f : 2f;
    for (float num2 = 0.0f; (double) num2 < (double) num1; ++num2)
    {
      UIBox2 uiBox2_3;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_3).\u002Ector(uiBox2_1.Left + num2, uiBox2_1.Top + num2, uiBox2_1.Right - num2, uiBox2_1.Bottom - num2);
      handle.DrawRect(uiBox2_3, Color.Transparent, true);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_3.TopLeft, ((UIBox2) ref uiBox2_3).TopRight, color);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_3).TopRight, uiBox2_3.BottomRight, color);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_3.BottomRight, ((UIBox2) ref uiBox2_3).BottomLeft, color);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_3).BottomLeft, uiBox2_3.TopLeft, color);
    }
    if (!this._isEquipped)
      return;
    float num3 = (float) (0.20000000298023224 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2f) * 0.10000000149011612);
    UIBox2 uiBox2_4;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_4).\u002Ector(uiBox2_1.Left - 3f, uiBox2_1.Top - 3f, uiBox2_1.Right + 3f, uiBox2_1.Bottom + 3f);
    handle.DrawRect(uiBox2_4, ((Color) ref PubgEmoteCard.GreenSuccess).WithAlpha(num3), true);
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
    Action<string> onCardClicked = this.OnCardClicked;
    if (onCardClicked != null)
      onCardClicked(this._emoteId);
    ((BoundKeyEventArgs) args).Handle();
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._isEquipped)
      return;
    this.InvalidateMeasure();
  }
}
