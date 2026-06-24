using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
		get
		{
			return ((Control)_uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Visible;
		}
		set
		{
			((Control)_uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Visible = value;
			if (!value)
			{
				_constructionView.Close();
			}
		}
	}

	private bool IsAtFront
	{
		get
		{
			if (_constructionView.IsOpen)
			{
				return _constructionView.IsAtFront();
			}
			return false;
		}
	}

	private bool WindowOpen
	{
		get
		{
			return _constructionView.IsOpen;
		}
		set
		{
			if (value && CraftingAvailable)
			{
				if (_constructionView.IsOpen)
				{
					_constructionView.MoveToFront();
				}
				else
				{
					_constructionView.OpenCentered();
				}
				if (_selected != null)
				{
					PopulateInfo(_selected);
				}
			}
			else
			{
				_constructionView.Close();
			}
		}
	}

	public ConstructionMenuPresenter()
	{
		IoCManager.InjectDependencies<ConstructionMenuPresenter>(this);
		_constructionView = new ConstructionMenu();
		_whitelistSystem = _entManager.System<EntityWhitelistSystem>();
		_spriteSystem = _entManager.System<SpriteSystem>();
		ConstructionSystem newSystem = default(ConstructionSystem);
		if (_systemManager.TryGetEntitySystem<ConstructionSystem>(ref newSystem))
		{
			SystemBindingChanged(newSystem);
		}
		_systemManager.SystemLoaded += OnSystemLoaded;
		_systemManager.SystemUnloaded += OnSystemUnloaded;
		_placementManager.PlacementChanged += OnPlacementChanged;
		_constructionView.OnClose += delegate
		{
			((BaseButton)_uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).Pressed = false;
		};
		_constructionView.ClearAllGhosts += delegate
		{
			_constructionSystem?.ClearAllGhosts();
		};
		_constructionView.PopulateRecipes += OnViewPopulateRecipes;
		_constructionView.RecipeSelected += OnViewRecipeSelected;
		_constructionView.BuildButtonToggled += delegate(object? _, bool b)
		{
			BuildButtonToggled(b);
		};
		_constructionView.EraseButtonToggled += delegate(object? _, bool b)
		{
			if (_constructionSystem != null)
			{
				if (b)
				{
					_placementManager.Clear();
				}
				_placementManager.ToggleEraserHijacked((PlacementHijack)(object)new ConstructionPlacementHijack(_constructionSystem, null));
				_constructionView.EraseButtonPressed = b;
			}
		};
		_constructionView.RecipeFavorited += delegate
		{
			OnViewFavoriteRecipe();
		};
		SetFavorites(_preferencesManager.Preferences?.ConstructionFavorites ?? new List<ProtoId<ConstructionPrototype>>());
		OnViewPopulateRecipes(_constructionView, (search: string.Empty, catagory: string.Empty));
	}

	public void OnHudCraftingButtonToggled(ButtonToggledEventArgs args)
	{
		WindowOpen = args.Pressed;
	}

	public void Dispose()
	{
		_constructionView.Dispose();
		SystemBindingChanged(null);
		_systemManager.SystemLoaded -= OnSystemLoaded;
		_systemManager.SystemUnloaded -= OnSystemUnloaded;
		_placementManager.PlacementChanged -= OnPlacementChanged;
	}

	private void OnPlacementChanged(object? sender, EventArgs e)
	{
		_constructionView.ResetPlacement();
	}

	private void OnViewRecipeSelected(object? sender, ConstructionMenu.ConstructionMenuListData? item)
	{
		if ((object)item == null)
		{
			_selected = null;
			_constructionView.ClearRecipeInfo();
			return;
		}
		_selected = item.Prototype;
		IPlacementManager placementManager = _placementManager;
		if (placementManager != null && placementManager.IsActive && !placementManager.Eraser)
		{
			UpdateGhostPlacement();
		}
		PopulateInfo(_selected);
	}

	private void OnGridViewRecipeSelected(object? _, ConstructionPrototype? recipe)
	{
		if (recipe == null)
		{
			_selected = null;
			_constructionView.ClearRecipeInfo();
			return;
		}
		_selected = recipe;
		IPlacementManager placementManager = _placementManager;
		if (placementManager != null && placementManager.IsActive && !placementManager.Eraser)
		{
			UpdateGhostPlacement();
		}
		PopulateInfo(_selected);
	}

	private void OnViewPopulateRecipes(object? sender, (string search, string catagory) args)
	{
		if (_constructionSystem != null)
		{
			List<ConstructionMenu.ConstructionMenuListData> andSortRecipes = GetAndSortRecipes(args);
			ListContainer recipes = _constructionView.Recipes;
			GridContainer recipesGrid = _constructionView.RecipesGrid;
			((Control)recipesGrid).RemoveAllChildren();
			((Control)_constructionView.RecipesGridScrollContainer).Visible = _constructionView.GridViewButtonPressed;
			((Control)_constructionView.Recipes).Visible = !_constructionView.GridViewButtonPressed;
			if (_constructionView.GridViewButtonPressed)
			{
				recipes.PopulateList(Array.Empty<ListData>());
				PopulateGrid(recipesGrid, andSortRecipes);
			}
			else
			{
				recipes.PopulateList(andSortRecipes);
			}
		}
	}

	private void PopulateGrid(GridContainer recipesGrid, IEnumerable<ConstructionMenu.ConstructionMenuListData> actualRecipes)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
		foreach (ConstructionMenu.ConstructionMenuListData recipe in actualRecipes)
		{
			EntityPrototypeView val = new EntityPrototypeView
			{
				Scale = new Vector2(1.2f),
				Modulate = recipe.Prototype.IconColor
			};
			val.SetPrototype((EntProtoId?)EntProtoId.op_Implicit(recipe.TargetPrototype));
			ContainerButton val2 = new ContainerButton
			{
				VerticalAlignment = (VAlignment)2,
				Name = recipe.Prototype.Name,
				ToolTip = recipe.Prototype.Name,
				ToggleMode = true
			};
			((Control)val2).Children.Add((Control)(object)val);
			ContainerButton itemButton = val2;
			PanelContainer val3 = new PanelContainer
			{
				PanelOverride = (StyleBox)new StyleBoxFlat
				{
					BackgroundColor = StyleNano.ButtonColorDefault
				}
			};
			((Control)val3).Children.Add((Control)(object)itemButton);
			PanelContainer val4 = val3;
			((BaseButton)itemButton).OnToggled += delegate(ButtonToggledEventArgs buttonToggledEventArgs)
			{
				SelectGridButton((BaseButton)(object)itemButton, buttonToggledEventArgs.Pressed);
				if (buttonToggledEventArgs.Pressed && _selected != null && _recipeButtons.TryGetValue(_selected.ID, out ContainerButton value))
				{
					((BaseButton)value).Pressed = false;
					SelectGridButton((BaseButton)(object)value, select: false);
				}
				OnGridViewRecipeSelected(this, buttonToggledEventArgs.Pressed ? recipe.Prototype : null);
			};
			((Control)recipesGrid).AddChild((Control)(object)val4);
			_recipeButtons[recipe.Prototype.ID] = itemButton;
			bool flag = _selected == recipe.Prototype;
			((BaseButton)itemButton).Pressed = flag;
			SelectGridButton((BaseButton)(object)itemButton, flag);
		}
	}

	private List<ConstructionMenu.ConstructionMenuListData> GetAndSortRecipes((string, string) args)
	{
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		List<ConstructionMenu.ConstructionMenuListData> list = new List<ConstructionMenu.ConstructionMenuListData>();
		(string, string) tuple = args;
		string item = tuple.Item1;
		string item2 = tuple.Item2;
		bool flag = string.IsNullOrEmpty(item2) || item2 == "construction-category-all";
		_selectedCategory = (flag ? string.Empty : item2);
		EntityPrototype targetPrototype = default(EntityPrototype);
		foreach (ConstructionPrototype item3 in _prototypeManager.EnumerateCM<ConstructionPrototype>())
		{
			if (item3.Hide || ((ISharedPlayerManager)_playerManager).LocalSession == null || !((ISharedPlayerManager)_playerManager).LocalEntity.HasValue || _whitelistSystem.IsWhitelistFail(item3.EntityWhitelist, ((ISharedPlayerManager)_playerManager).LocalEntity.Value))
			{
				continue;
			}
			if (!string.IsNullOrEmpty(item))
			{
				string name = item3.Name;
				if (name != null && !name.Contains(item.Trim(), StringComparison.InvariantCultureIgnoreCase))
				{
					continue;
				}
			}
			if (flag || (!(item2 != "construction-category-favorites") && _favoritedRecipes.Contains(item3)) || !(item3.Category != item2))
			{
				if (!_constructionSystem.TryGetRecipePrototype(item3.ID, out string targetProtoId))
				{
					_sawmill.Error("Cannot find the target prototype in the recipe cache with the id \"{0}\" of {1}.", new object[2] { item3.ID, "ConstructionPrototype" });
				}
				else if (_prototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref targetPrototype))
				{
					list.Add(new ConstructionMenu.ConstructionMenuListData(item3, targetPrototype));
				}
			}
		}
		list.Sort((ConstructionMenu.ConstructionMenuListData a, ConstructionMenu.ConstructionMenuListData b) => string.Compare(a.Prototype.Name, b.Prototype.Name, StringComparison.InvariantCulture));
		return list;
	}

	private void SelectGridButton(BaseButton button, bool select)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		Control parent = ((Control)button).Parent;
		PanelContainer val = (PanelContainer)(object)((parent is PanelContainer) ? parent : null);
		if (val != null)
		{
			Color backgroundColor = (select ? StyleNano.ButtonColorDefault : Color.Transparent);
			val.PanelOverride = (StyleBox)new StyleBoxFlat
			{
				BackgroundColor = backgroundColor
			};
		}
	}

	private void PopulateCategories(string? selectCategory = null)
	{
		HashSet<string> hashSet = new HashSet<string>();
		foreach (ConstructionPrototype item in _prototypeManager.EnumerateCM<ConstructionPrototype>())
		{
			string category = item.Category;
			if (!string.IsNullOrEmpty(category))
			{
				hashSet.Add(category);
			}
		}
		bool num = _favoritedRecipes.Count > 0;
		string[] array = new string[num ? (hashSet.Count + 2) : (hashSet.Count + 1)];
		int num2 = 0;
		array[num2++] = "construction-category-all";
		if (num)
		{
			array[num2++] = "construction-category-favorites";
		}
		foreach (string item2 in ((IEnumerable<string>)hashSet).OrderBy((Func<string, string>)Loc.GetString))
		{
			array[num2++] = item2;
		}
		_constructionView.OptionCategories.Clear();
		for (int i = 0; i < array.Length; i++)
		{
			_constructionView.OptionCategories.AddItem(Loc.GetString(array[i]), (int?)i);
			if (!string.IsNullOrEmpty(selectCategory) && selectCategory == array[i])
			{
				_constructionView.OptionCategories.SelectId(i);
			}
		}
		_constructionView.Categories = array;
	}

	private void PopulateInfo(ConstructionPrototype? prototype)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (_constructionSystem != null)
		{
			_constructionView.ClearRecipeInfo();
			EntityPrototype targetPrototype = default(EntityPrototype);
			if (prototype != null && _constructionSystem.TryGetRecipePrototype(prototype.ID, out string targetProtoId) && _prototypeManager.TryIndex<EntityPrototype>(targetProtoId, ref targetPrototype))
			{
				_constructionView.SetRecipeInfo(prototype.Name, prototype.Description, targetPrototype, prototype.IconColor, prototype.Type != ConstructionType.Item, !_favoritedRecipes.Contains(prototype), prototype);
				ItemList recipeStepList = _constructionView.RecipeStepList;
				GenerateStepList(prototype, recipeStepList);
			}
		}
	}

	private void GenerateStepList(ConstructionPrototype prototype, ItemList stepList)
	{
		ConstructionGuide constructionGuide = _constructionSystem?.GetGuide(prototype);
		if (constructionGuide == null)
		{
			return;
		}
		ConstructionGuideEntry[] entries = constructionGuide.Entries;
		foreach (ConstructionGuideEntry constructionGuideEntry in entries)
		{
			string text = ((constructionGuideEntry.Arguments != null) ? Loc.GetString(constructionGuideEntry.Localization, constructionGuideEntry.Arguments) : Loc.GetString(constructionGuideEntry.Localization));
			int? entryNumber = constructionGuideEntry.EntryNumber;
			if (entryNumber.HasValue)
			{
				int valueOrDefault = entryNumber.GetValueOrDefault();
				text = Loc.GetString("construction-presenter-step-wrapper", new(string, object)[2]
				{
					("step-number", valueOrDefault),
					("text", text)
				});
			}
			text = text.PadLeft(text.Length + constructionGuideEntry.Padding);
			Texture val = ((constructionGuideEntry.Icon != null) ? _spriteSystem.Frame0(constructionGuideEntry.Icon) : Texture.Transparent);
			stepList.AddItem(text, val, false, (object)null, 1f);
		}
	}

	private void BuildButtonToggled(bool pressed)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		if (pressed)
		{
			if (_selected == null)
			{
				return;
			}
			if (_constructionSystem == null)
			{
				_constructionView.BuildButtonPressed = false;
				return;
			}
			if (_selected.Type == ConstructionType.Item || _selected.RMCPrototype.HasValue)
			{
				_constructionSystem.TryStartItemConstruction(_selected.ID);
				_constructionView.BuildButtonPressed = false;
				return;
			}
			_placementManager.BeginPlacing(new PlacementInformation
			{
				IsTile = false,
				PlacementOption = _selected.PlacementMode
			}, (PlacementHijack)(object)new ConstructionPlacementHijack(_constructionSystem, _selected));
			UpdateGhostPlacement();
		}
		else
		{
			_placementManager.Clear();
		}
		_constructionView.BuildButtonPressed = pressed;
	}

	private void UpdateGhostPlacement()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		if (_selected != null)
		{
			if (_selected.Type != ConstructionType.Structure)
			{
				_placementManager.Clear();
				return;
			}
			ConstructionSystem entitySystem = _systemManager.GetEntitySystem<ConstructionSystem>();
			_placementManager.BeginPlacing(new PlacementInformation
			{
				IsTile = false,
				PlacementOption = _selected.PlacementMode
			}, (PlacementHijack)(object)new ConstructionPlacementHijack(entitySystem, _selected));
			_constructionView.BuildButtonPressed = true;
		}
	}

	private void OnSystemLoaded(object? sender, SystemChangedArgs args)
	{
		if (args.System is ConstructionSystem newSystem)
		{
			SystemBindingChanged(newSystem);
		}
	}

	private void OnSystemUnloaded(object? sender, SystemChangedArgs args)
	{
		if (args.System is ConstructionSystem)
		{
			SystemBindingChanged(null);
		}
	}

	private void OnViewFavoriteRecipe()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (_selected == null)
		{
			return;
		}
		if (!_favoritedRecipes.Remove(_selected))
		{
			_favoritedRecipes.Add(_selected);
		}
		if (_selectedCategory == "construction-category-favorites")
		{
			OnViewPopulateRecipes(_constructionView, (_favoritedRecipes.Count > 0) ? (search: string.Empty, catagory: "construction-category-favorites") : (search: string.Empty, catagory: string.Empty));
		}
		List<ProtoId<ConstructionPrototype>> list = new List<ProtoId<ConstructionPrototype>>(_favoritedRecipes.Count);
		foreach (ConstructionPrototype favoritedRecipe in _favoritedRecipes)
		{
			list.Add(ProtoId<ConstructionPrototype>.op_Implicit(favoritedRecipe.ID));
		}
		_preferencesManager.UpdateConstructionFavorites(list);
		PopulateInfo(_selected);
		PopulateCategories(_selectedCategory);
	}

	public void SetFavorites(IReadOnlyList<ProtoId<ConstructionPrototype>> favorites)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		_favoritedRecipes.Clear();
		ConstructionPrototype item = default(ConstructionPrototype);
		foreach (ProtoId<ConstructionPrototype> favorite in favorites)
		{
			if (_prototypeManager.TryIndex<ConstructionPrototype>(favorite, ref item))
			{
				_favoritedRecipes.Add(item);
			}
		}
		if (_selectedCategory == "construction-category-favorites")
		{
			OnViewPopulateRecipes(_constructionView, (_favoritedRecipes.Count > 0) ? (search: string.Empty, catagory: "construction-category-favorites") : (search: string.Empty, catagory: string.Empty));
		}
		PopulateCategories(_selectedCategory);
	}

	private void SystemBindingChanged(ConstructionSystem? newSystem)
	{
		if (newSystem == null)
		{
			if (_constructionSystem != null)
			{
				UnbindFromSystem();
			}
		}
		else if (_constructionSystem == null)
		{
			BindToSystem(newSystem);
		}
		else
		{
			UnbindFromSystem();
			BindToSystem(newSystem);
		}
	}

	private void BindToSystem(ConstructionSystem system)
	{
		_constructionSystem = system;
		OnViewPopulateRecipes(_constructionView, (search: string.Empty, catagory: string.Empty));
		system.ToggleCraftingWindow += SystemOnToggleMenu;
		system.FlipConstructionPrototype += SystemFlipConstructionPrototype;
		system.CraftingAvailabilityChanged += SystemCraftingAvailabilityChanged;
		system.ConstructionGuideAvailable += SystemGuideAvailable;
		if (_uiManager.GetActiveUIWidgetOrNull<GameTopMenuBar>() != null)
		{
			CraftingAvailable = system.CraftingEnabled;
		}
	}

	private void UnbindFromSystem()
	{
		ConstructionSystem? obj = _constructionSystem ?? throw new InvalidOperationException();
		obj.ToggleCraftingWindow -= SystemOnToggleMenu;
		obj.FlipConstructionPrototype -= SystemFlipConstructionPrototype;
		obj.CraftingAvailabilityChanged -= SystemCraftingAvailabilityChanged;
		obj.ConstructionGuideAvailable -= SystemGuideAvailable;
		_constructionSystem = null;
	}

	private void SystemCraftingAvailabilityChanged(object? sender, CraftingAvailabilityChangedArgs e)
	{
		if (_uiManager.ActiveScreen != null)
		{
			CraftingAvailable = e.Available;
		}
	}

	private void SystemOnToggleMenu(object? sender, EventArgs eventArgs)
	{
		if (!CraftingAvailable)
		{
			return;
		}
		if (WindowOpen)
		{
			if (IsAtFront)
			{
				WindowOpen = false;
				((BaseButton)_uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).SetClickPressed(false);
			}
			else
			{
				_constructionView.MoveToFront();
			}
		}
		else
		{
			WindowOpen = true;
			((BaseButton)_uiManager.GetActiveUIWidget<GameTopMenuBar>().CraftingButton).SetClickPressed(true);
		}
	}

	private void SystemFlipConstructionPrototype(object? sender, EventArgs eventArgs)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (_placementManager.IsActive && !_placementManager.Eraser && _selected != null && _selected.Mirror.HasValue)
		{
			IPrototypeManager prototypeManager = _prototypeManager;
			ProtoId<ConstructionPrototype>? mirror = _selected.Mirror;
			_selected = prototypeManager.Index<ConstructionPrototype>(mirror.HasValue ? ProtoId<ConstructionPrototype>.op_Implicit(mirror.GetValueOrDefault()) : null);
			UpdateGhostPlacement();
		}
	}

	private void SystemGuideAvailable(object? sender, string e)
	{
		if (CraftingAvailable && WindowOpen && _selected != null)
		{
			PopulateInfo(_selected);
		}
	}
}
