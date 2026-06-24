using System;
using Content.Shared.Atmos.Piping.Binary.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;

namespace Content.Client.Atmos.UI;

public sealed class GasPressureRegulatorBoundUserInterface : BoundUserInterface
{
	private GasPressureRegulatorWindow? _window;

	public GasPressureRegulatorBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasPressureRegulatorWindow>((BoundUserInterface)(object)this);
		_window.SetEntity(((BoundUserInterface)this).Owner);
		_window.ThresholdPressureChanged += OnThresholdChanged;
		GasPressureRegulatorComponent gasPressureRegulatorComponent = default(GasPressureRegulatorComponent);
		if (base.EntMan.TryGetComponent<GasPressureRegulatorComponent>(((BoundUserInterface)this).Owner, ref gasPressureRegulatorComponent))
		{
			_window.SetThresholdPressureInput(gasPressureRegulatorComponent.Threshold);
		}
		((BoundUserInterface)this).Update();
	}

	public override void Update()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			_window.Title = Identity.Name(((BoundUserInterface)this).Owner, base.EntMan);
			GasPressureRegulatorComponent gasPressureRegulatorComponent = default(GasPressureRegulatorComponent);
			if (base.EntMan.TryGetComponent<GasPressureRegulatorComponent>(((BoundUserInterface)this).Owner, ref gasPressureRegulatorComponent))
			{
				_window.SetThresholdPressureLabel(gasPressureRegulatorComponent.Threshold);
				_window.UpdateInfo(gasPressureRegulatorComponent.InletPressure, gasPressureRegulatorComponent.OutletPressure, gasPressureRegulatorComponent.FlowRate);
			}
		}
	}

	private void OnThresholdChanged(string newThreshold)
	{
		float num = 0f;
		if (UserInputParser.TryFloat(newThreshold, out var result) && result >= 0f && !float.IsInfinity(result))
		{
			num = result;
		}
		_window?.SetThresholdPressureInput(num);
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new GasPressureRegulatorChangeThresholdMessage(num));
	}
}
