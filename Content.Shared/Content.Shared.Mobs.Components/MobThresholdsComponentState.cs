using System;
using System.Collections.Generic;
using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs.Components;

[Serializable]
[NetSerializable]
public sealed class MobThresholdsComponentState : ComponentState
{
	public Dictionary<FixedPoint2, MobState> UnsortedThresholds;

	public bool TriggersAlerts;

	public MobState CurrentThresholdState;

	public Dictionary<MobState, ProtoId<AlertPrototype>> StateAlertDict;

	public bool ShowOverlays;

	public bool AllowRevives;

	public bool DisplayDamageInAlert;

	public MobThresholdsComponentState(Dictionary<FixedPoint2, MobState> unsortedThresholds, bool triggersAlerts, MobState currentThresholdState, Dictionary<MobState, ProtoId<AlertPrototype>> stateAlertDict, bool showOverlays, bool allowRevives, bool displayDamageInAlert)
	{
		UnsortedThresholds = unsortedThresholds;
		TriggersAlerts = triggersAlerts;
		CurrentThresholdState = currentThresholdState;
		StateAlertDict = stateAlertDict;
		ShowOverlays = showOverlays;
		AllowRevives = allowRevives;
		DisplayDamageInAlert = displayDamageInAlert;
	}
}
