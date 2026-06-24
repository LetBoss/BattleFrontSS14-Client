using System;
using System.Collections.Generic;
using Content.Client.Cargo.UI;
using Content.Shared.Cargo;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Components;
using Content.Shared.Cargo.Events;
using Content.Shared.Cargo.Prototypes;
using Content.Shared.IdentityManagement;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client.Cargo.BUI;

public sealed class CargoOrderConsoleBoundUserInterface : BoundUserInterface
{
	private readonly SharedCargoSystem _cargoSystem;

	[ViewVariables]
	private CargoConsoleMenu? _menu;

	[ViewVariables]
	private CargoConsoleOrderMenu? _orderMenu;

	[ViewVariables]
	private CargoProductPrototype? _product;

	[ViewVariables]
	public string? AccountName { get; private set; }

	[ViewVariables]
	public int BankBalance { get; private set; }

	[ViewVariables]
	public int OrderCapacity { get; private set; }

	[ViewVariables]
	public int OrderCount { get; private set; }

	public CargoOrderConsoleBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_cargoSystem = base.EntMan.System<SharedCargoSystem>();
	}

	protected override void Open()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		SpriteSystem spriteSystem = base.EntMan.System<SpriteSystem>();
		IDependencyCollection instance = IoCManager.Instance;
		_menu = new CargoConsoleMenu(((BoundUserInterface)this).Owner, base.EntMan, instance.Resolve<IPrototypeManager>(), spriteSystem);
		EntityUid? localEntity = ((ISharedPlayerManager)instance.Resolve<IPlayerManager>()).LocalEntity;
		FormattedMessage description = new FormattedMessage();
		string orderRequester;
		if (base.EntMan.EntityExists(localEntity))
		{
			orderRequester = Identity.Name(localEntity.Value, base.EntMan);
		}
		else
		{
			orderRequester = string.Empty;
		}
		_orderMenu = new CargoConsoleOrderMenu();
		((BaseWindow)_menu).OnClose += base.Close;
		_menu.OnItemSelected += delegate(ButtonEventArgs args)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (((Control)args.Button).Parent is CargoProductRow cargoProductRow)
			{
				description.Clear();
				description.PushColor(Color.White);
				if (((Control)cargoProductRow.MainButton).ToolTip != null)
				{
					description.AddText(((Control)cargoProductRow.MainButton).ToolTip);
				}
				_orderMenu.Description.SetMessage(description, (Color?)null);
				_product = cargoProductRow.Product;
				_orderMenu.ProductName.Text = cargoProductRow.ProductName.Text;
				_orderMenu.PointCost.Text = cargoProductRow.PointCost.Text;
				_orderMenu.Requester.Text = orderRequester;
				_orderMenu.Reason.Text = "";
				_orderMenu.Amount.Value = 1;
				((BaseWindow)_orderMenu).OpenCentered();
			}
		};
		_menu.OnOrderApproved += ApproveOrder;
		_menu.OnOrderCanceled += RemoveOrder;
		((BaseButton)_orderMenu.SubmitButton).OnPressed += delegate
		{
			if (AddOrder())
			{
				((BaseWindow)_orderMenu).Close();
			}
		};
		_menu.OnAccountAction += delegate(ProtoId<CargoAccountPrototype>? account, int amount)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoConsoleWithdrawFundsMessage(account, amount));
		};
		_menu.OnToggleUnboundedLimit += delegate
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoConsoleToggleLimitMessage());
		};
		((BaseWindow)_menu).OpenCentered();
	}

	private void Populate(List<CargoOrderData> orders)
	{
		if (_menu != null)
		{
			_menu.PopulateProducts();
			_menu.PopulateCategories();
			_menu.PopulateOrders(orders);
			_menu.PopulateAccountActions();
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		CargoOrderConsoleComponent cargoOrderConsoleComponent = default(CargoOrderConsoleComponent);
		if (state is CargoConsoleInterfaceState cargoConsoleInterfaceState && base.EntMan.TryGetComponent<CargoOrderConsoleComponent>(((BoundUserInterface)this).Owner, ref cargoOrderConsoleComponent))
		{
			EntityUid entity = base.EntMan.GetEntity(cargoConsoleInterfaceState.Station);
			OrderCapacity = cargoConsoleInterfaceState.Capacity;
			OrderCount = cargoConsoleInterfaceState.Count;
			BankBalance = _cargoSystem.GetBalanceFromAccount(Entity<StationBankAccountComponent>.op_Implicit(entity), cargoOrderConsoleComponent.Account);
			AccountName = cargoConsoleInterfaceState.Name;
			if (_menu != null)
			{
				_menu.ProductCatalogue = cargoConsoleInterfaceState.Products;
				_menu?.UpdateStation(entity);
				Populate(cargoConsoleInterfaceState.Orders);
			}
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			CargoConsoleMenu? menu = _menu;
			if (menu != null)
			{
				((Control)menu).Orphan();
			}
			CargoConsoleOrderMenu? orderMenu = _orderMenu;
			if (orderMenu != null)
			{
				((Control)orderMenu).Orphan();
			}
		}
	}

	private bool AddOrder()
	{
		CargoConsoleOrderMenu? orderMenu = _orderMenu;
		int num = ((orderMenu != null) ? orderMenu.Amount.Value : 0);
		if (num < 1 || num > OrderCapacity)
		{
			return false;
		}
		CargoConsoleOrderMenu? orderMenu2 = _orderMenu;
		string requester = ((orderMenu2 != null) ? orderMenu2.Requester.Text : null) ?? "";
		CargoConsoleOrderMenu? orderMenu3 = _orderMenu;
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoConsoleAddOrderMessage(requester, ((orderMenu3 != null) ? orderMenu3.Reason.Text : null) ?? "", _product?.ID ?? "", num));
		return true;
	}

	private void RemoveOrder(ButtonEventArgs args)
	{
		Control parent = ((Control)args.Button).Parent;
		if (((parent != null) ? parent.Parent : null) is CargoOrderRow { Order: not null } cargoOrderRow)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoConsoleRemoveOrderMessage(cargoOrderRow.Order.OrderId));
		}
	}

	private void ApproveOrder(ButtonEventArgs args)
	{
		Control parent = ((Control)args.Button).Parent;
		if (((parent != null) ? parent.Parent : null) is CargoOrderRow { Order: not null } cargoOrderRow && OrderCount < OrderCapacity)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new CargoConsoleApproveOrderMessage(cargoOrderRow.Order.OrderId));
		}
	}
}
