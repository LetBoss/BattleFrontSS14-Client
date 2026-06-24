using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._RMC14.Actions;

[Serializable]
[NetSerializable]
public sealed class RMCMissedTargetActionEvent : EntityEventArgs
{
	public readonly NetEntity Action;

	public RMCMissedTargetActionEvent(NetEntity actionId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Action = actionId;
	}
}
