using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Client.Eui;
using Content.Shared._RMC14.Admin;
using Content.Shared._RMC14.Marines.Squads;
using Content.Shared._RMC14.Xenonids;
using Content.Shared._RMC14.Xenonids.Strain;
using Content.Shared.Eui;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Prototypes;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;

namespace Content.Client._RMC14.Admin;

public sealed class RMCAdminEui : BaseEui
{
	[Dependency]
	private IComponentFactory _compFactory;

	[Dependency]
	private IPrototypeManager _prototypes;

	private static readonly Comparer<EntityPrototype> EntityComparer = Comparer<EntityPrototype>.Create((EntityPrototype a, EntityPrototype b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

	private static readonly Comparer<SpeciesPrototype> SpeciesComparer = Comparer<SpeciesPrototype>.Create((SpeciesPrototype a, SpeciesPrototype b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

	private RMCAdminWindow _adminWindow;

	private RMCCreateHiveWindow? _createHiveWindow;

	private bool _isFirstState = true;

	public override void Opened()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		_adminWindow = new RMCAdminWindow();
		_adminWindow.XenoTab.HiveList.OnItemSelected += OnHiveSelected;
		((BaseButton)_adminWindow.XenoTab.CreateHiveButton).OnPressed += OnCreateHivePressed;
		SortedSet<SpeciesPrototype> sortedSet = new SortedSet<SpeciesPrototype>(SpeciesComparer);
		foreach (SpeciesPrototype item in _prototypes.EnumeratePrototypes<SpeciesPrototype>())
		{
			if (item.RoundStart)
			{
				sortedSet.Add(item);
			}
		}
		RMCTransformRow rMCTransformRow = new RMCTransformRow();
		rMCTransformRow.Label.Text = Loc.GetString("rmc-ui-humanoid");
		((Control)_adminWindow.TransformTab.Container).AddChild((Control)(object)rMCTransformRow);
		int num = 0;
		foreach (SpeciesPrototype species in sortedSet)
		{
			if (num > 0 && num % 5 == 0)
			{
				((Control)rMCTransformRow.Container).Margin = default(Thickness);
				rMCTransformRow = new RMCTransformRow();
				((Control)rMCTransformRow.Label).Visible = false;
				((Control)rMCTransformRow.Separator).Visible = false;
				((Control)_adminWindow.TransformTab.Container).AddChild((Control)(object)rMCTransformRow);
			}
			RMCTransformButton rMCTransformButton = new RMCTransformButton
			{
				Type = TransformType.Humanoid
			};
			rMCTransformButton.TransformName.Text = Loc.GetString(species.Name);
			((BaseButton)rMCTransformButton).OnPressed += delegate
			{
				SendMessage(new RMCAdminTransformHumanoidMsg(species.ID));
			};
			((Control)rMCTransformRow.Container).AddChild((Control)(object)rMCTransformButton);
			num++;
		}
		SortedDictionary<int, SortedSet<EntityPrototype>> sortedDictionary = new SortedDictionary<int, SortedSet<EntityPrototype>>();
		XenoComponent xenoComponent = default(XenoComponent);
		foreach (EntityPrototype item2 in _prototypes.EnumeratePrototypes<EntityPrototype>())
		{
			if (!item2.Abstract && item2.TryGetComponent<XenoComponent>(ref xenoComponent, _compFactory) && !item2.HasComponent<XenoStrainComponent>(_compFactory) && !item2.HasComponent<XenoHiddenComponent>(_compFactory))
			{
				if (!sortedDictionary.TryGetValue(xenoComponent.Tier, out var value))
				{
					value = new SortedSet<EntityPrototype>(EntityComparer);
					sortedDictionary.Add(xenoComponent.Tier, value);
				}
				value.Add(item2);
			}
		}
		foreach (KeyValuePair<int, SortedSet<EntityPrototype>> item3 in sortedDictionary)
		{
			item3.Deconstruct(out var key, out var value2);
			int num2 = key;
			SortedSet<EntityPrototype> sortedSet2 = value2;
			RMCTransformRow rMCTransformRow2 = new RMCTransformRow();
			rMCTransformRow2.Label.Text = Loc.GetString("rmc-ui-tier", new(string, object)[1] { ("tier", num2) });
			foreach (EntityPrototype xeno in sortedSet2)
			{
				RMCTransformButton rMCTransformButton2 = new RMCTransformButton
				{
					Type = TransformType.Xeno
				};
				rMCTransformButton2.TransformName.Text = xeno.Name;
				((Control)rMCTransformRow2.Container).AddChild((Control)(object)rMCTransformButton2);
				((BaseButton)rMCTransformButton2).OnPressed += delegate
				{
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					SendMessage(new RMCAdminTransformXenoMsg(EntProtoId.op_Implicit(xeno.ID)));
				};
			}
			((Control)_adminWindow.TransformTab.Container).AddChild((Control)(object)rMCTransformRow2);
		}
		_adminWindow.MarineTab.PointsSpinBox.ValueChanged += delegate(ValueChangedEventArgs args)
		{
			SendMessage(new RMCAdminSetVendorPointsMsg(args.Value));
		};
		_adminWindow.MarineTab.SpecialistPointsSpinBox.ValueChanged += delegate(ValueChangedEventArgs args)
		{
			SendMessage(new RMCAdminSetSpecialistVendorPointsMsg(args.Value));
		};
		((BaseWindow)_adminWindow).OpenCentered();
	}

	private void OnHiveSelected(ItemListSelectedEventArgs args)
	{
		RMCAdminChangeHiveMsg msg = new RMCAdminChangeHiveMsg((Hive)((ItemListEventArgs)args).ItemList[args.ItemIndex].Metadata);
		SendMessage(msg);
	}

	private void OnCreateHivePressed(ButtonEventArgs args)
	{
		if (_createHiveWindow != null)
		{
			((BaseWindow)_createHiveWindow).RecenterWindow(new Vector2(0.5f, 0.5f));
			return;
		}
		_createHiveWindow = new RMCCreateHiveWindow();
		((BaseWindow)_createHiveWindow).OnClose += OnCreateHiveClosed;
		_createHiveWindow.HiveName.OnTextEntered += OnCreateHiveEntered;
		((BaseWindow)_createHiveWindow).OpenCentered();
	}

	private void OnCreateHiveClosed()
	{
		RMCCreateHiveWindow? createHiveWindow = _createHiveWindow;
		if (createHiveWindow != null)
		{
			((BaseWindow)createHiveWindow).Close();
		}
		_createHiveWindow = null;
	}

	private void OnCreateHiveEntered(LineEditEventArgs args)
	{
		RMCAdminCreateHiveMsg msg = new RMCAdminCreateHiveMsg(args.Text);
		SendMessage(msg);
		RMCCreateHiveWindow? createHiveWindow = _createHiveWindow;
		if (createHiveWindow != null)
		{
			((BaseWindow)createHiveWindow).Close();
		}
	}

	public override void HandleState(EuiStateBase state)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		if (!(state is RMCAdminEuiTargetState rMCAdminEuiTargetState))
		{
			return;
		}
		_adminWindow.XenoTab.HiveList.Clear();
		foreach (Hive hive in rMCAdminEuiTargetState.Hives)
		{
			ItemList hiveList = _adminWindow.XenoTab.HiveList;
			hiveList.Add(new Item(hiveList)
			{
				Text = hive.Name,
				Metadata = hive
			});
		}
		((Control)_adminWindow.MarineTab.SpecialistSkills).DisposeAllChildren();
		foreach (var comp in rMCAdminEuiTargetState.SpecialistSkills)
		{
			Button val = new Button
			{
				Text = comp.Name,
				ToggleMode = true,
				StyleClasses = { "OpenBoth" }
			};
			((BaseButton)val).Pressed = comp.Present;
			((BaseButton)val).OnPressed += delegate(ButtonEventArgs args)
			{
				if (args.Button.Pressed)
				{
					SendMessage(new RMCAdminAddSpecSkillMsg(comp.Name));
				}
				else
				{
					SendMessage(new RMCAdminRemoveSpecSkillMsg(comp.Name));
				}
			};
			((Control)_adminWindow.MarineTab.SpecialistSkills).AddChild((Control)(object)val);
		}
		if (_isFirstState)
		{
			_adminWindow.MarineTab.PointsSpinBox.OverrideValue(rMCAdminEuiTargetState.Points);
			int valueOrDefault = rMCAdminEuiTargetState.ExtraPoints.GetValueOrDefault("Specialist");
			_adminWindow.MarineTab.SpecialistPointsSpinBox.OverrideValue(valueOrDefault);
		}
		((Control)_adminWindow.MarineTab.Squads).DisposeAllChildren();
		EntityPrototype val2 = default(EntityPrototype);
		SquadTeamComponent squadTeamComponent = default(SquadTeamComponent);
		foreach (Squad squad in rMCAdminEuiTargetState.Squads)
		{
			RMCSquadRow rMCSquadRow = new RMCSquadRow();
			((Control)rMCSquadRow).HorizontalExpand = true;
			((Control)rMCSquadRow).Margin = new Thickness(0f, 0f, 0f, 10f);
			RMCSquadRow rMCSquadRow2 = rMCSquadRow;
			((BaseButton)rMCSquadRow2.AddToSquadButton).OnPressed += delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				SendMessage(new RMCAdminAddToSquadMsg(squad.Id));
			};
			string name = string.Empty;
			Color color = Color.White;
			if (_prototypes.TryIndex(squad.Id, ref val2))
			{
				name = val2.Name;
				if (val2.TryGetComponent<SquadTeamComponent>(ref squadTeamComponent, _compFactory))
				{
					color = squadTeamComponent.Color;
				}
			}
			rMCSquadRow2.CreateSquadButton(squad.Exists, delegate
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				SendMessage(new RMCAdminCreateSquadMsg(squad.Id));
			}, squad.Members, name, color);
			((Control)_adminWindow.MarineTab.Squads).AddChild((Control)(object)rMCSquadRow2);
		}
		_isFirstState = false;
	}

	public override void Closed()
	{
		((BaseWindow)_adminWindow).Close();
	}
}
