using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Timing;

[Serializable]
[NetSerializable]
public sealed class UseDelayComponentState : IComponentState
{
	public Dictionary<string, UseDelayInfo> Delays = new Dictionary<string, UseDelayInfo>();
}
