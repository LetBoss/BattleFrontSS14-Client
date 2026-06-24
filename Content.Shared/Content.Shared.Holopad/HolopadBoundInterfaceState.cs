using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Holopad;

[Serializable]
[NetSerializable]
public sealed class HolopadBoundInterfaceState : BoundUserInterfaceState
{
	public readonly Dictionary<NetEntity, string> Holopads;

	public HolopadBoundInterfaceState(Dictionary<NetEntity, string> holopads)
	{
		Holopads = holopads;
	}
}
