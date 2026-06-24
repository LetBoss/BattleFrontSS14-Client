using System;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI;

public sealed class CargoShuttleConsoleBoundUserInterface : BoundUserInterface
{
	[Dependency]
	private IPrototypeManager _protoManager;

	[ViewVariables]
	private CargoShuttleMenu? _menu;

	public CargoShuttleConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_menu = BoundUserInterfaceExt.CreateWindow<CargoShuttleMenu>((BoundUserInterface)(object)this);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CargoShuttleConsoleBoundUserInterfaceState cargoShuttleConsoleBoundUserInterfaceState)
		{
			_menu?.SetAccountName(cargoShuttleConsoleBoundUserInterfaceState.AccountName);
			_menu?.SetShuttleName(cargoShuttleConsoleBoundUserInterfaceState.ShuttleName);
			_menu?.SetOrders(base.EntMan.System<SpriteSystem>(), _protoManager, cargoShuttleConsoleBoundUserInterfaceState.Orders);
		}
	}
}
