using System;
using Content.Shared.Ame.Components;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Ame.UI;

public sealed class AmeControllerBoundUserInterface : BoundUserInterface
{
	private AmeWindow? _window;

	public AmeControllerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<AmeWindow>((BoundUserInterface)(object)this);
		_window.OnAmeButton += ButtonPressed;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		AmeControllerBoundUserInterfaceState state2 = (AmeControllerBoundUserInterfaceState)(object)state;
		_window?.UpdateState((BoundUserInterfaceState)(object)state2);
	}

	public void ButtonPressed(UiButton button)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new UiButtonPressedMessage(button));
	}
}
