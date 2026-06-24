using System;
using System.Collections.Generic;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Prototypes;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI;

public sealed class FundingAllocationConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private FundingAllocationMenu? _menu;

	public FundingAllocationConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<FundingAllocationMenu>((BoundUserInterface)(object)this);
		_menu.OnSavePressed += delegate(Dictionary<ProtoId<CargoAccountPrototype>, int> dicts, double primary, double lockbox)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SetFundingAllocationBuiMessage(dicts, primary, lockbox));
		};
	}

	protected override void UpdateState(BoundUserInterfaceState message)
	{
		((BoundUserInterface)this).UpdateState(message);
		if (message is FundingAllocationConsoleBuiState state)
		{
			_menu?.Update(state);
		}
	}
}
