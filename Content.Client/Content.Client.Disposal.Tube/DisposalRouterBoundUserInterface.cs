using System;
using Content.Shared.Disposal.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Disposal.Tube;

public sealed class DisposalRouterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private DisposalRouterWindow? _window;

	public DisposalRouterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<DisposalRouterWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.Confirm).OnPressed += delegate
		{
			ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, _window.TagInput.Text);
		};
		_window.TagInput.OnTextEntered += delegate(LineEditEventArgs args)
		{
			ButtonPressed(SharedDisposalRouterComponent.UiAction.Ok, args.Text);
		};
	}

	private void ButtonPressed(SharedDisposalRouterComponent.UiAction action, string tag)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SharedDisposalRouterComponent.UiActionMessage(action, tag));
		((BoundUserInterface)this).Close();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is SharedDisposalRouterComponent.DisposalRouterUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
