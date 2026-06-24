using System;
using Content.Client.Message;
using Content.Shared._RMC14.Dropship;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Dropship;

public sealed class DropshipHijackerBui : BoundUserInterface
{
	[ViewVariables]
	private DropshipHijackerWindow? _window;

	public DropshipHijackerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		if (((BoundUserInterface)this).State is DropshipHijackerBuiState s)
		{
			Set(s);
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		if (state is DropshipHijackerBuiState s)
		{
			Set(s);
		}
	}

	private void Set(DropshipHijackerBuiState s)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		if (_window == null)
		{
			_window = BoundUserInterfaceExt.CreateWindow<DropshipHijackerWindow>((BoundUserInterface)(object)this);
			_window.Header.SetMarkup("[bold]Where to 'land'?[/bold]");
		}
		((Control)_window.Destinations).DisposeAllChildren();
		foreach (var destination in s.Destinations)
		{
			NetEntity id = destination.Id;
			string item = destination.Name;
			Button val = new Button
			{
				Text = item,
				StyleClasses = { "OpenBoth" }
			};
			((BaseButton)val).OnPressed += delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipHijackerDestinationChosenBuiMsg(id));
				((BoundUserInterface)this).Close();
			};
			((Control)_window.Destinations).AddChild((Control)(object)val);
		}
	}
}
