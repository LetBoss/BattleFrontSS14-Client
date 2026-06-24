using System;
using Content.Shared.Anomaly;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Anomaly.Ui;

public sealed class AnomalyGeneratorBoundUserInterface : BoundUserInterface
{
	private AnomalyGeneratorWindow? _window;

	public AnomalyGeneratorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<AnomalyGeneratorWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(((BoundUserInterface)this).Owner);
		AnomalyGeneratorWindow? window = _window;
		window.OnGenerateButtonPressed = (Action)Delegate.Combine(window.OnGenerateButtonPressed, (Action)delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new AnomalyGeneratorGenerateButtonPressedEvent());
		});
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is AnomalyGeneratorUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
