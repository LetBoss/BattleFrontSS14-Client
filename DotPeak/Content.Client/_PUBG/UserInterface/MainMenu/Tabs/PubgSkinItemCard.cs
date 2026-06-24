// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.PubgSkinItemCard
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Input;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class PubgSkinItemCard : Control
{
  [Dependency]
  private IEntityManager _entMan;
  [Dependency]
  private IPrototypeManager _protoManager;
  [Dependency]
  private IGameTiming _timing;
  private static readonly Color GoldAccent = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color GreenSuccess = Color.FromHex((ReadOnlySpan<char>) "#00FF88", new Color?());
  private static readonly Color DarkPanel = Color.FromHex((ReadOnlySpan<char>) "#0a0a0f", new Color?());
  private static readonly Color CardBorderColor = Color.FromHex((ReadOnlySpan<char>) "#2a2a3a", new Color?());
  private static readonly Color CardHoverColor = Color.FromHex((ReadOnlySpan<char>) "#252530", new Color?());
  private static readonly Color AccentGlow = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
  private static readonly Color SpriteBgColor = Color.FromHex((ReadOnlySpan<char>) "#0f0a1e", new Color?());
  private static readonly Color OwnedColor = Color.FromHex((ReadOnlySpan<char>) "#4CAF50", new Color?());
  private static readonly Color RareColor = Color.FromHex((ReadOnlySpan<char>) "#9C27B0", new Color?());
  private static readonly Color EpicColor = Color.FromHex((ReadOnlySpan<char>) "#FF9800", new Color?());
  private static readonly Color LegendaryColor = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
  private static readonly Color UniqueColor = Color.FromHex((ReadOnlySpan<char>) "#00E5FF", new Color?());
  private string _itemName = "";
  private string _protoId = "";
  private bool _isOwned;
  private bool _isEquipped;
  private bool _isNew;
  private string _rarity = "common";
  private bool _hovered;
  private SpriteView? _spriteView;
  private PanelContainer? _spriteContainer;
  private Label? _nameLabel;
  private Label? _statusLabel;
  private PanelContainer? _rarityBadge;
  private Label? _rarityLabel;
  private StyleBoxFlat? _rarityBadgeStyle;

  public event Action<string>? OnCardClicked;

  public event Action<string>? OnCardRightClicked;

  public string ItemName
  {
    get => this._itemName;
    set
    {
      this._itemName = value;
      if (this._nameLabel == null)
        return;
      this._nameLabel.Text = value;
    }
  }

  public string ProtoId
  {
    get => this._protoId;
    set
    {
      this._protoId = value;
      this.UpdateSprite();
    }
  }

  public bool IsOwned
  {
    get => this._isOwned;
    set
    {
      this._isOwned = value;
      this.UpdateStatus();
    }
  }

  public bool IsEquipped
  {
    get => this._isEquipped;
    set
    {
      this._isEquipped = value;
      this.UpdateStatus();
      this.UpdateRarityBadge();
    }
  }

  public bool IsNew
  {
    get => this._isNew;
    set => this._isNew = value;
  }

  public string Rarity
  {
    get => this._rarity;
    set
    {
      this._rarity = value;
      this.UpdateRarityBadge();
    }
  }

  public PubgSkinItemCard()
  {
    IoCManager.InjectDependencies<PubgSkinItemCard>(this);
    this.MouseFilter = (Control.MouseFilterMode) 0;
    this.MinSize = new Vector2(95f, 102f);
    this.BuildUI();
  }

  private void BuildUI()
  {
    PanelContainer panelContainer1 = new PanelContainer();
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = ((Color) ref PubgSkinItemCard.SpriteBgColor).WithAlpha(0.6f)
    };
    ((Control) panelContainer1).Margin = new Thickness(4f, 4f, 4f, 0.0f);
    this._spriteContainer = panelContainer1;
    SpriteView spriteView = new SpriteView(this._entMan);
    ((Control) spriteView).HorizontalExpand = true;
    ((Control) spriteView).VerticalExpand = true;
    spriteView.Stretch = (SpriteView.StretchMode) 1;
    this._spriteView = spriteView;
    ((Control) this._spriteContainer).AddChild((Control) this._spriteView);
    this.AddChild((Control) this._spriteContainer);
    Label label1 = new Label();
    label1.Text = this._itemName;
    label1.FontColorOverride = new Color?(Color.White);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).MaxWidth = 90f;
    label1.ClipText = true;
    this._nameLabel = label1;
    this.AddChild((Control) this._nameLabel);
    Label label2 = new Label();
    label2.Text = "";
    label2.FontColorOverride = new Color?(Color.Gray);
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    this._statusLabel = label2;
    this.AddChild((Control) this._statusLabel);
    this._rarityBadgeStyle = new StyleBoxFlat()
    {
      BackgroundColor = Color.Transparent
    };
    Label label3 = new Label();
    label3.Text = "";
    label3.FontColorOverride = new Color?(Color.White);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    this._rarityLabel = label3;
    PanelContainer panelContainer2 = new PanelContainer();
    panelContainer2.PanelOverride = (StyleBox) this._rarityBadgeStyle;
    ((Control) panelContainer2).MinSize = new Vector2(48f, 16f);
    ((Control) panelContainer2).MouseFilter = (Control.MouseFilterMode) 2;
    this._rarityBadge = panelContainer2;
    ((Control) this._rarityLabel).MouseFilter = (Control.MouseFilterMode) 2;
    ((Control) this._rarityBadge).AddChild((Control) this._rarityLabel);
    this.AddChild((Control) this._rarityBadge);
    this.UpdateRarityBadge();
  }

  private void UpdateSprite()
  {
    if (this._spriteView == null || string.IsNullOrEmpty(this._protoId))
      return;
    if (!this._protoManager.HasIndex<EntityPrototype>(this._protoId))
      return;
    try
    {
      this._spriteView.SetEntity(new EntityUid?(this._entMan.SpawnEntity(this._protoId, MapCoordinates.Nullspace, (ComponentRegistry) null)));
      this._spriteView.Scale = new Vector2(1.8f, 1.8f);
    }
    catch
    {
    }
  }

  private void UpdateStatus()
  {
    if (this._statusLabel == null)
      return;
    if (this._isEquipped)
    {
      this._statusLabel.Text = "ЭКИПИРОВАН";
      this._statusLabel.FontColorOverride = new Color?(PubgSkinItemCard.GreenSuccess);
    }
    else if (this._isOwned)
    {
      this._statusLabel.Text = "В НАЛИЧИИ";
      this._statusLabel.FontColorOverride = new Color?(PubgSkinItemCard.OwnedColor);
    }
    else
    {
      this._statusLabel.Text = "НЕ КУПЛЕН";
      this._statusLabel.FontColorOverride = new Color?(Color.Gray);
    }
  }

  protected virtual Vector2 ArrangeOverride(Vector2 finalSize)
  {
    if (this._spriteContainer != null)
      ((Control) this._spriteContainer).Arrange(new UIBox2(4f, 4f, finalSize.X - 4f, 75f));
    if (this._nameLabel != null)
      ((Control) this._nameLabel).Arrange(new UIBox2(0.0f, 67f, finalSize.X, 79f));
    if (this._statusLabel != null)
      ((Control) this._statusLabel).Arrange(new UIBox2(0.0f, 79f, finalSize.X, 91f));
    if (this._rarityBadge != null)
      ((Control) this._rarityBadge).Arrange(new UIBox2(6f, 6f, 60f, 22f));
    return finalSize;
  }

  protected virtual void Draw(DrawingHandleScreen handle)
  {
    UIBox2 uiBox2_1;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_1).\u002Ector(0.0f, 0.0f, (float) this.PixelSize.X, (float) this.PixelSize.Y);
    float num1 = this._isOwned ? 0.8f : 0.4f;
    handle.DrawRect(uiBox2_1, ((Color) ref PubgSkinItemCard.DarkPanel).WithAlpha(num1), true);
    if (this._hovered)
    {
      float num2 = 2f;
      UIBox2 uiBox2_2;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_2).\u002Ector(uiBox2_1.Left - num2, uiBox2_1.Top - num2, uiBox2_1.Right + num2, uiBox2_1.Bottom + num2);
      handle.DrawRect(uiBox2_2, ((Color) ref PubgSkinItemCard.CardHoverColor).WithAlpha(0.5f), true);
    }
    Color rarityColor = this.GetRarityColor();
    float num3 = this._isEquipped ? 3f : 2f;
    for (float num4 = 0.0f; (double) num4 < (double) num3; ++num4)
    {
      UIBox2 uiBox2_3;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_3).\u002Ector(uiBox2_1.Left + num4, uiBox2_1.Top + num4, uiBox2_1.Right - num4, uiBox2_1.Bottom - num4);
      handle.DrawRect(uiBox2_3, Color.Transparent, true);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_3.TopLeft, ((UIBox2) ref uiBox2_3).TopRight, rarityColor);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_3).TopRight, uiBox2_3.BottomRight, rarityColor);
      ((DrawingHandleBase) handle).DrawLine(uiBox2_3.BottomRight, ((UIBox2) ref uiBox2_3).BottomLeft, rarityColor);
      ((DrawingHandleBase) handle).DrawLine(((UIBox2) ref uiBox2_3).BottomLeft, uiBox2_3.TopLeft, rarityColor);
    }
    if (this._isNew && this._isOwned)
    {
      float num5 = (float) (0.30000001192092896 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 3f) * 0.20000000298023224);
      UIBox2 uiBox2_4;
      // ISSUE: explicit constructor call
      ((UIBox2) ref uiBox2_4).\u002Ector((float) (this.PixelSize.X - 50), 6f, (float) (this.PixelSize.X - 6), 20f);
      handle.DrawRect(uiBox2_4, ((Color) ref PubgSkinItemCard.GoldAccent).WithAlpha(num5), true);
    }
    if (!this._isEquipped)
      return;
    float num6 = (float) (0.20000000298023224 + (double) MathF.Sin((float) this._timing.RealTime.TotalSeconds * 2f) * 0.10000000149011612);
    UIBox2 uiBox2_5;
    // ISSUE: explicit constructor call
    ((UIBox2) ref uiBox2_5).\u002Ector(uiBox2_1.Left - 3f, uiBox2_1.Top - 3f, uiBox2_1.Right + 3f, uiBox2_1.Bottom + 3f);
    handle.DrawRect(uiBox2_5, ((Color) ref PubgSkinItemCard.GreenSuccess).WithAlpha(num6), true);
  }

  private Color GetRarityColor()
  {
    Color rarityColor;
    switch (this._rarity.ToLowerInvariant())
    {
      case "unique":
        rarityColor = PubgSkinItemCard.UniqueColor;
        break;
      case "legendary":
        rarityColor = PubgSkinItemCard.LegendaryColor;
        break;
      case "epic":
        rarityColor = PubgSkinItemCard.EpicColor;
        break;
      case "rare":
        rarityColor = PubgSkinItemCard.RareColor;
        break;
      default:
        rarityColor = this._isEquipped ? PubgSkinItemCard.GreenSuccess : PubgSkinItemCard.CardBorderColor;
        break;
    }
    return rarityColor;
  }

  private void UpdateRarityBadge()
  {
    if (this._rarityBadge == null || this._rarityLabel == null || this._rarityBadgeStyle == null)
      return;
    string rarityLocKey = PubgSkinItemCard.GetRarityLocKey(this._rarity);
    if (rarityLocKey == null)
    {
      ((Control) this._rarityBadge).Visible = false;
    }
    else
    {
      this._rarityLabel.Text = Loc.GetString(rarityLocKey);
      StyleBoxFlat rarityBadgeStyle = this._rarityBadgeStyle;
      Color rarityColor = this.GetRarityColor();
      Color color = ((Color) ref rarityColor).WithAlpha(0.65f);
      rarityBadgeStyle.BackgroundColor = color;
      ((Control) this._rarityBadge).Visible = true;
    }
  }

  private static string? GetRarityLocKey(string rarity)
  {
    string rarityLocKey;
    switch (rarity.ToLowerInvariant())
    {
      case "common":
        rarityLocKey = "mainmenu-skin-rarity-common";
        break;
      case "rare":
        rarityLocKey = "mainmenu-skin-rarity-rare";
        break;
      case "epic":
        rarityLocKey = "mainmenu-skin-rarity-epic";
        break;
      case "legendary":
        rarityLocKey = "mainmenu-skin-rarity-legendary";
        break;
      case "unique":
        rarityLocKey = "mainmenu-skin-rarity-unique";
        break;
      default:
        rarityLocKey = (string) null;
        break;
    }
    return rarityLocKey;
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
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      this.UserInterfaceManager.ClickSound();
      Action<string> onCardClicked = this.OnCardClicked;
      if (onCardClicked != null)
        onCardClicked(this._protoId);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
        return;
      Action<string> cardRightClicked = this.OnCardRightClicked;
      if (cardRightClicked != null)
        cardRightClicked(this._protoId);
      ((BoundKeyEventArgs) args).Handle();
    }
  }

  protected virtual void FrameUpdate(FrameEventArgs args)
  {
    base.FrameUpdate(args);
    if (!this._isNew && !this._isEquipped)
      return;
    this.InvalidateMeasure();
  }
}
