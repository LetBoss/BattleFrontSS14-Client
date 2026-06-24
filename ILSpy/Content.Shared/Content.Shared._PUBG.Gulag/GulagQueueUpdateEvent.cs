using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagQueueUpdateEvent : EntityEventArgs
{
	public int QueuePosition { get; }

	public int TotalInQueue { get; }

	public GulagQueueUpdateEvent(int queuePosition, int totalInQueue)
	{
		QueuePosition = queuePosition;
		TotalInQueue = totalInQueue;
	}
}
