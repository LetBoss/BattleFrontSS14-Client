using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Roles.FindParasite;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Roles.FindParasite;

public sealed class FindParasiteBoundUserInterface : BoundUserInterface
{
	private Item? _selectedItem;

	private bool _impledDeselect;

	private ItemList? _spawnerList;

	[ViewVariables]
	private FindParasiteWindow? _window;

	public FindParasiteBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<FindParasiteWindow>((BoundUserInterface)(object)this);
		_spawnerList = _window.ParasiteSpawners;
		Button spawnButton = _window.SpawnButton;
		_spawnerList.OnItemSelected += OnItemSelect;
		_spawnerList.OnItemDeselected += OnItemDeselect;
		spawnButton.Text = Loc.GetString("xeno-ui-find-parasite-spawn-button");
		((BaseButton)spawnButton).Disabled = true;
		((BaseButton)spawnButton).OnButtonDown += delegate(ButtonEventArgs args)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			if (_selectedItem == null)
			{
				args.Button.Disabled = true;
			}
			else
			{
				NetEntity spawner = (NetEntity)_selectedItem.Metadata;
				TakeParasiteRole(spawner);
				((BoundUserInterface)this).Close();
			}
		};
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		((BoundUserInterface)this).UpdateState(state);
		if (!(state is FindParasiteUIState findParasiteUIState) || _spawnerList == null)
		{
			return;
		}
		List<SpawnerData> activeParasiteSpawners = findParasiteUIState.ActiveParasiteSpawners;
		_spawnerList.Clear();
		foreach (SpawnerData item in activeParasiteSpawners)
		{
			Item val = new Item(_spawnerList)
			{
				Text = item.Name,
				Metadata = item.Spawner
			};
			_spawnerList.Add(val);
		}
	}

	private void OnItemSelect(ItemListSelectedEventArgs args)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		((BaseButton)_window.SpawnButton).Disabled = false;
		Item val = ((ItemListEventArgs)args).ItemList[args.ItemIndex];
		NetEntity val2 = (NetEntity)val.Metadata;
		if (_selectedItem == null)
		{
			FollowParasiteSpawner(val2);
			_selectedItem = val;
			return;
		}
		NetEntity val3 = (NetEntity)_selectedItem.Metadata;
		if (val2 == val3)
		{
			TakeParasiteRole(val3);
			((BoundUserInterface)this).Close();
			return;
		}
		_impledDeselect = true;
		_selectedItem.Selected = false;
		_selectedItem = val;
		FollowParasiteSpawner(val2);
	}

	private void OnItemDeselect(ItemListDeselectedEventArgs args)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		NetEntity val = (NetEntity)((ItemListEventArgs)args).ItemList[args.ItemIndex].Metadata;
		if (_selectedItem == null)
		{
			return;
		}
		if (_impledDeselect)
		{
			_impledDeselect = false;
			return;
		}
		NetEntity val2 = (NetEntity)_selectedItem.Metadata;
		if (val == val2)
		{
			TakeParasiteRole(val2);
			((BoundUserInterface)this).Close();
		}
	}

	public void FollowParasiteSpawner(NetEntity spawner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new FollowParasiteSpawnerMessage(spawner));
	}

	public void TakeParasiteRole(NetEntity spawner)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new TakeParasiteRoleMessage(spawner));
	}
}
