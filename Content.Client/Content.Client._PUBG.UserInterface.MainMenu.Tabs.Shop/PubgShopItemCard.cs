using System;
using System.Numerics;
using Content.Shared._PUBG.Skin;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;

public sealed class PubgShopItemCard : PanelContainer
{
	private static readonly Color BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#111217", (Color?)null);

	private static readonly Color HoverBackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#171A21", (Color?)null);

	private static readonly Color SelectedBorderColor = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null);

	private static readonly Color BorderColor = Color.FromHex((ReadOnlySpan<char>)"#353B48", (Color?)null);

	private readonly StyleBoxFlat _panelStyle = new StyleBoxFlat
	{
		BorderThickness = new Thickness(2f),
		BackgroundColor = BackgroundColor,
		BorderColor = BorderColor,
		ContentMarginLeftOverride = 8f,
		ContentMarginTopOverride = 8f,
		ContentMarginRightOverride = 8f,
		ContentMarginBottomOverride = 8f
	};

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

	public string ItemId { get; private set; } = string.Empty;

	public bool IsSelected { get; private set; }

	public event Action<string>? OnCardClicked;

	public event Action<string>? OnCardRightClicked;

	public PubgShopItemCard()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Expected O, but got Unknown
		//IL_023a: Expected O, but got Unknown
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Expected O, but got Unknown
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Expected O, but got Unknown
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Expected O, but got Unknown
		((PanelContainer)this).PanelOverride = (StyleBox)(object)_panelStyle;
		((Control)this).MouseFilter = (MouseFilterMode)0;
		((Control)this).MinSize = new Vector2(151f, 224f);
		((Control)this).MaxSize = new Vector2(151f, 224f);
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true
		};
		_ownedTag = CreateTag(Color.FromHex((ReadOnlySpan<char>)"#103820", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#2D7A43", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#8DFFB0", (Color?)null), out _ownedTagText);
		((Control)_ownedTag).Margin = new Thickness(0f, 0f, 4f, 0f);
		_limitedTag = CreateTag(Color.FromHex((ReadOnlySpan<char>)"#3A2507", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#93641E", (Color?)null), Color.FromHex((ReadOnlySpan<char>)"#FFD36A", (Color?)null), out _limitedTagText);
		((Control)val2).AddChild((Control)(object)_ownedTag);
		((Control)val2).AddChild((Control)(object)_limitedTag);
		PanelContainer val3 = new PanelContainer
		{
			MinSize = new Vector2(135f, 100f),
			HorizontalExpand = true,
			Margin = new Thickness(0f, 6f, 0f, 0f),
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#0A0D13", (Color?)null)
			}
		};
		_iconView = new TextureRect
		{
			Stretch = (StretchMode)7,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val3).AddChild((Control)(object)_iconView);
		_nameLabel = new Label
		{
			FontColorOverride = Color.White,
			HorizontalAlignment = (HAlignment)2,
			MaxWidth = 135f,
			ClipText = true,
			Margin = new Thickness(0f, 6f, 0f, 0f)
		};
		_priceLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 2f, 0f, 0f)
		};
		_stateLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#9AA7B9", (Color?)null),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 2f, 0f, 0f),
			MaxWidth = 135f,
			ClipText = true
		};
		_badgeLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FBC02D", (Color?)null),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 2f, 0f, 0f),
			MaxWidth = 135f,
			ClipText = true
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)_nameLabel);
		((Control)val).AddChild((Control)(object)_priceLabel);
		((Control)val).AddChild((Control)(object)_stateLabel);
		((Control)val).AddChild((Control)(object)_badgeLabel);
		((Control)this).AddChild((Control)(object)val);
		RefreshStyle();
	}

	public void SetData(PubgShopPresentedItem item, Texture? iconTexture, bool isSelected)
	{
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		ItemId = item.ItemId;
		IsSelected = isSelected;
		_ownedTagText.Text = Loc.GetString("mainmenu-shop-owned");
		((Control)_ownedTag).Visible = item.IsOwnedPermanent;
		int? remainingCount = GetRemainingCount(item);
		if (remainingCount.HasValue)
		{
			_limitedTagText.Text = Loc.GetString("mainmenu-shop-card-remaining-short", new(string, object)[1] { ("remaining", remainingCount.Value) });
			((Control)_limitedTag).Visible = true;
		}
		else
		{
			_limitedTagText.Text = Loc.GetString("mainmenu-shop-card-limited");
			((Control)_limitedTag).Visible = item.IsLimited;
		}
		_iconView.Texture = iconTexture;
		_nameLabel.Text = item.Prototype.Name;
		_priceLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD05C", (Color?)null);
		if (item.IsOwnedPermanent)
		{
			_priceLabel.Text = Loc.GetString("mainmenu-shop-owned");
			_priceLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#66E08A", (Color?)null);
		}
		else
		{
			_priceLabel.Text = BuildPriceText(item.LowestOffer);
		}
		if (item.IsTimedOwned && item.ExpiresAt.HasValue)
		{
			string item2 = item.ExpiresAt.Value.ToLocalTime().ToString("g");
			_stateLabel.Text = Loc.GetString("mainmenu-skin-tooltip-expires-at", new(string, object)[1] { ("time", item2) });
			_stateLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#66D9FF", (Color?)null);
		}
		else if (item.IsTimedOwned)
		{
			_stateLabel.Text = Loc.GetString("mainmenu-shop-timed-active");
			_stateLabel.FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#66D9FF", (Color?)null);
		}
		else
		{
			_stateLabel.Text = string.Empty;
		}
		if (remainingCount.HasValue)
		{
			_badgeLabel.Text = Loc.GetString("mainmenu-shop-collectible-remaining", new(string, object)[1] { ("remaining", remainingCount.Value) });
		}
		else
		{
			_badgeLabel.Text = string.Empty;
		}
		RefreshStyle();
	}

	public void SetSelected(bool isSelected)
	{
		IsSelected = isSelected;
		RefreshStyle();
	}

	private string BuildPriceText(SkinShopOfferInfo offer)
	{
		string item = ((offer.Currency == "coins") ? Loc.GetString("mainmenu-shop-currency-coins") : Loc.GetString("mainmenu-shop-currency-premium"));
		return Loc.GetString("mainmenu-shop-card-price", new(string, object)[2]
		{
			("price", offer.Price),
			("currency", item)
		});
	}

	private void RefreshStyle()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		_panelStyle.BorderColor = (IsSelected ? SelectedBorderColor : BorderColor);
		_panelStyle.BackgroundColor = (_hovered ? HoverBackgroundColor : BackgroundColor);
	}

	private static PanelContainer CreateTag(Color backgroundColor, Color borderColor, Color textColor, out Label textLabel)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		textLabel = new Label
		{
			FontColorOverride = textColor
		};
		PanelContainer val = new PanelContainer
		{
			Visible = false,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor,
				BorderColor = borderColor,
				BorderThickness = new Thickness(1f),
				ContentMarginLeftOverride = 5f,
				ContentMarginTopOverride = 1f,
				ContentMarginRightOverride = 5f,
				ContentMarginBottomOverride = 1f
			}
		};
		((Control)val).AddChild((Control)(object)textLabel);
		return val;
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

	protected override void MouseEntered()
	{
		((Control)this).MouseEntered();
		_hovered = true;
		((Control)this).UserInterfaceManager.HoverSound();
		RefreshStyle();
	}

	protected override void MouseExited()
	{
		((Control)this).MouseExited();
		_hovered = false;
		RefreshStyle();
	}

	protected override void KeyBindDown(GUIBoundKeyEventArgs args)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		((Control)this).KeyBindDown(args);
		if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIClick)
		{
			((Control)this).UserInterfaceManager.ClickSound();
			this.OnCardClicked?.Invoke(ItemId);
			((BoundKeyEventArgs)args).Handle();
		}
		else if (((BoundKeyEventArgs)args).Function == EngineKeyFunctions.UIRightClick)
		{
			this.OnCardRightClicked?.Invoke(ItemId);
			((BoundKeyEventArgs)args).Handle();
		}
	}
}
