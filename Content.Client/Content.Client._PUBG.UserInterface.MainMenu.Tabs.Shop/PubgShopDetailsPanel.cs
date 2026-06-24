using System;
using System.Collections.Generic;
using System.Linq;
using Content.Client._PUBG.UserInterface.Controls;
using Content.Shared._PUBG.Skin;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Expected O, but got Unknown
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Expected O, but got Unknown
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Expected O, but got Unknown
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		((PanelContainer)this).PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#10141D", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#2C3445", (Color?)null),
			BorderThickness = new Thickness(1f),
			ContentMarginLeftOverride = 14f,
			ContentMarginTopOverride = 14f,
			ContentMarginRightOverride = 14f,
			ContentMarginBottomOverride = 14f
		};
		((Control)this).MinWidth = 340f;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		_titleLabel = new Label
		{
			Text = Loc.GetString("mainmenu-shop-details-select-item"),
			HorizontalAlignment = (HAlignment)1
		};
		((Control)_titleLabel).SetOnlyStyleClass("LabelHeading");
		_rarityLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null),
			Margin = new Thickness(0f, 4f, 0f, 0f)
		};
		_descriptionLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#CBD5E1", (Color?)null),
			Margin = new Thickness(0f, 8f, 0f, 0f),
			VerticalExpand = false
		};
		_statusLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#9FC2FF", (Color?)null),
			Margin = new Thickness(0f, 6f, 0f, 0f)
		};
		_offersTitleLabel = new Label
		{
			Text = Loc.GetString("mainmenu-shop-details-offers"),
			Margin = new Thickness(0f, 12f, 0f, 6f)
		};
		((Control)_offersTitleLabel).SetOnlyStyleClass("LabelHeading");
		ScrollContainer val2 = new ScrollContainer
		{
			VerticalExpand = true,
			HScrollEnabled = false,
			MinHeight = 120f
		};
		_offersList = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)val2).AddChild((Control)(object)_offersList);
		_warningLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF6B6B", (Color?)null),
			Margin = new Thickness(0f, 8f, 0f, 0f)
		};
		_buyButton = new Button
		{
			Text = Loc.GetString("mainmenu-shop-details-buy"),
			Disabled = true,
			MinHeight = 42f,
			Margin = new Thickness(0f, 10f, 0f, 0f)
		};
		((Control)_buyButton).StyleClasses.Add("ButtonBig");
		((BaseButton)_buyButton).OnPressed += delegate
		{
			TryBuySelected();
		};
		((Control)val).AddChild((Control)(object)_titleLabel);
		((Control)val).AddChild((Control)(object)_rarityLabel);
		((Control)val).AddChild((Control)(object)_descriptionLabel);
		((Control)val).AddChild((Control)(object)_statusLabel);
		((Control)val).AddChild((Control)(object)_offersTitleLabel);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)_warningLabel);
		((Control)val).AddChild((Control)(object)_buyButton);
		((Control)this).AddChild((Control)(object)val);
	}

	public void UpdateSelection(PubgShopPresentedItem? item, int playerCoins, int playerPremiumCoins)
	{
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Expected O, but got Unknown
		_selectedItem = item;
		_playerCoins = playerCoins;
		_playerPremiumCoins = playerPremiumCoins;
		if (_selectedItem == null)
		{
			_selectedOfferId = null;
			_titleLabel.Text = Loc.GetString("mainmenu-shop-details-select-item");
			_rarityLabel.Text = string.Empty;
			_descriptionLabel.Text = Loc.GetString("mainmenu-shop-details-select-item-hint");
			_statusLabel.Text = string.Empty;
			_warningLabel.Text = string.Empty;
			((Control)_offersList).RemoveAllChildren();
			((Control)_offersList).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-shop-details-no-offers"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#7D8A9F", (Color?)null)
			});
			_buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
			((BaseButton)_buyButton).Disabled = true;
		}
		else
		{
			_titleLabel.Text = _selectedItem.Prototype.Name;
			_rarityLabel.Text = Loc.GetString("mainmenu-shop-details-rarity", new(string, object)[1] { ("rarity", GetRarityText(_selectedItem.SkinComponent.Rarity)) });
			_rarityLabel.FontColorOverride = GetRarityColor(_selectedItem.SkinComponent.Rarity);
			_descriptionLabel.Text = (string.IsNullOrWhiteSpace(_selectedItem.SkinComponent.Description) ? Loc.GetString("mainmenu-skin-tooltip-no-description") : _selectedItem.SkinComponent.Description);
			_statusLabel.Text = BuildStatusText(_selectedItem);
			RebuildOffers();
			UpdateBuyState();
		}
	}

	public void UpdateBalance(int playerCoins, int playerPremiumCoins)
	{
		_playerCoins = playerCoins;
		_playerPremiumCoins = playerPremiumCoins;
		UpdateBuyState();
	}

	private void RebuildOffers()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		((Control)_offersList).RemoveAllChildren();
		if (_selectedItem == null)
		{
			_selectedOfferId = null;
			return;
		}
		List<SkinShopOfferInfo> list = _selectedItem.Offers.ToList();
		if (list.Count == 0)
		{
			_selectedOfferId = null;
			((Control)_offersList).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-shop-details-no-offers"),
				FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#7D8A9F", (Color?)null)
			});
			return;
		}
		if (_selectedOfferId == null || list.All((SkinShopOfferInfo o) => o.OfferId != _selectedOfferId))
		{
			_selectedOfferId = list[0].OfferId;
		}
		ButtonGroup val = new ButtonGroup(true);
		foreach (SkinShopOfferInfo offer in list)
		{
			Button val2 = new Button
			{
				Text = SkinContextMenuBuilder.GetShopOfferText(offer),
				HorizontalExpand = true,
				ToggleMode = true,
				Group = val,
				Pressed = (offer.OfferId == _selectedOfferId),
				Margin = new Thickness(0f, 0f, 0f, 4f)
			};
			((Control)val2).StyleClasses.Add("OpenBoth");
			((BaseButton)val2).OnPressed += delegate
			{
				_selectedOfferId = offer.OfferId;
				UpdateBuyState();
			};
			((Control)_offersList).AddChild((Control)(object)val2);
		}
	}

	private void UpdateBuyState()
	{
		if (_selectedItem == null)
		{
			_buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
			((BaseButton)_buyButton).Disabled = true;
			_warningLabel.Text = string.Empty;
			return;
		}
		if (_selectedItem.IsOwnedPermanent)
		{
			_buyButton.Text = Loc.GetString("mainmenu-shop-owned");
			((BaseButton)_buyButton).Disabled = true;
			_warningLabel.Text = Loc.GetString("mainmenu-shop-already-owned");
			return;
		}
		SkinShopOfferInfo skinShopOfferInfo = _selectedItem.Offers.FirstOrDefault((SkinShopOfferInfo o) => o.OfferId == _selectedOfferId);
		if (skinShopOfferInfo == null)
		{
			_buyButton.Text = Loc.GetString("mainmenu-shop-details-buy");
			((BaseButton)_buyButton).Disabled = true;
			_warningLabel.Text = string.Empty;
			return;
		}
		_buyButton.Text = SkinContextMenuBuilder.GetShopOfferText(skinShopOfferInfo);
		bool flag = SkinContextMenuBuilder.HasEnoughCurrency(skinShopOfferInfo, _playerCoins, _playerPremiumCoins);
		((BaseButton)_buyButton).Disabled = !flag;
		if (flag)
		{
			_warningLabel.Text = string.Empty;
		}
		else
		{
			_warningLabel.Text = ((skinShopOfferInfo.Currency == "coins") ? Loc.GetString("mainmenu-shop-not-enough-coins") : Loc.GetString("mainmenu-shop-not-enough-diamonds"));
		}
	}

	private void TryBuySelected()
	{
		if (_selectedItem != null && !string.IsNullOrEmpty(_selectedOfferId))
		{
			SkinShopOfferInfo skinShopOfferInfo = _selectedItem.Offers.FirstOrDefault((SkinShopOfferInfo o) => o.OfferId == _selectedOfferId);
			if (skinShopOfferInfo != null && SkinContextMenuBuilder.HasEnoughCurrency(skinShopOfferInfo, _playerCoins, _playerPremiumCoins))
			{
				this.OnBuyRequested?.Invoke(_selectedItem.ItemId, skinShopOfferInfo.OfferId);
			}
		}
	}

	private string BuildStatusText(PubgShopPresentedItem item)
	{
		if (item.IsOwnedPermanent)
		{
			return Loc.GetString("mainmenu-shop-owned");
		}
		if (item.IsTimedOwned && item.ExpiresAt.HasValue)
		{
			string item2 = item.ExpiresAt.Value.ToLocalTime().ToString("g");
			return Loc.GetString("mainmenu-skin-tooltip-expires-at", new(string, object)[1] { ("time", item2) });
		}
		int? remainingCount = GetRemainingCount(item);
		if (remainingCount.HasValue)
		{
			return Loc.GetString("mainmenu-shop-collectible-remaining", new(string, object)[1] { ("remaining", remainingCount.Value) });
		}
		if (item.IsLimited)
		{
			return Loc.GetString("mainmenu-shop-details-status-limited");
		}
		return string.Empty;
	}

	private static int? GetRemainingCount(PubgShopPresentedItem item)
	{
		if (item.Remaining > 0)
		{
			return item.Remaining;
		}
		if (item.CollectibleLimit > 0)
		{
			return Math.Max(item.CollectibleLimit - item.SoldCount, 0);
		}
		if (item.IsCollectible)
		{
			return 0;
		}
		return null;
	}

	private string GetRarityText(SkinRarity rarity)
	{
		return rarity switch
		{
			SkinRarity.Common => Loc.GetString("mainmenu-skin-rarity-common"), 
			SkinRarity.Rare => Loc.GetString("mainmenu-skin-rarity-rare"), 
			SkinRarity.Epic => Loc.GetString("mainmenu-skin-rarity-epic"), 
			SkinRarity.Legendary => Loc.GetString("mainmenu-skin-rarity-legendary"), 
			SkinRarity.Unique => Loc.GetString("mainmenu-skin-rarity-unique"), 
			_ => rarity.ToString(), 
		};
	}

	private Color GetRarityColor(SkinRarity rarity)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		return (Color)(rarity switch
		{
			SkinRarity.Unique => Color.FromHex((ReadOnlySpan<char>)"#00E5FF", (Color?)null), 
			SkinRarity.Legendary => Color.FromHex((ReadOnlySpan<char>)"#FFD700", (Color?)null), 
			SkinRarity.Epic => Color.FromHex((ReadOnlySpan<char>)"#FF9800", (Color?)null), 
			SkinRarity.Rare => Color.FromHex((ReadOnlySpan<char>)"#9C27B0", (Color?)null), 
			_ => Color.FromHex((ReadOnlySpan<char>)"#9E9E9E", (Color?)null), 
		});
	}
}
