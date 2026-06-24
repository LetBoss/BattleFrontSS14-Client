using System;
using Content.Shared.Alert;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Mobs.Events;

[Serializable]
[NetSerializable]
public sealed class BeforeAlertSeverityCheckEvent(ProtoId<AlertPrototype> currentAlert, short severity) : EntityEventArgs
{
	public bool CancelUpdate;

	public ProtoId<AlertPrototype> CurrentAlert = currentAlert;

	public short Severity = severity;
}
