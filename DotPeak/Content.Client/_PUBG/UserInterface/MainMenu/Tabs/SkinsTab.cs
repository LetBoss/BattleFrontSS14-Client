// Decompiled with JetBrains decompiler
// Type: Content.Client._PUBG.UserInterface.MainMenu.Tabs.SkinsTab
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

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
using Content.Shared.Humanoid;
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client._PUBG.UserInterface.MainMenu.Tabs;

public sealed class SkinsTab : BoxContainer
{
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
    IoCManager.InjectDependencies<SkinsTab>(this);
    this.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) this).HorizontalExpand = true;
    ((Control) this).VerticalExpand = true;
    BoxContainer boxContainer = new BoxContainer();
    boxContainer.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer).HorizontalExpand = true;
    ((Control) boxContainer).VerticalExpand = true;
    this._mainContent = boxContainer;
    ((Control) this).AddChild((Control) this._mainContent);
    this.CreatePreviewEntities();
  }

  private void CreatePreviewEntities()
  {
    this._previewEntity = new EntityUid?(this.SpawnProfilePreviewEntity());
    this._shopPreviewEntity = new EntityUid?(this.SpawnProfilePreviewEntity());
  }

  private EntityUid SpawnProfilePreviewEntity()
  {
    if (!(this._preferencesManager.Preferences?.SelectedCharacter is HumanoidCharacterProfile selectedCharacter))
      return this._entityManager.SpawnEntity("MobHumanDummy", MapCoordinates.Nullspace, (ComponentRegistry) null);
    EntityUid uid = this._entityManager.SpawnEntity(EntProtoId.op_Implicit(this._prototypeManager.Index<SpeciesPrototype>(selectedCharacter.Species).DollPrototype), MapCoordinates.Nullspace, (ComponentRegistry) null);
    this._entityManager.System<HumanoidAppearanceSystem>().LoadProfile(uid, selectedCharacter, (HumanoidAppearanceComponent) null);
    return uid;
  }

  public void LoadSubcategory(SkinsSubcategory subcategory)
  {
    this._currentSubcategory = subcategory;
    ((Control) this._mainContent).RemoveAllChildren();
    if (subcategory != SkinsSubcategory.Shop)
    {
      this._shopView = (PubgShopView) null;
      this.ResetShopPreviewToHumanoid();
    }
    switch (subcategory)
    {
      case SkinsSubcategory.MySkins:
        this.LoadMySkinsView();
        break;
      case SkinsSubcategory.Shop:
        this.LoadShopView();
        break;
      case SkinsSubcategory.Emotes:
        this.LoadEmotesView();
        break;
      case SkinsSubcategory.Traits:
        this.LoadTraitsView();
        break;
      case SkinsSubcategory.Banners:
        this.LoadBannersView();
        break;
    }
  }

  public void UpdateData(
    Dictionary<string, List<string>> allItems,
    Dictionary<string, List<string>> unlockedItems,
    Dictionary<string, DateTime?> itemExpiresAt,
    Dictionary<string, int> recipePrices,
    List<SkinShopItemInfo> shopItems,
    Dictionary<string, string> currentOutfit,
    int playerCoins,
    int playerScrap,
    int playerPremiumCoins,
    List<string> allEmotes,
    List<string> unlockedEmotes,
    List<string> equippedEmotes,
    int maxEmoteSlots,
    int totalCaseDropSkins,
    int unlockedCaseDropSkins,
    int totalUniqueSkins,
    int totalEmotes,
    int availableEmotes)
  {
    this._allItems = allItems;
    this._unlockedItems = unlockedItems;
    this._itemExpiresAt = itemExpiresAt;
    this._recipePrices = recipePrices;
    this._shopItems = shopItems;
    this._currentOutfit = currentOutfit;
    this._temporaryOutfit = new Dictionary<string, string>((IDictionary<string, string>) currentOutfit);
    this._playerCoins = playerCoins;
    this._playerScrap = playerScrap;
    this._playerPremiumCoins = playerPremiumCoins;
    this._allEmotes = allEmotes;
    this._unlockedEmotes = unlockedEmotes;
    this._equippedEmotes = equippedEmotes;
    this._temporaryEquippedEmotes = new List<string>((IEnumerable<string>) equippedEmotes);
    this._maxEmoteSlots = maxEmoteSlots;
    this._totalCaseDropSkins = totalCaseDropSkins;
    this._unlockedCaseDropSkins = unlockedCaseDropSkins;
    this._totalUniqueSkins = totalUniqueSkins;
    this._totalEmotes = totalEmotes;
    this._availableEmotes = availableEmotes;
    string str;
    this._selectedFirstKillBanner = !currentOutfit.TryGetValue("firstKillBanner", out str) || !this._prototypeManager.HasIndex<PubgFirstKillBannerPrototype>(str) ? "PubgFirstKillBannerTest" : str;
    this.LoadSubcategory(this._currentSubcategory);
    this.ApplyHeroCaseUiState();
  }

  public void UpdateBalance(int coins, int scrap, int premiumCoins)
  {
    this._playerCoins = coins;
    this._playerScrap = scrap;
    this._playerPremiumCoins = premiumCoins;
    this._shopView?.UpdateBalance(coins, premiumCoins);
    this.ApplyHeroCaseUiState();
  }

  public void SetHeroCasePending(bool pending)
  {
    this._heroCasePending = pending;
    this.ApplyHeroCaseUiState();
  }

  public void SetHeroCaseError(string? errorCode)
  {
    this._heroCaseErrorCode = errorCode;
    this.ApplyHeroCaseUiState();
  }

  private void ApplyHeroCaseUiState()
  {
    this._shopView?.SetHeroCaseState(this._heroCasePending, this._heroCaseErrorCode);
  }

  public void UpdateShopItems(List<SkinShopItemInfo> shopItems)
  {
    this._shopItems = shopItems;
    if (this._currentSubcategory != SkinsSubcategory.Shop)
      return;
    if (this._shopView != null)
    {
      this._shopView.UpdateState(this._shopItems, this._itemExpiresAt, this._playerCoins, this._playerPremiumCoins);
      this.ApplyHeroCaseUiState();
    }
    else
      this.LoadSubcategory(this._currentSubcategory);
  }

  private void LoadMySkinsView()
  {
    this._categoryButtons.Clear();
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).MinWidth = 200f;
    ((Control) boxContainer1).MaxWidth = 200f;
    ((Control) boxContainer1).HorizontalExpand = false;
    ((Control) boxContainer1).Margin = new Thickness(15f);
    BoxContainer boxContainer2 = boxContainer1;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-category-label");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    ((Control) boxContainer2).AddChild((Control) label2);
    List<(string, string, SkinCategory)> list = ((IEnumerable<(string, string, SkinCategory)>) new (string, string, SkinCategory)[6]
    {
      ("mainmenu-category-head", "mainmenu-category-icon-head", SkinCategory.Head),
      ("mainmenu-category-jumpsuit", "mainmenu-category-icon-jumpsuit", SkinCategory.Jumpsuit),
      ("mainmenu-category-outer", "mainmenu-category-icon-outer", SkinCategory.OuterClothing),
      ("mainmenu-category-neck", "mainmenu-category-icon-neck", SkinCategory.Neck),
      ("mainmenu-category-shoes", "mainmenu-category-icon-shoes", SkinCategory.Shoes),
      ("mainmenu-category-ghost", "mainmenu-category-icon-ghost", SkinCategory.Ghost)
    }).Where<(string, string, SkinCategory)>((Func<(string, string, SkinCategory), bool>) (entry => this.AllowedCategories == null || this.AllowedCategories.Contains(entry.Category))).ToList<(string, string, SkinCategory)>();
    if (list.Count > 0 && list.All<(string, string, SkinCategory)>((Func<(string, string, SkinCategory), bool>) (entry => entry.Category != this._currentCategory)))
      this._currentCategory = list[0].Item3;
    foreach ((string, string, SkinCategory) tuple in list)
      ((Control) boxContainer2).AddChild((Control) this.CreateCategoryButton(Loc.GetString(tuple.Item1), Loc.GetString(tuple.Item2), tuple.Item3));
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalExpand = true;
    ((Control) boxContainer3).Margin = new Thickness(0.0f, 15f, 15f, 15f);
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-category-select");
    ((Control) label3).HorizontalExpand = true;
    ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label categoryLabel = label3;
    ((Control) categoryLabel).SetOnlyStyleClass("LabelHeading");
    Label label4 = new Label();
    label4.Text = Loc.GetString("mainmenu-collection-stats", new (string, object)[3]
    {
      ("unlockedCase", (object) this._unlockedCaseDropSkins),
      ("totalCase", (object) this._totalCaseDropSkins),
      ("totalUnique", (object) this._totalUniqueSkins)
    });
    label4.FontColorOverride = new Color?(Color.Gray);
    ((Control) label4).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) label4).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label statsLabel = label4;
    ((Control) statsLabel).Visible = this.ShowCollectionProgress;
    ((Control) boxContainer5).AddChild((Control) categoryLabel);
    ((Control) boxContainer5).AddChild((Control) statsLabel);
    BoxContainer boxContainer6 = new BoxContainer();
    boxContainer6.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer6).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    ((Control) boxContainer6).Visible = this.ShowCollectionProgress;
    BoxContainer boxContainer7 = boxContainer6;
    ProgressBar progressBar1 = new ProgressBar();
    ((Control) progressBar1).MinHeight = 20f;
    ((Control) progressBar1).HorizontalExpand = true;
    ((Range) progressBar1).MaxValue = this._totalCaseDropSkins > 0 ? (float) this._totalCaseDropSkins : 1f;
    ((Range) progressBar1).Value = (float) this._unlockedCaseDropSkins;
    ProgressBar progressBar2 = progressBar1;
    float num = this._totalCaseDropSkins > 0 ? (float) ((double) this._unlockedCaseDropSkins / (double) this._totalCaseDropSkins * 100.0) : 0.0f;
    Label label5 = new Label();
    label5.Text = Loc.GetString("mainmenu-collection-progress", new (string, object)[3]
    {
      ("percent", (object) $"{num:F1}"),
      ("unlockedCase", (object) this._unlockedCaseDropSkins),
      ("totalCase", (object) this._totalCaseDropSkins)
    });
    label5.FontColorOverride = new Color?((double) num >= 100.0 ? Color.Gold : Color.LightGreen);
    ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label5).Margin = new Thickness(0.0f, 5f, 0.0f, 0.0f);
    Label label6 = label5;
    ((Control) boxContainer7).AddChild((Control) progressBar2);
    ((Control) boxContainer7).AddChild((Control) label6);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer itemsGrid = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    ((Control) scrollContainer2).AddChild((Control) itemsGrid);
    ((Control) boxContainer4).AddChild((Control) boxContainer5);
    ((Control) boxContainer4).AddChild((Control) boxContainer7);
    ((Control) boxContainer4).AddChild((Control) scrollContainer2);
    CharacterPreviewPanel previewPanel = this.CreatePreviewPanel();
    ((Control) this._mainContent).AddChild((Control) boxContainer2);
    ((Control) this._mainContent).AddChild((Control) boxContainer4);
    ((Control) this._mainContent).AddChild((Control) previewPanel);
    this.ShowCategory(this._currentCategory, categoryLabel, statsLabel, itemsGrid);
  }

  private PubgCategoryButton CreateCategoryButton(string text, string icon, SkinCategory category)
  {
    PubgCategoryButton pubgCategoryButton = new PubgCategoryButton();
    pubgCategoryButton.Text = text;
    pubgCategoryButton.Icon = icon;
    pubgCategoryButton.Margin = new Thickness(0.0f, 0.0f, 0.0f, 8f);
    PubgCategoryButton button = pubgCategoryButton;
    button.OnPressed += (Action) (() =>
    {
      foreach (PubgCategoryButton categoryButton in this._categoryButtons)
        categoryButton.IsActive = false;
      button.IsActive = true;
      Control control;
      BoxContainer boxContainer = ((IEnumerable<Control>) (control = ((IEnumerable<Control>) ((Control) this._mainContent).Children).ElementAt<Control>(1)).Children).ElementAt<Control>(0) as BoxContainer;
      this.ShowCategory(category, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(0) as Label, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(1) as Label, ((IEnumerable<Control>) ((Control) (((IEnumerable<Control>) control.Children).ElementAt<Control>(2) as ScrollContainer)).Children).ElementAt<Control>(0) as BoxContainer);
    });
    this._categoryButtons.Add(button);
    if (category == this._currentCategory)
      button.IsActive = true;
    return button;
  }

  private CharacterPreviewPanel CreatePreviewPanel()
  {
    CharacterPreviewPanel characterPreviewPanel = new CharacterPreviewPanel();
    ((Control) characterPreviewPanel).Margin = new Thickness(0.0f, 15f, 15f, 15f);
    ((Control) characterPreviewPanel).VerticalAlignment = (Control.VAlignment) 1;
    this._previewPanel = characterPreviewPanel;
    if (this._previewEntity.HasValue)
      this._previewPanel.SetPreviewEntity(this._previewEntity.Value);
    this._previewPanel.OnApplyPressed += new Action(this.ApplyChanges);
    return this._previewPanel;
  }

  private void ApplyChanges()
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._playerManager).LocalEntity;
    if (localEntity.HasValue && this._entityManager.HasComponent<PubgPlayerComponent>(localEntity.GetValueOrDefault()))
      return;
    Dictionary<string, string> dictionary = new Dictionary<string, string>();
    foreach ((string key, string str1) in this._temporaryOutfit)
    {
      List<string> stringList;
      string str2;
      dictionary[key] = !(key == "equippedEmotes") ? (!string.IsNullOrEmpty(str1) ? (!this._unlockedItems.TryGetValue(key, out stringList) || !stringList.Contains(str1) ? (this._currentOutfit.TryGetValue(key, out str2) ? str2 : "") : str1) : str1) : str1;
    }
    Action<Dictionary<string, string>> onApply = this.OnApply;
    if (onApply != null)
      onApply(new Dictionary<string, string>((IDictionary<string, string>) dictionary));
    this._currentOutfit = new Dictionary<string, string>((IDictionary<string, string>) dictionary);
    this._temporaryOutfit = new Dictionary<string, string>((IDictionary<string, string>) dictionary);
    this.LoadSubcategory(this._currentSubcategory);
  }

  private void ShowCategory(
    SkinCategory category,
    Label categoryLabel,
    Label statsLabel,
    BoxContainer itemsGrid)
  {
    this._currentCategory = category;
    ((Control) itemsGrid).RemoveAllChildren();
    string str1;
    switch (category)
    {
      case SkinCategory.Jumpsuit:
        str1 = Loc.GetString("mainmenu-category-jumpsuit");
        break;
      case SkinCategory.OuterClothing:
        str1 = Loc.GetString("mainmenu-category-outer");
        break;
      case SkinCategory.Shoes:
        str1 = Loc.GetString("mainmenu-category-shoes");
        break;
      case SkinCategory.Neck:
        str1 = Loc.GetString("mainmenu-category-neck");
        break;
      case SkinCategory.Head:
        str1 = Loc.GetString("mainmenu-category-head");
        break;
      case SkinCategory.Ghost:
        str1 = Loc.GetString("mainmenu-category-ghost");
        break;
      default:
        str1 = Loc.GetString("mainmenu-category-unknown");
        break;
    }
    string str2 = str1;
    categoryLabel.Text = str2;
    statsLabel.Text = Loc.GetString("mainmenu-collection-stats", new (string, object)[3]
    {
      ("unlockedCase", (object) this._unlockedCaseDropSkins),
      ("totalCase", (object) this._totalCaseDropSkins),
      ("totalUnique", (object) this._totalUniqueSkins)
    });
    string key2;
    switch (category)
    {
      case SkinCategory.Jumpsuit:
        key2 = "jumpsuit";
        break;
      case SkinCategory.OuterClothing:
        key2 = "outerClothing";
        break;
      case SkinCategory.Shoes:
        key2 = "shoes";
        break;
      case SkinCategory.Neck:
        key2 = "neck";
        break;
      case SkinCategory.Head:
        key2 = "head";
        break;
      case SkinCategory.Ghost:
        key2 = "ghost";
        break;
      default:
        key2 = "";
        break;
    }
    string str3 = key2;
    string str4;
    string currentEquipped = this._temporaryOutfit.TryGetValue(str3, out str4) ? str4 : "";
    List<string> stringList1;
    List<string> stringList2 = this._unlockedItems.TryGetValue(str3, out stringList1) ? stringList1 : new List<string>();
    List<string> stringList3 = new List<string>();
    foreach ((key2, _) in this._recipePrices)
    {
      string str5 = key2;
      EntityPrototype entityPrototype;
      PubgSkinItemComponent skinItemComponent;
      if (this._prototypeManager.TryIndex<EntityPrototype>(str5, ref entityPrototype) && entityPrototype.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref skinItemComponent) && skinItemComponent.Category == str3)
        stringList3.Add(str5);
    }
    if (stringList2.Count > 0)
    {
      BoxContainer boxContainer1 = new BoxContainer();
      boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 10f);
      BoxContainer boxContainer2 = boxContainer1;
      Label label1 = new Label();
      label1.Text = Loc.GetString("mainmenu-items-available");
      ((Control) label1).Margin = new Thickness(5f, 0.0f, 0.0f, 5f);
      Label label2 = label1;
      ((Control) label2).SetOnlyStyleClass("LabelHeading");
      GridContainer grid = new GridContainer()
      {
        Columns = 4
      };
      foreach (string itemId in stringList2)
        this.AddItemButton(grid, itemId, str3, currentEquipped, false);
      ((Control) boxContainer2).AddChild((Control) label2);
      ((Control) boxContainer2).AddChild((Control) grid);
      ((Control) itemsGrid).AddChild((Control) boxContainer2);
    }
    if (stringList3.Count > 0)
    {
      BoxContainer boxContainer3 = new BoxContainer();
      boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
      ((Control) boxContainer3).Margin = new Thickness(0.0f, 10f, 0.0f, 0.0f);
      BoxContainer boxContainer4 = boxContainer3;
      Label label3 = new Label();
      label3.Text = Loc.GetString("mainmenu-items-recipes");
      ((Control) label3).Margin = new Thickness(5f, 0.0f, 0.0f, 5f);
      Label label4 = label3;
      ((Control) label4).SetOnlyStyleClass("LabelHeading");
      GridContainer grid = new GridContainer()
      {
        Columns = 4
      };
      foreach (string itemId in stringList3)
        this.AddItemButton(grid, itemId, str3, currentEquipped, true);
      ((Control) boxContainer4).AddChild((Control) label4);
      ((Control) boxContainer4).AddChild((Control) grid);
      ((Control) itemsGrid).AddChild((Control) boxContainer4);
    }
    this.UpdatePreview();
  }

  private void AddItemButton(
    GridContainer grid,
    string itemId,
    string slotName,
    string currentEquipped,
    bool isRecipe)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    SkinsTab.\u003C\u003Ec__DisplayClass76_0 cDisplayClass760 = new SkinsTab.\u003C\u003Ec__DisplayClass76_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.slotName = slotName;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.itemId = itemId;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.isRecipe = isRecipe;
    EntityPrototype entityPrototype;
    // ISSUE: reference to a compiler-generated field
    if (!this._prototypeManager.TryIndex<EntityPrototype>(cDisplayClass760.itemId, ref entityPrototype))
      return;
    PubgSkinItemComponent skinItemComponent1 = (PubgSkinItemComponent) null;
    PubgSkinItemComponent skinItemComponent2;
    if (entityPrototype.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref skinItemComponent2))
      skinItemComponent1 = skinItemComponent2;
    // ISSUE: reference to a compiler-generated field
    bool flag = cDisplayClass760.itemId == currentEquipped;
    PubgSkinItemCard pubgSkinItemCard = new PubgSkinItemCard();
    pubgSkinItemCard.ItemName = entityPrototype.Name;
    // ISSUE: reference to a compiler-generated field
    pubgSkinItemCard.ProtoId = cDisplayClass760.itemId;
    // ISSUE: reference to a compiler-generated field
    pubgSkinItemCard.IsOwned = !cDisplayClass760.isRecipe;
    // ISSUE: reference to a compiler-generated field
    pubgSkinItemCard.IsEquipped = flag && !cDisplayClass760.isRecipe;
    pubgSkinItemCard.Margin = new Thickness(3f);
    // ISSUE: reference to a compiler-generated field
    cDisplayClass760.itemCard = pubgSkinItemCard;
    if (skinItemComponent1 != null)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      SkinsTab.\u003C\u003Ec__DisplayClass76_1 cDisplayClass761 = new SkinsTab.\u003C\u003Ec__DisplayClass76_1()
      {
        CS\u0024\u003C\u003E8__locals1 = cDisplayClass760
      };
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      cDisplayClass761.CS\u0024\u003C\u003E8__locals1.itemCard.Rarity = skinItemComponent1.Rarity.ToString();
      DateTime? expiresAt;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this._itemExpiresAt.TryGetValue(cDisplayClass761.CS\u0024\u003C\u003E8__locals1.itemId, out expiresAt);
      // ISSUE: reference to a compiler-generated field
      cDisplayClass761.tooltipMessage = this.BuildSkinTooltipMessage(entityPrototype.Name, skinItemComponent1.Rarity, skinItemComponent1.Description, expiresAt);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      cDisplayClass761.CS\u0024\u003C\u003E8__locals1.itemCard.TooltipSupplier = new TooltipSupplier((object) cDisplayClass761, __methodptr(\u003CAddItemButton\u003Eb__2));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      cDisplayClass761.CS\u0024\u003C\u003E8__locals1.itemCard.TooltipDelay = new float?(0.1f);
    }
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    cDisplayClass760.itemCard.OnCardClicked += new Action<string>(cDisplayClass760.\u003CAddItemButton\u003Eb__0);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    cDisplayClass760.itemCard.OnCardRightClicked += new Action<string>(cDisplayClass760.\u003CAddItemButton\u003Eb__1);
    // ISSUE: reference to a compiler-generated field
    ((Control) grid).AddChild((Control) cDisplayClass760.itemCard);
  }

  private void UpdatePreview()
  {
    if (!this._previewEntity.HasValue)
      return;
    InventorySystem inventorySystem = this._entityManager.System<InventorySystem>();
    SlotDefinition[] slotDefinitions;
    if (!inventorySystem.TryGetSlots(this._previewEntity.Value, out slotDefinitions))
      return;
    foreach (SlotDefinition slotDefinition in ((IEnumerable<SlotDefinition>) slotDefinitions).ToList<SlotDefinition>())
    {
      EntityUid? removedItem;
      if (inventorySystem.TryUnequip(this._previewEntity.Value, slotDefinition.Name, out removedItem, true, true, reparent: false))
        this._entityManager.DeleteEntity(new EntityUid?(removedItem.Value));
    }
    foreach ((string str1, string str2) in this._temporaryOutfit)
    {
      if (!(str1 == "equippedEmotes") && !(str1 == "ghost") && !string.IsNullOrEmpty(str2) && this._prototypeManager.HasIndex<EntityPrototype>(str2))
      {
        EntityUid itemUid = this._entityManager.SpawnEntity(str2, MapCoordinates.Nullspace, (ComponentRegistry) null);
        inventorySystem.TryEquip(this._previewEntity.Value, itemUid, str1, true, true);
      }
    }
  }

  private void LoadShopView()
  {
    this._shopView = new PubgShopView(this.CreateShopPreviewPanel());
    this._shopView.OnItemSelected += new Action<string>(this.HandleShopItemSelected);
    this._shopView.OnItemRightClicked += new Action<string, SkinShopItemInfo, Control>(this.HandleShopItemRightClicked);
    this._shopView.OnBuyRequested += new Action<string, string>(this.HandleShopBuyRequested);
    this._shopView.OnHeroCasePressed += new Action<string>(this.HandleHeroCasePressed);
    this._shopView.UpdateState(this._shopItems, this._itemExpiresAt, this._playerCoins, this._playerPremiumCoins);
    this.ApplyHeroCaseUiState();
    ((Control) this._mainContent).AddChild((Control) this._shopView);
    this.UpdateShopPreview();
  }

  private CharacterPreviewPanel CreateShopPreviewPanel()
  {
    CharacterPreviewPanel characterPreviewPanel = new CharacterPreviewPanel();
    ((Control) characterPreviewPanel).Margin = new Thickness(0.0f, 15f, 15f, 15f);
    ((Control) characterPreviewPanel).VerticalAlignment = (Control.VAlignment) 1;
    this._shopPreviewPanel = characterPreviewPanel;
    if (this._shopPreviewEntity.HasValue)
      this._shopPreviewPanel.SetPreviewEntity(this._shopPreviewEntity.Value);
    this._shopPreviewPanel.SetApplyButtonVisible(false);
    this._shopPreviewPanel.SetPreviewScale(3.2f);
    return this._shopPreviewPanel;
  }

  private void HandleShopItemSelected(string itemId) => this.PreviewShopItem(itemId);

  private void HandleShopItemRightClicked(string itemId, SkinShopItemInfo shopItem, Control parent)
  {
    this.ShowShopContextMenu(itemId, shopItem, parent);
  }

  private void HandleShopBuyRequested(string itemId, string offerId)
  {
    this._entityManager.RaisePredictiveEvent<SkinBuyMessage>(new SkinBuyMessage(itemId, offerId));
    this.CloseCurrentPopup();
  }

  private void HandleHeroCasePressed(string caseId)
  {
    if (this._heroCasePending)
      return;
    this._heroCasePending = true;
    this._heroCaseErrorCode = (string) null;
    this.ApplyHeroCaseUiState();
    Action<string> caseOpenRequested = this.OnHeroCaseOpenRequested;
    if (caseOpenRequested != null)
      caseOpenRequested(caseId);
    this.CloseCurrentPopup();
  }

  private void PreviewShopItem(string itemId)
  {
    EntityPrototype entityPrototype;
    PubgSkinItemComponent skinComp;
    if (!this._shopPreviewEntity.HasValue || !this._prototypeManager.TryIndex<EntityPrototype>(itemId, ref entityPrototype) || !entityPrototype.TryGetComponent<PubgSkinItemComponent>("PubgSkinItem", ref skinComp))
      return;
    if (skinComp.Category == "emote")
    {
      this.ResetShopPreviewToHumanoid();
      this.UpdateShopPreview();
      this.PreviewEmote(itemId);
    }
    else if (skinComp.Category == "ghost")
    {
      if (this.TryPreviewGhostItem(itemId, skinComp))
        this.ClearEmotePreview();
      else
        this.UpdateShopPreview();
    }
    else
    {
      this.ResetShopPreviewToHumanoid();
      this.ClearEmotePreview();
      InventorySystem inventorySystem = this._entityManager.System<InventorySystem>();
      SlotDefinition[] slotDefinitions;
      if (!inventorySystem.TryGetSlots(this._shopPreviewEntity.Value, out slotDefinitions))
        return;
      foreach (SlotDefinition slotDefinition in ((IEnumerable<SlotDefinition>) slotDefinitions).ToList<SlotDefinition>())
      {
        EntityUid? removedItem;
        if (inventorySystem.TryUnequip(this._shopPreviewEntity.Value, slotDefinition.Name, out removedItem, true, true, reparent: false))
          this._entityManager.DeleteEntity(new EntityUid?(removedItem.Value));
      }
      foreach ((string str1, string str2) in this._currentOutfit)
      {
        if (!(str1 == "ghost") && !(str1 == skinComp.Category) && !string.IsNullOrEmpty(str2) && this._prototypeManager.HasIndex<EntityPrototype>(str2))
        {
          EntityUid itemUid = this._entityManager.SpawnEntity(str2, MapCoordinates.Nullspace, (ComponentRegistry) null);
          inventorySystem.TryEquip(this._shopPreviewEntity.Value, itemUid, str1, true, true);
        }
      }
      EntityUid itemUid1 = this._entityManager.SpawnEntity(itemId, MapCoordinates.Nullspace, (ComponentRegistry) null);
      inventorySystem.TryEquip(this._shopPreviewEntity.Value, itemUid1, skinComp.Category, true, true);
    }
  }

  private bool TryPreviewGhostItem(string itemId, PubgSkinItemComponent skinComp)
  {
    if (this._shopPreviewPanel == null)
      return false;
    this.ResetShopPreviewToHumanoid();
    string str = itemId;
    EntProtoId<GhostComponent>? ghostPrototype = skinComp.GhostPrototype;
    if (ghostPrototype.HasValue)
      str = ghostPrototype.GetValueOrDefault().ToString();
    if (!this._prototypeManager.HasIndex<EntityPrototype>(str))
      return false;
    this._shopSpecialPreviewEntity = new EntityUid?(this._entityManager.SpawnEntity(str, MapCoordinates.Nullspace, (ComponentRegistry) null));
    this._shopPreviewPanel.SetPreviewEntity(this._shopSpecialPreviewEntity.Value);
    return true;
  }

  private void ResetShopPreviewToHumanoid()
  {
    if (this._shopSpecialPreviewEntity.HasValue)
    {
      this._entityManager.DeleteEntity(new EntityUid?(this._shopSpecialPreviewEntity.Value));
      this._shopSpecialPreviewEntity = new EntityUid?();
    }
    if (!this._shopPreviewEntity.HasValue || this._shopPreviewPanel == null)
      return;
    this._shopPreviewPanel.SetPreviewEntity(this._shopPreviewEntity.Value);
  }

  private void UpdateShopPreview()
  {
    if (!this._shopPreviewEntity.HasValue)
      return;
    this.ResetShopPreviewToHumanoid();
    this.ClearEmotePreview();
    InventorySystem inventorySystem = this._entityManager.System<InventorySystem>();
    SlotDefinition[] slotDefinitions;
    if (!inventorySystem.TryGetSlots(this._shopPreviewEntity.Value, out slotDefinitions))
      return;
    foreach (SlotDefinition slotDefinition in ((IEnumerable<SlotDefinition>) slotDefinitions).ToList<SlotDefinition>())
    {
      EntityUid? removedItem;
      if (inventorySystem.TryUnequip(this._shopPreviewEntity.Value, slotDefinition.Name, out removedItem, true, true, reparent: false))
        this._entityManager.DeleteEntity(new EntityUid?(removedItem.Value));
    }
    foreach ((string str1, string str2) in this._currentOutfit)
    {
      if (!(str1 == "ghost") && !string.IsNullOrEmpty(str2) && this._prototypeManager.HasIndex<EntityPrototype>(str2))
      {
        EntityUid itemUid = this._entityManager.SpawnEntity(str2, MapCoordinates.Nullspace, (ComponentRegistry) null);
        inventorySystem.TryEquip(this._shopPreviewEntity.Value, itemUid, str1, true, true);
      }
    }
  }

  private void PreviewEmote(string emoteId)
  {
    EntityPrototype entityPrototype;
    PubgEmoteComponent pubgEmoteComponent;
    SpriteComponent spriteComponent;
    if (!this._shopPreviewEntity.HasValue || !this._prototypeManager.TryIndex<EntityPrototype>(emoteId, ref entityPrototype) || !entityPrototype.TryGetComponent<PubgEmoteComponent>("PubgEmote", ref pubgEmoteComponent) || !this._entityManager.TryGetComponent<SpriteComponent>(this._shopPreviewEntity.Value, ref spriteComponent))
      return;
    SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
    int num1;
    if (spriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), (Enum) SkinsTab.EmotePreviewLayer.Emote, ref num1, false))
      spriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), num1, true);
    if (string.IsNullOrEmpty(pubgEmoteComponent.RsiPath) || string.IsNullOrEmpty(pubgEmoteComponent.StateName))
      return;
    ResPath resPath;
    // ISSUE: explicit constructor call
    ((ResPath) ref resPath).\u002Ector(pubgEmoteComponent.RsiPath);
    int num2 = spriteSystem.AddLayer(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), (SpriteSpecifier) new SpriteSpecifier.Rsi(resPath, pubgEmoteComponent.StateName), new int?());
    spriteSystem.LayerMapSet(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), (Enum) SkinsTab.EmotePreviewLayer.Emote, num2);
    Box2 localBounds = spriteSystem.GetLocalBounds(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)));
    float num3 = (float) ((double) ((Box2) ref localBounds).Height / 2.0 + 5.0 / 16.0);
    float num4 = (float) ((double) num3 * ((double) pubgEmoteComponent.Scale - 1.0) * 0.20000000298023224);
    spriteSystem.LayerSetOffset(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), num2, new Vector2(0.0f, num3 + num4));
    spriteSystem.LayerSetScale(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), num2, new Vector2(pubgEmoteComponent.Scale, pubgEmoteComponent.Scale));
    spriteComponent.LayerSetShader(num2, "unshaded");
  }

  private void ClearEmotePreview()
  {
    SpriteComponent spriteComponent;
    if (!this._shopPreviewEntity.HasValue || !this._entityManager.TryGetComponent<SpriteComponent>(this._shopPreviewEntity.Value, ref spriteComponent))
      return;
    SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
    int num;
    if (!spriteSystem.LayerMapTryGet(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), (Enum) SkinsTab.EmotePreviewLayer.Emote, ref num, false))
      return;
    spriteSystem.RemoveLayer(Entity<SpriteComponent>.op_Implicit((this._shopPreviewEntity.Value, spriteComponent)), num, true);
  }

  private void LoadEmotesView()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer3).HorizontalExpand = true;
    ((Control) boxContainer3).Margin = new Thickness(15f);
    BoxContainer boxContainer4 = boxContainer3;
    BoxContainer boxContainer5 = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 0
    };
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-emotes-title");
    ((Control) label1).HorizontalExpand = true;
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-emotes-stats", new (string, object)[3]
    {
      ("available", (object) this._availableEmotes),
      ("unlocked", (object) this._availableEmotes),
      ("total", (object) this._totalEmotes)
    });
    label3.FontColorOverride = new Color?(Color.Gray);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 3;
    ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label4 = label3;
    ((Control) boxContainer5).AddChild((Control) label2);
    ((Control) boxContainer5).AddChild((Control) label4);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    BoxContainer emotesGrid = new BoxContainer()
    {
      Orientation = (BoxContainer.LayoutOrientation) 1
    };
    ((Control) scrollContainer2).AddChild((Control) emotesGrid);
    ((Control) boxContainer4).AddChild((Control) boxContainer5);
    ((Control) boxContainer4).AddChild((Control) scrollContainer2);
    BoxContainer emoteSlotsPanel = this.CreateEmoteSlotsPanel();
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ((Control) boxContainer2).AddChild((Control) emoteSlotsPanel);
    ((Control) this._mainContent).AddChild((Control) boxContainer2);
    this._temporaryEquippedEmotes = new List<string>((IEnumerable<string>) this._equippedEmotes);
    this.LoadAvailableEmotes(emotesGrid);
  }

  private void LoadTraitsView()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).HorizontalExpand = true;
    ((Control) boxContainer1).VerticalExpand = true;
    ((Control) boxContainer1).Margin = new Thickness(16f);
    BoxContainer boxContainer2 = boxContainer1;
    BoxContainer boxContainer3 = new BoxContainer();
    boxContainer3.Orientation = (BoxContainer.LayoutOrientation) 0;
    ((Control) boxContainer3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 12f);
    BoxContainer boxContainer4 = boxContainer3;
    this._traitsHeadTab = new PubgSubcategoryTab()
    {
      Text = Loc.GetString("mainmenu-traits-head")
    };
    this._traitsHeadTab.OnPressed += (Action) (() => this.ShowTraitCategory("head"));
    PubgSubcategoryTab pubgSubcategoryTab = new PubgSubcategoryTab();
    pubgSubcategoryTab.Text = Loc.GetString("mainmenu-traits-tail");
    pubgSubcategoryTab.Margin = new Thickness(10f, 0.0f, 0.0f, 0.0f);
    this._traitsTailTab = pubgSubcategoryTab;
    this._traitsTailTab.OnPressed += (Action) (() => this.ShowTraitCategory("tail"));
    ((Control) boxContainer4).AddChild((Control) this._traitsHeadTab);
    ((Control) boxContainer4).AddChild((Control) this._traitsTailTab);
    ((Control) boxContainer2).AddChild((Control) boxContainer4);
    ScrollContainer scrollContainer1 = new ScrollContainer();
    ((Control) scrollContainer1).HorizontalExpand = true;
    ((Control) scrollContainer1).VerticalExpand = true;
    ScrollContainer scrollContainer2 = scrollContainer1;
    GridContainer gridContainer = new GridContainer();
    gridContainer.Columns = 4;
    ((Control) gridContainer).Margin = new Thickness(6f);
    this._traitsGrid = gridContainer;
    Label label = new Label();
    label.Text = Loc.GetString("mainmenu-traits-empty");
    label.FontColorOverride = new Color?(Color.Gray);
    ((Control) label).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label).VerticalAlignment = (Control.VAlignment) 2;
    ((Control) label).Visible = false;
    this._traitsEmptyLabel = label;
    BoxContainer boxContainer5 = new BoxContainer();
    boxContainer5.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer5).HorizontalExpand = true;
    ((Control) boxContainer5).VerticalExpand = true;
    BoxContainer boxContainer6 = boxContainer5;
    ((Control) boxContainer6).AddChild((Control) this._traitsGrid);
    ((Control) boxContainer6).AddChild((Control) this._traitsEmptyLabel);
    ((Control) scrollContainer2).AddChild((Control) boxContainer6);
    ((Control) boxContainer2).AddChild((Control) scrollContainer2);
    this._traitsContent = boxContainer2;
    ((Control) this._mainContent).AddChild((Control) boxContainer2);
    this.ShowTraitCategory(this._currentTraitKey);
  }

  private void LoadBannersView()
  {
    // ISSUE: unable to decompile the method.
  }

  private Control BuildFirstKillBannerPreview(PubgFirstKillBannerPrototype prototype)
  {
    int num1 = Math.Max(1, prototype.Width);
    int num2 = Math.Max(1, prototype.Height);
    PanelContainer panelContainer = new PanelContainer();
    ((Control) panelContainer).MinWidth = (float) num1;
    ((Control) panelContainer).MinHeight = (float) num2;
    ((Control) panelContainer).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) panelContainer).VerticalAlignment = (Control.VAlignment) 1;
    panelContainer.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = Color.FromHex((ReadOnlySpan<char>) "#00000000", new Color?()),
      BorderColor = Color.FromHex((ReadOnlySpan<char>) "#00000000", new Color?()),
      BorderThickness = new Thickness(0.0f)
    };
    LayoutContainer layoutContainer1 = new LayoutContainer();
    ((Control) layoutContainer1).MinWidth = (float) num1;
    ((Control) layoutContainer1).MinHeight = (float) num2;
    LayoutContainer layoutContainer2 = layoutContainer1;
    AnimatedTextureRect animatedTextureRect1 = new AnimatedTextureRect();
    ((Control) animatedTextureRect1).HorizontalExpand = true;
    ((Control) animatedTextureRect1).VerticalExpand = true;
    AnimatedTextureRect animatedTextureRect2 = animatedTextureRect1;
    animatedTextureRect2.SetFromSpriteSpecifier((SpriteSpecifier) new SpriteSpecifier.Rsi(prototype.BackgroundRsi, prototype.BackgroundState));
    animatedTextureRect2.DisplayRect.Stretch = (TextureRect.StretchMode) 7;
    LayoutContainer.SetAnchorAndMarginPreset((Control) animatedTextureRect2, (LayoutContainer.LayoutPreset) 15, (LayoutContainer.LayoutPresetMode) 0, 0);
    ((Control) layoutContainer2).AddChild((Control) animatedTextureRect2);
    ((Control) panelContainer).AddChild((Control) layoutContainer2);
    return (Control) panelContainer;
  }

  private void ShowTraitCategory(string traitKey)
  {
    this._currentTraitKey = traitKey;
    if (this._traitsHeadTab != null)
      this._traitsHeadTab.IsActive = traitKey == "head";
    if (this._traitsTailTab != null)
      this._traitsTailTab.IsActive = traitKey == "tail";
    this.PopulateTraitsGrid(traitKey);
  }

  private void PopulateTraitsGrid(string traitKey)
  {
    if (this._traitsGrid == null || this._traitsEmptyLabel == null)
      return;
    ((Control) this._traitsGrid).RemoveAllChildren();
    List<string> traitItemsByKey = this.GetTraitItemsByKey(traitKey);
    List<string> collection;
    HashSet<string> stringSet = this._unlockedItems.TryGetValue("trait", out collection) ? new HashSet<string>((IEnumerable<string>) collection) : new HashSet<string>();
    if (traitItemsByKey.Count == 0)
    {
      ((Control) this._traitsEmptyLabel).Visible = true;
    }
    else
    {
      ((Control) this._traitsEmptyLabel).Visible = false;
      foreach (string str in (IEnumerable<string>) traitItemsByKey.OrderBy<string, string>((Func<string, string>) (id => id)))
      {
        // ISSUE: object of a compiler-generated type is created
        // ISSUE: variable of a compiler-generated type
        SkinsTab.\u003C\u003Ec__DisplayClass96_0 cDisplayClass960 = new SkinsTab.\u003C\u003Ec__DisplayClass96_0();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass960.\u003C\u003E4__this = this;
        EntityPrototype entityPrototype;
        PubgSkinItemComponent skinItemComponent;
        if (this._prototypeManager.TryIndex<EntityPrototype>(str, ref entityPrototype) && entityPrototype.TryGetComponent<PubgSkinItemComponent>(ref skinItemComponent, this._componentFactory))
        {
          PubgSkinItemCard pubgSkinItemCard1 = new PubgSkinItemCard();
          pubgSkinItemCard1.ItemName = entityPrototype.Name;
          pubgSkinItemCard1.ProtoId = entityPrototype.ID;
          pubgSkinItemCard1.IsOwned = stringSet.Contains(str);
          pubgSkinItemCard1.IsEquipped = false;
          pubgSkinItemCard1.Rarity = skinItemComponent.Rarity.ToString().ToLowerInvariant();
          pubgSkinItemCard1.Margin = new Thickness(4f);
          PubgSkinItemCard pubgSkinItemCard2 = pubgSkinItemCard1;
          // ISSUE: reference to a compiler-generated field
          cDisplayClass960.tooltipMessage = this.BuildSkinTooltipMessage(entityPrototype.Name, skinItemComponent.Rarity, skinItemComponent.Description, new DateTime?());
          // ISSUE: method pointer
          pubgSkinItemCard2.TooltipSupplier = new TooltipSupplier((object) cDisplayClass960, __methodptr(\u003CPopulateTraitsGrid\u003Eb__1));
          ((Control) this._traitsGrid).AddChild((Control) pubgSkinItemCard2);
        }
      }
    }
  }

  private List<string> GetTraitItemsByKey(string traitKey)
  {
    List<string> stringList = new List<string>();
    List<string> collection1;
    if (this._unlockedItems.TryGetValue("trait", out collection1))
    {
      stringList.AddRange((IEnumerable<string>) collection1);
    }
    else
    {
      List<string> collection2;
      if (this._allItems.TryGetValue("trait", out collection2))
        stringList.AddRange((IEnumerable<string>) collection2);
    }
    List<string> source = new List<string>();
    foreach (string str in stringList)
    {
      EntityPrototype entityPrototype;
      PubgSkinItemComponent skinItemComponent;
      PubgMarkingItemComponent markingItemComponent;
      if (this._prototypeManager.TryIndex<EntityPrototype>(str, ref entityPrototype) && entityPrototype.TryGetComponent<PubgSkinItemComponent>(ref skinItemComponent, this._componentFactory) && string.Equals(skinItemComponent.Category, "trait", StringComparison.OrdinalIgnoreCase) && entityPrototype.TryGetComponent<PubgMarkingItemComponent>(ref markingItemComponent, this._componentFactory) && string.Equals(markingItemComponent.TraitCategory, traitKey, StringComparison.OrdinalIgnoreCase))
        source.Add(str);
    }
    return source.Distinct<string>().ToList<string>();
  }

  private BoxContainer CreateEmoteSlotsPanel()
  {
    BoxContainer boxContainer1 = new BoxContainer();
    boxContainer1.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer1).MinWidth = 600f;
    ((Control) boxContainer1).Margin = new Thickness(0.0f, 15f, 15f, 15f);
    ((Control) boxContainer1).VerticalAlignment = (Control.VAlignment) 1;
    BoxContainer emoteSlotsPanel = boxContainer1;
    Label label1 = new Label();
    label1.Text = Loc.GetString("mainmenu-emotes-slots-title");
    ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label1).Margin = new Thickness(0.0f, 0.0f, 0.0f, 15f);
    Label label2 = label1;
    ((Control) label2).SetOnlyStyleClass("LabelHeading");
    Label label3 = new Label();
    label3.Text = Loc.GetString("mainmenu-emotes-slots-hint");
    label3.FontColorOverride = new Color?(Color.Gray);
    ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
    ((Control) label3).Margin = new Thickness(0.0f, 0.0f, 0.0f, 20f);
    Label label4 = label3;
    BoxContainer boxContainer2 = new BoxContainer();
    boxContainer2.Orientation = (BoxContainer.LayoutOrientation) 1;
    ((Control) boxContainer2).HorizontalAlignment = (Control.HAlignment) 2;
    BoxContainer boxContainer3 = boxContainer2;
    GridContainer gridContainer1 = new GridContainer();
    gridContainer1.Columns = 2;
    ((Control) gridContainer1).HorizontalAlignment = (Control.HAlignment) 2;
    GridContainer gridContainer2 = gridContainer1;
    for (int index = 0; index < 6; ++index)
    {
      PubgEmoteSlot pubgEmoteSlot1 = new PubgEmoteSlot();
      pubgEmoteSlot1.SlotNumber = index + 1;
      pubgEmoteSlot1.Margin = new Thickness(5f);
      PubgEmoteSlot pubgEmoteSlot2 = pubgEmoteSlot1;
      this._emoteSlots[index] = pubgEmoteSlot2;
      ((Control) gridContainer2).AddChild((Control) pubgEmoteSlot2);
    }
    ((Control) boxContainer3).AddChild((Control) gridContainer2);
    this.UpdateEmoteSlots();
    Button button1 = new Button();
    button1.Text = Loc.GetString("mainmenu-emotes-apply");
    ((Control) button1).MinHeight = 45f;
    ((Control) button1).Margin = new Thickness(0.0f, 25f, 0.0f, 0.0f);
    Button button2 = button1;
    ((Control) button2).StyleClasses.Add("ButtonColorGreen");
    ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ => this.ApplyEmotes());
    ((Control) emoteSlotsPanel).AddChild((Control) label2);
    ((Control) emoteSlotsPanel).AddChild((Control) label4);
    ((Control) emoteSlotsPanel).AddChild((Control) boxContainer3);
    ((Control) emoteSlotsPanel).AddChild((Control) button2);
    return emoteSlotsPanel;
  }

  private void LoadAvailableEmotes(BoxContainer emotesGrid)
  {
    ((Control) emotesGrid).RemoveAllChildren();
    if (this._unlockedEmotes.Count == 0)
    {
      Label label1 = new Label();
      label1.Text = Loc.GetString("mainmenu-emotes-no-emotes").Replace("\\n", "\n");
      label1.FontColorOverride = new Color?(Color.Gray);
      ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
      ((Control) label1).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) label1).Margin = new Thickness(0.0f, 50f, 0.0f, 0.0f);
      Label label2 = label1;
      ((Control) emotesGrid).AddChild((Control) label2);
    }
    else
    {
      GridContainer gridContainer1 = new GridContainer();
      gridContainer1.Columns = 4;
      ((Control) gridContainer1).Margin = new Thickness(10f);
      GridContainer gridContainer2 = gridContainer1;
      SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
      foreach (string unlockedEmote in this._unlockedEmotes)
      {
        EntityPrototype entityPrototype;
        if (this._prototypeManager.TryIndex<EntityPrototype>(unlockedEmote, ref entityPrototype))
        {
          PubgEmoteCard pubgEmoteCard1 = new PubgEmoteCard();
          pubgEmoteCard1.EmoteName = entityPrototype.Name;
          pubgEmoteCard1.EmoteId = unlockedEmote;
          pubgEmoteCard1.IsEquipped = this._temporaryEquippedEmotes.Contains(unlockedEmote);
          pubgEmoteCard1.Margin = new Thickness(5f);
          PubgEmoteCard pubgEmoteCard2 = pubgEmoteCard1;
          ActionComponent actionComponent;
          if (entityPrototype.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
          {
            Texture texture = spriteSystem.Frame0(actionComponent.Icon);
            pubgEmoteCard2.EmoteIcon = texture;
          }
          pubgEmoteCard2.OnCardClicked += new Action<string>(this.ToggleEmoteEquip);
          ((Control) gridContainer2).AddChild((Control) pubgEmoteCard2);
        }
      }
      ((Control) emotesGrid).AddChild((Control) gridContainer2);
    }
  }

  private void ToggleEmoteEquip(string emoteId)
  {
    if (this._temporaryEquippedEmotes.Contains(emoteId))
    {
      this._temporaryEquippedEmotes.Remove(emoteId);
    }
    else
    {
      if (this._temporaryEquippedEmotes.Count >= this._maxEmoteSlots)
        this._temporaryEquippedEmotes.RemoveAt(0);
      this._temporaryEquippedEmotes.Add(emoteId);
    }
    this.UpdateEmoteSlots();
    this.LoadAvailableEmotesRefresh();
  }

  private void LoadAvailableEmotesRefresh()
  {
    if (!(((IEnumerable<Control>) ((Control) this._mainContent).Children).FirstOrDefault<Control>() is BoxContainer boxContainer1) || !(((((IEnumerable<Control>) ((Control) boxContainer1).Children).FirstOrDefault<Control>() is BoxContainer boxContainer2 ? ((IEnumerable<Control>) ((Control) boxContainer2).Children).LastOrDefault<Control>() : (Control) null) is ScrollContainer scrollContainer ? ((IEnumerable<Control>) ((Control) scrollContainer).Children).FirstOrDefault<Control>() : (Control) null) is BoxContainer emotesGrid))
      return;
    this.LoadAvailableEmotes(emotesGrid);
  }

  private void UpdateEmoteSlots()
  {
    SpriteSystem spriteSystem = this._entityManager.System<SpriteSystem>();
    for (int index = 0; index < this._emoteSlots.Length; ++index)
    {
      PubgEmoteSlot emoteSlot = this._emoteSlots[index];
      if (emoteSlot != null)
      {
        if (index >= this._maxEmoteSlots)
        {
          emoteSlot.IsLocked = true;
          emoteSlot.IsFilled = false;
          BoxContainer boxContainer = new BoxContainer();
          boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
          ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 2;
          ((Control) boxContainer).VerticalAlignment = (Control.VAlignment) 2;
          BoxContainer content = boxContainer;
          Label label1 = new Label();
          label1.Text = Loc.GetString("mainmenu-emotes-slot-locked");
          label1.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#AA3333", new Color?()));
          ((Control) label1).HorizontalAlignment = (Control.HAlignment) 2;
          Label label2 = label1;
          ((Control) content).AddChild((Control) label2);
          emoteSlot.SetContent((Control) content);
        }
        else
        {
          emoteSlot.IsLocked = false;
          if (index < this._temporaryEquippedEmotes.Count)
          {
            string temporaryEquippedEmote = this._temporaryEquippedEmotes[index];
            EntityPrototype entityPrototype;
            if (this._prototypeManager.TryIndex<EntityPrototype>(temporaryEquippedEmote, ref entityPrototype))
            {
              emoteSlot.IsFilled = true;
              BoxContainer boxContainer = new BoxContainer();
              boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
              ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 2;
              ((Control) boxContainer).VerticalAlignment = (Control.VAlignment) 2;
              BoxContainer content = boxContainer;
              ActionComponent actionComponent;
              if (entityPrototype.TryGetComponent<ActionComponent>("Action", ref actionComponent) && actionComponent.Icon != null)
              {
                Texture texture = spriteSystem.Frame0(actionComponent.Icon);
                TextureRect textureRect1 = new TextureRect();
                textureRect1.Texture = texture;
                ((Control) textureRect1).MinSize = new Vector2(48f, 48f);
                ((Control) textureRect1).HorizontalAlignment = (Control.HAlignment) 2;
                ((Control) textureRect1).Margin = new Thickness(0.0f, 5f, 0.0f, 5f);
                textureRect1.Stretch = (TextureRect.StretchMode) 7;
                TextureRect textureRect2 = textureRect1;
                ((Control) content).AddChild((Control) textureRect2);
              }
              Label label3 = new Label();
              label3.Text = entityPrototype.Name;
              ((Control) label3).HorizontalAlignment = (Control.HAlignment) 2;
              label3.FontColorOverride = new Color?(Color.White);
              Label label4 = label3;
              ((Control) content).AddChild((Control) label4);
              Button button1 = new Button();
              button1.Text = Loc.GetString("mainmenu-emotes-remove");
              ((Control) button1).MinWidth = 100f;
              ((Control) button1).Margin = new Thickness(0.0f, 8f, 0.0f, 0.0f);
              Button button2 = button1;
              string currentEmoteId = temporaryEquippedEmote;
              ((BaseButton) button2).OnPressed += (Action<BaseButton.ButtonEventArgs>) (_ =>
              {
                this._temporaryEquippedEmotes.Remove(currentEmoteId);
                this.UpdateEmoteSlots();
                this.LoadAvailableEmotesRefresh();
              });
              ((Control) content).AddChild((Control) button2);
              emoteSlot.SetContent((Control) content);
            }
          }
          else
          {
            emoteSlot.IsFilled = false;
            BoxContainer boxContainer = new BoxContainer();
            boxContainer.Orientation = (BoxContainer.LayoutOrientation) 1;
            ((Control) boxContainer).HorizontalAlignment = (Control.HAlignment) 2;
            ((Control) boxContainer).VerticalAlignment = (Control.VAlignment) 2;
            BoxContainer content = boxContainer;
            Label label5 = new Label();
            label5.Text = Loc.GetString("mainmenu-emotes-slot-empty");
            label5.FontColorOverride = new Color?(Color.FromHex((ReadOnlySpan<char>) "#888888", new Color?()));
            ((Control) label5).HorizontalAlignment = (Control.HAlignment) 2;
            Label label6 = label5;
            ((Control) content).AddChild((Control) label6);
            emoteSlot.SetContent((Control) content);
          }
        }
      }
    }
  }

  private void ApplyEmotes()
  {
    this._equippedEmotes = new List<string>((IEnumerable<string>) this._temporaryEquippedEmotes);
    Dictionary<string, string> dictionary = new Dictionary<string, string>((IDictionary<string, string>) this._currentOutfit);
    dictionary["equippedEmotes"] = string.Join(",", (IEnumerable<string>) this._equippedEmotes);
    Action<Dictionary<string, string>> onApply = this.OnApply;
    if (onApply != null)
      onApply(dictionary);
    this._currentOutfit = dictionary;
  }

  private void ShowContextMenu(string itemId, string slotName, bool isRecipe, Control parent)
  {
    this.CloseCurrentPopup();
    PanelContainer content = SkinContextMenuBuilder.BuildItemContextMenu(this._prototypeManager, itemId, isRecipe, this._playerScrap, (Action) (() =>
    {
      this._entityManager.RaisePredictiveEvent<SkinCraftMessage>(new SkinCraftMessage(itemId));
      this.CloseCurrentPopup();
    }), (Action) (() =>
    {
      this._entityManager.RaisePredictiveEvent<SkinSellMessage>(new SkinSellMessage(itemId));
      this.CloseCurrentPopup();
    }), (Action) (() =>
    {
      this._temporaryOutfit[slotName] = itemId;
      this.UpdatePreview();
      Control control;
      BoxContainer boxContainer = ((IEnumerable<Control>) (control = ((IEnumerable<Control>) ((Control) this._mainContent).Children).ElementAt<Control>(1)).Children).ElementAt<Control>(0) as BoxContainer;
      this.ShowCategory(this._currentCategory, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(0) as Label, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(1) as Label, ((IEnumerable<Control>) ((Control) (((IEnumerable<Control>) control.Children).ElementAt<Control>(2) as ScrollContainer)).Children).ElementAt<Control>(0) as BoxContainer);
      this.CloseCurrentPopup();
    }), slotName != "ghost", (Action) (() =>
    {
      this._temporaryOutfit[slotName] = string.Empty;
      this.UpdatePreview();
      Control control;
      BoxContainer boxContainer = ((IEnumerable<Control>) (control = ((IEnumerable<Control>) ((Control) this._mainContent).Children).ElementAt<Control>(1)).Children).ElementAt<Control>(0) as BoxContainer;
      this.ShowCategory(this._currentCategory, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(0) as Label, ((IEnumerable<Control>) ((Control) boxContainer).Children).ElementAt<Control>(1) as Label, ((IEnumerable<Control>) ((Control) (((IEnumerable<Control>) control.Children).ElementAt<Control>(2) as ScrollContainer)).Children).ElementAt<Control>(0) as BoxContainer);
      this.CloseCurrentPopup();
    }));
    this._currentContextPopup = PopupHelper.OpenContextPopup(this._uiManager, parent, (Control) content, new Vector2(150f, 80f));
  }

  private void ShowShopContextMenu(string itemId, SkinShopItemInfo shopItem, Control parent)
  {
    this.CloseCurrentPopup();
    PanelContainer content = SkinContextMenuBuilder.BuildShopContextMenu(this._prototypeManager, this._entityManager.System<SpriteSystem>(), this._playerCoins, this._playerPremiumCoins, shopItem, (Action<string, string, int, int?>) ((offerId, currency, amount, durationDays) =>
    {
      this._entityManager.RaisePredictiveEvent<SkinBuyMessage>(new SkinBuyMessage(itemId, offerId));
      this.CloseCurrentPopup();
    }));
    this._currentContextPopup = PopupHelper.OpenContextPopup(this._uiManager, parent, (Control) content, new Vector2(260f, 140f));
  }

  private void CloseCurrentPopup()
  {
    if (this._currentContextPopup == null)
      return;
    this._currentContextPopup.Close();
    this._currentContextPopup = (Popup) null;
  }

  private FormattedMessage BuildSkinTooltipMessage(
    string name,
    SkinRarity rarity,
    string? description,
    DateTime? expiresAt)
  {
    string str1 = string.IsNullOrWhiteSpace(description) ? Loc.GetString("mainmenu-skin-tooltip-no-description") : description;
    string rarityText = this.GetRarityText(rarity);
    string rarityColorHex = this.GetRarityColorHex(rarity);
    string str2 = string.Empty;
    if (expiresAt.HasValue)
    {
      DateTime localTime = expiresAt.Value;
      localTime = localTime.ToLocalTime();
      str2 = "\n" + FormattedMessage.EscapeText(Loc.GetString("mainmenu-skin-tooltip-expires-at", new (string, object)[1]
      {
        ("time", (object) localTime.ToString("g", (IFormatProvider) CultureInfo.CurrentCulture))
      }));
    }
    string str3;
    return FormattedMessage.FromMarkupPermissive(Loc.GetString("mainmenu-skin-tooltip", new (string, object)[5]
    {
      (nameof (name), (object) $"[bold]{FormattedMessage.EscapeText(name)}[/bold]"),
      ("rarityLabel", (object) FormattedMessage.EscapeText(Loc.GetString("mainmenu-skin-tooltip-rarity-label"))),
      (nameof (rarity), (object) $"[color={rarityColorHex}]{FormattedMessage.EscapeText(rarityText)}[/color]"),
      (nameof (description), (object) FormattedMessage.EscapeText(str1)),
      ("expiresLine", (object) str2)
    }), ref str3);
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

  private string GetRarityColorHex(SkinRarity rarity)
  {
    string rarityColorHex;
    switch (rarity)
    {
      case SkinRarity.Rare:
        rarityColorHex = "#9C27B0";
        break;
      case SkinRarity.Epic:
        rarityColorHex = "#FF9800";
        break;
      case SkinRarity.Legendary:
        rarityColorHex = "#FFD700";
        break;
      case SkinRarity.Unique:
        rarityColorHex = "#00E5FF";
        break;
      default:
        rarityColorHex = "#9E9E9E";
        break;
    }
    return rarityColorHex;
  }

  private Control CreateSkinTooltip(FormattedMessage message)
  {
    Tooltip skinTooltip = new Tooltip();
    skinTooltip.SetMessage(new FormattedMessage(message));
    return (Control) skinTooltip;
  }

  public void Cleanup()
  {
    this.CloseCurrentPopup();
    if (this._previewEntity.HasValue)
    {
      this._entityManager.DeleteEntity(new EntityUid?(this._previewEntity.Value));
      this._previewEntity = new EntityUid?();
    }
    if (this._shopPreviewEntity.HasValue)
    {
      this._entityManager.DeleteEntity(new EntityUid?(this._shopPreviewEntity.Value));
      this._shopPreviewEntity = new EntityUid?();
    }
    if (this._shopSpecialPreviewEntity.HasValue)
    {
      this._entityManager.DeleteEntity(new EntityUid?(this._shopSpecialPreviewEntity.Value));
      this._shopSpecialPreviewEntity = new EntityUid?();
    }
    this._shopView = (PubgShopView) null;
  }

  private enum EmotePreviewLayer : byte
  {
    Emote,
  }
}
