using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.DoAfter;

[Serializable]
[NetSerializable]
public sealed class DoAfterComponentState : ComponentState
{
	public readonly ushort NextId;

	public readonly Dictionary<ushort, DoAfter> DoAfters;

	public DoAfterComponentState(IEntityManager entManager, DoAfterComponent component)
	{
		NextId = component.NextId;
		DoAfters = component.DoAfters;
	}
}
