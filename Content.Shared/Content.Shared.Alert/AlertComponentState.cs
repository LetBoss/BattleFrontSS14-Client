using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert;

[Serializable]
[NetSerializable]
public sealed class AlertComponentState : ComponentState
{
	public Dictionary<AlertKey, AlertState> Alerts { get; }

	public AlertComponentState(Dictionary<AlertKey, AlertState> alerts)
	{
		Alerts = alerts;
	}
}
