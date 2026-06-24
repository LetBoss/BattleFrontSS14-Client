using System;
using Content.Shared._PUBG.Console;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client._PUBG.Console.UI;

public sealed class PubgWeaponVendingConsoleBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private PubgWeaponVendingConsoleWindow? _window;

	public PubgWeaponVendingConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = new PubgWeaponVendingConsoleWindow();
		((BaseWindow)_window).OpenCentered();
		((BaseWindow)_window).OnClose += base.Close;
		PubgWeaponVendingConsoleWindow? window = _window;
		window.OnItemSelected = (Action<string>)Delegate.Combine(window.OnItemSelected, (Action<string>)delegate(string itemId)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PubgWeaponVendingDispenseMessage(itemId));
		});
		PubgWeaponVendingConsoleComponent pubgWeaponVendingConsoleComponent = default(PubgWeaponVendingConsoleComponent);
		PubgWeaponVendingInventoryPrototype inventory = default(PubgWeaponVendingInventoryPrototype);
		if (base.EntMan.TryGetComponent<PubgWeaponVendingConsoleComponent>(((BoundUserInterface)this).Owner, ref pubgWeaponVendingConsoleComponent) && IoCManager.Resolve<IPrototypeManager>().TryIndex<PubgWeaponVendingInventoryPrototype>(pubgWeaponVendingConsoleComponent.InventoryPrototype, ref inventory))
		{
			_window.Populate(inventory);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			PubgWeaponVendingConsoleWindow? window = _window;
			if (window != null)
			{
				((Control)window).Dispose();
			}
			_window = null;
		}
	}
}
