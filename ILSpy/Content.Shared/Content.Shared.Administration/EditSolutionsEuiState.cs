using System;
using System.Collections.Generic;
using Content.Shared.Eui;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared.Administration;

[Serializable]
[NetSerializable]
public sealed class EditSolutionsEuiState : EuiStateBase
{
	public readonly NetEntity Target;

	public readonly List<(string, NetEntity)>? Solutions;

	public readonly GameTick Tick;

	public EditSolutionsEuiState(NetEntity target, List<(string, NetEntity)>? solutions, GameTick tick)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		Target = target;
		Solutions = solutions;
		Tick = tick;
	}
}
