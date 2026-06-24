using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivBotStateUpdateEvent : EntityEventArgs
{
	public List<CivBotStateInfo> Bots { get; }

	public CivBotStateUpdateEvent(IEnumerable<CivBotStateInfo> bots)
	{
		Bots = bots.ToList();
	}
}
