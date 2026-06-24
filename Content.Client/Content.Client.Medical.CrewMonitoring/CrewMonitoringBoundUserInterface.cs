using System;
using Content.Shared.Medical.CrewMonitoring;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.ViewVariables;

namespace Content.Client.Medical.CrewMonitoring;

public sealed class CrewMonitoringBoundUserInterface : BoundUserInterface
{
	[ViewVariables]
	private CrewMonitoringWindow? _menu;

	public CrewMonitoringBoundUserInterface(EntityUid owner, Enum uiKey)
		: base(owner, uiKey)
	{
	}//IL_0001: Unknown result type (might be due to invalid IL or missing references)


	protected override void Open()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).Open();
		EntityUid? val = null;
		string stationName = string.Empty;
		TransformComponent val2 = default(TransformComponent);
		if (base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val2))
		{
			val = val2.GridUid;
			MetaDataComponent val3 = default(MetaDataComponent);
			if (base.EntMan.TryGetComponent<MetaDataComponent>(val, ref val3))
			{
				stationName = val3.EntityName;
			}
		}
		_menu = BoundUserInterfaceExt.CreateWindow<CrewMonitoringWindow>((BoundUserInterface)(object)this);
		_menu.Set(stationName, val);
	}

	protected override void UpdateState(BoundUserInterfaceState state)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		((BoundUserInterface)this).UpdateState(state);
		if (state is CrewMonitoringState crewMonitoringState)
		{
			TransformComponent val = default(TransformComponent);
			base.EntMan.TryGetComponent<TransformComponent>(((BoundUserInterface)this).Owner, ref val);
			_menu?.ShowSensors(crewMonitoringState.Sensors, ((BoundUserInterface)this).Owner, (val != null) ? new EntityCoordinates?(val.Coordinates) : ((EntityCoordinates?)null));
		}
	}
}
