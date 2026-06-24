using System;
using Content.Shared.Thief;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Thief;

public sealed class ThiefBackpackBoundUserInterface : BoundUserInterface
{
	private ThiefBackpackMenu? _window;

	public ThiefBackpackBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<ThiefBackpackMenu>((BoundUserInterface)(object)this);
		_window.OnApprove += SendApprove;
		_window.OnSetChange += SendChangeSelected;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is ThiefBackpackBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	public void SendChangeSelected(int setNumber)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ThiefBackpackChangeSetMessage(setNumber));
	}

	public void SendApprove()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new ThiefBackpackApproveMessage());
	}
}
