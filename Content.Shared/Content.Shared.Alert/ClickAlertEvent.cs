using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Alert;

[Serializable]
[NetSerializable]
public sealed class ClickAlertEvent : EntityEventArgs
{
	public readonly ProtoId<AlertPrototype> Type;

	public ClickAlertEvent(ProtoId<AlertPrototype> alertType)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Type = alertType;
	}
}
