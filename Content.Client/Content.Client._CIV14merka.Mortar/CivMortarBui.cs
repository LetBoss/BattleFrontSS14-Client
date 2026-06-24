using System;
using Content.Shared._CIV14merka.Mortar;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Maths;

namespace Content.Client._CIV14merka.Mortar;

public sealed class CivMortarBui : BoundUserInterface
{
	private CivMortarWindow? _window;

	public CivMortarBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<CivMortarWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.SetTargetButton).OnPressed += delegate
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CivMortarTargetBuiMsg(Vector2i.op_Implicit((Parse(_window.TargetX), Parse(_window.TargetY)))));
		};
		_window.DialControl.OnDialChanged += delegate(Vector2i dial)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CivMortarDialBuiMsg(dial));
		};
		Refresh();
		static int Parse(FloatSpinBox spinBox)
		{
			return (int)spinBox.Value;
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is CivMortarBuiState state2)
		{
			CivMortarWindow window = _window;
			if (window != null && ((BaseWindow)window).IsOpen)
			{
				window.UpdateState(state2, AcceptRequest, RejectRequest);
			}
		}
	}

	public void Refresh()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		CivMortarWindow window = _window;
		if (window != null && ((BaseWindow)window).IsOpen)
		{
			CivMortarComponent civMortarComponent = default(CivMortarComponent);
			if (!base.EntMan.TryGetComponent<CivMortarComponent>(((BoundUserInterface)this).Owner, ref civMortarComponent))
			{
				window.SetEmptyRequests();
			}
			else
			{
				window.SetTargetDial(civMortarComponent.Target, civMortarComponent.Dial);
			}
		}
	}

	private void AcceptRequest(int requestId)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CivMortarAcceptRequestBuiMsg(requestId));
	}

	private void RejectRequest(int requestId)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new CivMortarRejectRequestBuiMsg(requestId));
	}
}
