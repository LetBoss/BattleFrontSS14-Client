using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Connection;

[Serializable]
[NetSerializable]
public sealed class QueuePositionMessage : EntityEventArgs
{
	public int Position { get; }

	public int TotalInQueue { get; }

	public QueuePositionMessage(int position, int totalInQueue)
	{
		Position = position;
		TotalInQueue = totalInQueue;
	}
}
