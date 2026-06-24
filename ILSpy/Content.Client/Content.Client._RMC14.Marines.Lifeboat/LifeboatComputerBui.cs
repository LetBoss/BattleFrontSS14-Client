using System;
using Content.Shared._RMC14.Evacuation;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;

namespace Content.Client._RMC14.Marines.Lifeboat;

public sealed class LifeboatComputerBui : BoundUserInterface
{
	private LifeboatComputerWindow? _window;

	public LifeboatComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<LifeboatComputerWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.EmergencyLaunchButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new LifeboatComputerLaunchBuiMsg());
		};
		((BaseButton)_window.NoButton).OnPressed += delegate
		{
			((BaseWindow)_window).Close();
		};
		((BaseButton)_window.YesButton).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new LifeboatComputerLaunchBuiMsg());
		};
	}
}
