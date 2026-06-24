using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Content.Client._PUBG.UserInterface.Controls;
using Content.Client._PUBG.UserInterface.MainMenu.Controls;
using Content.Client._PUBG.UserInterface.MainMenu.Tabs.Shop;
using Content.Client.Humanoid;
using Content.Client.Lobby;
using Content.Shared._PUBG;
using Content.Shared._PUBG.Emote;
using Content.Shared._PUBG.Markings;
using Content.Shared._PUBG.Skin;
using Content.Shared.Actions.Components;
using Content.Shared.Ghost;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class SkinsTab : BoxContainer
{
	private enum EmotePreviewLayer : byte
	{
		Emote
	}

	[Dependency]
	private IEntityManager _entityManager;

	[Dependency]
	private IPrototypeManager _prototypeManager;

	[Dependency]
	private IComponentFactory _componentFactory;

	[Dependency]
	private IUserInterfaceManager _uiManager;

	[Dependency]
	private IPlayerManager _playerManager;

	[Dependency]
	private IClientPreferencesManager _preferencesManager;

	private readonly ISawmill _logger = Logger.GetSawmill("skin");

	private SkinCategory _currentCategory;

	private SkinsSubcategory _currentSubcategory;

	private const string TraitCategory = "trait";

	private const string TraitHeadKey = "head";

	private const string TraitTailKey = "tail";

	private const string FirstKillBannerOutfitKey = "firstKillBanner";

	private const string DefaultFirstKillBannerId = "PubgFirstKillBannerTest";

	private Dictionary<string, List<string>> _allItems = new Dictionary<string, List<string>>();

	private Dictionary<string, List<string>> _unlockedItems = new Dictionary<string, List<string>>();

	private Dictionary<string, DateTime?> _itemExpiresAt = new Dictionary<string, DateTime?>();

	private Dictionary<string, int> _recipePrices = new Dictionary<string, int>();

	private List<SkinShopItemInfo> _shopItems = new List<SkinShopItemInfo>();

	private Dictionary<string, string> _currentOutfit = new Dictionary<string, string>();

	private Dictionary<string, string> _temporaryOutfit = new Dictionary<string, string>();

	private EntityUid? _previewEntity;

	private EntityUid? _shopPreviewEntity;

	private EntityUid? _shopSpecialPreviewEntity;

	private CharacterPreviewPanel? _previewPanel;

	private CharacterPreviewPanel? _shopPreviewPanel;

	private int _playerCoins;

	private int _playerScrap;

	private int _playerPremiumCoins;

	private List<string> _allEmotes = new List<string>();

	private List<string> _unlockedEmotes = new List<string>();

	private List<string> _equippedEmotes = new List<string>();

	private List<string> _temporaryEquippedEmotes = new List<string>();

	private int _maxEmoteSlots = 1;

	private int _totalCaseDropSkins;

	private int _unlockedCaseDropSkins;

	private int _totalUniqueSkins;

	private int _totalEmotes;

	private int _availableEmotes;

	private BoxContainer _mainContent;

	private BoxContainer? _traitsContent;

	private PubgSubcategoryTab? _traitsHeadTab;

	private PubgSubcategoryTab? _traitsTailTab;

	private GridContainer? _traitsGrid;

	private Label? _traitsEmptyLabel;

	private string _selectedFirstKillBanner = "PubgFirstKillBannerTest";

	private string _currentTraitKey = "head";

	private PubgEmoteSlot[] _emoteSlots = new PubgEmoteSlot[6];

	private List<PubgCategoryButton> _categoryButtons = new List<PubgCategoryButton>();

	private PubgShopView? _shopView;

	private Popup? _currentContextPopup;

	private bool _heroCasePending;

	private string? _heroCaseErrorCode;

	public HashSet<SkinCategory>? AllowedCategories;

	public bool ShowCollectionProgress = true;

	public event Action<Dictionary<string, string>>? OnApply;

	public event Action<string>? OnHeroCaseOpenRequested;

	public SkinsTab()
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Expected O, but got Unknown
		IoCManager.InjectDependencies<SkinsTab>(this);
		((BoxContainer)this).Orientation = (LayoutOrientation)1;
		((Control)this).HorizontalExpand = true;
		((Control)this).VerticalExpand = true;
		_mainContent = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)this).AddChild((Control)(object)_mainContent);
		CreatePreviewEntities();
	}

	private void CreatePreviewEntities()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		_previewEntity = SpawnProfilePreviewEntity();
		_shopPreviewEntity = SpawnProfilePreviewEntity();
	}

	private EntityUid SpawnProfilePreviewEntity()
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (_preferencesManager.Preferences?.SelectedCharacter is HumanoidCharacterProfile humanoidCharacterProfile)
		{
			SpeciesPrototype speciesPrototype = _prototypeManager.Index<SpeciesPrototype>(humanoidCharacterProfile.Species);
			EntityUid val = _entityManager.SpawnEntity(EntProtoId.op_Implicit(speciesPrototype.DollPrototype), MapCoordinates.Nullspace, (ComponentRegistry)null);
			_entityManager.System<HumanoidAppearanceSystem>().LoadProfile(val, humanoidCharacterProfile);
			return val;
		}
		return _entityManager.SpawnEntity("MobHumanDummy", MapCoordinates.Nullspace, (ComponentRegistry)null);
	}

	public void LoadSubcategory(SkinsSubcategory subcategory)
	{
		_currentSubcategory = subcategory;
		((Control)_mainContent).RemoveAllChildren();
		if (subcategory != SkinsSubcategory.Shop)
		{
			_shopView = null;
			ResetShopPreviewToHumanoid();
		}
		switch (subcategory)
		{
		case SkinsSubcategory.MySkins:
			LoadMySkinsView();
			break;
		case SkinsSubcategory.Shop:
			LoadShopView();
			break;
		case SkinsSubcategory.Emotes:
			LoadEmotesView();
			break;
		case SkinsSubcategory.Traits:
			LoadTraitsView();
			break;
		case SkinsSubcategory.Banners:
			LoadBannersView();
			break;
		}
	}

	public void UpdateData(Dictionary<string, List<string>> allItems, Dictionary<string, List<string>> unlockedItems, Dictionary<string, DateTime?> itemExpiresAt, Dictionary<string, int> recipePrices, List<SkinShopItemInfo> shopItems, Dictionary<string, string> currentOutfit, int playerCoins, int playerScrap, int playerPremiumCoins, List<string> allEmotes, List<string> unlockedEmotes, List<string> equippedEmotes, int maxEmoteSlots, int totalCaseDropSkins, int unlockedCaseDropSkins, int totalUniqueSkins, int totalEmotes, int availableEmotes)
	{
		_allItems = allItems;
		_unlockedItems = unlockedItems;
		_itemExpiresAt = itemExpiresAt;
		_recipePrices = recipePrices;
		_shopItems = shopItems;
		_currentOutfit = currentOutfit;
		_temporaryOutfit = new Dictionary<string, string>(currentOutfit);
		_playerCoins = playerCoins;
		_playerScrap = playerScrap;
		_playerPremiumCoins = playerPremiumCoins;
		_allEmotes = allEmotes;
		_unlockedEmotes = unlockedEmotes;
		_equippedEmotes = equippedEmotes;
		_temporaryEquippedEmotes = new List<string>(equippedEmotes);
		_maxEmoteSlots = maxEmoteSlots;
		_totalCaseDropSkins = totalCaseDropSkins;
		_unlockedCaseDropSkins = unlockedCaseDropSkins;
		_totalUniqueSkins = totalUniqueSkins;
		_totalEmotes = totalEmotes;
		_availableEmotes = availableEmotes;
		_selectedFirstKillBanner = ((currentOutfit.TryGetValue("firstKillBanner", out string value) && _prototypeManager.HasIndex<PubgFirstKillBannerPrototype>(value)) ? value : "PubgFirstKillBannerTest");
		LoadSubcategory(_currentSubcategory);
		ApplyHeroCaseUiState();
	}

	public void UpdateBalance(int coins, int scrap, int premiumCoins)
	{
		_playerCoins = coins;
		_playerScrap = scrap;
		_playerPremiumCoins = premiumCoins;
		_shopView?.UpdateBalance(coins, premiumCoins);
		ApplyHeroCaseUiState();
	}

	public void SetHeroCasePending(bool pending)
	{
		_heroCasePending = pending;
		ApplyHeroCaseUiState();
	}

	public void SetHeroCaseError(string? errorCode)
	{
		_heroCaseErrorCode = errorCode;
		ApplyHeroCaseUiState();
	}

	private void ApplyHeroCaseUiState()
	{
		_shopView?.SetHeroCaseState(_heroCasePending, _heroCaseErrorCode);
	}

	public void UpdateShopItems(List<SkinShopItemInfo> shopItems)
	{
		_shopItems = shopItems;
		if (_currentSubcategory == SkinsSubcategory.Shop)
		{
			if (_shopView != null)
			{
				_shopView.UpdateState(_shopItems, _itemExpiresAt, _playerCoins, _playerPremiumCoins);
				ApplyHeroCaseUiState();
			}
			else
			{
				LoadSubcategory(_currentSubcategory);
			}
		}
	}

	private void LoadMySkinsView()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Expected O, but got Unknown
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Expected O, but got Unknown
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Expected O, but got Unknown
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Expected O, but got Unknown
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_047b: Unknown result type (might be due to invalid IL or missing references)
		//IL_049b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a9: Expected O, but got Unknown
		//IL_04a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Expected O, but got Unknown
		_categoryButtons.Clear();
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 200f,
			MaxWidth = 200f,
			HorizontalExpand = false,
			Margin = new Thickness(15f)
		};
		Label val2 = new Label
		{
			Text = Loc.GetString("mainmenu-category-label"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		((Control)val).AddChild((Control)(object)val2);
		List<(string, string, SkinCategory)> list = new(string, string, SkinCategory)[6]
		{
			("mainmenu-category-head", "mainmenu-category-icon-head", SkinCategory.Head),
			("mainmenu-category-jumpsuit", "mainmenu-category-icon-jumpsuit", SkinCategory.Jumpsuit),
			("mainmenu-category-outer", "mainmenu-category-icon-outer", SkinCategory.OuterClothing),
			("mainmenu-category-neck", "mainmenu-category-icon-neck", SkinCategory.Neck),
			("mainmenu-category-shoes", "mainmenu-category-icon-shoes", SkinCategory.Shoes),
			("mainmenu-category-ghost", "mainmenu-category-icon-ghost", SkinCategory.Ghost)
		}.Where(((string LocKey, string IconKey, SkinCategory Category) entry) => AllowedCategories == null || AllowedCategories.Contains(entry.Category)).ToList();
		if (list.Count > 0 && list.All<(string, string, SkinCategory)>(((string LocKey, string IconKey, SkinCategory Category) entry) => entry.Category != _currentCategory))
		{
			_currentCategory = list[0].Item3;
		}
		foreach (var item in list)
		{
			((Control)val).AddChild((Control)(object)CreateCategoryButton(Loc.GetString(item.Item1), Loc.GetString(item.Item2), item.Item3));
		}
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(0f, 15f, 15f, 15f)
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		Label val5 = new Label
		{
			Text = Loc.GetString("mainmenu-category-select"),
			HorizontalExpand = true,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val5).SetOnlyStyleClass("LabelHeading");
		Label val6 = new Label();
		val6.Text = Loc.GetString("mainmenu-collection-stats", new(string, object)[3]
		{
			("unlockedCase", _unlockedCaseDropSkins),
			("totalCase", _totalCaseDropSkins),
			("totalUnique", _totalUniqueSkins)
		});
		val6.FontColorOverride = Color.Gray;
		((Control)val6).HorizontalAlignment = (HAlignment)3;
		((Control)val6).Margin = new Thickness(0f, 0f, 0f, 15f);
		Label val7 = val6;
		((Control)val7).Visible = ShowCollectionProgress;
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val4).AddChild((Control)(object)val7);
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			Margin = new Thickness(0f, 0f, 0f, 15f),
			Visible = ShowCollectionProgress
		};
		ProgressBar val9 = new ProgressBar
		{
			MinHeight = 20f,
			HorizontalExpand = true,
			MaxValue = ((_totalCaseDropSkins <= 0) ? 1 : _totalCaseDropSkins),
			Value = _unlockedCaseDropSkins
		};
		float num = ((_totalCaseDropSkins > 0) ? ((float)_unlockedCaseDropSkins / (float)_totalCaseDropSkins * 100f) : 0f);
		val6 = new Label();
		val6.Text = Loc.GetString("mainmenu-collection-progress", new(string, object)[3]
		{
			("percent", $"{num:F1}"),
			("unlockedCase", _unlockedCaseDropSkins),
			("totalCase", _totalCaseDropSkins)
		});
		val6.FontColorOverride = ((num >= 100f) ? Color.Gold : Color.LightGreen);
		((Control)val6).HorizontalAlignment = (HAlignment)2;
		((Control)val6).Margin = new Thickness(0f, 5f, 0f, 0f);
		Label val10 = val6;
		((Control)val8).AddChild((Control)(object)val9);
		((Control)val8).AddChild((Control)(object)val10);
		ScrollContainer val11 = new ScrollContainer
		{
			VerticalExpand = true
		};
		BoxContainer val12 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val11).AddChild((Control)(object)val12);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val8);
		((Control)val3).AddChild((Control)(object)val11);
		CharacterPreviewPanel characterPreviewPanel = CreatePreviewPanel();
		((Control)_mainContent).AddChild((Control)(object)val);
		((Control)_mainContent).AddChild((Control)(object)val3);
		((Control)_mainContent).AddChild((Control)(object)characterPreviewPanel);
		ShowCategory(_currentCategory, val5, val7, val12);
	}

	private PubgCategoryButton CreateCategoryButton(string text, string icon, SkinCategory category)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		PubgCategoryButton obj = new PubgCategoryButton
		{
			Text = text,
			Icon = icon
		};
		((Control)obj).Margin = new Thickness(0f, 0f, 0f, 8f);
		PubgCategoryButton button = obj;
		button.OnPressed += delegate
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			foreach (PubgCategoryButton categoryButton in _categoryButtons)
			{
				categoryButton.IsActive = false;
			}
			button.IsActive = true;
			Control obj2 = ((IEnumerable<Control>)((Control)_mainContent).Children).ElementAt(1);
			Control obj3 = ((IEnumerable<Control>)((Control)(BoxContainer)obj2).Children).ElementAt(0);
			Control obj4 = ((obj3 is BoxContainer) ? obj3 : null);
			Control obj5 = ((IEnumerable<Control>)obj4.Children).ElementAt(0);
			Label categoryLabel = (Label)(object)((obj5 is Label) ? obj5 : null);
			Control obj6 = ((IEnumerable<Control>)obj4.Children).ElementAt(1);
			Label statsLabel = (Label)(object)((obj6 is Label) ? obj6 : null);
			Control obj7 = ((IEnumerable<Control>)((Control)(BoxContainer)obj2).Children).ElementAt(2);
			Control obj8 = ((IEnumerable<Control>)((obj7 is ScrollContainer) ? obj7 : null).Children).ElementAt(0);
			BoxContainer itemsGrid = (BoxContainer)(object)((obj8 is BoxContainer) ? obj8 : null);
			ShowCategory(category, categoryLabel, statsLabel, itemsGrid);
		};
		_categoryButtons.Add(button);
		if (category == _currentCategory)
		{
			button.IsActive = true;
		}
		return button;
	}

	private CharacterPreviewPanel CreatePreviewPanel()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		CharacterPreviewPanel characterPreviewPanel = new CharacterPreviewPanel();
		((Control)characterPreviewPanel).Margin = new Thickness(0f, 15f, 15f, 15f);
		((Control)characterPreviewPanel).VerticalAlignment = (VAlignment)1;
		_previewPanel = characterPreviewPanel;
		if (_previewEntity.HasValue)
		{
			_previewPanel.SetPreviewEntity(_previewEntity.Value);
		}
		_previewPanel.OnApplyPressed += ApplyChanges;
		return _previewPanel;
	}

	private void ApplyChanges()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		EntityUid? localEntity = ((ISharedPlayerManager)_playerManager).LocalEntity;
		if (localEntity.HasValue)
		{
			EntityUid valueOrDefault = localEntity.GetValueOrDefault();
			if (_entityManager.HasComponent<PubgPlayerComponent>(valueOrDefault))
			{
				return;
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (var (text3, text4) in _temporaryOutfit)
		{
			List<string> value;
			if (text3 == "equippedEmotes")
			{
				dictionary[text3] = text4;
			}
			else if (string.IsNullOrEmpty(text4))
			{
				dictionary[text3] = text4;
			}
			else if (_unlockedItems.TryGetValue(text3, out value) && value.Contains(text4))
			{
				dictionary[text3] = text4;
			}
			else
			{
				dictionary[text3] = (_currentOutfit.TryGetValue(text3, out string value2) ? value2 : "");
			}
		}
		this.OnApply?.Invoke(new Dictionary<string, string>(dictionary));
		_currentOutfit = new Dictionary<string, string>(dictionary);
		_temporaryOutfit = new Dictionary<string, string>(dictionary);
		LoadSubcategory(_currentSubcategory);
	}

	private void ShowCategory(SkinCategory category, Label categoryLabel, Label statsLabel, BoxContainer itemsGrid)
	{
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Expected O, but got Unknown
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Expected O, but got Unknown
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Expected O, but got Unknown
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		_currentCategory = category;
		((Control)itemsGrid).RemoveAllChildren();
		categoryLabel.Text = category switch
		{
			SkinCategory.Jumpsuit => Loc.GetString("mainmenu-category-jumpsuit"), 
			SkinCategory.OuterClothing => Loc.GetString("mainmenu-category-outer"), 
			SkinCategory.Shoes => Loc.GetString("mainmenu-category-shoes"), 
			SkinCategory.Neck => Loc.GetString("mainmenu-category-neck"), 
			SkinCategory.Head => Loc.GetString("mainmenu-category-head"), 
			SkinCategory.Ghost => Loc.GetString("mainmenu-category-ghost"), 
			_ => Loc.GetString("mainmenu-category-unknown"), 
		};
		statsLabel.Text = Loc.GetString("mainmenu-collection-stats", new(string, object)[3]
		{
			("unlockedCase", _unlockedCaseDropSkins),
			("totalCase", _totalCaseDropSkins),
			("totalUnique", _totalUniqueSkins)
		});
		string key = category switch
		{
			SkinCategory.Jumpsuit => "jumpsuit", 
			SkinCategory.OuterClothing => "outerClothing", 
			SkinCategory.Shoes => "shoes", 
			SkinCategory.Neck => "neck", 
			SkinCategory.Head => "head", 
			SkinCategory.Ghost => "ghost", 
			_ => "", 
		};
		string text = key;
		string value;
		string currentEquipped = (_temporaryOutfit.TryGetValue(text, out value) ? value : "");
		List<string> value2;
		List<string> list = (_unlockedItems.TryGetValue(text, out value2) ? value2 : new List<string>());
		List<string> list2 = new List<string>();
		EntityPrototype val = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		foreach (KeyValuePair<string, int> recipePrice in _recipePrices)
		{
			recipePrice.Deconstruct(out key, out var _);
			string text2 = key;
			if (_prototypeManager.TryIndex<EntityPrototype>(text2, ref val) && val.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref pubgSkinItemComponent) && pubgSkinItemComponent.Category == text)
			{
				list2.Add(text2);
			}
		}
		if (list.Count > 0)
		{
			BoxContainer val2 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				Margin = new Thickness(0f, 0f, 0f, 10f)
			};
			Label val3 = new Label
			{
				Text = Loc.GetString("mainmenu-items-available"),
				Margin = new Thickness(5f, 0f, 0f, 5f)
			};
			((Control)val3).SetOnlyStyleClass("LabelHeading");
			GridContainer val4 = new GridContainer
			{
				Columns = 4
			};
			foreach (string item in list)
			{
				AddItemButton(val4, item, text, currentEquipped, isRecipe: false);
			}
			((Control)val2).AddChild((Control)(object)val3);
			((Control)val2).AddChild((Control)(object)val4);
			((Control)itemsGrid).AddChild((Control)(object)val2);
		}
		if (list2.Count > 0)
		{
			BoxContainer val5 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				Margin = new Thickness(0f, 10f, 0f, 0f)
			};
			Label val6 = new Label
			{
				Text = Loc.GetString("mainmenu-items-recipes"),
				Margin = new Thickness(5f, 0f, 0f, 5f)
			};
			((Control)val6).SetOnlyStyleClass("LabelHeading");
			GridContainer val7 = new GridContainer
			{
				Columns = 4
			};
			foreach (string item2 in list2)
			{
				AddItemButton(val7, item2, text, currentEquipped, isRecipe: true);
			}
			((Control)val5).AddChild((Control)(object)val6);
			((Control)val5).AddChild((Control)(object)val7);
			((Control)itemsGrid).AddChild((Control)(object)val5);
		}
		UpdatePreview();
	}

	private void AddItemButton(GridContainer grid, string itemId, string slotName, string currentEquipped, bool isRecipe)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Expected O, but got Unknown
		EntityPrototype val = default(EntityPrototype);
		if (!_prototypeManager.TryIndex<EntityPrototype>(itemId, ref val))
		{
			return;
		}
		PubgSkinItemComponent pubgSkinItemComponent = null;
		PubgSkinItemComponent pubgSkinItemComponent2 = default(PubgSkinItemComponent);
		if (val.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref pubgSkinItemComponent2))
		{
			pubgSkinItemComponent = pubgSkinItemComponent2;
		}
		bool flag = itemId == currentEquipped;
		PubgSkinItemCard obj = new PubgSkinItemCard
		{
			ItemName = val.Name,
			ProtoId = itemId,
			IsOwned = !isRecipe,
			IsEquipped = (flag && !isRecipe)
		};
		((Control)obj).Margin = new Thickness(3f);
		PubgSkinItemCard itemCard = obj;
		if (pubgSkinItemComponent != null)
		{
			itemCard.Rarity = pubgSkinItemComponent.Rarity.ToString();
			_itemExpiresAt.TryGetValue(itemId, out var value);
			FormattedMessage tooltipMessage = BuildSkinTooltipMessage(val.Name, pubgSkinItemComponent.Rarity, pubgSkinItemComponent.Description, value);
			((Control)itemCard).TooltipSupplier = (TooltipSupplier)((Control _) => CreateSkinTooltip(tooltipMessage));
			((Control)itemCard).TooltipDelay = 0.1f;
		}
		itemCard.OnCardClicked += delegate
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			_temporaryOutfit[slotName] = itemId;
			UpdatePreview();
			if (!isRecipe)
			{
				Control obj2 = ((IEnumerable<Control>)((Control)_mainContent).Children).ElementAt(1);
				Control obj3 = ((IEnumerable<Control>)((Control)(BoxContainer)obj2).Children).ElementAt(0);
				Control obj4 = ((obj3 is BoxContainer) ? obj3 : null);
				Control obj5 = ((IEnumerable<Control>)obj4.Children).ElementAt(0);
				Label categoryLabel = (Label)(object)((obj5 is Label) ? obj5 : null);
				Control obj6 = ((IEnumerable<Control>)obj4.Children).ElementAt(1);
				Label statsLabel = (Label)(object)((obj6 is Label) ? obj6 : null);
				Control obj7 = ((IEnumerable<Control>)((Control)(BoxContainer)obj2).Children).ElementAt(2);
				Control obj8 = ((IEnumerable<Control>)((obj7 is ScrollContainer) ? obj7 : null).Children).ElementAt(0);
				BoxContainer itemsGrid = (BoxContainer)(object)((obj8 is BoxContainer) ? obj8 : null);
				ShowCategory(_currentCategory, categoryLabel, statsLabel, itemsGrid);
			}
		};
		itemCard.OnCardRightClicked += delegate
		{
			ShowContextMenu(itemId, slotName, isRecipe, (Control)(object)itemCard);
		};
		((Control)grid).AddChild((Control)(object)itemCard);
	}

	private void UpdatePreview()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		if (!_previewEntity.HasValue)
		{
			return;
		}
		InventorySystem inventorySystem = _entityManager.System<InventorySystem>();
		if (!inventorySystem.TryGetSlots(_previewEntity.Value, out SlotDefinition[] slotDefinitions))
		{
			return;
		}
		foreach (SlotDefinition item in slotDefinitions.ToList())
		{
			if (inventorySystem.TryUnequip(_previewEntity.Value, item.Name, out var removedItem, silent: true, force: true, predicted: false, null, null, reparent: false))
			{
				_entityManager.DeleteEntity((EntityUid?)removedItem.Value);
			}
		}
		foreach (var (text3, text4) in _temporaryOutfit)
		{
			if (!(text3 == "equippedEmotes") && !(text3 == "ghost") && !string.IsNullOrEmpty(text4) && _prototypeManager.HasIndex<EntityPrototype>(text4))
			{
				EntityUid itemUid = _entityManager.SpawnEntity(text4, MapCoordinates.Nullspace, (ComponentRegistry)null);
				inventorySystem.TryEquip(_previewEntity.Value, itemUid, text3, silent: true, force: true);
			}
		}
	}

	private void LoadShopView()
	{
		CharacterPreviewPanel previewPanel = CreateShopPreviewPanel();
		_shopView = new PubgShopView(previewPanel);
		_shopView.OnItemSelected += HandleShopItemSelected;
		_shopView.OnItemRightClicked += HandleShopItemRightClicked;
		_shopView.OnBuyRequested += HandleShopBuyRequested;
		_shopView.OnHeroCasePressed += HandleHeroCasePressed;
		_shopView.UpdateState(_shopItems, _itemExpiresAt, _playerCoins, _playerPremiumCoins);
		ApplyHeroCaseUiState();
		((Control)_mainContent).AddChild((Control)(object)_shopView);
		UpdateShopPreview();
	}

	private CharacterPreviewPanel CreateShopPreviewPanel()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		CharacterPreviewPanel characterPreviewPanel = new CharacterPreviewPanel();
		((Control)characterPreviewPanel).Margin = new Thickness(0f, 15f, 15f, 15f);
		((Control)characterPreviewPanel).VerticalAlignment = (VAlignment)1;
		_shopPreviewPanel = characterPreviewPanel;
		if (_shopPreviewEntity.HasValue)
		{
			_shopPreviewPanel.SetPreviewEntity(_shopPreviewEntity.Value);
		}
		_shopPreviewPanel.SetApplyButtonVisible(visible: false);
		_shopPreviewPanel.SetPreviewScale(3.2f);
		return _shopPreviewPanel;
	}

	private void HandleShopItemSelected(string itemId)
	{
		PreviewShopItem(itemId);
	}

	private void HandleShopItemRightClicked(string itemId, SkinShopItemInfo shopItem, Control parent)
	{
		ShowShopContextMenu(itemId, shopItem, parent);
	}

	private void HandleShopBuyRequested(string itemId, string offerId)
	{
		SkinBuyMessage skinBuyMessage = new SkinBuyMessage(itemId, offerId);
		_entityManager.RaisePredictiveEvent<SkinBuyMessage>(skinBuyMessage);
		CloseCurrentPopup();
	}

	private void HandleHeroCasePressed(string caseId)
	{
		if (!_heroCasePending)
		{
			_heroCasePending = true;
			_heroCaseErrorCode = null;
			ApplyHeroCaseUiState();
			this.OnHeroCaseOpenRequested?.Invoke(caseId);
			CloseCurrentPopup();
		}
	}

	private void PreviewShopItem(string itemId)
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype val = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		if (!_shopPreviewEntity.HasValue || !_prototypeManager.TryIndex<EntityPrototype>(itemId, ref val) || !val.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref pubgSkinItemComponent))
		{
			return;
		}
		if (pubgSkinItemComponent.Category == "emote")
		{
			ResetShopPreviewToHumanoid();
			UpdateShopPreview();
			PreviewEmote(itemId);
			return;
		}
		if (pubgSkinItemComponent.Category == "ghost")
		{
			if (TryPreviewGhostItem(itemId, pubgSkinItemComponent))
			{
				ClearEmotePreview();
			}
			else
			{
				UpdateShopPreview();
			}
			return;
		}
		ResetShopPreviewToHumanoid();
		ClearEmotePreview();
		InventorySystem inventorySystem = _entityManager.System<InventorySystem>();
		if (!inventorySystem.TryGetSlots(_shopPreviewEntity.Value, out SlotDefinition[] slotDefinitions))
		{
			return;
		}
		foreach (SlotDefinition item in slotDefinitions.ToList())
		{
			if (inventorySystem.TryUnequip(_shopPreviewEntity.Value, item.Name, out var removedItem, silent: true, force: true, predicted: false, null, null, reparent: false))
			{
				_entityManager.DeleteEntity((EntityUid?)removedItem.Value);
			}
		}
		foreach (var (text3, text4) in _currentOutfit)
		{
			if (!(text3 == "ghost") && !(text3 == pubgSkinItemComponent.Category) && !string.IsNullOrEmpty(text4) && _prototypeManager.HasIndex<EntityPrototype>(text4))
			{
				EntityUid itemUid = _entityManager.SpawnEntity(text4, MapCoordinates.Nullspace, (ComponentRegistry)null);
				inventorySystem.TryEquip(_shopPreviewEntity.Value, itemUid, text3, silent: true, force: true);
			}
		}
		EntityUid itemUid2 = _entityManager.SpawnEntity(itemId, MapCoordinates.Nullspace, (ComponentRegistry)null);
		inventorySystem.TryEquip(_shopPreviewEntity.Value, itemUid2, pubgSkinItemComponent.Category, silent: true, force: true);
	}

	private bool TryPreviewGhostItem(string itemId, PubgSkinItemComponent skinComp)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (_shopPreviewPanel == null)
		{
			return false;
		}
		ResetShopPreviewToHumanoid();
		string text = itemId;
		EntProtoId<GhostComponent>? ghostPrototype = skinComp.GhostPrototype;
		if (ghostPrototype.HasValue)
		{
			text = ((object)ghostPrototype.GetValueOrDefault()/*cast due to constrained. prefix*/).ToString();
		}
		if (!_prototypeManager.HasIndex<EntityPrototype>(text))
		{
			return false;
		}
		_shopSpecialPreviewEntity = _entityManager.SpawnEntity(text, MapCoordinates.Nullspace, (ComponentRegistry)null);
		_shopPreviewPanel.SetPreviewEntity(_shopSpecialPreviewEntity.Value);
		return true;
	}

	private void ResetShopPreviewToHumanoid()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (_shopSpecialPreviewEntity.HasValue)
		{
			_entityManager.DeleteEntity((EntityUid?)_shopSpecialPreviewEntity.Value);
			_shopSpecialPreviewEntity = null;
		}
		if (_shopPreviewEntity.HasValue && _shopPreviewPanel != null)
		{
			_shopPreviewPanel.SetPreviewEntity(_shopPreviewEntity.Value);
		}
	}

	private void UpdateShopPreview()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if (!_shopPreviewEntity.HasValue)
		{
			return;
		}
		ResetShopPreviewToHumanoid();
		ClearEmotePreview();
		InventorySystem inventorySystem = _entityManager.System<InventorySystem>();
		if (!inventorySystem.TryGetSlots(_shopPreviewEntity.Value, out SlotDefinition[] slotDefinitions))
		{
			return;
		}
		foreach (SlotDefinition item in slotDefinitions.ToList())
		{
			if (inventorySystem.TryUnequip(_shopPreviewEntity.Value, item.Name, out var removedItem, silent: true, force: true, predicted: false, null, null, reparent: false))
			{
				_entityManager.DeleteEntity((EntityUid?)removedItem.Value);
			}
		}
		foreach (var (text3, text4) in _currentOutfit)
		{
			if (!(text3 == "ghost") && !string.IsNullOrEmpty(text4) && _prototypeManager.HasIndex<EntityPrototype>(text4))
			{
				EntityUid itemUid = _entityManager.SpawnEntity(text4, MapCoordinates.Nullspace, (ComponentRegistry)null);
				inventorySystem.TryEquip(_shopPreviewEntity.Value, itemUid, text3, silent: true, force: true);
			}
		}
	}

	private void PreviewEmote(string emoteId)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		EntityPrototype val = default(EntityPrototype);
		PubgEmoteComponent pubgEmoteComponent = default(PubgEmoteComponent);
		SpriteComponent val2 = default(SpriteComponent);
		if (_shopPreviewEntity.HasValue && _prototypeManager.TryIndex<EntityPrototype>(emoteId, ref val) && val.TryGetComponent<PubgEmoteComponent>("PubgEmote", ref pubgEmoteComponent) && _entityManager.TryGetComponent<SpriteComponent>(_shopPreviewEntity.Value, ref val2))
		{
			SpriteSystem val3 = _entityManager.System<SpriteSystem>();
			int num = default(int);
			if (val3.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), (Enum)EmotePreviewLayer.Emote, ref num, false))
			{
				val3.RemoveLayer(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), num, true);
			}
			if (!string.IsNullOrEmpty(pubgEmoteComponent.RsiPath) && !string.IsNullOrEmpty(pubgEmoteComponent.StateName))
			{
				ResPath val4 = default(ResPath);
				((ResPath)(ref val4))._002Ector(pubgEmoteComponent.RsiPath);
				int num2 = val3.AddLayer(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), (SpriteSpecifier)new Rsi(val4, pubgEmoteComponent.StateName), (int?)null);
				val3.LayerMapSet(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), (Enum)EmotePreviewLayer.Emote, num2);
				Box2 localBounds = val3.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)));
				float num3 = ((Box2)(ref localBounds)).Height / 2f + 0.3125f;
				float num4 = num3 * (pubgEmoteComponent.Scale - 1f) * 0.2f;
				val3.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), num2, new Vector2(0f, num3 + num4));
				val3.LayerSetScale(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, val2)), num2, new Vector2(pubgEmoteComponent.Scale, pubgEmoteComponent.Scale));
				val2.LayerSetShader(num2, "unshaded");
			}
		}
	}

	private void ClearEmotePreview()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		SpriteComponent item = default(SpriteComponent);
		if (_shopPreviewEntity.HasValue && _entityManager.TryGetComponent<SpriteComponent>(_shopPreviewEntity.Value, ref item))
		{
			SpriteSystem val = _entityManager.System<SpriteSystem>();
			int num = default(int);
			if (val.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, item)), (Enum)EmotePreviewLayer.Emote, ref num, false))
			{
				val.RemoveLayer(Entity<SpriteComponent>.op_Implicit((_shopPreviewEntity.Value, item)), num, true);
			}
		}
	}

	private void LoadEmotesView()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Expected O, but got Unknown
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			Margin = new Thickness(15f)
		};
		BoxContainer val3 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0
		};
		Label val4 = new Label
		{
			Text = Loc.GetString("mainmenu-emotes-title"),
			HorizontalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val4).SetOnlyStyleClass("LabelHeading");
		Label val5 = new Label();
		val5.Text = Loc.GetString("mainmenu-emotes-stats", new(string, object)[3]
		{
			("available", _availableEmotes),
			("unlocked", _availableEmotes),
			("total", _totalEmotes)
		});
		val5.FontColorOverride = Color.Gray;
		((Control)val5).HorizontalAlignment = (HAlignment)3;
		((Control)val5).Margin = new Thickness(0f, 0f, 0f, 15f);
		Label val6 = val5;
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val3).AddChild((Control)(object)val6);
		ScrollContainer val7 = new ScrollContainer
		{
			VerticalExpand = true
		};
		BoxContainer val8 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1
		};
		((Control)val7).AddChild((Control)(object)val8);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val2).AddChild((Control)(object)val7);
		BoxContainer val9 = CreateEmoteSlotsPanel();
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val9);
		((Control)_mainContent).AddChild((Control)(object)val);
		_temporaryEquippedEmotes = new List<string>(_equippedEmotes);
		LoadAvailableEmotes(val8);
	}

	private void LoadTraitsView()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Expected O, but got Unknown
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(16f)
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			Margin = new Thickness(0f, 0f, 0f, 12f)
		};
		_traitsHeadTab = new PubgSubcategoryTab
		{
			Text = Loc.GetString("mainmenu-traits-head")
		};
		_traitsHeadTab.OnPressed += delegate
		{
			ShowTraitCategory("head");
		};
		PubgSubcategoryTab obj = new PubgSubcategoryTab
		{
			Text = Loc.GetString("mainmenu-traits-tail")
		};
		((Control)obj).Margin = new Thickness(10f, 0f, 0f, 0f);
		_traitsTailTab = obj;
		_traitsTailTab.OnPressed += delegate
		{
			ShowTraitCategory("tail");
		};
		((Control)val2).AddChild((Control)(object)_traitsHeadTab);
		((Control)val2).AddChild((Control)(object)_traitsTailTab);
		((Control)val).AddChild((Control)(object)val2);
		ScrollContainer val3 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		_traitsGrid = new GridContainer
		{
			Columns = 4,
			Margin = new Thickness(6f)
		};
		_traitsEmptyLabel = new Label
		{
			Text = Loc.GetString("mainmenu-traits-empty"),
			FontColorOverride = Color.Gray,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)2,
			Visible = false
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		((Control)val4).AddChild((Control)(object)_traitsGrid);
		((Control)val4).AddChild((Control)(object)_traitsEmptyLabel);
		((Control)val3).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val3);
		_traitsContent = val;
		((Control)_mainContent).AddChild((Control)(object)val);
		ShowTraitCategory(_currentTraitKey);
	}

	private void LoadBannersView()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Expected O, but got Unknown
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Expected O, but got Unknown
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)0,
			HorizontalExpand = true,
			VerticalExpand = true,
			Margin = new Thickness(16f)
		};
		BoxContainer val2 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 280f,
			MaxWidth = 280f,
			VerticalExpand = true,
			Margin = new Thickness(0f, 0f, 16f, 0f)
		};
		Label val3 = new Label
		{
			Text = Loc.GetString("mainmenu-banners-title"),
			Margin = new Thickness(0f, 0f, 0f, 12f)
		};
		((Control)val3).SetOnlyStyleClass("LabelHeading");
		((Control)val2).AddChild((Control)(object)val3);
		ScrollContainer val4 = new ScrollContainer
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		BoxContainer val5 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true
		};
		((Control)val4).AddChild((Control)(object)val5);
		((Control)val2).AddChild((Control)(object)val4);
		Button val6 = new Button
		{
			Text = Loc.GetString("mainmenu-banners-apply"),
			MinHeight = 40f,
			Margin = new Thickness(0f, 12f, 0f, 0f)
		};
		((Control)val6).StyleClasses.Add("ButtonColorGreen");
		((BaseButton)val6).OnPressed += delegate
		{
			_temporaryOutfit["firstKillBanner"] = _selectedFirstKillBanner;
			Dictionary<string, string> dictionary = new Dictionary<string, string>(_currentOutfit) { ["firstKillBanner"] = _selectedFirstKillBanner };
			this.OnApply?.Invoke(dictionary);
			_currentOutfit = dictionary;
			_temporaryOutfit = new Dictionary<string, string>(dictionary);
		};
		((Control)val2).AddChild((Control)(object)val6);
		BoxContainer val7 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true
		};
		Label val8 = new Label
		{
			Text = Loc.GetString("mainmenu-banners-preview"),
			Margin = new Thickness(0f, 0f, 0f, 12f)
		};
		((Control)val8).SetOnlyStyleClass("LabelHeading");
		((Control)val7).AddChild((Control)(object)val8);
		BoxContainer previewContainer = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalExpand = true,
			VerticalExpand = true,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)1
		};
		List<(Button Button, string PrototypeId)> bannerButtons = new List<(Button, string)>();
		List<PubgFirstKillBannerPrototype> list = (from p in _prototypeManager.EnumeratePrototypes<PubgFirstKillBannerPrototype>()
			orderby p.ID
			select p).ToList();
		if (list.Count == 0)
		{
			((Control)val5).AddChild((Control)new Label
			{
				Text = Loc.GetString("mainmenu-banners-empty"),
				FontColorOverride = Color.Gray
			});
			((Control)val7).AddChild((Control)(object)previewContainer);
			((Control)val).AddChild((Control)(object)val2);
			((Control)val).AddChild((Control)(object)val7);
			((Control)_mainContent).AddChild((Control)(object)val);
			return;
		}
		if (!_prototypeManager.HasIndex<PubgFirstKillBannerPrototype>(_selectedFirstKillBanner))
		{
			_selectedFirstKillBanner = list[0].ID;
		}
		foreach (PubgFirstKillBannerPrototype item in list)
		{
			string currentPrototypeId = item.ID;
			Button val9 = new Button
			{
				HorizontalExpand = true,
				Margin = new Thickness(0f, 0f, 0f, 6f)
			};
			((BaseButton)val9).OnPressed += delegate
			{
				_selectedFirstKillBanner = currentPrototypeId;
				RefreshBannerButtons();
				RefreshPreview();
			};
			bannerButtons.Add((val9, currentPrototypeId));
			((Control)val5).AddChild((Control)(object)val9);
		}
		RefreshBannerButtons();
		RefreshPreview();
		((Control)val7).AddChild((Control)(object)previewContainer);
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val7);
		((Control)_mainContent).AddChild((Control)(object)val);
		void RefreshBannerButtons()
		{
			string text = Loc.GetString("mainmenu-selected-prefix");
			foreach (var (val10, text2) in bannerButtons)
			{
				val10.Text = ((text2 == _selectedFirstKillBanner) ? (text + text2) : text2);
			}
		}
		void RefreshPreview()
		{
			((Control)previewContainer).RemoveAllChildren();
			PubgFirstKillBannerPrototype prototype = default(PubgFirstKillBannerPrototype);
			if (_prototypeManager.TryIndex<PubgFirstKillBannerPrototype>(_selectedFirstKillBanner, ref prototype))
			{
				((Control)previewContainer).AddChild(BuildFirstKillBannerPreview(prototype));
			}
		}
	}

	private Control BuildFirstKillBannerPreview(PubgFirstKillBannerPrototype prototype)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Expected O, but got Unknown
		int num = Math.Max(1, prototype.Width);
		int num2 = Math.Max(1, prototype.Height);
		PanelContainer val = new PanelContainer
		{
			MinWidth = num,
			MinHeight = num2,
			HorizontalAlignment = (HAlignment)2,
			VerticalAlignment = (VAlignment)1,
			PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = Color.FromHex((ReadOnlySpan<char>)"#00000000", (Color?)null),
				BorderColor = Color.FromHex((ReadOnlySpan<char>)"#00000000", (Color?)null),
				BorderThickness = new Thickness(0f)
			}
		};
		LayoutContainer val2 = new LayoutContainer
		{
			MinWidth = num,
			MinHeight = num2
		};
		AnimatedTextureRect val3 = new AnimatedTextureRect
		{
			HorizontalExpand = true,
			VerticalExpand = true
		};
		val3.SetFromSpriteSpecifier((SpriteSpecifier)new Rsi(prototype.BackgroundRsi, prototype.BackgroundState));
		val3.DisplayRect.Stretch = (StretchMode)7;
		LayoutContainer.SetAnchorAndMarginPreset((Control)(object)val3, (LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Control)val2).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val2);
		return (Control)val;
	}

	private void ShowTraitCategory(string traitKey)
	{
		_currentTraitKey = traitKey;
		if (_traitsHeadTab != null)
		{
			_traitsHeadTab.IsActive = traitKey == "head";
		}
		if (_traitsTailTab != null)
		{
			_traitsTailTab.IsActive = traitKey == "tail";
		}
		PopulateTraitsGrid(traitKey);
	}

	private void PopulateTraitsGrid(string traitKey)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		if (_traitsGrid == null || _traitsEmptyLabel == null)
		{
			return;
		}
		((Control)_traitsGrid).RemoveAllChildren();
		List<string> traitItemsByKey = GetTraitItemsByKey(traitKey);
		List<string> value;
		HashSet<string> hashSet = (_unlockedItems.TryGetValue("trait", out value) ? new HashSet<string>(value) : new HashSet<string>());
		if (traitItemsByKey.Count == 0)
		{
			((Control)_traitsEmptyLabel).Visible = true;
			return;
		}
		((Control)_traitsEmptyLabel).Visible = false;
		EntityPrototype val = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		foreach (string item in traitItemsByKey.OrderBy((string id) => id))
		{
			if (_prototypeManager.TryIndex<EntityPrototype>(item, ref val) && val.TryGetComponent<PubgSkinItemComponent>(ref pubgSkinItemComponent, _componentFactory))
			{
				PubgSkinItemCard obj = new PubgSkinItemCard
				{
					ItemName = val.Name,
					ProtoId = val.ID,
					IsOwned = hashSet.Contains(item),
					IsEquipped = false,
					Rarity = pubgSkinItemComponent.Rarity.ToString().ToLowerInvariant()
				};
				((Control)obj).Margin = new Thickness(4f);
				PubgSkinItemCard pubgSkinItemCard = obj;
				FormattedMessage tooltipMessage = BuildSkinTooltipMessage(val.Name, pubgSkinItemComponent.Rarity, pubgSkinItemComponent.Description, null);
				((Control)pubgSkinItemCard).TooltipSupplier = (TooltipSupplier)((Control _) => CreateSkinTooltip(tooltipMessage));
				((Control)_traitsGrid).AddChild((Control)(object)pubgSkinItemCard);
			}
		}
	}

	private List<string> GetTraitItemsByKey(string traitKey)
	{
		List<string> list = new List<string>();
		List<string> value2;
		if (_unlockedItems.TryGetValue("trait", out List<string> value))
		{
			list.AddRange(value);
		}
		else if (_allItems.TryGetValue("trait", out value2))
		{
			list.AddRange(value2);
		}
		List<string> list2 = new List<string>();
		EntityPrototype val = default(EntityPrototype);
		PubgSkinItemComponent pubgSkinItemComponent = default(PubgSkinItemComponent);
		PubgMarkingItemComponent pubgMarkingItemComponent = default(PubgMarkingItemComponent);
		foreach (string item in list)
		{
			if (_prototypeManager.TryIndex<EntityPrototype>(item, ref val) && val.TryGetComponent<PubgSkinItemComponent>(ref pubgSkinItemComponent, _componentFactory) && string.Equals(pubgSkinItemComponent.Category, "trait", StringComparison.OrdinalIgnoreCase) && val.TryGetComponent<PubgMarkingItemComponent>(ref pubgMarkingItemComponent, _componentFactory) && string.Equals(pubgMarkingItemComponent.TraitCategory, traitKey, StringComparison.OrdinalIgnoreCase))
			{
				list2.Add(item);
			}
		}
		return list2.Distinct().ToList();
	}

	private BoxContainer CreateEmoteSlotsPanel()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Expected O, but got Unknown
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Expected O, but got Unknown
		BoxContainer val = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			MinWidth = 600f,
			Margin = new Thickness(0f, 15f, 15f, 15f),
			VerticalAlignment = (VAlignment)1
		};
		Label val2 = new Label
		{
			Text = Loc.GetString("mainmenu-emotes-slots-title"),
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 15f)
		};
		((Control)val2).SetOnlyStyleClass("LabelHeading");
		Label val3 = new Label
		{
			Text = Loc.GetString("mainmenu-emotes-slots-hint"),
			FontColorOverride = Color.Gray,
			HorizontalAlignment = (HAlignment)2,
			Margin = new Thickness(0f, 0f, 0f, 20f)
		};
		BoxContainer val4 = new BoxContainer
		{
			Orientation = (LayoutOrientation)1,
			HorizontalAlignment = (HAlignment)2
		};
		GridContainer val5 = new GridContainer
		{
			Columns = 2,
			HorizontalAlignment = (HAlignment)2
		};
		for (int i = 0; i < 6; i++)
		{
			PubgEmoteSlot obj = new PubgEmoteSlot
			{
				SlotNumber = i + 1
			};
			((Control)obj).Margin = new Thickness(5f);
			PubgEmoteSlot pubgEmoteSlot = obj;
			_emoteSlots[i] = pubgEmoteSlot;
			((Control)val5).AddChild((Control)(object)pubgEmoteSlot);
		}
		((Control)val4).AddChild((Control)(object)val5);
		UpdateEmoteSlots();
		Button val6 = new Button
		{
			Text = Loc.GetString("mainmenu-emotes-apply"),
			MinHeight = 45f,
			Margin = new Thickness(0f, 25f, 0f, 0f)
		};
		((Control)val6).StyleClasses.Add("ButtonColorGreen");
		((BaseButton)val6).OnPressed += delegate
		{
			ApplyEmotes();
		};
		((Control)val).AddChild((Control)(object)val2);
		((Control)val).AddChild((Control)(object)val3);
		((Control)val).AddChild((Control)(object)val4);
		((Control)val).AddChild((Control)(object)val6);
		return val;
	}

	private void LoadAvailableEmotes(BoxContainer emotesGrid)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		((Control)emotesGrid).RemoveAllChildren();
		if (_unlockedEmotes.Count == 0)
		{
			Label val = new Label
			{
				Text = Loc.GetString("mainmenu-emotes-no-emotes").Replace("\\n", "\n"),
				FontColorOverride = Color.Gray,
				HorizontalAlignment = (HAlignment)2,
				VerticalAlignment = (VAlignment)2,
				Margin = new Thickness(0f, 50f, 0f, 0f)
			};
			((Control)emotesGrid).AddChild((Control)(object)val);
			return;
		}
		GridContainer val2 = new GridContainer
		{
			Columns = 4,
			Margin = new Thickness(10f)
		};
		SpriteSystem val3 = _entityManager.System<SpriteSystem>();
		EntityPrototype val4 = default(EntityPrototype);
		ActionComponent actionComponent = default(ActionComponent);
		foreach (string unlockedEmote in _unlockedEmotes)
		{
			if (_prototypeManager.TryIndex<EntityPrototype>(unlockedEmote, ref val4))
			{
				PubgEmoteCard obj = new PubgEmoteCard
				{
					EmoteName = val4.Name,
					EmoteId = unlockedEmote,
					IsEquipped = _temporaryEquippedEmotes.Contains(unlockedEmote)
				};
				((Control)obj).Margin = new Thickness(5f);
				PubgEmoteCard pubgEmoteCard = obj;
				if (val4.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
				{
					Texture emoteIcon = val3.Frame0(actionComponent.Icon);
					pubgEmoteCard.EmoteIcon = emoteIcon;
				}
				pubgEmoteCard.OnCardClicked += ToggleEmoteEquip;
				((Control)val2).AddChild((Control)(object)pubgEmoteCard);
			}
		}
		((Control)emotesGrid).AddChild((Control)(object)val2);
	}

	private void ToggleEmoteEquip(string emoteId)
	{
		if (_temporaryEquippedEmotes.Contains(emoteId))
		{
			_temporaryEquippedEmotes.Remove(emoteId);
		}
		else
		{
			if (_temporaryEquippedEmotes.Count >= _maxEmoteSlots)
			{
				_temporaryEquippedEmotes.RemoveAt(0);
			}
			_temporaryEquippedEmotes.Add(emoteId);
		}
		UpdateEmoteSlots();
		LoadAvailableEmotesRefresh();
	}

	private void LoadAvailableEmotesRefresh()
	{
		Control? obj = ((IEnumerable<Control>)((Control)_mainContent).Children).FirstOrDefault();
		BoxContainer val = (BoxContainer)(object)((obj is BoxContainer) ? obj : null);
		if (val != null)
		{
			Control? obj2 = ((IEnumerable<Control>)((Control)val).Children).FirstOrDefault();
			Control? obj3 = ((obj2 is BoxContainer) ? obj2 : null);
			Control? obj4 = ((obj3 != null) ? ((IEnumerable<Control>)obj3.Children).LastOrDefault() : null);
			Control? obj5 = ((obj4 is ScrollContainer) ? obj4 : null);
			Control? obj6 = ((obj5 != null) ? ((IEnumerable<Control>)obj5.Children).FirstOrDefault() : null);
			BoxContainer val2 = (BoxContainer)(object)((obj6 is BoxContainer) ? obj6 : null);
			if (val2 != null)
			{
				LoadAvailableEmotes(val2);
			}
		}
	}

	private void UpdateEmoteSlots()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Expected O, but got Unknown
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Expected O, but got Unknown
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Expected O, but got Unknown
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Expected O, but got Unknown
		SpriteSystem val = _entityManager.System<SpriteSystem>();
		EntityPrototype val4 = default(EntityPrototype);
		ActionComponent actionComponent = default(ActionComponent);
		for (int i = 0; i < _emoteSlots.Length; i++)
		{
			PubgEmoteSlot pubgEmoteSlot = _emoteSlots[i];
			if (pubgEmoteSlot == null)
			{
				continue;
			}
			if (i >= _maxEmoteSlots)
			{
				pubgEmoteSlot.IsLocked = true;
				pubgEmoteSlot.IsFilled = false;
				BoxContainer val2 = new BoxContainer
				{
					Orientation = (LayoutOrientation)1,
					HorizontalAlignment = (HAlignment)2,
					VerticalAlignment = (VAlignment)2
				};
				Label val3 = new Label
				{
					Text = Loc.GetString("mainmenu-emotes-slot-locked"),
					FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#AA3333", (Color?)null),
					HorizontalAlignment = (HAlignment)2
				};
				((Control)val2).AddChild((Control)(object)val3);
				pubgEmoteSlot.SetContent((Control)(object)val2);
				continue;
			}
			pubgEmoteSlot.IsLocked = false;
			if (i < _temporaryEquippedEmotes.Count)
			{
				string text = _temporaryEquippedEmotes[i];
				if (_prototypeManager.TryIndex<EntityPrototype>(text, ref val4))
				{
					pubgEmoteSlot.IsFilled = true;
					BoxContainer val5 = new BoxContainer
					{
						Orientation = (LayoutOrientation)1,
						HorizontalAlignment = (HAlignment)2,
						VerticalAlignment = (VAlignment)2
					};
					if (val4.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
					{
						Texture texture = val.Frame0(actionComponent.Icon);
						TextureRect val6 = new TextureRect
						{
							Texture = texture,
							MinSize = new Vector2(48f, 48f),
							HorizontalAlignment = (HAlignment)2,
							Margin = new Thickness(0f, 5f, 0f, 5f),
							Stretch = (StretchMode)7
						};
						((Control)val5).AddChild((Control)(object)val6);
					}
					Label val7 = new Label
					{
						Text = val4.Name,
						HorizontalAlignment = (HAlignment)2,
						FontColorOverride = Color.White
					};
					((Control)val5).AddChild((Control)(object)val7);
					Button val8 = new Button
					{
						Text = Loc.GetString("mainmenu-emotes-remove"),
						MinWidth = 100f,
						Margin = new Thickness(0f, 8f, 0f, 0f)
					};
					string currentEmoteId = text;
					((BaseButton)val8).OnPressed += delegate
					{
						_temporaryEquippedEmotes.Remove(currentEmoteId);
						UpdateEmoteSlots();
						LoadAvailableEmotesRefresh();
					};
					((Control)val5).AddChild((Control)(object)val8);
					pubgEmoteSlot.SetContent((Control)(object)val5);
				}
			}
			else
			{
				pubgEmoteSlot.IsFilled = false;
				BoxContainer val9 = new BoxContainer
				{
					Orientation = (LayoutOrientation)1,
					HorizontalAlignment = (HAlignment)2,
					VerticalAlignment = (VAlignment)2
				};
				Label val10 = new Label
				{
					Text = Loc.GetString("mainmenu-emotes-slot-empty"),
					FontColorOverride = Color.FromHex((ReadOnlySpan<char>)"#888888", (Color?)null),
					HorizontalAlignment = (HAlignment)2
				};
				((Control)val9).AddChild((Control)(object)val10);
				pubgEmoteSlot.SetContent((Control)(object)val9);
			}
		}
	}

	private void ApplyEmotes()
	{
		_equippedEmotes = new List<string>(_temporaryEquippedEmotes);
		Dictionary<string, string> dictionary = new Dictionary<string, string>(_currentOutfit);
		dictionary["equippedEmotes"] = string.Join(",", _equippedEmotes);
		this.OnApply?.Invoke(dictionary);
		_currentOutfit = dictionary;
	}

	private void ShowContextMenu(string itemId, string slotName, bool isRecipe, Control parent)
	{
		CloseCurrentPopup();
		PanelContainer content = SkinContextMenuBuilder.BuildItemContextMenu(_prototypeManager, itemId, isRecipe, _playerScrap, delegate
		{
			SkinCraftMessage skinCraftMessage = new SkinCraftMessage(itemId);
			_entityManager.RaisePredictiveEvent<SkinCraftMessage>(skinCraftMessage);
			CloseCurrentPopup();
		}, delegate
		{
			SkinSellMessage skinSellMessage = new SkinSellMessage(itemId);
			_entityManager.RaisePredictiveEvent<SkinSellMessage>(skinSellMessage);
			CloseCurrentPopup();
		}, delegate
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			_temporaryOutfit[slotName] = itemId;
			UpdatePreview();
			Control obj = ((IEnumerable<Control>)((Control)_mainContent).Children).ElementAt(1);
			Control obj2 = ((IEnumerable<Control>)((Control)(BoxContainer)obj).Children).ElementAt(0);
			Control obj3 = ((obj2 is BoxContainer) ? obj2 : null);
			Control obj4 = ((IEnumerable<Control>)obj3.Children).ElementAt(0);
			Label categoryLabel = (Label)(object)((obj4 is Label) ? obj4 : null);
			Control obj5 = ((IEnumerable<Control>)obj3.Children).ElementAt(1);
			Label statsLabel = (Label)(object)((obj5 is Label) ? obj5 : null);
			Control obj6 = ((IEnumerable<Control>)((Control)(BoxContainer)obj).Children).ElementAt(2);
			Control obj7 = ((IEnumerable<Control>)((obj6 is ScrollContainer) ? obj6 : null).Children).ElementAt(0);
			BoxContainer itemsGrid = (BoxContainer)(object)((obj7 is BoxContainer) ? obj7 : null);
			ShowCategory(_currentCategory, categoryLabel, statsLabel, itemsGrid);
			CloseCurrentPopup();
		}, slotName != "ghost", delegate
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			_temporaryOutfit[slotName] = string.Empty;
			UpdatePreview();
			Control obj = ((IEnumerable<Control>)((Control)_mainContent).Children).ElementAt(1);
			Control obj2 = ((IEnumerable<Control>)((Control)(BoxContainer)obj).Children).ElementAt(0);
			Control obj3 = ((obj2 is BoxContainer) ? obj2 : null);
			Control obj4 = ((IEnumerable<Control>)obj3.Children).ElementAt(0);
			Label categoryLabel = (Label)(object)((obj4 is Label) ? obj4 : null);
			Control obj5 = ((IEnumerable<Control>)obj3.Children).ElementAt(1);
			Label statsLabel = (Label)(object)((obj5 is Label) ? obj5 : null);
			Control obj6 = ((IEnumerable<Control>)((Control)(BoxContainer)obj).Children).ElementAt(2);
			Control obj7 = ((IEnumerable<Control>)((obj6 is ScrollContainer) ? obj6 : null).Children).ElementAt(0);
			BoxContainer itemsGrid = (BoxContainer)(object)((obj7 is BoxContainer) ? obj7 : null);
			ShowCategory(_currentCategory, categoryLabel, statsLabel, itemsGrid);
			CloseCurrentPopup();
		});
		_currentContextPopup = PopupHelper.OpenContextPopup(_uiManager, parent, (Control)(object)content, new Vector2(150f, 80f));
	}

	private void ShowShopContextMenu(string itemId, SkinShopItemInfo shopItem, Control parent)
	{
		CloseCurrentPopup();
		SpriteSystem spriteSystem = _entityManager.System<SpriteSystem>();
		PanelContainer content = SkinContextMenuBuilder.BuildShopContextMenu(_prototypeManager, spriteSystem, _playerCoins, _playerPremiumCoins, shopItem, delegate(string offerId, string currency, int amount, int? durationDays)
		{
			SkinBuyMessage skinBuyMessage = new SkinBuyMessage(itemId, offerId);
			_entityManager.RaisePredictiveEvent<SkinBuyMessage>(skinBuyMessage);
			CloseCurrentPopup();
		});
		_currentContextPopup = PopupHelper.OpenContextPopup(_uiManager, parent, (Control)(object)content, new Vector2(260f, 140f));
	}

	private void CloseCurrentPopup()
	{
		if (_currentContextPopup != null)
		{
			_currentContextPopup.Close();
			_currentContextPopup = null;
		}
	}

	private FormattedMessage BuildSkinTooltipMessage(string name, SkinRarity rarity, string? description, DateTime? expiresAt)
	{
		string text = (string.IsNullOrWhiteSpace(description) ? Loc.GetString("mainmenu-skin-tooltip-no-description") : description);
		string rarityText = GetRarityText(rarity);
		string rarityColorHex = GetRarityColorHex(rarity);
		string item = string.Empty;
		if (expiresAt.HasValue)
		{
			string item2 = expiresAt.Value.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
			string text2 = Loc.GetString("mainmenu-skin-tooltip-expires-at", new(string, object)[1] { ("time", item2) });
			item = "\n" + FormattedMessage.EscapeText(text2);
		}
		string text3 = default(string);
		return FormattedMessage.FromMarkupPermissive(Loc.GetString("mainmenu-skin-tooltip", new(string, object)[5]
		{
			("name", "[bold]" + FormattedMessage.EscapeText(name) + "[/bold]"),
			("rarityLabel", FormattedMessage.EscapeText(Loc.GetString("mainmenu-skin-tooltip-rarity-label"))),
			("rarity", $"[color={rarityColorHex}]{FormattedMessage.EscapeText(rarityText)}[/color]"),
			("description", FormattedMessage.EscapeText(text)),
			("expiresLine", item)
		}), ref text3);
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

	private string GetRarityColorHex(SkinRarity rarity)
	{
		return rarity switch
		{
			SkinRarity.Unique => "#00E5FF", 
			SkinRarity.Legendary => "#FFD700", 
			SkinRarity.Epic => "#FF9800", 
			SkinRarity.Rare => "#9C27B0", 
			_ => "#9E9E9E", 
		};
	}

	private Control CreateSkinTooltip(FormattedMessage message)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_0012: Expected O, but got Unknown
		Tooltip val = new Tooltip();
		val.SetMessage(new FormattedMessage(message));
		return (Control)val;
	}

	public void Cleanup()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		CloseCurrentPopup();
		if (_previewEntity.HasValue)
		{
			_entityManager.DeleteEntity((EntityUid?)_previewEntity.Value);
			_previewEntity = null;
		}
		if (_shopPreviewEntity.HasValue)
		{
			_entityManager.DeleteEntity((EntityUid?)_shopPreviewEntity.Value);
			_shopPreviewEntity = null;
		}
		if (_shopSpecialPreviewEntity.HasValue)
		{
			_entityManager.DeleteEntity((EntityUid?)_shopSpecialPreviewEntity.Value);
			_shopSpecialPreviewEntity = null;
		}
		_shopView = null;
	}
}
