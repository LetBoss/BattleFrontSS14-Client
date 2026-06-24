using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.PurchaseRequest;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client._CIV14merka.PurchaseRequests;

public sealed class PurchaseConsoleBui : BoundUserInterface
{
	private PurchaseConsoleWindow? _window;

	public PurchaseConsoleBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<PurchaseConsoleWindow>((BoundUserInterface)(object)this);
		_window.OnPurchaseSubmit += OnPurchaseSubmit;
		_window.OnRequestApprove += OnRequestApprove;
		_window.OnRequestDeny += OnRequestDeny;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is PurchaseConsoleBuiState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			PurchaseConsoleWindow? window = _window;
			if (window != null)
			{
				((Control)window).Dispose();
			}
		}
	}

	private void OnPurchaseSubmit(List<PurchaseItem> items)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PurchaseRequestSubmitMessage
		{
			Items = items
		});
	}

	private void OnRequestApprove(Guid requestId)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PurchaseRequestApproveMessage
		{
			RequestId = requestId
		});
	}

	private void OnRequestDeny(Guid requestId)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new PurchaseRequestDenyMessage
		{
			RequestId = requestId
		});
	}
}
