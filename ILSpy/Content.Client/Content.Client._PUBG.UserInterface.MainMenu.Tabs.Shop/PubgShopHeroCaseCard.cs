using System;
using System.Numerics;
using Content.Shared._PUBG.Shop;
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

public sealed class PubgShopHeroCaseCard : PanelContainer
{
	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	private readonly TextureRect _iconView;

	private readonly Label _titleLabel;

	private readonly Label _descriptionLabel;

	private readonly Label _priceLabel;

	private readonly Label _statusLabel;

	private readonly Button _ctaButton;

	private bool _featureEnabled;

	public event Action? OnPressed;

	public PubgShopHeroCaseCard()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Expected O, but got Unknown
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Expected O, but got Unknown
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Expected O, but got Unknown
		IoCManager.InjectDependencies<PubgShopHeroCaseCard>(this);
		((PanelContainer)this).PanelOverride = (StyleBox)new StyleBoxFlat
		{
			BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#1A1410", (Color?)null),
			BorderColor = Color.FromHex((ReadOnlySpan<char>)"#FFB800", (Color?)null),
			BorderThickness = new Thickness(2f),
			ContentMarginLeftOverride = 10f,
			ContentMarginTopOverride = 10f,
			ContentMarginRightOverride = 10f,
			ContentMarginBottomOverride = 10f
		};
		((Control)this).MinHeight = 126f;
		((Control)this).MaxHeight = 146f;
		((Control)this).HorizontalExpand = true;
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		_iconView = new TextureRect
		{
			MinSize = new Vector2(76f, 80f),
			Stretch = (StretchMode)7,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(10f, 0f, 0f, 0f)
		};
		_titleLabel = new Label
		{
			Text = Loc.GetString("mainmenu-shop-hero-case-title"),
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFD36A", (Color?)null),
			HorizontalAlignment = (HAlignment)1
		};
		((Control)_titleLabel).SetOnlyStyleClass("LabelHeading");
		_descriptionLabel = new Label
		{
			Text = Loc.GetString("mainmenu-shop-hero-case-description"),
			HorizontalAlignment = (HAlignment)1,
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#D6CBA7", (Color?)null),
			Margin = new Thickness(0f, 4f, 0f, 0f),
			MaxWidth = 250f,
			ClipText = true
		};
		_priceLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FFDF8C", (Color?)null),
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(0f, 4f, 0f, 0f)
		};
		_statusLabel = new Label
		{
			FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#FF8F8F", (Color?)null),
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(0f, 4f, 0f, 0f)
		};
		_ctaButton = new Button
		{
			Text = Loc.GetString("mainmenu-shop-hero-case-soon"),
			Disabled = true,
			MinHeight = 30f,
			HorizontalAlignment = (HAlignment)1,
			Margin = new Thickness(0f, 6f, 0f, 0f)
		};
		((Control)_ctaButton).StyleClasses.Add("OpenBoth");
		((BaseButton)_ctaButton).OnPressed += delegate
		{
			this.OnPressed?.Invoke();
		};
		((Control)val).AddChild((Control)(object)_iconView);
		((Control)val2).AddChild((Control)(object)_titleLabel);
		((Control)val2).AddChild((Control)(object)_descriptionLabel);
		((Control)val2).AddChild((Control)(object)_priceLabel);
		((Control)val2).AddChild((Control)(object)_statusLabel);
		((Control)val2).AddChild((Control)(object)_ctaButton);
		((Control)val).AddChild((Control)(object)val2);
		((Control)this).AddChild((Control)(object)val);
	}

	public void UpdateFeature(PubgShopFeaturePrototype? feature)
	{
		if (feature == null || !feature.Enabled)
		{
			((Control)this).Visible = false;
			_featureEnabled = false;
			return;
		}
		((Control)this).Visible = true;
		_featureEnabled = true;
		_titleLabel.Text = Loc.GetString(feature.TitleLocKey);
		_descriptionLabel.Text = Loc.GetString(feature.DescriptionLocKey);
		string item = ((feature.Currency == "coins") ? Loc.GetString("mainmenu-shop-currency-coins") : Loc.GetString("mainmenu-shop-currency-premium"));
		_priceLabel.Text = Loc.GetString("mainmenu-shop-card-price", new(string, object)[2]
		{
			("price", feature.Price),
			("currency", item)
		});
		Texture texture = null;
		SpriteSystem val = _entityManager.System<SpriteSystem>();
		EntityPrototype val2 = default(EntityPrototype);
		if (feature.Icon != null)
		{
			texture = val.Frame0(feature.Icon);
		}
		else if (!string.IsNullOrWhiteSpace(feature.IconEntity) && _prototypeManager.TryIndex<EntityPrototype>(feature.IconEntity, ref val2))
		{
			texture = ((IDirectionalTextureProvider)val.GetPrototypeIcon(val2)).Default;
		}
		_iconView.Texture = texture;
		_statusLabel.Text = string.Empty;
		_ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-open");
		((BaseButton)_ctaButton).Disabled = false;
	}

	public void UpdateAvailability(bool enabled, bool hasEnoughCurrency, bool pending, string? errorCode, int price, string currency)
	{
		if (!_featureEnabled || !enabled)
		{
			((BaseButton)_ctaButton).Disabled = true;
			_ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-soon");
			_statusLabel.Text = string.Empty;
			return;
		}
		string item = ((currency == "premium") ? Loc.GetString("mainmenu-shop-currency-premium") : Loc.GetString("mainmenu-shop-currency-coins"));
		if (pending)
		{
			((BaseButton)_ctaButton).Disabled = true;
			_ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-opening");
			_statusLabel.Text = Loc.GetString("mainmenu-shop-hero-case-opening-status");
			return;
		}
		((BaseButton)_ctaButton).Disabled = !hasEnoughCurrency;
		_ctaButton.Text = Loc.GetString("mainmenu-shop-hero-case-open-for", new(string, object)[2]
		{
			("price", price),
			("currency", item)
		});
		if (!hasEnoughCurrency)
		{
			_statusLabel.Text = Loc.GetString("mainmenu-shop-hero-case-error-not-enough-currency");
			return;
		}
		if (string.IsNullOrWhiteSpace(errorCode))
		{
			_statusLabel.Text = string.Empty;
			return;
		}
		_statusLabel.Text = Loc.GetString(errorCode switch
		{
			"CASE_NOT_FOUND" => "mainmenu-shop-hero-case-error-case-not-found", 
			"CASE_DISABLED" => "mainmenu-shop-hero-case-error-case-disabled", 
			"CASE_POOL_EMPTY" => "mainmenu-shop-hero-case-error-case-pool-empty", 
			"NOT_ENOUGH_COINS" => "mainmenu-shop-hero-case-error-not-enough-coins", 
			"NOT_ENOUGH_PREMIUM" => "mainmenu-shop-hero-case-error-not-enough-premium", 
			"PLAYER_NOT_FOUND" => "mainmenu-shop-hero-case-error-player-not-found", 
			_ => "mainmenu-shop-hero-case-error-internal", 
		});
	}
}
