using System;
using System.Collections.Generic;
using System.Linq;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivBotSelectRequestEvent : EntityEventArgs
{
	public List<NetEntity> Bots { get; }

	public bool AddToSelection { get; }

	public CivBotSelectRequestEvent(IEnumerable<NetEntity> bots, bool addToSelection = false)
	{
		Bots = bots.ToList();
		AddToSelection = addToSelection;
	}
}
