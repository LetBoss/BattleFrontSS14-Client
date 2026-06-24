using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivCommanderLineAddedEvent : EntityEventArgs
{
	public CivCommanderLineState Line { get; }

	public CivCommanderLineAddedEvent(CivCommanderLineState line)
	{
		Line = line;
	}
}
