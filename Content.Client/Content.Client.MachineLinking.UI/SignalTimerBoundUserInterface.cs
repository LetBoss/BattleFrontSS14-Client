using System;
using Content.Shared.MachineLinking;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.ViewVariables;

namespace Content.Client.MachineLinking.UI;

public sealed class SignalTimerBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private SignalTimerWindow? _window;

	public SignalTimerBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<SignalTimerWindow>((BoundUserInterface)(object)this);
		_window.OnStartTimer += StartTimer;
		_window.OnCurrentTextChanged += OnTextChanged;
		_window.OnCurrentDelayMinutesChanged += OnDelayChanged;
		_window.OnCurrentDelaySecondsChanged += OnDelayChanged;
	}

	public void StartTimer()
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SignalTimerStartMessage());
	}

	private void OnTextChanged(string newText)
	{
		((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SignalTimerTextChangedMessage(newText));
	}

	private void OnDelayChanged(string newDelay)
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendMessage((BoundUserInterfaceMessage)(object)new SignalTimerDelayChangedMessage(_window.GetDelay()));
		}
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (_window != null && state is SignalTimerBoundUserInterfaceState signalTimerBoundUserInterfaceState)
		{
			_window.SetCurrentText(signalTimerBoundUserInterfaceState.CurrentText);
			_window.SetCurrentDelayMinutes(signalTimerBoundUserInterfaceState.CurrentDelayMinutes);
			_window.SetCurrentDelaySeconds(signalTimerBoundUserInterfaceState.CurrentDelaySeconds);
			_window.SetShowText(signalTimerBoundUserInterfaceState.ShowText);
			_window.SetTriggerTime(signalTimerBoundUserInterfaceState.TriggerTime);
			_window.SetTimerStarted(signalTimerBoundUserInterfaceState.TimerStarted);
			_window.SetHasAccess(signalTimerBoundUserInterfaceState.HasAccess);
		}
	}
}
