using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Actions;

[Serializable]
[NetSerializable]
public sealed class RequestPerformActionEvent : EntityEventArgs
{
	public readonly NetEntity Action;

	public readonly NetEntity? EntityTarget;

	public readonly NetCoordinates? EntityCoordinatesTarget;

	public readonly GameTick LastRealTick;

	public RequestPerformActionEvent(NetEntity action, GameTick lastRealTick)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		LastRealTick = lastRealTick;
	}

	public RequestPerformActionEvent(NetEntity action, NetEntity entityTarget, GameTick lastRealTick)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		EntityTarget = entityTarget;
		LastRealTick = lastRealTick;
	}

	public RequestPerformActionEvent(NetEntity action, NetCoordinates entityCoordinatesTarget, GameTick lastRealTick)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		EntityCoordinatesTarget = entityCoordinatesTarget;
		LastRealTick = lastRealTick;
	}

	public RequestPerformActionEvent(NetEntity action, NetEntity? entityTarget, NetCoordinates entityCoordinatesTarget, GameTick lastRealTick)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		Action = action;
		EntityTarget = entityTarget;
		EntityCoordinatesTarget = entityCoordinatesTarget;
		LastRealTick = lastRealTick;
	}
}
