using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Robust.Client.Placement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;

namespace Content.Client._PUBG.Sponsor;

public sealed class SponsorEntitySpawningUIController : UIController
{
	[Dependency]
	private IConfigurationManager _cfg;

	[Dependency]
	private IPlacementManager _placement;

	[Dependency]
	private IPrototypeManager _prototypes;

	private EntitySpawnWindow? _window;

	private readonly List<EntityPrototype> _shownEntities = new List<EntityPrototype>();

	private bool _init;

	private bool _allowEraseEntities;

	private HashSet<string> _disallowedEntityIds = new HashSet<string>();

	private (int start, int end) _lastEntityIndices;

	public override void Initialize()
	{
		_init = true;
		_placement.DirectionChanged += OnDirectionChanged;
		_placement.PlacementChanged += ClearSelection;
	}

	public void UpdatePermissions(SponsorSandboxState state)
	{
		_allowEraseEntities = state.AllowEraseEntities;
		_disallowedEntityIds = new HashSet<string>(state.DisallowedEntityIds.Select((string id) => id.ToLowerInvariant()));
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseButton)_window.EraseButton).Disabled = !_allowEraseEntities;
			if (!_allowEraseEntities && ((BaseButton)_window.EraseButton).Pressed)
			{
				((BaseButton)_window.EraseButton).Pressed = false;
				_placement.Clear();
			}
			BuildEntityList(_window.SearchBar.Text);
		}
	}

	public void ToggleWindow()
	{
		EnsureWindow();
		if (((BaseWindow)_window).IsOpen)
		{
			((BaseWindow)_window).Close();
			return;
		}
		((BaseWindow)_window).Open();
		UpdateEntityDirectionLabel();
		((Control)_window.SearchBar).GrabKeyboardFocus();
	}

	public void CloseWindow()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseWindow)_window).Close();
		}
	}

	private void EnsureWindow()
	{
		EntitySpawnWindow window = _window;
		if (window == null || ((Control)window).Disposed)
		{
			_window = base.UIManager.CreateWindow<EntitySpawnWindow>();
			LayoutContainer.SetAnchorPreset((Control)(object)_window, (LayoutPreset)4, false);
			((BaseWindow)_window).OnClose += WindowClosed;
			((BaseButton)_window.ReplaceButton).Pressed = _placement.Replacement;
			((BaseButton)_window.ReplaceButton).OnToggled += OnEntityReplaceToggled;
			((BaseButton)_window.EraseButton).Pressed = _placement.Eraser;
			((BaseButton)_window.EraseButton).OnToggled += OnEntityEraseToggled;
			((BaseButton)_window.EraseButton).Disabled = !_allowEraseEntities;
			_window.OverrideMenu.OnItemSelected += OnEntityOverrideSelected;
			_window.SearchBar.OnTextChanged += OnEntitySearchChanged;
			((BaseButton)_window.ClearButton).OnPressed += OnEntityClearPressed;
			_window.PrototypeScrollContainer.OnScrolled += UpdateVisiblePrototypes;
			((Control)_window).OnResized += UpdateVisiblePrototypes;
			BuildEntityList();
		}
	}

	private void WindowClosed()
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			if (_window.SelectedButton != null)
			{
				((BaseButton)_window.SelectedButton.ActualButton).Pressed = false;
				_window.SelectedButton = null;
			}
			_placement.Clear();
		}
	}

	private void OnEntityReplaceToggled(ButtonToggledEventArgs args)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			if (args.Pressed)
			{
				IPlacementManager placement = _placement;
				placement.Replacement = !placement.Replacement;
			}
			((ButtonEventArgs)args).Button.Pressed = args.Pressed;
		}
	}

	private void OnEntityEraseToggled(ButtonToggledEventArgs args)
	{
		if (_window == null || ((Control)_window).Disposed)
		{
			return;
		}
		if (!_allowEraseEntities)
		{
			((ButtonEventArgs)args).Button.Pressed = false;
			return;
		}
		_placement.Clear();
		if (args.Pressed)
		{
			_placement.ToggleEraser();
		}
		((ButtonEventArgs)args).Button.Pressed = args.Pressed;
		((BaseButton)_window.OverrideMenu).Disabled = args.Pressed;
	}

	private void ClearSelection(object? sender, EventArgs e)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			if (_window.SelectedButton != null)
			{
				((BaseButton)_window.SelectedButton.ActualButton).Pressed = false;
				_window.SelectedButton = null;
			}
			((BaseButton)_window.EraseButton).Pressed = false;
			((BaseButton)_window.OverrideMenu).Disabled = false;
		}
	}

	private void OnEntityOverrideSelected(ItemSelectedEventArgs args)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		if (_window != null && !((Control)_window).Disposed)
		{
			_window.OverrideMenu.SelectId(args.Id);
			if (_placement.CurrentMode != null)
			{
				PlacementInformation val = new PlacementInformation
				{
					PlacementOption = _placement.AllModeNames[args.Id],
					EntityType = _placement.CurrentPermission.EntityType,
					Range = 2,
					IsTile = _placement.CurrentPermission.IsTile
				};
				_placement.Clear();
				_placement.BeginPlacing(val, (PlacementHijack)null);
			}
		}
	}

	private void OnEntitySearchChanged(LineEditEventArgs args)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_placement.Clear();
			BuildEntityList(args.Text);
			((BaseButton)_window.ClearButton).Disabled = string.IsNullOrEmpty(args.Text);
		}
	}

	private void OnEntityClearPressed(ButtonEventArgs args)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			_placement.Clear();
			_window.SearchBar.Clear();
			BuildEntityList(string.Empty);
		}
	}

	private void BuildEntityList(string? searchStr = null)
	{
		if (_window == null || ((Control)_window).Disposed)
		{
			return;
		}
		_shownEntities.Clear();
		((Control)_window.PrototypeList).RemoveAllChildren();
		_lastEntityIndices = (start: 0, end: -1);
		((Control)_window.PrototypeList).RemoveAllChildren();
		_window.SelectedButton = null;
		searchStr = searchStr?.ToLowerInvariant();
		string cVar = _cfg.GetCVar<string>(CVars.EntitiesCategoryFilter);
		EntityCategoryPrototype val = default(EntityCategoryPrototype);
		_prototypes.TryIndex<EntityCategoryPrototype>(cVar, ref val);
		foreach (EntityPrototype item in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (!item.Abstract && !item.HideSpawnMenu && !_disallowedEntityIds.Contains(item.ID.ToLowerInvariant()) && (val == null || item.Categories.Contains(val)) && (searchStr == null || DoesEntityMatchSearch(item, searchStr)))
			{
				_shownEntities.Add(item);
			}
		}
		_shownEntities.Sort(delegate(EntityPrototype a, EntityPrototype b)
		{
			int num = string.Compare(a.Name, b.Name, StringComparison.Ordinal);
			return (num == 0) ? string.Compare(a.EditorSuffix, b.EditorSuffix, StringComparison.Ordinal) : num;
		});
		_window.PrototypeList.TotalItemCount = _shownEntities.Count;
		_window.PrototypeScrollContainer.SetScrollValue(new Vector2(0f, 0f));
		UpdateVisiblePrototypes();
	}

	private static bool DoesEntityMatchSearch(EntityPrototype prototype, string searchStr)
	{
		if (string.IsNullOrEmpty(searchStr))
		{
			return true;
		}
		if (prototype.ID.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		if (prototype.EditorSuffix != null && prototype.EditorSuffix.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		if (string.IsNullOrEmpty(prototype.Name))
		{
			return false;
		}
		if (prototype.Name.Contains(searchStr, StringComparison.InvariantCultureIgnoreCase))
		{
			return true;
		}
		return false;
	}

	private void UpdateEntityDirectionLabel()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null && !((Control)_window).Disposed)
		{
			_window.RotationLabel.Text = ((object)_placement.Direction/*cast due to constrained. prefix*/).ToString();
		}
	}

	private void OnDirectionChanged(object? sender, EventArgs e)
	{
		UpdateEntityDirectionLabel();
	}

	private void UpdateVisiblePrototypes()
	{
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		if (_window == null || ((Control)_window).Disposed)
		{
			return;
		}
		float num = ((Control)_window.MeasureButton).DesiredSize.Y + 2f;
		int num2 = (int)Math.Floor(Math.Max(0f - ((Control)_window.PrototypeList).Position.Y, 0f) / num);
		_window.PrototypeList.ItemOffset = num2;
		(int start, int end) lastEntityIndices = _lastEntityIndices;
		int item = lastEntityIndices.start;
		int item2 = lastEntityIndices.end;
		int num3 = num2 - 1;
		float num4 = 0f - num;
		while (num4 < ((Control)_window.PrototypeList).Parent.Height)
		{
			num4 += num;
			num3++;
		}
		num3 = Math.Min(num3, _shownEntities.Count - 1);
		if (num3 != item2 || num2 != item)
		{
			_lastEntityIndices = (start: num2, end: num3);
			for (int i = item; i < num2 && i <= item2; i++)
			{
				EntitySpawnButton val = (EntitySpawnButton)((Control)_window.PrototypeList).GetChild(0);
				((Control)_window.PrototypeList).RemoveChild((Control)(object)val);
			}
			int num5 = item2;
			while (num5 > num3 && num5 >= item)
			{
				EntitySpawnButton val2 = (EntitySpawnButton)((Control)_window.PrototypeList).GetChild(((Control)_window.PrototypeList).ChildCount - 1);
				((Control)_window.PrototypeList).RemoveChild((Control)(object)val2);
				num5--;
			}
			for (int num6 = Math.Min(item - 1, num3); num6 >= num2; num6--)
			{
				InsertEntityButton(_shownEntities[num6], insertFirst: true, num6);
			}
			for (int j = Math.Max(item2 + 1, num2); j <= num3; j++)
			{
				InsertEntityButton(_shownEntities[j], insertFirst: false, j);
			}
		}
	}

	private void InsertEntityButton(EntityPrototype prototype, bool insertFirst, int index)
	{
		if (_window != null && !((Control)_window).Disposed)
		{
			((BaseButton)_window.InsertEntityButton(prototype, insertFirst, index).ActualButton).OnToggled += OnEntityButtonToggled;
		}
	}

	private void OnEntityButtonToggled(ButtonToggledEventArgs args)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		if (_window == null || ((Control)_window).Disposed)
		{
			return;
		}
		EntitySpawnButton val = (EntitySpawnButton)((Control)((ButtonEventArgs)args).Button).Parent;
		if (_window.SelectedButton == val)
		{
			_window.SelectedButton = null;
			_window.SelectedPrototype = null;
			_placement.Clear();
			return;
		}
		if (_window.SelectedButton != null)
		{
			((BaseButton)_window.SelectedButton.ActualButton).Pressed = false;
		}
		_window.SelectedButton = null;
		_window.SelectedPrototype = null;
		string text = _placement.AllModeNames[_window.OverrideMenu.SelectedId];
		PlacementInformation val2 = new PlacementInformation
		{
			PlacementOption = ((text != "Default") ? text : val.Prototype.PlacementMode),
			EntityType = val.PrototypeID,
			Range = 2,
			IsTile = false
		};
		_placement.BeginPlacing(val2, (PlacementHijack)null);
		_window.SelectedButton = val;
		_window.SelectedPrototype = val.Prototype;
	}
}
