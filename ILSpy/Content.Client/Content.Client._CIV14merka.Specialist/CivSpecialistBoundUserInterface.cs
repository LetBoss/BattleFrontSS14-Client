using System;
using Content.Shared._CIV14merka.Specialist;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.Specialist;

public sealed class CivSpecialistBoundUserInterface : BoundUserInterface
{
	private CivSpecialistMenu? _window;

	public CivSpecialistBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CivSpecialistMenu>((BoundUserInterface)(object)this);
		_window.OnApprove += SendApprove;
		_window.OnSetChange += SendChangeSelected;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CivSpecialistBoundUserInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	public void SendChangeSelected(int setNumber)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CivSpecialistChangeSetMessage(setNumber));
	}

	public void SendApprove()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CivSpecialistApproveMessage());
	}
}
