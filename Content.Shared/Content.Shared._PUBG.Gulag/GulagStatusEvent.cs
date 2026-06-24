using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagStatusEvent : EntityEventArgs
{
	public bool InGulag { get; }

	public int QueuePosition { get; }

	public GulagStatusEvent(bool inGulag, int queuePosition)
	{
		InGulag = inGulag;
		QueuePosition = queuePosition;
	}
}
