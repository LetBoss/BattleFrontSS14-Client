// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop.PubgShopView
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._PUBG.UserInterface.MainMenu.Controls;
using Content.Shared._PUBG.Shop;
using Content.Shared._PUBG.Skin;
using Content.Shared.Actions.Components;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopView : BoxContainer
{
  [Dependency]
  private IEntityManager _entityManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  private readonly CharacterPreviewPanel _previewPanel;
  private readonly PubgShopPresenter _presenter;
  private readonly PubgShopHeroCaseCard _heroCaseCard;
  private readonly PubgShopDetailsPanel _detailsPanel;
  private readonly ScrollContainer _cardsScroll;
  private readonly BoxContainer _cardsContainer;
  private readonly Dictionary<PubgShopFilter, PubgSubcategoryTab> _filterButtons = new Dictionary<PubgShopFilter, PubgSubcategoryTab>();
  private readonly Dictionary<string, PubgShopItemCard> _cardsById = new Dictionary<string, PubgShopItemCard>();
  private List<SkinShopItemInfo> _shopItems = new List<SkinShopItemInfo>();
  private Dictionary<string, DateTime?> _itemExpiresAt = new Dictionary<string, DateTime?>();
  private List<PubgShopPresentedItem> _presentedItems = new List<PubgShopPresentedItem>();
  private Dictionary<string, SkinShopItemInfo> _shopItemsById = new Dictionary<string, SkinShopItemInfo>();
  private PubgShopFeaturePrototype? _heroCaseFeature;
  private bool _heroCasePending;
  private string? _heroCaseErrorCode;
  private PubgShopFilter _currentFilter;
  private string? _selectedItemId;
  private int _playerCoins;
  private int _playerPremiumCoins;

  public event Action<string>? OnItemSelected;

  public event Action<string, SkinShopItemInfo, Control>? OnItemRightClicked;

  public event Action<string, string>? OnBuyRequested;

  public event Action<string>? OnHeroCasePressed;

  public PubgShopView(CharacterPreviewPanel previewPanel)
  {
    IoCManager.InjectDependencies<PubgShopView>(this);
    this._previewPanel = previewPanel;
    this._presenter = new PubgShopPresenter(this._prototypeManager);
    this.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(15f, 15f, 10f, 15f);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer filtersRow = this.CreateFiltersRow();
    ((Control) boxContainer2).AddChild((Control) filtersRow);
    BoxContainer boxContainer3 = boxContainer2;
    Label label = new Label();
    label.Text = Loc.GetString("mainmenu-shop-cases-hint");
    label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#9AA8C0", new Color?()));
    ((Control) label).Margin = new Thickness(0.0f, 6f, 0.0f, 0.0f);
    ((Control) label).HorizontalExpand = true;
    ((Control) boxContainer3).AddChild((Control) label);
    ScrollContainer scrollContainer = new ScrollContainer();
    ((Control) scrollContainer).VerticalExpand = true;
    scrollContainer.HScrollEnabled = false;
    ((Control) scrollContainer).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
    this._cardsScroll = scrollContainer;
    ((Control) this._cardsScroll).OnResized += new Action(this.RebuildCards);
    BoxContainer boxContainer4 = new BoxContainer();
    boxContainer4.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer4).HorizontalExpand = true;
    ((Control) boxContainer4).VerticalExpand = true;
    this._cardsContainer = boxContainer4;
    ((Control) this._cardsScroll).AddChild((Control) this._cardsContainer);
    ((Control) boxContainer2).AddChild((Control) this._cardsScroll);
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer5).MinWidth = 380f;
    ((Control) boxContainer5).MaxWidth = 380f;
    ((Control) boxContainer5).Margin = new Thickness(0.0f, 15f, 15f, 15f);
    ((Control) boxContainer5).VerticalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    PubgShopHeroCaseCard shopHeroCaseCard = new PubgShopHeroCaseCard();
    ((Control) shopHeroCaseCard).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    this._heroCaseCard = shopHeroCaseCard;
    this._heroCaseFeature = this.GetCaseFeature();
    this._heroCaseCard.OnPressed += new Action(this.OnHeroCaseCardPressed);
    this._heroCaseCard.UpdateFeature(this._heroCaseFeature);
    PubgShopDetailsPanel shopDetailsPanel = new PubgShopDetailsPanel();
    ((Control) shopDetailsPanel).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
    ((Control) shopDetailsPanel).VerticalExpand = false;
    ((Control) shopDetailsPanel).MinHeight = 250f;
    ((Control) shopDetailsPanel).MaxHeight = 310f;
    this._detailsPanel = shopDetailsPanel;
    this._detailsPanel.OnBuyRequested += (Action<string, string>) ((itemId, offerId) =>
    {
      Action<string, string> onBuyRequested = this.OnBuyRequested;
      if (onBuyRequested == null)
        return;
      onBuyRequested(itemId, offerId);
    });
    ((Control) this._previewPanel).Orphan();
    ((Control) this._previewPanel).VerticalExpand = true;
    ((Control) this._previewPanel).HorizontalExpand = true;
    ((Control) this._previewPanel).MinWidth = 340f;
    ((Control) this._previewPanel).Margin = new Thickness(0.0f);
    ((Control) boxContainer6).AddChild((Control) this._heroCaseCard);
    ((Control) boxContainer6).AddChild((Control) this._detailsPanel);
    ((Control) boxContainer6).AddChild((Control) this._previewPanel);
    ((Control) this).AddChild((Control) boxContainer2);
    ((Control) this).AddChild((Control) boxContainer6);
  }

  public void UpdateState(
    List<SkinShopItemInfo> shopItems,
    Dictionary<string, DateTime?> itemExpiresAt,
    int playerCoins,
    int playerPremiumCoins)
  {
    this._shopItems = shopItems;
    this._itemExpiresAt = itemExpiresAt;
    this._playerCoins = playerCoins;
    this._playerPremiumCoins = playerPremiumCoins;
    this._shopItemsById = new Dictionary<string, SkinShopItemInfo>();
    foreach (SkinShopItemInfo shopItem in this._shopItems)
      this._shopItemsById[shopItem.ItemId] = shopItem;
    this._heroCaseFeature = this.GetCaseFeature();
    this._heroCaseCard.UpdateFeature(this._heroCaseFeature);
    this.RefreshHeroCaseState();
    this.RebuildCards();
  }

  public void UpdateBalance(int playerCoins, int playerPremiumCoins)
  {
    this._playerCoins = playerCoins;
    this._playerPremiumCoins = playerPremiumCoins;
    this.RefreshHeroCaseState();
    this.UpdateDetails();
  }

  public void SetHeroCaseState(bool pending, string? errorCode)
  {
    this._heroCasePending = pending;
    this._heroCaseErrorCode = errorCode;
    this.RefreshHeroCaseState();
  }

  private BoxContainer CreateFiltersRow()
  {
    BoxContainer row = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    this.AddFilterButton(row, PubgShopFilter.All, "mainmenu-shop-filter-all");
    this.AddFilterButton(row, PubgShopFilter.Clothes, "mainmenu-shop-filter-clothes");
    this.AddFilterButton(row, PubgShopFilter.Ghosts, "mainmenu-shop-filter-ghosts");
    this.AddFilterButton(row, PubgShopFilter.Emotes, "mainmenu-shop-filter-emotes");
    this.AddFilterButton(row, PubgShopFilter.Limited, "mainmenu-shop-filter-limited");
    this.SyncFilterButtonState();
    return row;
  }

  private void AddFilterButton(BoxContainer row, PubgShopFilter filter, string textKey)
  {
    PubgSubcategoryTab pubgSubcategoryTab1 = new PubgSubcategoryTab();
    pubgSubcategoryTab1.Text = Loc.GetString(textKey);
    pubgSubcategoryTab1.Margin = new Thickness(0.0f, 0.0f, 8f, 0.0f);
    PubgSubcategoryTab pubgSubcategoryTab2 = pubgSubcategoryTab1;
    pubgSubcategoryTab2.OnPressed += (Action) (() =>
    {
      this._currentFilter = filter;
      this.SyncFilterButtonState();
      this.RebuildCards();
    });
    this._filterButtons[filter] = pubgSubcategoryTab2;
    ((Control) row).AddChild((Control) pubgSubcategoryTab2);
  }

  private void SyncFilterButtonState()
  {
    foreach ((PubgShopFilter key, PubgSubcategoryTab pubgSubcategoryTab) in this._filterButtons)
      pubgSubcategoryTab.IsActive = key == this._currentFilter;
  }

  private void RebuildCards()
  {
    this._cardsById.Clear();
    ((Control) this._cardsContainer).RemoveAllChildren();
    this._presentedItems = this._presenter.BuildPresentedItems((IReadOnlyList<SkinShopItemInfo>) this._shopItems, (IReadOnlyDictionary<string, DateTime?>) this._itemExpiresAt, this._currentFilter);
    if (this._presentedItems.Count == 0)
    {
      this._selectedItemId = (string) null;
      this.UpdateDetails();
      BoxContainer cardsContainer = this._cardsContainer;
      Label label = new Label();
      label.Text = Loc.GetString("mainmenu-shop-no-items");
      label.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#7D8A9F", new Color?()));
      ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label).Margin = new Thickness(0.0f, 24f, 0.0f, 0.0f);
      ((Control) cardsContainer).AddChild((Control) label);
    }
    else
    {
      int columns = this.CalculateColumns();
      GridContainer gridContainer1 = new GridContainer();
      gridContainer1.Columns = columns;
      ((Control) gridContainer1).HorizontalExpand = true;
      ((Control) gridContainer1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
      GridContainer gridContainer2 = gridContainer1;
      SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
      foreach (PubgShopPresentedItem presentedItem in this._presentedItems)
      {
        ActionComponent actionComponent;
        Texture iconTexture = !string.Equals(presentedItem.SkinComponent.Category, "emote", StringComparison.OrdinalIgnoreCase) || !presentedItem.Prototype.TryGetComponent<ActionComponent>("Action", ref actionComponent) || actionComponent.Icon == null ? ((IDirectionalTextureProvider) spriteSystem.GetPrototypeIcon(presentedItem.Prototype)).Default : spriteSystem.Frame0(actionComponent.Icon);
        PubgShopItemCard card = new PubgShopItemCard();
        card.SetData(presentedItem, iconTexture, string.Equals(this._selectedItemId, presentedItem.ItemId, StringComparison.Ordinal));
        card.OnCardClicked += (Action<string>) (itemId => this.SelectItem(itemId, true));
        card.OnCardRightClicked += (Action<string>) (itemId =>
        {
          SkinShopItemInfo skinShopItemInfo;
          if (!this._shopItemsById.TryGetValue(itemId, out skinShopItemInfo))
            return;
          Action<string, SkinShopItemInfo, Control> itemRightClicked = this.OnItemRightClicked;
          if (itemRightClicked == null)
            return;
          itemRightClicked(itemId, skinShopItemInfo, (Control) card);
        });
        this._cardsById[presentedItem.ItemId] = card;
        ((Control) gridContainer2).AddChild((Control) card);
      }
      ((Control) this._cardsContainer).AddChild((Control) gridContainer2);
      if (this._selectedItemId == null || this._presentedItems.All<PubgShopPresentedItem>((Func<PubgShopPresentedItem, bool>) (i => i.ItemId != this._selectedItemId)))
      {
        this._selectedItemId = this._presentedItems[0].ItemId;
        Action<string> onItemSelected = this.OnItemSelected;
        if (onItemSelected != null)
          onItemSelected(this._selectedItemId);
      }
      this.RefreshSelectionVisuals();
      this.UpdateDetails();
    }
  }

  private int CalculateColumns()
  {
    int x = ((Control) this._cardsScroll).PixelSize.X;
    if (x <= 0)
      return 4;
    int columns = x / 168;
    if (columns < 2)
      columns = 2;
    if (columns > 7)
      columns = 7;
    return columns;
  }

  private void SelectItem(string itemId, bool raisePreview)
  {
    this._selectedItemId = itemId;
    this.RefreshSelectionVisuals();
    this.UpdateDetails();
    if (!raisePreview)
      return;
    Action<string> onItemSelected = this.OnItemSelected;
    if (onItemSelected == null)
      return;
    onItemSelected(itemId);
  }

  private void RefreshSelectionVisuals()
  {
    foreach ((string str, PubgShopItemCard pubgShopItemCard) in this._cardsById)
      pubgShopItemCard.SetSelected(string.Equals(str, this._selectedItemId, StringComparison.Ordinal));
  }

  private void UpdateDetails()
  {
    this._detailsPanel.UpdateSelection(this._selectedItemId == null ? (PubgShopPresentedItem) null : this._presentedItems.FirstOrDefault<PubgShopPresentedItem>((Func<PubgShopPresentedItem, bool>) (i => i.ItemId == this._selectedItemId)), this._playerCoins, this._playerPremiumCoins);
  }

  private void OnHeroCaseCardPressed()
  {
    if (this._heroCaseFeature == null)
      return;
    Action<string> onHeroCasePressed = this.OnHeroCasePressed;
    if (onHeroCasePressed == null)
      return;
    onHeroCasePressed(this._heroCaseFeature.ID);
  }

  private void RefreshHeroCaseState()
  {
    PubgShopFeaturePrototype heroCaseFeature = this._heroCaseFeature;
    if (heroCaseFeature == null)
      this._heroCaseCard.UpdateAvailability(false, false, false, (string) null, 0, "coins");
    else
      this._heroCaseCard.UpdateAvailability(true, heroCaseFeature.Currency == "premium" ? this._playerPremiumCoins >= heroCaseFeature.Price : this._playerCoins >= heroCaseFeature.Price, this._heroCasePending, this._heroCaseErrorCode, heroCaseFeature.Price, heroCaseFeature.Currency);
  }

  private PubgShopFeaturePrototype? GetCaseFeature()
  {
    return this._prototypeManager.EnumeratePrototypes<PubgShopFeaturePrototype>().Where<PubgShopFeaturePrototype>((Func<PubgShopFeaturePrototype, bool>) (p => p.Enabled && string.Equals(p.Kind, "case", StringComparison.OrdinalIgnoreCase))).OrderBy<PubgShopFeaturePrototype, int>((Func<PubgShopFeaturePrototype, int>) (p => p.Order)).FirstOrDefault<PubgShopFeaturePrototype>();
  }
}
