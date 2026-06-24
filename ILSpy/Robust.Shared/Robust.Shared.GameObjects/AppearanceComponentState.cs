using System;
using System.Collections.Generic;
using Robust.Shared.Serialization;

namespace Robust.Shared.GameObjects;

[Serializable]
[NetSerializable]
public sealed class AppearanceComponentState : ComponentState
{
	public readonly Dictionary<Enum, object> Data;

	public AppearanceComponentState(Dictionary<Enum, object> data)
	{
		Data = data;
	}
}
