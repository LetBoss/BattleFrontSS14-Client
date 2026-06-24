using System;
using System.Collections.Generic;
using Content.Shared._RMC14.Intel;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Maths;

namespace Content.Client._RMC14.Intel;

public sealed class ViewIntelObjectivesBui : BoundUserInterface
{
	private ViewIntelObjectivesWindow? _window;

	public ViewIntelObjectivesBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ViewIntelObjectivesWindow>((BoundUserInterface)(object)this);
		Refresh();
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Expected O, but got Unknown
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Expected O, but got Unknown
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Expected O, but got Unknown
		//IL_03c0: Unknown result type (might be due to invalid IL or missing references)
		ViewIntelObjectivesWindow window = _window;
		ViewIntelObjectivesComponent viewIntelObjectivesComponent = default(ViewIntelObjectivesComponent);
		if (window == null || !((BaseWindow)window).IsOpen || !base.EntMan.TryGetComponent<ViewIntelObjectivesComponent>(((BoundUserInterface)this).Owner, ref viewIntelObjectivesComponent))
		{
			return;
		}
		IntelTechTree tree = viewIntelObjectivesComponent.Tree;
		_window.CurrentPointsLabel.Text = Loc.GetString("rmc-ui-intel-points-value", new(string, object)[1] { ("value", tree.Points.Double().ToString("F1")) });
		_window.CurrentTierLabel.Text = Loc.GetString("rmc-ui-intel-tier-value", new(string, object)[1] { ("value", tree.Tier) });
		_window.TotalPointsLabel.Text = Loc.GetString("rmc-ui-intel-total-credits", new(string, object)[1] { ("value", tree.TotalEarned.Double().ToString("F1")) });
		_window.DocumentsLabel.Text = Loc.GetString("rmc-ui-intel-progress", new(string, object)[2]
		{
			("current", tree.Documents.Current),
			("total", tree.Documents.Total)
		});
		_window.RetrieveItemsLabel.Text = Loc.GetString("rmc-ui-intel-progress", new(string, object)[2]
		{
			("current", tree.RetrieveItems.Current),
			("total", tree.RetrieveItems.Total)
		});
		_window.RescueSurvivorsLabel.Text = Loc.GetString("rmc-ui-intel-infinite-progress", new(string, object)[1] { ("current", tree.RescueSurvivors) });
		_window.RecoverCorpsesLabel.Text = Loc.GetString("rmc-ui-intel-infinite-progress", new(string, object)[1] { ("current", tree.RecoverCorpses) });
		_window.ColonyCommunicationsLabel.Text = Loc.GetString("rmc-ui-intel-colony-status", new(string, object)[1] { ("online", tree.ColonyCommunications) });
		_window.ColonyPowerLabel.Text = Loc.GetString("rmc-ui-intel-colony-status", new(string, object)[1] { ("online", tree.ColonyPower) });
		((Control)_window.CluesContainer).DisposeAllChildren();
		foreach (KeyValuePair<LocId, Dictionary<NetEntity, string>> clue in viewIntelObjectivesComponent.Tree.Clues)
		{
			clue.Deconstruct(out var key, out var value);
			LocId val = key;
			Dictionary<NetEntity, string> dictionary = value;
			ScrollContainer val2 = new ScrollContainer
			{
				HScrollEnabled = false,
				VScrollEnabled = true
			};
			BoxContainer val3 = new BoxContainer
			{
				Orientation = (LayoutOrientation)1,
				Margin = new Thickness(4f)
			};
			foreach (var (_, text2) in dictionary)
			{
				((Control)val3).AddChild((Control)new Label
				{
					Text = text2,
					Margin = new Thickness(2f, 1f, 2f, 1f),
					StyleClasses = { "Label" }
				});
			}
			((Control)val2).AddChild((Control)(object)val3);
			((Control)_window.CluesContainer).AddChild((Control)(object)val2);
			TabContainer.SetTabTitle((Control)(object)val2, Loc.GetString(LocId.op_Implicit(val)));
		}
	}
}
