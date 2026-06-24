using System;
using Content.Shared.SurveillanceCamera;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.SurveillanceCamera.UI;

public sealed class SurveillanceCameraSetupBoundUi : BoundUserInterface
{
	[ViewVariables]
	private readonly SurveillanceCameraSetupUiKey _type;

	[ViewVariables]
	private SurveillanceCameraSetupWindow? _window;

	public SurveillanceCameraSetupBoundUi(EntityUid component, Enum uiKey)
		: base(component, uiKey)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (uiKey is SurveillanceCameraSetupUiKey type)
		{
			_type = type;
		}
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = new SurveillanceCameraSetupWindow();
		if (_type == SurveillanceCameraSetupUiKey.Router)
		{
			_window.HideNameSelector();
		}
		((BaseWindow)_window).OpenCentered();
		SurveillanceCameraSetupWindow? window = _window;
		window.OnNameConfirm = (Action<string>)Delegate.Combine(window.OnNameConfirm, new Action<string>(SendDeviceName));
		SurveillanceCameraSetupWindow? window2 = _window;
		window2.OnNetworkConfirm = (Action<int>)Delegate.Combine(window2.OnNetworkConfirm, new Action<int>(SendSelectedNetwork));
		((BaseWindow)_window).OnClose += base.Close;
	}

	private void SendSelectedNetwork(int idx)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraSetupSetNetwork(idx));
	}

	private void SendDeviceName(string name)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SurveillanceCameraSetupSetName(name));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is SurveillanceCameraSetupBoundUiState surveillanceCameraSetupBoundUiState)
		{
			_window.UpdateState(surveillanceCameraSetupBoundUiState.Name, surveillanceCameraSetupBoundUiState.NameDisabled, surveillanceCameraSetupBoundUiState.NetworkDisabled);
			_window.LoadAvailableNetworks(surveillanceCameraSetupBoundUiState.Network, surveillanceCameraSetupBoundUiState.Networks);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			SurveillanceCameraSetupWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
			_window = null;
		}
	}
}
