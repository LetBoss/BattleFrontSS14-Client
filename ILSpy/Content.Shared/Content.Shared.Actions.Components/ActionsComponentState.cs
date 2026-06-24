using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Actions.Components;

[Serializable]
[NetSerializable]
public sealed class ActionsComponentState : ComponentState
{
	public readonly HashSet<NetEntity> Actions;

	public ActionsComponentState(HashSet<NetEntity> actions)
	{
		Actions = actions;
	}
}
