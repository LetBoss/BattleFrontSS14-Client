using System;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Ghost.Roles;

[Serializable]
[NetSerializable]
public sealed class MakeGhostRoleEuiState : EuiStateBase
{
	public NetEntity Entity { get; }

	public MakeGhostRoleEuiState(NetEntity entity)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		Entity = entity;
	}
}
