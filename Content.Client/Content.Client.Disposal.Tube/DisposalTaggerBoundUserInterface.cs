using System;
using Content.Shared.Disposal.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Disposal.Tube;

public sealed class DisposalTaggerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private DisposalTaggerWindow? _window;

	public DisposalTaggerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<DisposalTaggerWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.Confirm).OnPressed += delegate
		{
			ButtonPressed(SharedDisposalTaggerComponent.UiAction.Ok, _window.TagInput.Text);
		};
		_window.TagInput.OnTextEntered += delegate(LineEditEventArgs args)
		{
			ButtonPressed(SharedDisposalTaggerComponent.UiAction.Ok, args.Text);
		};
	}

	private void ButtonPressed(SharedDisposalTaggerComponent.UiAction action, string tag)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SharedDisposalTaggerComponent.UiActionMessage(action, tag));
		((BoundUserInterface)this).Close();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is SharedDisposalTaggerComponent.DisposalTaggerUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}
}
