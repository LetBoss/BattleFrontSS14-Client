using System;
using Content.Client.UserInterface.Controls;
using Content.Shared._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.ControlComputer;
using Content.Shared._RMC14.Overwatch;
using Content.Shared._RMC14.TacticalMap;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.GameObjects;
using Robust.Shared.Utility;
using Robust.Shared.ViewVariables;

namespace Content.Client._RMC14.Marines.Announce;

public sealed class MarineCommunicationsComputerBui : BoundUserInterface
{
	[ViewVariables]
	private MarineCommunicationsComputerWindow? _window;

	private MarineAnnounceSystem? _marineAnnounce;

	private bool _confirmingEvacuation;

	public MarineCommunicationsComputerBui(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		if (_window != null)
		{
			return;
		}
		_window = BoundUserInterfaceExt.CreateWindow<MarineCommunicationsComputerWindow>((BoundUserInterface)(object)this);
		MarineCommunicationsComputerComponent marineCommunicationsComputerComponent = default(MarineCommunicationsComputerComponent);
		if (base.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(((BoundUserInterface)this).Owner, ref marineCommunicationsComputerComponent) && marineCommunicationsComputerComponent.CanGiveMedals)
		{
			((BaseButton)_window.MedalButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerMedalMsg());
			};
			((Control)_window.MedalButton).Visible = true;
		}
		else
		{
			((Control)_window.MedalButton).Visible = false;
		}
		if (base.EntMan.HasComponent<TacticalMapComputerComponent>(((BoundUserInterface)this).Owner))
		{
			((BaseButton)_window.TacticalMapButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineCommunicationsOpenMapMsg());
			};
		}
		else
		{
			((Control)_window.TacticalMapButton).Visible = false;
		}
		MarineCommunicationsComputerComponent marineCommunicationsComputerComponent2 = default(MarineCommunicationsComputerComponent);
		if (base.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(((BoundUserInterface)this).Owner, ref marineCommunicationsComputerComponent2) && marineCommunicationsComputerComponent2.CanCreateEcho)
		{
			((BaseButton)_window.EchoButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineCommunicationsEchoSquadMsg());
			};
		}
		MarineCommunicationsComputerComponent marineCommunicationsComputerComponent3 = default(MarineCommunicationsComputerComponent);
		if (base.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(((BoundUserInterface)this).Owner, ref marineCommunicationsComputerComponent3) && marineCommunicationsComputerComponent3.CanInitiateEvac)
		{
			((BaseButton)_window.EvacuationButton).OnPressed += delegate
			{
				if (_confirmingEvacuation)
				{
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineControlComputerToggleEvacuationMsg());
					_confirmingEvacuation = false;
				}
				else
				{
					_confirmingEvacuation = true;
				}
				OnStateUpdate();
			};
			((Control)_window.EvacuationButton).Visible = true;
		}
		else
		{
			((Control)_window.EvacuationButton).Visible = false;
		}
		if (base.EntMan.HasComponent<OverwatchConsoleComponent>(((BoundUserInterface)this).Owner))
		{
			((BaseButton)_window.OverwatchButton).OnPressed += delegate
			{
				((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineCommunicationsOverwatchMsg());
			};
		}
		else
		{
			((Control)_window.OverwatchButton).Visible = false;
		}
		_window.Text.OnTextChanged += delegate(TextEditEventArgs args)
		{
			OnTextChanged((int)Rope.CalcTotalLength(args.TextRope));
		};
		((BaseButton)_window.Send).OnPressed += delegate
		{
			((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineCommunicationsComputerMsg(Rope.Collapse(_window.Text.TextRope)));
		};
		OnStateUpdate();
		OnTextChanged(0);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		OnStateUpdate();
	}

	public void OnStateUpdate()
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		if (_window == null)
		{
			return;
		}
		if (((BoundUserInterface)this).State is MarineCommunicationsComputerBuiState marineCommunicationsComputerBuiState)
		{
			((Control)_window.LandingZonesContainer).DisposeAllChildren();
			_window.PlanetName.Text = marineCommunicationsComputerBuiState.Planet;
			_window.OperationName.Text = marineCommunicationsComputerBuiState.Operation;
			foreach (LandingZone zone in marineCommunicationsComputerBuiState.LandingZones)
			{
				ConfirmButton obj = new ConfirmButton
				{
					Text = zone.Name
				};
				((Control)obj).StyleClasses.Add("OpenBoth");
				ConfirmButton confirmButton = obj;
				confirmButton.OnPressed += delegate
				{
					//IL_000c: Unknown result type (might be due to invalid IL or missing references)
					((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new MarineCommunicationsDesignatePrimaryLZMsg(zone.Id));
				};
				((Control)_window.LandingZonesContainer).AddChild((Control)(object)confirmButton);
			}
			((Control)_window.LandingZonesSection).Visible = marineCommunicationsComputerBuiState.LandingZones.Count > 0;
		}
		MarineCommunicationsComputerComponent marineCommunicationsComputerComponent = default(MarineCommunicationsComputerComponent);
		((Control)_window.EchoButton).Visible = base.EntMan.TryGetComponent<MarineCommunicationsComputerComponent>(((BoundUserInterface)this).Owner, ref marineCommunicationsComputerComponent) && marineCommunicationsComputerComponent.CanCreateEcho;
		((Control)_window.EchoSeparator).Visible = ((Control)_window.EchoButton).Visible;
		MarineControlComputerComponent marineControlComputerComponent = default(MarineControlComputerComponent);
		if (base.EntMan.TryGetComponent<MarineControlComputerComponent>(((BoundUserInterface)this).Owner, ref marineControlComputerComponent))
		{
			if (_confirmingEvacuation)
			{
				_window.EvacuationButton.Text = "Confirm?";
			}
			else
			{
				_window.EvacuationButton.Text = (marineControlComputerComponent.Evacuating ? "Cancel Evacuation" : "Initiate Evacuation");
			}
			((BaseButton)_window.EvacuationButton).Disabled = !marineControlComputerComponent.CanEvacuate;
		}
	}

	private void OnTextChanged(int textLength)
	{
		if (_window != null)
		{
			if (_marineAnnounce == null)
			{
				_marineAnnounce = base.EntMan.System<MarineAnnounceSystem>();
			}
			_window.CharacterCount.Text = $"{textLength} / {_marineAnnounce.CharacterLimit}";
			((BaseButton)_window.Send).Disabled = textLength == 0;
		}
	}
}
