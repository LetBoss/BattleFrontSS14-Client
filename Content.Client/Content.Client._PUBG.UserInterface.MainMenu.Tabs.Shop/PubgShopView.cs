using System;
using System.Collections.Generic;
using System.Linq;
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
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Expected O, but got Unknown
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Expected O, but got Unknown
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<PubgShopView>(this);
		_previewPanel = previewPanel;
		_presenter = new PubgShopPresenter(_prototypeManager);
		((BoxContainer)this).Orientation = (LayoutOrientation)0;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(15f, 15f, 10f, 15f)
		};
		BoxContainer val2 = CreateFiltersRow();
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)new Label
		{
			Text = Loc.GetString("mainmenu-shop-cases-hint"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#9AA8C0", (Color?)null),
			Margin = new Thickness(0f, 6f, 0f, 0f),
			HorizontalExpand = true
		});
		_cardsScroll = new ScrollContainer
		{
			VerticalExpand = true,
			HScrollEnabled = false,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		((Control)_cardsScroll).OnResized += RebuildCards;
		_cardsContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)_cardsScroll).AddChild((Control)(object)_cardsContainer);
		((Control)val).AddChild((Control)(object)_cardsScroll);
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 380f,
			MaxWidth = 380f,
			Margin = new Thickness(0f, 15f, 15f, 15f),
			VerticalExpand = true
		};
		PubgShopHeroCaseCard pubgShopHeroCaseCard = new PubgShopHeroCaseCard();
		((Control)pubgShopHeroCaseCard).Margin = new Thickness(0f, 0f, 0f, 10f);
		_heroCaseCard = pubgShopHeroCaseCard;
		_heroCaseFeature = GetCaseFeature();
		_heroCaseCard.OnPressed += OnHeroCaseCardPressed;
		_heroCaseCard.UpdateFeature(_heroCaseFeature);
		PubgShopDetailsPanel pubgShopDetailsPanel = new PubgShopDetailsPanel();
		((Control)pubgShopDetailsPanel).Margin = new Thickness(0f, 0f, 0f, 10f);
		((Control)pubgShopDetailsPanel).VerticalExpand = false;
		((Control)pubgShopDetailsPanel).MinHeight = 250f;
		((Control)pubgShopDetailsPanel).MaxHeight = 310f;
		_detailsPanel = pubgShopDetailsPanel;
		_detailsPanel.OnBuyRequested += delegate(string itemId, string offerId)
		{
			this.OnBuyRequested?.Invoke(itemId, offerId);
		};
		((Control)_previewPanel).Orphan();
		((Control)_previewPanel).VerticalExpand = true;
		((Control)_previewPanel).HorizontalExpand = true;
		((Control)_previewPanel).MinWidth = 340f;
		((Control)_previewPanel).Margin = new Thickness(0f);
		((Control)val3).AddChild((Control)(object)_heroCaseCard);
		((Control)val3).AddChild((Control)(object)_detailsPanel);
		((Control)val3).AddChild((Control)(object)_previewPanel);
		((Control)this).AddChild((Control)(object)val);
		((Control)this).AddChild((Control)(object)val3);
	}

	public void UpdateState(List<SkinShopItemInfo> shopItems, Dictionary<string, DateTime?> itemExpiresAt, int playerCoins, int playerPremiumCoins)
	{
		_shopItems = shopItems;
		_itemExpiresAt = itemExpiresAt;
		_playerCoins = playerCoins;
		_playerPremiumCoins = playerPremiumCoins;
		_shopItemsById = new Dictionary<string, SkinShopItemInfo>();
		foreach (SkinShopItemInfo shopItem in _shopItems)
		{
			_shopItemsById[shopItem.ItemId] = shopItem;
		}
		_heroCaseFeature = GetCaseFeature();
		_heroCaseCard.UpdateFeature(_heroCaseFeature);
		RefreshHeroCaseState();
		RebuildCards();
	}

	public void UpdateBalance(int playerCoins, int playerPremiumCoins)
	{
		_playerCoins = playerCoins;
		_playerPremiumCoins = playerPremiumCoins;
		RefreshHeroCaseState();
		UpdateDetails();
	}

	public void SetHeroCaseState(bool pending, string? errorCode)
	{
		_heroCasePending = pending;
		_heroCaseErrorCode = errorCode;
		RefreshHeroCaseState();
	}

	private BoxContainer CreateFiltersRow()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		AddFilterButton(val, PubgShopFilter.All, "mainmenu-shop-filter-all");
		AddFilterButton(val, PubgShopFilter.Clothes, "mainmenu-shop-filter-clothes");
		AddFilterButton(val, PubgShopFilter.Ghosts, "mainmenu-shop-filter-ghosts");
		AddFilterButton(val, PubgShopFilter.Emotes, "mainmenu-shop-filter-emotes");
		AddFilterButton(val, PubgShopFilter.Limited, "mainmenu-shop-filter-limited");
		SyncFilterButtonState();
		return val;
	}

	private void AddFilterButton(BoxContainer row, PubgShopFilter filter, string textKey)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		PubgSubcategoryTab obj = new PubgSubcategoryTab
		{
			Text = Loc.GetString(textKey)
		};
		((Control)obj).Margin = new Thickness(0f, 0f, 8f, 0f);
		PubgSubcategoryTab pubgSubcategoryTab = obj;
		pubgSubcategoryTab.OnPressed += delegate
		{
			_currentFilter = filter;
			SyncFilterButtonState();
			RebuildCards();
		};
		_filterButtons[filter] = pubgSubcategoryTab;
		((Control)row).AddChild((Control)(object)pubgSubcategoryTab);
	}

	private void SyncFilterButtonState()
	{
		foreach (KeyValuePair<PubgShopFilter, PubgSubcategoryTab> filterButton in _filterButtons)
		{
			filterButton.Deconstruct(out var key, out var value);
			PubgShopFilter pubgShopFilter = key;
			value.IsActive = pubgShopFilter == _currentFilter;
		}
	}

	private void RebuildCards()
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		_cardsById.Clear();
		((Control)_cardsContainer).RemoveAllChildren();
		_presentedItems = _presenter.BuildPresentedItems(_shopItems, _itemExpiresAt, _currentFilter);
		if (_presentedItems.Count == 0)
		{
			_selectedItemId = null;
			UpdateDetails();
			((Control)_cardsContainer).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-shop-no-items"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#7D8A9F", (Color?)null),
				HorizontalAlignment = (HAlignment)2,
				Margin = new Thickness(0f, 24f, 0f, 0f)
			});
			return;
		}
		int columns = CalculateColumns();
		GridContainer val = new GridContainer
		{
			Columns = columns,
			HorizontalExpand = true,
			Margin = new Thickness(0f, 0f, 0f, 10f)
		};
		SpriteSystem val2 = _entityManager.System<SpriteSystem>();
		ActionComponent actionComponent = default(ActionComponent);
		foreach (PubgShopPresentedItem presentedItem in _presentedItems)
		{
			Texture iconTexture = ((!string.Equals(presentedItem.SkinComponent.Category, "emote", StringComparison.OrdinalIgnoreCase) || !presentedItem.Prototype.TryGetComponent<ActionComponent>("Action", ref actionComponent) || actionComponent.Icon == null) ? ((IDirectionalTextureProvider)val2.GetPrototypeIcon(presentedItem.Prototype)).Default : val2.Frame0(actionComponent.Icon));
			PubgShopItemCard card = new PubgShopItemCard();
			card.SetData(presentedItem, iconTexture, string.Equals(_selectedItemId, presentedItem.ItemId, StringComparison.Ordinal));
			card.OnCardClicked += delegate(string itemId)
			{
				SelectItem(itemId, raisePreview: true);
			};
			card.OnCardRightClicked += delegate(string itemId)
			{
				if (_shopItemsById.TryGetValue(itemId, out SkinShopItemInfo value))
				{
					this.OnItemRightClicked?.Invoke(itemId, value, (Control)(object)card);
				}
			};
			_cardsById[presentedItem.ItemId] = card;
			((Control)val).AddChild((Control)(object)card);
		}
		((Control)_cardsContainer).AddChild((Control)(object)val);
		if (_selectedItemId == null || _presentedItems.All((PubgShopPresentedItem i) => i.ItemId != _selectedItemId))
		{
			_selectedItemId = _presentedItems[0].ItemId;
			this.OnItemSelected?.Invoke(_selectedItemId);
		}
		RefreshSelectionVisuals();
		UpdateDetails();
	}

	private int CalculateColumns()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		int x = ((Control)_cardsScroll).PixelSize.X;
		if (x <= 0)
		{
			return 4;
		}
		int num = x / 168;
		if (num < 2)
		{
			num = 2;
		}
		if (num > 7)
		{
			num = 7;
		}
		return num;
	}

	private void SelectItem(string itemId, bool raisePreview)
	{
		_selectedItemId = itemId;
		RefreshSelectionVisuals();
		UpdateDetails();
		if (raisePreview)
		{
			this.OnItemSelected?.Invoke(itemId);
		}
	}

	private void RefreshSelectionVisuals()
	{
		foreach (var (a, pubgShopItemCard2) in _cardsById)
		{
			pubgShopItemCard2.SetSelected(string.Equals(a, _selectedItemId, StringComparison.Ordinal));
		}
	}

	private void UpdateDetails()
	{
		PubgShopPresentedItem item = ((_selectedItemId == null) ? null : _presentedItems.FirstOrDefault((PubgShopPresentedItem i) => i.ItemId == _selectedItemId));
		_detailsPanel.UpdateSelection(item, _playerCoins, _playerPremiumCoins);
	}

	private void OnHeroCaseCardPressed()
	{
		if (_heroCaseFeature != null)
		{
			this.OnHeroCasePressed?.Invoke(_heroCaseFeature.ID);
		}
	}

	private void RefreshHeroCaseState()
	{
		PubgShopFeaturePrototype heroCaseFeature = _heroCaseFeature;
		if (heroCaseFeature == null)
		{
			_heroCaseCard.UpdateAvailability(enabled: false, hasEnoughCurrency: false, pending: false, null, 0, "coins");
			return;
		}
		bool hasEnoughCurrency = ((heroCaseFeature.Currency == "premium") ? (_playerPremiumCoins >= heroCaseFeature.Price) : (_playerCoins >= heroCaseFeature.Price));
		_heroCaseCard.UpdateAvailability(enabled: true, hasEnoughCurrency, _heroCasePending, _heroCaseErrorCode, heroCaseFeature.Price, heroCaseFeature.Currency);
	}

	private PubgShopFeaturePrototype? GetCaseFeature()
	{
		return (from p in _prototypeManager.EnumeratePrototypes<PubgShopFeaturePrototype>()
			where p.Enabled && string.Equals(p.Kind, "case", StringComparison.OrdinalIgnoreCase)
			orderby p.Order
			select p).FirstOrDefault();
	}
}
