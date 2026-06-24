using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderTeleportTargetState
{
	public NetEntity Entity { get; }

	public string Name { get; }

	public CivCommanderTeleportTargetState(NetEntity entity, string name)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
		Name = name;
	}
}
