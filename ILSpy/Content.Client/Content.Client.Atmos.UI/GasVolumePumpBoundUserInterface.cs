using System;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasVolumePumpBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private float _maxTransferRate;

	[ViewVariables]
	private GasVolumePumpWindow? _window;

	public GasVolumePumpBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasVolumePumpWindow>((BoundUserInterface)(object)this);
		GasVolumePumpComponent gasVolumePumpComponent = default(GasVolumePumpComponent);
		if (base.EntMan.TryGetComponent<GasVolumePumpComponent>(((BoundUserInterface)this).Owner, ref gasVolumePumpComponent))
		{
			_maxTransferRate = gasVolumePumpComponent.MaxTransferRate;
		}
		_window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
		_window.PumpTransferRateChanged += OnPumpTransferRatePressed;
		((BoundUserInterface)this).Update();
	}

	private void OnToggleStatusButtonPressed()
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasVolumePumpToggleStatusMessage(_window.PumpStatus));
		}
	}

	private void OnPumpTransferRatePressed(string value)
	{
		float value2 = (UserInputParser.TryFloat(value, out var result) ? result : 0f);
		value2 = Math.Clamp(value2, 0f, _maxTransferRate);
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasVolumePumpChangeTransferRateMessage(value2));
	}

	public override void Update()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Update();
		GasVolumePumpComponent gasVolumePumpComponent = default(GasVolumePumpComponent);
		if (_window != null && base.EntMan.TryGetComponent<GasVolumePumpComponent>(((BoundUserInterface)this).Owner, ref gasVolumePumpComponent))
		{
			_window.Title = Identity.Name(((BoundUserInterface)this).Owner, base.EntMan);
			_window.SetPumpStatus(gasVolumePumpComponent.Enabled);
			_window.SetTransferRate(gasVolumePumpComponent.TransferRate);
		}
	}
}
