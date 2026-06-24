using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Robust.Shared.Replays;

[Serializable]
[NetSerializable]
public sealed class ReplayMessage
{
	[Serializable]
	[NetSerializable]
	public sealed class CvarChangeMsg
	{
		public List<(string name, object value)> ReplicatedCvars;

		public (TimeSpan, GameTick) TimeBase;
	}

	[Serializable]
	[NetSerializable]
	public sealed class LeavePvs
	{
		public readonly List<NetEntity> Entities;

		public readonly GameTick Tick;

		public LeavePvs(List<NetEntity> entities, GameTick tick)
		{
			Entities = entities;
			Tick = tick;
		}
	}

	public List<object> Messages;
}
