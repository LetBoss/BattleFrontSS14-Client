using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Lathe;

[Serializable]
[NetSerializable]
public sealed class LatheQueueRecipeMessage : BoundUserInterfaceMessage
{
	public readonly string ID;

	public readonly int Quantity;

	public LatheQueueRecipeMessage(string id, int quantity)
	{
		ID = id;
		Quantity = quantity;
	}
}
