// Decompiled with JetBrains decompiler
// Type: Content.Client.Construction.UI.ConstructionMenuPresenter
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Lobby;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.MenuBar.Widgets;
using Content.Shared._RMC14.Prototypes;
using Content.Shared.Construction;
using Content.Shared.Construction.Prototypes;
using Content.Shared.Whitelist;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Placement;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Log;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

#nullable enable
namespace Content.Client.Construction.UI;

internal sealed class ConstructionMenuPresenter : IDisposable
{
  [Dependency]
  private EntityManager _entManager;
  [Dependency]
  private IEntitySystemManager _systemManager;
  [Dependency]
  private IPrototypeManager _prototypeManager;
  [Dependency]
  private IPlacementManager _placementManager;
  [Dependency]
  private IUserInterfaceManager _uiManager;
  [Dependency]
  private IPlayerManager _playerManager;
  [Dependency]
  private IClientPreferencesManager _preferencesManager;
  private readonly SpriteSystem _spriteSystem;
  private readonly ISawmill _sawmill = Logger.GetSawmill("construction.menu");
  private readonly IConstructionMenuView _constructionView;
  private readonly EntityWhitelistSystem _whitelistSystem;
  private ConstructionSystem? _constructionSystem;
  private ConstructionPrototype? _selected;
  private List<ConstructionPrototype> _favoritedRecipes = new List<ConstructionPrototype>();
  private readonly Dictionary<string, ContainerButton> _recipeButtons = new Dictionary<string, ContainerButton>();
  private string _selectedCategory = string.Empty;
  private const string FavoriteCatName = "construction-category-favorites";
  private const string ForAllCategoryName = "construction-category-all";

  private bool CraftingAvailable
  {
    get => ((Control) this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Visible;
    set
    {
      ((Control) this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Visible = value;
      if (value)
        return;
      this._constructionView.Close();
    }
  }

  private bool IsAtFront => this._constructionView.IsOpen && this._constructionView.IsAtFront();

  private bool WindowOpen
  {
    get => this._constructionView.IsOpen;
    set
    {
      if (value && this.CraftingAvailable)
      {
        if (this._constructionView.IsOpen)
          this._constructionView.MoveToFront();
        else
          this._constructionView.OpenCentered();
        if (this._selected == null)
          return;
        this.PopulateInfo(this._selected);
      }
      else
        this._constructionView.Close();
    }
  }

  public ConstructionMenuPresenter()
  {
    IoCManager.InjectDependencies<ConstructionMenuPresenter>(this);
    this._constructionView = (IConstructionMenuView) new ConstructionMenu();
    this._whitelistSystem = this._entManager.System<EntityWhitelistSystem>();
    this._spriteSystem = this._entManager.System<SpriteSystem>();
    ConstructionSystem newSystem;
    if (this._systemManager.TryGetEntitySystem<ConstructionSystem>(ref newSystem))
      this.SystemBindingChanged(newSystem);
    this._systemManager.SystemLoaded += new EventHandler<SystemChangedArgs>(this.OnSystemLoaded);
    this._systemManager.SystemUnloaded += new EventHandler<SystemChangedArgs>(this.OnSystemUnloaded);
    this._placementManager.PlacementChanged += new EventHandler(this.OnPlacementChanged);
    this._constructionView.OnClose += (Action) (() => ((BaseButton) this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Pressed = false);
    this._constructionView.ClearAllGhosts += (EventHandler) ((_1, _2) => this._constructionSystem?.ClearAllGhosts());
    this._constructionView.PopulateRecipes += new EventHandler<(string, string)>(this.OnViewPopulateRecipes);
    this._constructionView.RecipeSelected += new EventHandler<ConstructionMenu.ConstructionMenuListData>(this.OnViewRecipeSelected);
    this._constructionView.BuildButtonToggled += (EventHandler<bool>) ((_, b) => this.BuildButtonToggled(b));
    this._constructionView.EraseButtonToggled += (EventHandler<bool>) ((_, b) =>
    {
      if (this._constructionSystem == null)
        return;
      if (b)
        this._placementManager.Clear();
      this._placementManager.ToggleEraserHijacked((PlacementHijack) new ConstructionPlacementHijack(this._constructionSystem, (ConstructionPrototype) null));
      this._constructionView.EraseButtonPressed = b;
    });
    this._constructionView.RecipeFavorited += (EventHandler) ((_3, _4) => this.OnViewFavoriteRecipe());
    this.SetFavorites((IReadOnlyList<ProtoId<ConstructionPrototype>>) (this._preferencesManager.Preferences?.ConstructionFavorites ?? new List<ProtoId<ConstructionPrototype>>()));
    this.OnViewPopulateRecipes((object) this._constructionView, (string.Empty, string.Empty));
  }

  public void OnHudCraftingButtonToggled(BaseButton.ButtonToggledEventArgs args)
  {
    this.WindowOpen = args.Pressed;
  }

  public void Dispose()
  {
    this._constructionView.Dispose();
    this.SystemBindingChanged((ConstructionSystem) null);
    this._systemManager.SystemLoaded -= new EventHandler<SystemChangedArgs>(this.OnSystemLoaded);
    this._systemManager.SystemUnloaded -= new EventHandler<SystemChangedArgs>(this.OnSystemUnloaded);
    this._placementManager.PlacementChanged -= new EventHandler(this.OnPlacementChanged);
  }

  private void OnPlacementChanged(object? sender, EventArgs e)
  {
    this._constructionView.ResetPlacement();
  }

  private void OnViewRecipeSelected(object? sender, ConstructionMenu.ConstructionMenuListData? item)
  {
    if ((object) item == null)
    {
      this._selected = (ConstructionPrototype) null;
      this._constructionView.ClearRecipeInfo();
    }
    else
    {
      this._selected = item.Prototype;
      IPlacementManager placementManager = this._placementManager;
      if (placementManager != null && placementManager.IsActive && !placementManager.Eraser)
        this.UpdateGhostPlacement();
      this.PopulateInfo(this._selected);
    }
  }

  private void OnGridViewRecipeSelected(object? _, ConstructionPrototype? recipe)
  {
    if (recipe == null)
    {
      this._selected = (ConstructionPrototype) null;
      this._constructionView.ClearRecipeInfo();
    }
    else
    {
      this._selected = recipe;
      IPlacementManager placementManager = this._placementManager;
      if (placementManager != null && placementManager.IsActive && !placementManager.Eraser)
        this.UpdateGhostPlacement();
      this.PopulateInfo(this._selected);
    }
  }

  private void OnViewPopulateRecipes(object? sender, (string search, string catagory) args)
  {
    if (this._constructionSystem == null)
      return;
    List<ConstructionMenu.ConstructionMenuListData> andSortRecipes = this.GetAndSortRecipes(args);
    ListContainer recipes = this._constructionView.Recipes;
    GridContainer recipesGrid = this._constructionView.RecipesGrid;
    ((Control) recipesGrid).RemoveAllChildren();
    ((Control) this._constructionView.RecipesGridScrollContainer).Visible = this._constructionView.GridViewButtonPressed;
    this._constructionView.Recipes.Visible = !this._constructionView.GridViewButtonPressed;
    if (this._constructionView.GridViewButtonPressed)
    {
      recipes.PopulateList((IReadOnlyList<ListData>) Array.Empty<ListData>());
      this.PopulateGrid(recipesGrid, (IEnumerable<ConstructionMenu.ConstructionMenuListData>) andSortRecipes);
    }
    else
      recipes.PopulateList((IReadOnlyList<ListData>) andSortRecipes);
  }

  private void PopulateGrid(
    GridContainer recipesGrid,
    IEnumerable<ConstructionMenu.ConstructionMenuListData> actualRecipes)
  {
    foreach (ConstructionMenu.ConstructionMenuListData actualRecipe in actualRecipes)
    {
      ConstructionMenu.ConstructionMenuListData recipe = actualRecipe;
      EntityPrototypeView entityPrototypeView1 = new EntityPrototypeView();
      ((SpriteView) entityPrototypeView1).Scale = new Vector2(1.2f);
      ((Control) entityPrototypeView1).Modulate = recipe.Prototype.IconColor;
      EntityPrototypeView entityPrototypeView2 = entityPrototypeView1;
      entityPrototypeView2.SetPrototype(new EntProtoId?(EntProtoId.op_Implicit(recipe.TargetPrototype)));
      ContainerButton containerButton = new ContainerButton();
      ((Control) containerButton).VerticalAlignment = (Control.VAlignment) 2;
      ((Control) containerButton).Name = recipe.Prototype.Name;
      ((Control) containerButton).ToolTip = recipe.Prototype.Name;
      ((BaseButton) containerButton).ToggleMode = true;
      ((Control) containerButton).Children.Add((Control) entityPrototypeView2);
      ContainerButton itemButton = containerButton;
      PanelContainer panelContainer1 = new PanelContainer();
      panelContainer1.PanelOverride = (StyleBox) new StyleBoxFlat()
      {
        BackgroundColor = StyleNano.ButtonColorDefault
      };
      ((Control) panelContainer1).Children.Add((Control) itemButton);
      PanelContainer panelContainer2 = panelContainer1;
      ((BaseButton) itemButton).OnToggled += (Action<BaseButton.ButtonToggledEventArgs>) (buttonToggledEventArgs =>
      {
        this.SelectGridButton((BaseButton) itemButton, buttonToggledEventArgs.Pressed);
        ContainerButton button;
        if (buttonToggledEventArgs.Pressed && this._selected != null && this._recipeButtons.TryGetValue(this._selected.ID, out button))
        {
          ((BaseButton) button).Pressed = false;
          this.SelectGridButton((BaseButton) button, false);
        }
        this.OnGridViewRecipeSelected((object) this, buttonToggledEventArgs.Pressed ? recipe.Prototype : (ConstructionPrototype) null);
      });
      ((Control) recipesGrid).AddChild((Control) panelContainer2);
      this._recipeButtons[recipe.Prototype.ID] = itemButton;
      bool select = this._selected == recipe.Prototype;
      ((BaseButton) itemButton).Pressed = select;
      this.SelectGridButton((BaseButton) itemButton, select);
    }
  }

  private List<ConstructionMenu.ConstructionMenuListData> GetAndSortRecipes((string, string) args)
  {
    List<ConstructionMenu.ConstructionMenuListData> andSortRecipes = new List<ConstructionMenu.ConstructionMenuListData>();
    (string str1, string str2) = args;
    bool flag = string.IsNullOrEmpty(str2) || str2 == "construction-category-all";
    this._selectedCategory = flag ? string.Empty : str2;
    foreach (ConstructionPrototype Prototype in this._prototypeManager.EnumerateCM<ConstructionPrototype>())
    {
      if (!Prototype.Hide && ((ISharedPlayerManager) this._playerManager).LocalSession != null && ((ISharedPlayerManager) this._playerManager).LocalEntity.HasValue && !this._whitelistSystem.IsWhitelistFail(Prototype.EntityWhitelist, ((ISharedPlayerManager) this._playerManager).LocalEntity.Value))
      {
        if (!string.IsNullOrEmpty(str1))
        {
          string name = Prototype.Name;
          if (name != null && !name.Contains(str1.Trim(), StringComparison.InvariantCultureIgnoreCase))
            continue;
        }
        if (flag || !(str2 != "construction-category-favorites") && this._favoritedRecipes.Contains(Prototype) || !(Prototype.Category != str2))
        {
          string targetProtoId;
          if (!this._constructionSystem.TryGetRecipePrototype(Prototype.ID, out targetProtoId))
          {
            this._sawmill.Error("Cannot find the target prototype in the recipe cache with the id \"{0}\" of {1}.", new object[2]
            {
              (object) Prototype.ID,
              (object) "ConstructionPrototype"
            });
          }
          else
          {
            EntityPrototype TargetPrototype;
            if (this._prototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref TargetPrototype))
              andSortRecipes.Add(new ConstructionMenu.ConstructionMenuListData(Prototype, TargetPrototype));
          }
        }
      }
    }
    andSortRecipes.Sort((Comparison<ConstructionMenu.ConstructionMenuListData>) ((a, b) => string.Compare(a.Prototype.Name, b.Prototype.Name, StringComparison.InvariantCulture)));
    return andSortRecipes;
  }

  private void SelectGridButton(BaseButton button, bool select)
  {
    if (!(((Control) button).Parent is PanelContainer parent))
      return;
    Color color = select ? StyleNano.ButtonColorDefault : Color.Transparent;
    parent.PanelOverride = (StyleBox) new StyleBoxFlat()
    {
      BackgroundColor = color
    };
  }

  private void PopulateCategories(string? selectCategory = null)
  {
    HashSet<string> source = new HashSet<string>();
    foreach (ConstructionPrototype constructionPrototype in this._prototypeManager.EnumerateCM<ConstructionPrototype>())
    {
      string category = constructionPrototype.Category;
      if (!string.IsNullOrEmpty(category))
        source.Add(category);
    }
    int num1 = this._favoritedRecipes.Count > 0 ? 1 : 0;
    string[] strArray1 = new string[num1 != 0 ? source.Count + 2 : source.Count + 1];
    int num2 = 0;
    string[] strArray2 = strArray1;
    int index1 = num2;
    int num3 = index1 + 1;
    strArray2[index1] = "construction-category-all";
    if (num1 != 0)
      strArray1[num3++] = "construction-category-favorites";
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    foreach (string str in (IEnumerable<string>) source.OrderBy<string, string>(ConstructionMenuPresenter.\u003C\u003EO.\u003C0\u003E__GetString ?? (ConstructionMenuPresenter.\u003C\u003EO.\u003C0\u003E__GetString = new Func<string, string>(Loc.GetString))))
      strArray1[num3++] = str;
    this._constructionView.OptionCategories.Clear();
    for (int index2 = 0; index2 < strArray1.Length; ++index2)
    {
      this._constructionView.OptionCategories.AddItem(Loc.GetString(strArray1[index2]), new int?(index2));
      if (!string.IsNullOrEmpty(selectCategory) && selectCategory == strArray1[index2])
        this._constructionView.OptionCategories.SelectId(index2);
    }
    this._constructionView.Categories = strArray1;
  }

  private void PopulateInfo(ConstructionPrototype? prototype)
  {
    if (this._constructionSystem == null)
      return;
    this._constructionView.ClearRecipeInfo();
    string targetProtoId;
    EntityPrototype targetPrototype;
    if (prototype == null || !this._constructionSystem.TryGetRecipePrototype(prototype.ID, out targetProtoId) || !this._prototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref targetPrototype))
      return;
    this._constructionView.SetRecipeInfo(prototype.Name, prototype.Description, targetPrototype, prototype.IconColor, prototype.Type != ConstructionType.Item, !this._favoritedRecipes.Contains(prototype), prototype);
    ItemList recipeStepList = this._constructionView.RecipeStepList;
    this.GenerateStepList(prototype, recipeStepList);
  }

  private void GenerateStepList(ConstructionPrototype prototype, ItemList stepList)
  {
    ConstructionGuide guide = this._constructionSystem?.GetGuide(prototype);
    if (guide == null)
      return;
    foreach (ConstructionGuideEntry entry in guide.Entries)
    {
      string str1 = entry.Arguments != null ? Loc.GetString(entry.Localization, entry.Arguments) : Loc.GetString(entry.Localization);
      int? entryNumber = entry.EntryNumber;
      if (entryNumber.HasValue)
        str1 = Loc.GetString("construction-presenter-step-wrapper", new (string, object)[2]
        {
          ("step-number", (object) entryNumber.GetValueOrDefault()),
          ("text", (object) str1)
        });
      string str2 = str1.PadLeft(str1.Length + entry.Padding);
      Texture texture = entry.Icon != null ? this._spriteSystem.Frame0(entry.Icon) : Texture.Transparent;
      stepList.AddItem(str2, texture, false, (object) null, 1f);
    }
  }

  private void BuildButtonToggled(bool pressed)
  {
    if (pressed)
    {
      if (this._selected == null)
        return;
      if (this._constructionSystem == null)
      {
        this._constructionView.BuildButtonPressed = false;
        return;
      }
      if (this._selected.Type == ConstructionType.Item || this._selected.RMCPrototype.HasValue)
      {
        this._constructionSystem.TryStartItemConstruction(this._selected.ID);
        this._constructionView.BuildButtonPressed = false;
        return;
      }
      IPlacementManager placementManager = this._placementManager;
      PlacementInformation placementInformation = new PlacementInformation();
      placementInformation.IsTile = false;
      placementInformation.PlacementOption = this._selected.PlacementMode;
      ConstructionPlacementHijack constructionPlacementHijack = new ConstructionPlacementHijack(this._constructionSystem, this._selected);
      placementManager.BeginPlacing(placementInformation, (PlacementHijack) constructionPlacementHijack);
      this.UpdateGhostPlacement();
    }
    else
      this._placementManager.Clear();
    this._constructionView.BuildButtonPressed = pressed;
  }

  private void UpdateGhostPlacement()
  {
    if (this._selected == null)
      return;
    if (this._selected.Type != ConstructionType.Structure)
    {
      this._placementManager.Clear();
    }
    else
    {
      ConstructionSystem entitySystem = this._systemManager.GetEntitySystem<ConstructionSystem>();
      IPlacementManager placementManager = this._placementManager;
      PlacementInformation placementInformation = new PlacementInformation();
      placementInformation.IsTile = false;
      placementInformation.PlacementOption = this._selected.PlacementMode;
      ConstructionPlacementHijack constructionPlacementHijack = new ConstructionPlacementHijack(entitySystem, this._selected);
      placementManager.BeginPlacing(placementInformation, (PlacementHijack) constructionPlacementHijack);
      this._constructionView.BuildButtonPressed = true;
    }
  }

  private void OnSystemLoaded(object? sender, SystemChangedArgs args)
  {
    if (!(args.System is ConstructionSystem system))
      return;
    this.SystemBindingChanged(system);
  }

  private void OnSystemUnloaded(object? sender, SystemChangedArgs args)
  {
    if (!(args.System is ConstructionSystem))
      return;
    this.SystemBindingChanged((ConstructionSystem) null);
  }

  private void OnViewFavoriteRecipe()
  {
    if (this._selected == null)
      return;
    if (!this._favoritedRecipes.Remove(this._selected))
      this._favoritedRecipes.Add(this._selected);
    if (this._selectedCategory == "construction-category-favorites")
      this.OnViewPopulateRecipes((object) this._constructionView, this._favoritedRecipes.Count > 0 ? (string.Empty, "construction-category-favorites") : (string.Empty, string.Empty));
    List<ProtoId<ConstructionPrototype>> favorites = new List<ProtoId<ConstructionPrototype>>(this._favoritedRecipes.Count);
    foreach (ConstructionPrototype favoritedRecipe in this._favoritedRecipes)
      favorites.Add(ProtoId<ConstructionPrototype>.op_Implicit(favoritedRecipe.ID));
    this._preferencesManager.UpdateConstructionFavorites(favorites);
    this.PopulateInfo(this._selected);
    this.PopulateCategories(this._selectedCategory);
  }

  public void SetFavorites(
    IReadOnlyList<ProtoId<ConstructionPrototype>> favorites)
  {
    this._favoritedRecipes.Clear();
    foreach (ProtoId<ConstructionPrototype> favorite in (IEnumerable<ProtoId<ConstructionPrototype>>) favorites)
    {
      ConstructionPrototype constructionPrototype;
      if (this._prototypeManager.TryIndex<ConstructionPrototype>(favorite, ref constructionPrototype))
        this._favoritedRecipes.Add(constructionPrototype);
    }
    if (this._selectedCategory == "construction-category-favorites")
      this.OnViewPopulateRecipes((object) this._constructionView, this._favoritedRecipes.Count > 0 ? (string.Empty, "construction-category-favorites") : (string.Empty, string.Empty));
    this.PopulateCategories(this._selectedCategory);
  }

  private void SystemBindingChanged(ConstructionSystem? newSystem)
  {
    if (newSystem == null)
    {
      if (this._constructionSystem == null)
        return;
      this.UnbindFromSystem();
    }
    else if (this._constructionSystem == null)
    {
      this.BindToSystem(newSystem);
    }
    else
    {
      this.UnbindFromSystem();
      this.BindToSystem(newSystem);
    }
  }

  private void BindToSystem(ConstructionSystem system)
  {
    this._constructionSystem = system;
    this.OnViewPopulateRecipes((object) this._constructionView, (string.Empty, string.Empty));
    system.ToggleCraftingWindow += new EventHandler(this.SystemOnToggleMenu);
    system.FlipConstructionPrototype += new EventHandler(this.SystemFlipConstructionPrototype);
    system.CraftingAvailabilityChanged += new EventHandler<CraftingAvailabilityChangedArgs>(this.SystemCraftingAvailabilityChanged);
    system.ConstructionGuideAvailable += new EventHandler<string>(this.SystemGuideAvailable);
    if (this._uiManager.GetActiveUIWidgetOrNull<GameTopMenuBar>() == null)
      return;
    this.CraftingAvailable = system.CraftingEnabled;
  }

  private void UnbindFromSystem()
  {
    ConstructionSystem constructionSystem = this._constructionSystem;
    if (constructionSystem == null)
      throw new InvalidOperationException();
    constructionSystem.ToggleCraftingWindow -= new EventHandler(this.SystemOnToggleMenu);
    constructionSystem.FlipConstructionPrototype -= new EventHandler(this.SystemFlipConstructionPrototype);
    constructionSystem.CraftingAvailabilityChanged -= new EventHandler<CraftingAvailabilityChangedArgs>(this.SystemCraftingAvailabilityChanged);
    constructionSystem.ConstructionGuideAvailable -= new EventHandler<string>(this.SystemGuideAvailable);
    this._constructionSystem = (ConstructionSystem) null;
  }

  private void SystemCraftingAvailabilityChanged(object? sender, CraftingAvailabilityChangedArgs e)
  {
    if (this._uiManager.ActiveScreen == null)
      return;
    this.CraftingAvailable = e.Available;
  }

  private void SystemOnToggleMenu(object? sender, EventArgs eventArgs)
  {
    if (!this.CraftingAvailable)
      return;
    if (this.WindowOpen)
    {
      if (this.IsAtFront)
      {
        this.WindowOpen = false;
        ((BaseButton) this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).SetClickPressed(false);
      }
      else
        this._constructionView.MoveToFront();
    }
    else
    {
      this.WindowOpen = true;
      ((BaseButton) this._uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).SetClickPressed(true);
    }
  }

  private void SystemFlipConstructionPrototype(object? sender, EventArgs eventArgs)
  {
    if (!this._placementManager.IsActive || this._placementManager.Eraser || this._selected == null)
      return;
    ProtoId<ConstructionPrototype>? mirror = this._selected.Mirror;
    if (!mirror.HasValue)
      return;
    IPrototypeManager prototypeManager = this._prototypeManager;
    mirror = this._selected.Mirror;
    string str = mirror.HasValue ? ProtoId<ConstructionPrototype>.op_Implicit(mirror.GetValueOrDefault()) : (string) null;
    this._selected = prototypeManager.Index<ConstructionPrototype>(str);
    this.UpdateGhostPlacement();
  }

  private void SystemGuideAvailable(object? sender, string e)
  {
    if (!this.CraftingAvailable || !this.WindowOpen || this._selected == null)
      return;
    this.PopulateInfo(this._selected);
  }
}
