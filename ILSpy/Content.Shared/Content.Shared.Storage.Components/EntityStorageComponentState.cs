using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Storage.Components;

[Serializable]
[NetSerializable]
public sealed class EntityStorageComponentState : ComponentState
{
	public bool Open;

	public int Capacity;

	public bool IsCollidableWhenOpen;

	public bool OpenOnMove;

	public float EnteringRange;

	public TimeSpan NextInternalOpenAttempt;

	public EntityStorageComponentState(bool open, int capacity, bool isCollidableWhenOpen, bool openOnMove, float enteringRange, TimeSpan nextInternalOpenAttempt)
	{
		Open = open;
		Capacity = capacity;
		IsCollidableWhenOpen = isCollidableWhenOpen;
		OpenOnMove = openOnMove;
		EnteringRange = enteringRange;
		NextInternalOpenAttempt = nextInternalOpenAttempt;
	}
}
