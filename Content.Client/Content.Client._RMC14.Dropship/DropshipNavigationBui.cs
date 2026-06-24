using System;
using System.Collections.Generic;
using Content.Client.Message;
using Content.Shared._RMC14.Dropship;
using Content.Shared.Doors.Components;
using Content.Shared.Shuttles.Systems;
using Content.Shared.Timing;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Maths;
using Robust.Shared.Timing;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Dropship;

public sealed class DropshipNavigationBui : BoundUserInterface
{
	[Dependency]
	private IEntityManager _entities;

	[Dependency]
	private IGameTiming _timing;

	[ViewVariables]
	private DropshipNavigationWindow? _window;

	private readonly Dictionary<DropshipButton, string> _destinations = new Dictionary<DropshipButton, string>();

	private NetEntity? _selected;

	public DropshipNavigationBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		IoCManager.InjectDependencies<DropshipNavigationBui>(this);
	}

	protected override void Open()
	{
		((BoundUserInterface)this).Open();
		OpenWindow();
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		OpenWindow();
		if (!(state is DropshipNavigationDestinationsBuiState destinations))
		{
			if (state is DropshipNavigationTravellingBuiState travelling)
			{
				Set(travelling);
			}
		}
		else
		{
			Set(destinations);
		}
	}

	private void OpenWindow()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			return;
		}
		_window = BoundUserInterfaceExt.CreateWindow<DropshipNavigationWindow>((BoundUserInterface)(object)this);
		((BaseWindow)_window).OnClose += OnClose;
		SetFlightHeader("Flight Controls");
		SetDoorHeader("Door Controls");
		TransformComponent val = default(TransformComponent);
		MetaDataComponent val2 = default(MetaDataComponent);
		if (_entities.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val) && _entities.TryGetComponent<MetaDataComponent>(val.ParentUid, ref val2))
		{
			_window.Title = val2.EntityName + " " + _window.Title;
		}
		((BaseButton)_window.CancelButton.Button).OnPressed += delegate
		{
			SetLaunchDisabled(disabled: true);
			SetCancelDisabled(disabled: true);
			_selected = null;
			ResetDestinationButtons();
			CancelFlyby();
		};
		((BaseButton)_window.LaunchButton.Button).OnPressed += delegate
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (_selected.HasValue)
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipNavigationLaunchMsg(_selected.Value));
			}
			SetLaunchDisabled(disabled: true);
			_selected = null;
			ResetDestinationButtons();
		};
		((BaseButton)_window.LockdownButton.Button).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipLockdownMsg(DoorLocation.None));
		};
		((BaseButton)_window.LockdownButtonAft.Button).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipLockdownMsg(DoorLocation.Aft));
		};
		((BaseButton)_window.LockdownButtonPort.Button).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipLockdownMsg(DoorLocation.Port));
		};
		((BaseButton)_window.LockdownButtonStarboard.Button).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipLockdownMsg(DoorLocation.Starboard));
		};
		_entities.System<DropshipSystem>().Uis.Add(this);
	}

	private void OnClose()
	{
		_entities.System<DropshipSystem>().Uis.Remove(this);
		((BoundUserInterface)this).Close();
	}

	private void Set(DropshipNavigationDestinationsBuiState destinations)
	{
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		SetFlightHeader("Flight Controls");
		((Control)_window.DestinationsContainer).Visible = true;
		((Control)_window.ProgressBarContainer).Visible = false;
		((Control)_window.CancelButton).Visible = true;
		((Control)_window.LaunchButton).Visible = true;
		((Control)_window.DestinationsContainer).DisposeAllChildren();
		_destinations.Clear();
		NetEntity? flyBy = destinations.FlyBy;
		if (flyBy.HasValue)
		{
			NetEntity flyBy2 = flyBy.GetValueOrDefault();
			string text = "Flyby";
			DropshipButton dropshipButton = DestinationButton(text, disabled: false, delegate
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				_selected = flyBy2;
			});
			_destinations[dropshipButton] = text;
			((Control)_window.DestinationsContainer).AddChild((Control)(object)dropshipButton);
		}
		foreach (Destination destination in destinations.Destinations)
		{
			string text2 = destination.Name;
			if (destination.Primary)
			{
				text2 += " (Primary)";
			}
			DropshipButton dropshipButton2 = DestinationButton(text2, destination.Occupied, delegate
			{
				//IL_0011: Unknown result type (might be due to invalid IL or missing references)
				_selected = destination.Id;
			});
			_destinations[dropshipButton2] = text2;
			((Control)_window.DestinationsContainer).AddChild((Control)(object)dropshipButton2);
		}
		RefreshDoorLockStatus(destinations.DoorLockStatus);
		DropshipButton DestinationButton(string name, bool disabled, Action onPressed)
		{
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			DropshipButton button = new DropshipButton();
			button.Text = name;
			button.Disabled = disabled;
			button.BorderColor = Color.Transparent;
			button.BorderThickness = new Thickness(0f);
			((BaseButton)button.Button).ToggleMode = false;
			((BaseButton)button.Button).OnPressed += delegate
			{
				ResetDestinationButtons();
				button.Text = "> " + name;
				SetLaunchDisabled(disabled: false);
				SetCancelDisabled(disabled: false);
				onPressed();
			};
			return button;
		}
	}

	private void Set(DropshipNavigationTravellingBuiState travelling)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (_window != null)
		{
			((Control)_window.DestinationsContainer).Visible = false;
			((Control)_window.ProgressBarContainer).Visible = true;
			((Control)_window.LaunchButton).Visible = false;
			((Control)_window.ProgressBar).Margin = new Thickness(0f, 5f, 0f, 0f);
			if (travelling.Destination == travelling.DepartureLocation)
			{
				((Control)_window.CancelButton).Visible = true;
			}
			else
			{
				((Control)_window.CancelButton).Visible = false;
			}
			double num = Math.Ceiling((travelling.Time.End - _timing.CurTime).TotalSeconds);
			if (num < 0.01)
			{
				num = 0.0;
			}
			string destination = travelling.Destination;
			switch (travelling.State)
			{
			default:
				return;
			case FTLState.Starting:
				SetFlightHeader("Launch in progress");
				_window.ProgressBarHeader.SetMarkup(Msg($"Launching in T-{num}s to {destination}"));
				SetLockDownDisabled(disabled: false);
				break;
			case FTLState.Travelling:
				SetFlightHeader("In flight: " + destination);
				_window.ProgressBarHeader.SetMarkup(Msg($"Time until destination: T-{num}s"));
				SetLockDownDisabled(disabled: true);
				SetCancelDisabled(disabled: false);
				break;
			case FTLState.Arriving:
				SetFlightHeader("Final Approach: " + destination);
				_window.ProgressBarHeader.SetMarkup(Msg($"Time until landing: T-{num}s"));
				SetLockDownDisabled(disabled: true);
				SetCancelDisabled(disabled: true);
				break;
			case FTLState.Cooldown:
				SetFlightHeader("Refueling in progress");
				_window.ProgressBarHeader.SetMarkup(Msg($"Ready to launch in T-{num}s"));
				SetLockDownDisabled(disabled: false);
				SetCancelDisabled(disabled: true);
				break;
			}
			RefreshDoorLockStatus(travelling.DoorLockStatus);
			StartEndTime time = travelling.Time;
			((Range)_window.ProgressBar).MinValue = 0f;
			((Range)_window.ProgressBar).MaxValue = (float)time.Length.TotalSeconds;
			((Range)_window.ProgressBar).SetAsRatio(1f - time.ProgressAt(_timing.CurTime));
		}
		static string Msg(string msg)
		{
			return "[color=#02E74E][bold]" + msg + "[/bold][/color]";
		}
	}

	private void SetFlightHeader(string label)
	{
		_window?.Header.SetMarkup("[color=#0BDC49][font size=16][bold]" + label + "[/bold][/font][/color]");
	}

	private void SetDoorHeader(string label)
	{
		_window?.DoorHeader.SetMarkup("[color=#0BDC49][font size=16][bold]" + label + "[/bold][/font][/color]");
	}

	private void SetLaunchDisabled(bool disabled)
	{
		if (_window != null)
		{
			((BaseButton)_window.LaunchButton.Button).Disabled = disabled;
		}
	}

	private void SetCancelDisabled(bool disabled)
	{
		if (_window != null)
		{
			((BaseButton)_window.CancelButton.Button).Disabled = disabled;
		}
	}

	private void SetLockDownDisabled(bool disabled)
	{
		if (_window != null)
		{
			((BaseButton)_window.LockdownButton.Button).Disabled = disabled;
			((BaseButton)_window.LockdownButtonAft.Button).Disabled = disabled;
			((BaseButton)_window.LockdownButtonPort.Button).Disabled = disabled;
			((BaseButton)_window.LockdownButtonStarboard.Button).Disabled = disabled;
		}
	}

	private unsafe void ResetDestinationButtons()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		Enumerator enumerator = ((Control)_window.DestinationsContainer).Children.GetEnumerator();
		try
		{
			while (((Enumerator)(ref enumerator)).MoveNext())
			{
				if (((Enumerator)(ref enumerator)).Current is DropshipButton dropshipButton && _destinations.TryGetValue(dropshipButton, out string value))
				{
					dropshipButton.Text = value;
				}
			}
		}
		finally
		{
			((IDisposable)(*(Enumerator*)(&enumerator))/*cast due to constrained. prefix*/).Dispose();
		}
	}

	private void CancelFlyby()
	{
		if (_window != null)
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DropshipNavigationCancelMsg());
		}
	}

	private void RefreshDoorLockStatus(Dictionary<DoorLocation, bool> dooorLockStatus)
	{
		if (_window != null)
		{
			dooorLockStatus.TryGetValue(DoorLocation.Aft, out var value);
			dooorLockStatus.TryGetValue(DoorLocation.Port, out var value2);
			dooorLockStatus.TryGetValue(DoorLocation.Starboard, out var value3);
			bool flag = value && value2 && value3;
			_window.LockdownButton.Text = (flag ? "Lift Lockdown" : "Lockdown");
			_window.LockdownButtonAft.Text = (value ? "Unlock Aft" : "Lock Aft");
			_window.LockdownButtonPort.Text = (value2 ? "Unlock Port" : "Lock Port");
			_window.LockdownButtonStarboard.Text = (value3 ? "Unlock Starboard" : "Lock Starboard");
		}
	}

	public override void Update()
	{
		if (_window != null && !((Control)_window).Disposed && ((BoundUserInterface)this).State is DropshipNavigationTravellingBuiState travelling)
		{
			Set(travelling);
		}
	}
}
