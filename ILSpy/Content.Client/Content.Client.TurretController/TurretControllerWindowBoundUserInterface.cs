using System;
using System.Collections.Generic;
using Content.Shared.Access;
using Content.Shared.TurretController;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.ViewVariables;

namespace Content.Client.TurretController;

public sealed class TurretControllerWindowBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private TurretControllerWindow? _window;

	public TurretControllerWindowBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		_window = BoundUserInterfaceExt.CreateWindow<TurretControllerWindow>((BoundUserInterface)(object)this);
		_window.SetOwner(((BoundUserInterface)this).Owner);
		((BaseWindow)_window).OpenCentered();
		_window.OnAccessLevelsChangedEvent += OnAccessLevelChanged;
		_window.OnArmamentSettingChangedEvent += OnArmamentSettingChanged;
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		((BoundUserInterface)this).UpdateState(state);
		if (state is DeployableTurretControllerBoundInterfaceState state2)
		{
			_window?.UpdateState(state2);
		}
	}

	private void OnAccessLevelChanged(HashSet<ProtoId<AccessLevelPrototype>> accessLevels, bool enabled)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DeployableTurretExemptAccessLevelChangedMessage(accessLevels, enabled));
	}

	private void OnArmamentSettingChanged(TurretControllerWindow.TurretArmamentSetting setting)
	{
		((BoundUserInterface)this).SendPredictedMessage((BoundUserInterfaceMessage)(object)new DeployableTurretArmamentSettingChangedMessage((int)setting));
	}
}
