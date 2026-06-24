using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands.Components;

[Serializable]
[NetSerializable]
public sealed class HandsComponentState : ComponentState
{
	public readonly Dictionary<string, Hand> Hands;

	public readonly List<string> SortedHands;

	public readonly string? ActiveHandId;

	public HandsComponentState(HandsComponent handComp)
	{
		Hands = new Dictionary<string, Hand>(handComp.Hands);
		SortedHands = new List<string>(handComp.SortedHands);
		ActiveHandId = handComp.ActiveHandId;
	}
}
