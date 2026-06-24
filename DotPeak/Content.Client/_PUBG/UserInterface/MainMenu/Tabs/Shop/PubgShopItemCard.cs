// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopItemCard
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._PUBG.Skin;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopItemCard : PanelContainer
{
  private static readonly Color BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#111217", new Color?());
  private static readonly Color HoverBackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#171A21", new Color?());
  private static readonly Color SelectedBorderColor = Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?());
  private static readonly Color BorderColor = Color.FromHex((ReadOnlySpan<char>) "#353B48", new Color?());
  private readonly StyleBoxFlat _panelStyle;
  private readonly TextureRect _iconView;
  private readonly PanelContainer _ownedTag;
  private readonly Label _ownedTagText;
  private readonly PanelContainer _limitedTag;
  private readonly Label _limitedTagText;
  private readonly Label _nameLabel;
  private readonly Label _priceLabel;
  private readonly Label _stateLabel;
  private readonly Label _badgeLabel;
  private bool _hovered;

  public string ItemId { get; private set; }

  public bool IsSelected { get; private set; }

  public event Action<string>? OnCardClicked;

  public event Action<string>? OnCardRightClicked;

  public PubgShopItemCard()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BorderThickness = new Thickness(2f);
    styleBoxFlat.BackgroundColor = PubgShopItemCard.BackgroundColor;
    styleBoxFlat.BorderColor = PubgShopItemCard.BorderColor;
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(8f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(8f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(8f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(8f);
    this._panelStyle = styleBoxFlat;
    // ISSUE: reference to a compiler-generated field
    this.\u003CItemId\u003Ek__BackingField = string.Empty;
    // ISSUE: explicit constructor call
    base.\u002Ector();
    this.PanelOverride = (StyleBox) this._panelStyle;
    ((Control) this).MouseFilter = (Control.MouseFilterMode) 0;
    ((Control) this).MinSize = new Vector2(151f, 224f);
    ((Control) this).MaxSize = new Vector2(151f, 224f);
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).HorizontalExpand = true;
    BoxContainer boxContainer4 = boxContainer3;
    this._ownedTag = PubgShopItemCard.CreateTag(Color.FromHex((ReadOnlySpan<char>) "#103820", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#2D7A43", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#8DFFB0", new Color?()), out this._ownedTagText);
    ((Control) this._ownedTag).Margin = new Thickness(0.0f, 0.0f, 4f, 0.0f);
    this._limitedTag = PubgShopItemCard.CreateTag(Color.FromHex((ReadOnlySpan<char>) "#3A2507", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#93641E", new Color?()), Color.FromHex((ReadOnlySpan<char>) "#FFD36A", new Color?()), out this._limitedTagText);
    ((Control) boxContainer4).AddChild((Control) this._ownedTag);
    ((Control) boxContainer4).AddChild((Control) this._limitedTag);
    PanelContainer panelContainer1 = new PanelContainer();
    ((Control) panelContainer1).MinSize = new Vector2(135f, 100f);
    ((Control) panelContainer1).HorizontalExpand = true;
    ((Control) panelContainer1).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#0A0D13", new Color?())
    };
    PanelContainer panelContainer2 = panelContainer1;
    TextureRect textureRect = new TextureRect();
    textureRect.Stretch = (TextureRect.StretchMode) 7;
    ((Control) textureRect).HorizontalExpand = true;
    ((Control) textureRect).VerticalExpand = true;
    this._iconView = textureRect;
    ((Control) panelContainer2).AddChild((Control) this._iconView);
    Label label1 = new Label();
    label1.FontColorOverride = new Color?(Color.White);
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).MaxWidth = 135f;
    label1.ClipText = true;
    ((Control) label1).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    this._nameLabel = label1;
    Label label2 = new Label();
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
    ((Control) label2).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label2).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
    this._priceLabel = label2;
    Label label3 = new Label();
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#9AA7B9", new Color?()));
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label3).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
    ((Control) label3).MaxWidth = 135f;
    label3.ClipText = true;
    this._stateLabel = label3;
    Label label4 = new Label();
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FBC02D", new Color?()));
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label4).Margin = new Thickness(0.0f, 2f, 0.0f, 0.0f);
    ((Control) label4).MaxWidth = 135f;
    label4.ClipText = true;
    this._badgeLabel = label4;
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) panelContainer2);
    ((Control) boxContainer2).AddChild((Control) this._nameLabel);
    ((Control) boxContainer2).AddChild((Control) this._priceLabel);
    ((Control) boxContainer2).AddChild((Control) this._stateLabel);
    ((Control) boxContainer2).AddChild((Control) this._badgeLabel);
    ((Control) this).AddChild((Control) boxContainer2);
    this.RefreshStyle();
  }

  public void SetData(PubgShopPresentedItem item, Texture? iconTexture, bool isSelected)
  {
    this.ItemId = item.ItemId;
    this.IsSelected = isSelected;
    this._ownedTagText.Text = Loc.GetString("mainmenu-shop-owned");
    ((Control) this._ownedTag).Visible = item.IsOwnedPermanent;
    int? remainingCount = PubgShopItemCard.GetRemainingCount(item);
    if (remainingCount.HasValue)
    {
      this._limitedTagText.Text = Loc.GetString("mainmenu-shop-card-remaining-short", new (string, object)[1]
      {
        ("remaining", (object) remainingCount.Value)
      });
      ((Control) this._limitedTag).Visible = true;
    }
    else
    {
      this._limitedTagText.Text = Loc.GetString("mainmenu-shop-card-limited");
      ((Control) this._limitedTag).Visible = item.IsLimited;
    }
    this._iconView.Texture = iconTexture;
    this._nameLabel.Text = item.Prototype.Name;
    this._priceLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFD05C", new Color?()));
    if (item.IsOwnedPermanent)
    {
      this._priceLabel.Text = Loc.GetString("mainmenu-shop-owned");
      this._priceLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#66E08A", new Color?()));
    }
    else
      this._priceLabel.Text = this.BuildPriceText(item.LowestOffer);
    if (item.IsTimedOwned)
    {
      DateTime? expiresAt = item.ExpiresAt;
      if (expiresAt.HasValue)
      {
        expiresAt = item.ExpiresAt;
        DateTime localTime = expiresAt.Value;
        localTime = localTime.ToLocalTime();
        this._stateLabel.Text = Loc.GetString("mainmenu-skin-tooltip-expires-at", new (string, object)[1]
        {
          ("time", (object) localTime.ToString("g"))
        });
        this._stateLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#66D9FF", new Color?()));
        goto label_12;
      }
    }
    if (item.IsTimedOwned)
    {
      this._stateLabel.Text = Loc.GetString("mainmenu-shop-timed-active");
      this._stateLabel.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#66D9FF", new Color?()));
    }
    else
      this._stateLabel.Text = string.Empty;
label_12:
    if (remainingCount.HasValue)
      this._badgeLabel.Text = Loc.GetString("mainmenu-shop-collectible-remaining", new (string, object)[1]
      {
        ("remaining", (object) remainingCount.Value)
      });
    else
      this._badgeLabel.Text = string.Empty;
    this.RefreshStyle();
  }

  public void SetSelected(bool isSelected)
  {
    this.IsSelected = isSelected;
    this.RefreshStyle();
  }

  private string BuildPriceText(SkinShopOfferInfo offer)
  {
    string str = offer.Currency == "coins" ? Loc.GetString("mainmenu-shop-currency-coins") : Loc.GetString("mainmenu-shop-currency-premium");
    return Loc.GetString("mainmenu-shop-card-price", new (string, object)[2]
    {
      ("price", (object) offer.Price),
      ("currency", (object) str)
    });
  }

  private void RefreshStyle()
  {
    this._panelStyle.BorderColor = this.IsSelected ? PubgShopItemCard.SelectedBorderColor : PubgShopItemCard.BorderColor;
    this._panelStyle.BackgroundColor = this._hovered ? PubgShopItemCard.HoverBackgroundColor : PubgShopItemCard.BackgroundColor;
  }

  private static PanelContainer CreateTag(
    Color backgroundColor,
    Color borderColor,
    Color textColor,
    out Label textLabel)
  {
    textLabel = new Label()
    {
      FontColorOverride = new Color?(textColor)
    };
    PanelContainer tag = new PanelContainer();
    ((Control) tag).Visible = false;
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = backgroundColor;
    styleBoxFlat.BorderColor = borderColor;
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(5f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(1f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(5f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(1f);
    tag.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) tag).AddChild((Control) textLabel);
    return tag;
  }

  private static int? GetRemainingCount(PubgShopPresentedItem item)
  {
    if (item.Remaining > 0)
      return new int?(item.Remaining);
    if (item.CollectibleLimit > 0)
      return new int?(Math.Max(item.CollectibleLimit - item.SoldCount, 0));
    return item.IsCollectible ? new int?(0) : new int?();
  }

  protected virtual void MouseEntered()
  {
    ((Control) this).MouseEntered();
    this._hovered = true;
    ((Control) this).UserInterfaceManager.HoverSound();
    this.RefreshStyle();
  }

  protected virtual void MouseExited()
  {
    ((Control) this).MouseExited();
    this._hovered = false;
    this.RefreshStyle();
  }

  protected virtual void KeyBindDown(GUIBoundKeyEventArgs args)
  {
    ((Control) this).KeyBindDown(args);
    if (BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIClick))
    {
      ((Control) this).UserInterfaceManager.ClickSound();
      Action<string> onCardClicked = this.OnCardClicked;
      if (onCardClicked != null)
        onCardClicked(this.ItemId);
      ((BoundKeyEventArgs) args).Handle();
    }
    else
    {
      if (!BoundKeyFunction.op_Equality(((BoundKeyEventArgs) args).Function, EngineKeyFunctions.UIRightClick))
        return;
      Action<string> cardRightClicked = this.OnCardRightClicked;
      if (cardRightClicked != null)
        cardRightClicked(this.ItemId);
      ((BoundKeyEventArgs) args).Handle();
    }
  }
}
