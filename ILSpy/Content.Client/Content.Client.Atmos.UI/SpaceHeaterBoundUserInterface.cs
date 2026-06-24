using System;
using Content.Shared.Atmos.Piping.Portable.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.Atmos.UI;

public sealed class SpaceHeaterBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SpaceHeaterWindow? _window;

	public SpaceHeaterBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SpaceHeaterWindow>((BoundUserInterface)(object)this);
		((BaseButton)_window.ToggleStatusButton).OnPressed += delegate
		{
			OnToggleStatusButtonPressed();
		};
		((BaseButton)_window.IncreaseTempRange).OnPressed += delegate
		{
			OnTemperatureRangeChanged(_window.TemperatureChangeDelta);
		};
		((BaseButton)_window.DecreaseTempRange).OnPressed += delegate
		{
			OnTemperatureRangeChanged(-_window.TemperatureChangeDelta);
		};
		_window.ModeSelector.OnItemSelected += OnModeChanged;
		_window.PowerLevelSelector.OnItemSelected += OnPowerLevelChange;
	}

	private void OnToggleStatusButtonPressed()
	{
		_window?.SetActive(!_window.Active);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SpaceHeaterToggleMessage());
	}

	private void OnTemperatureRangeChanged(float changeAmount)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SpaceHeaterChangeTemperatureMessage(changeAmount));
	}

	private void OnModeChanged(ItemSelectedEventArgs args)
	{
		SpaceHeaterWindow? window = _window;
		if (window != null)
		{
			window.ModeSelector.SelectId(args.Id);
		}
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SpaceHeaterChangeModeMessage((SpaceHeaterMode)args.Id));
	}

	private void OnPowerLevelChange(RadioOptionItemSelectedEventArgs<int> args)
	{
		_window?.PowerLevelSelector.Select(args.Id);
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SpaceHeaterChangePowerLevelMessage((SpaceHeaterPowerLevel)args.Id));
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is SpaceHeaterBoundUserInterfaceState spaceHeaterBoundUserInterfaceState)
		{
			_window.SetActive(spaceHeaterBoundUserInterfaceState.Enabled);
			_window.ModeSelector.SelectId((int)spaceHeaterBoundUserInterfaceState.Mode);
			_window.PowerLevelSelector.Select((int)spaceHeaterBoundUserInterfaceState.PowerLevel);
			_window.MinTemp = spaceHeaterBoundUserInterfaceState.MinTemperature;
			_window.MaxTemp = spaceHeaterBoundUserInterfaceState.MaxTemperature;
			_window.SetTemperature(spaceHeaterBoundUserInterfaceState.TargetTemperature);
		}
	}

	protected override void Dispose(bool disposing)
	{
		((BoundUserInterface)this).Dispose(disposing);
		if (disposing)
		{
			SpaceHeaterWindow? window = _window;
			if (window != null)
			{
				((Control)window).Orphan();
			}
		}
	}
}
