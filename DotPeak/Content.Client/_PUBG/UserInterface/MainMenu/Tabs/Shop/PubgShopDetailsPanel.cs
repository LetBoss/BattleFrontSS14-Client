// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopDetailsPanel
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopDetailsPanel : PanelContainer
{
  private readonly Label _titleLabel;
  private readonly Label _rarityLabel;
  private readonly Label _descriptionLabel;
  private readonly Label _statusLabel;
  private readonly BoxContainer _offersList;
  private readonly Label _offersTitleLabel;
  private readonly Label _warningLabel;
  private readonly Button _buyButton;
  private PubgShopPresentedItem? _selectedItem;
  private string? _selectedOfferId;
  private int _playerCoins;
  private int _playerPremiumCoins;

  public event Action<string, string>? OnBuyRequested;

  public PubgShopDetailsPanel()
  {
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    styleBoxFlat.BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#10141D", new Color?());
    styleBoxFlat.BorderColor = Color.FromHex((ReadOnlySpan<char>) "#2C3445", new Color?());
    styleBoxFlat.BorderThickness = new Thickness(1f);
    ((StyleBox) styleBoxFlat).ContentMarginLeftOverride = new float?(14f);
    ((StyleBox) styleBoxFlat).ContentMarginTopOverride = new float?(14f);
    ((StyleBox) styleBoxFlat).ContentMarginRightOverride = new float?(14f);
    ((StyleBox) styleBoxFlat).ContentMarginBottomOverride = new float?(14f);
    this.PanelOverride = (StyleBox) styleBoxFlat;
    ((Control) this).MinWidth = 340f;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-shop-details-select-item");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 1;
    this._titleLabel = label1;
    ((Control) this._titleLabel).SetOnlyStyleClass("LabelHeading");
    Label label2 = new Label();
    label2.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FFB800", new Color?()));
    ((Control) label2).Margin = new Thickness(0.0f, 4f, 0.0f, 0.0f);
    this._rarityLabel = label2;
    Label label3 = new Label();
    label3.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#CBD5E1", new Color?()));
    ((Control) label3).Margin = new Thickness(0.0f, 8f, 0.0f, 0.0f);
    ((Control) label3).VerticalExpand = false;
    this._descriptionLabel = label3;
    Label label4 = new Label();
    label4.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#9FC2FF", new Color?()));
    ((Control) label4).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    this._statusLabel = label4;
    Label label5 = new Label();
    label5.Text = Loc.GetString("mainmenu-shop-details-offers");
    ((Control) label5).Margin = new Thickness(0.0f, 12f, 0.0f, 6f);
    this._offersTitleLabel = label5;
    ((Control) this._offersTitleLabel).SetOnlyStyleClass("LabelHeading");
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    scrollContainer1.HScrollEnabled = false;
    ((Control) scrollContainer1).MinHeight = 120f;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalExpand = true;
    this._offersList = boxContainer3;
    ((Control) scrollContainer2).AddChild((Control) this._offersList);
    Label label6 = new Label();
    label6.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#FF6B6B", new Color?()));
    ((Control) label6).Margin = new Thickness(0.0f, 8f, 0.0f, 0.0f);
    this._warningLabel = label6;
    Button button = new Button();
    button.Text = Loc.GetString("mainmenu-shop-details-buy");
    ((BaseButton) button).Disabled = true;
    ((Control) button).MinHeight = 42f;
    ((Control) button).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    this._buyButton = button;
    ((Control) this._buyButton).StyleClasses.Add("ButtonBig");
    ((BaseButton) this._buyButton).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.TryBuySelected());
    ((Control) boxContainer2).AddChild((Control) this._titleLabel);
    ((Control) boxContainer2).AddChild((Control) this._rarityLabel);
    ((Control) boxContainer2).AddChild((Control) this._descriptionLabel);
    ((Control) boxContainer2).AddChild((Control) this._statusLabel);
    ((Control) boxContainer2).AddChild((Control) this._offersTitleLabel);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    ((Control) boxContainer2).AddChild((Control) this._warningLabel);
    ((Control) boxContainer2).AddChild((Control) this._buyButton);
    ((Control) this).AddChild((Control) boxContainer2);
  }

  public void UpdateSelection(PubgShopPresentedItem? item, int playerCoins, int playerPremiumCoins)
  {
    this._selectedItem = item;
    this._playerCoins = playerCoins;
    this._playerPremiumCoins = playerPremiumCoins;
    if (this._selectedItem == null)
    {
      this._selectedOfferId = (string) null;
      this._titleLabel.Text = Loc.GetString("mainmenu-shop-details-select-item");
      this._rarityLabel.Text = string.Empty;
      this._descriptionLabel.Text = Loc.GetString("mainmenu-shop-details-select-item-hint");
      this._statusLabel.Text = string.Empty;
      this._warningLabel.Text = string.Empty;
      ((Control) this._offersList).RemoveAllChildren();
      ((Control) this._offersList).AddChild((Control) new Label()
      {
        Text = Loc.GetString("mainmenu-shop-details-no-offers"),
        FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#7D8A9F", new Color?()))
      });
      this._buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
      ((BaseButton) this._buyButton).Disabled = true;
    }
    else
    {
      this._titleLabel.Text = this._selectedItem.Prototype.Name;
      this._rarityLabel.Text = Loc.GetString("mainmenu-shop-details-rarity", new (string, object)[1]
      {
        ("rarity", (object) this.GetRarityText(this._selectedItem.SkinComponent.Rarity))
      });
      this._rarityLabel.FontColorOverride = new Color?(this.GetRarityColor(this._selectedItem.SkinComponent.Rarity));
      this._descriptionLabel.Text = string.IsNullOrWhiteSpace(this._selectedItem.SkinComponent.Description) ? Loc.GetString("mainmenu-skin-tooltip-no-description") : this._selectedItem.SkinComponent.Description;
      this._statusLabel.Text = this.BuildStatusText(this._selectedItem);
      this.RebuildOffers();
      this.UpdateBuyState();
    }
  }

  public void UpdateBalance(int playerCoins, int playerPremiumCoins)
  {
    this._playerCoins = playerCoins;
    this._playerPremiumCoins = playerPremiumCoins;
    this.UpdateBuyState();
  }

  private void RebuildOffers()
  {
    ((Control) this._offersList).RemoveAllChildren();
    if (this._selectedItem == null)
    {
      this._selectedOfferId = (string) null;
    }
    else
    {
      List<SkinShopOfferInfo> list = this._selectedItem.Offers.ToList<SkinShopOfferInfo>();
      if (list.Count == 0)
      {
        this._selectedOfferId = (string) null;
        ((Control) this._offersList).AddChild((Control) new Label()
        {
          Text = Loc.GetString("mainmenu-shop-details-no-offers"),
          FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#7D8A9F", new Color?()))
        });
      }
      else
      {
        if (this._selectedOfferId == null || list.All<SkinShopOfferInfo>((Func<SkinShopOfferInfo, bool>) (o => o.OfferId != this._selectedOfferId)))
          this._selectedOfferId = list[0].OfferId;
        ButtonGroup buttonGroup = new ButtonGroup(true);
        foreach (SkinShopOfferInfo skinShopOfferInfo in list)
        {
          SkinShopOfferInfo offer = skinShopOfferInfo;
          Button button1 = new Button();
          button1.Text = SkinContextMenuBuilder.GetShopOfferText(offer);
          ((Control) button1).HorizontalExpand = true;
          ((BaseButton) button1).ToggleMode = true;
          ((BaseButton) button1).Group = buttonGroup;
          ((BaseButton) button1).Pressed = offer.OfferId == this._selectedOfferId;
          ((Control) button1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 4f);
          Button button2 = button1;
          ((Control) button2).StyleClasses.Add("OpenBoth");
          ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
          {
            this._selectedOfferId = offer.OfferId;
            this.UpdateBuyState();
          });
          ((Control) this._offersList).AddChild((Control) button2);
        }
      }
    }
  }

  private void UpdateBuyState()
  {
    if (this._selectedItem == null)
    {
      this._buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
      ((BaseButton) this._buyButton).Disabled = true;
      this._warningLabel.Text = string.Empty;
    }
    else if (this._selectedItem.IsOwnedPermanent)
    {
      this._buyButton.Text = Loc.GetString("mainmenu-shop-owned");
      ((BaseButton) this._buyButton).Disabled = true;
      this._warningLabel.Text = Loc.GetString("mainmenu-shop-already-owned");
    }
    else
    {
      SkinShopOfferInfo offer = this._selectedItem.Offers.FirstOrDefault<SkinShopOfferInfo>((Func<SkinShopOfferInfo, bool>) (o => o.OfferId == this._selectedOfferId));
      if (offer == null)
      {
        this._buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
        ((BaseButton) this._buyButton).Disabled = true;
        this._warningLabel.Text = string.Empty;
      }
      else
      {
        this._buyButton.Text = SkinContextMenuBuilder.GetShopOfferText(offer);
        bool flag = SkinContextMenuBuilder.HasEnoughCurrency(offer, this._playerCoins, this._playerPremiumCoins);
        ((BaseButton) this._buyButton).Disabled = !flag;
        if (flag)
          this._warningLabel.Text = string.Empty;
        else
          this._warningLabel.Text = offer.Currency == "coins" ? Loc.GetString("mainmenu-shop-not-enough-coins") : Loc.GetString("mainmenu-shop-not-enough-diamonds");
      }
    }
  }

  private void TryBuySelected()
  {
    if (this._selectedItem == null || string.IsNullOrEmpty(this._selectedOfferId))
      return;
    SkinShopOfferInfo offer = this._selectedItem.Offers.FirstOrDefault<SkinShopOfferInfo>((Func<SkinShopOfferInfo, bool>) (o => o.OfferId == this._selectedOfferId));
    if (offer == null || !SkinContextMenuBuilder.HasEnoughCurrency(offer, this._playerCoins, this._playerPremiumCoins))
      return;
    Action<string, string> onBuyRequested = this.OnBuyRequested;
    if (onBuyRequested == null)
      return;
    onBuyRequested(this._selectedItem.ItemId, offer.OfferId);
  }

  private string BuildStatusText(PubgShopPresentedItem item)
  {
    if (item.IsOwnedPermanent)
      return Loc.GetString("mainmenu-shop-owned");
    if (item.IsTimedOwned && item.ExpiresAt.HasValue)
    {
      DateTime localTime = item.ExpiresAt.Value;
      localTime = localTime.ToLocalTime();
      return Loc.GetString("mainmenu-skin-tooltip-expires-at", new (string, object)[1]
      {
        ("time", (object) localTime.ToString("g"))
      });
    }
    int? remainingCount = PubgShopDetailsPanel.GetRemainingCount(item);
    return remainingCount.HasValue ? Loc.GetString("mainmenu-shop-collectible-remaining", new (string, object)[1]
    {
      ("remaining", (object) remainingCount.Value)
    }) : (item.IsLimited ? Loc.GetString("mainmenu-shop-details-status-limited") : string.Empty);
  }

  private static int? GetRemainingCount(PubgShopPresentedItem item)
  {
    if (item.Remaining > 0)
      return new int?(item.Remaining);
    if (item.CollectibleLimit > 0)
      return new int?(Math.Max(item.CollectibleLimit - item.SoldCount, 0));
    return item.IsCollectible ? new int?(0) : new int?();
  }

  private string GetRarityText(SkinRarity rarity)
  {
    string rarityText;
    switch (rarity)
    {
      case SkinRarity.Common:
        rarityText = Loc.GetString("mainmenu-skin-rarity-common");
        break;
      case SkinRarity.Rare:
        rarityText = Loc.GetString("mainmenu-skin-rarity-rare");
        break;
      case SkinRarity.Epic:
        rarityText = Loc.GetString("mainmenu-skin-rarity-epic");
        break;
      case SkinRarity.Legendary:
        rarityText = Loc.GetString("mainmenu-skin-rarity-legendary");
        break;
      case SkinRarity.Unique:
        rarityText = Loc.GetString("mainmenu-skin-rarity-unique");
        break;
      default:
        rarityText = rarity.ToString();
        break;
    }
    return rarityText;
  }

  private Color GetRarityColor(SkinRarity rarity)
  {
    Color rarityColor;
    switch (rarity)
    {
      case SkinRarity.Rare:
        rarityColor = Color.FromHex((ReadOnlySpan<char>) "#9C27B0", new Color?());
        break;
      case SkinRarity.Epic:
        rarityColor = Color.FromHex((ReadOnlySpan<char>) "#FF9800", new Color?());
        break;
      case SkinRarity.Legendary:
        rarityColor = Color.FromHex((ReadOnlySpan<char>) "#FFD700", new Color?());
        break;
      case SkinRarity.Unique:
        rarityColor = Color.FromHex((ReadOnlySpan<char>) "#00E5FF", new Color?());
        break;
      default:
        rarityColor = Color.FromHex((ReadOnlySpan<char>) "#9E9E9E", new Color?());
        break;
    }
    return rarityColor;
  }
}
