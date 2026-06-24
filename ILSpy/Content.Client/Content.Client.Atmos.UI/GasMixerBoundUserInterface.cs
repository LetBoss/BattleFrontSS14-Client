using System;
using Content.Shared.Atmos.Piping.Trinary.Components;
using Content.Shared.Localizations;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class GasMixerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private const float MaxPressure = 4500f;

	[ViewVariables]
	private GasMixerWindow? _window;

	public GasMixerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<GasMixerWindow>((BoundUserInterface)(object)this);
		_window.ToggleStatusButtonPressed += OnToggleStatusButtonPressed;
		_window.MixerOutputPressureChanged += OnMixerOutputPressurePressed;
		_window.MixerNodePercentageChanged += OnMixerSetPercentagePressed;
	}

	private void OnToggleStatusButtonPressed()
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasMixerToggleStatusMessage(_window.MixerStatus));
		}
	}

	private void OnMixerOutputPressurePressed(string value)
	{
		float result;
		float num = (UserInputParser.TryFloat(value, out result) ? result : 0f);
		if (num > 4500f)
		{
			num = 4500f;
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasMixerChangeOutputPressureMessage(num));
	}

	private void OnMixerSetPercentagePressed(string value)
	{
		float value2 = (UserInputParser.TryFloat(value, out var result) ? result : 1f);
		value2 = Math.Clamp(value2, 0f, 100f);
		if (_window != null)
		{
			value2 = (_window.NodeOneLastEdited ? value2 : (100f - value2));
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new GasMixerChangeNodePercentageMessage(value2));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is GasMixerBoundUserInterfaceState gasMixerBoundUserInterfaceState)
		{
			((DefaultWindow)_window).Title = gasMixerBoundUserInterfaceState.MixerLabel;
			_window.SetMixerStatus(gasMixerBoundUserInterfaceState.Enabled);
			_window.SetOutputPressure(gasMixerBoundUserInterfaceState.OutputPressure);
			_window.SetNodePercentages(gasMixerBoundUserInterfaceState.NodeOne);
		}
	}
}
