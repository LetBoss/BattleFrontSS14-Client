using System.Collections.Generic;
using Content.Shared.Hands.Components;
using Robust.Shared.GameObjects;

namespace Content.Shared.Hands;

public sealed class GetInhandVisualsEvent : EntityEventArgs
{
	public readonly EntityUid User;

	public readonly HandLocation Location;

	public List<(string, PrototypeLayerData)> Layers = new List<(string, PrototypeLayerData)>();

	public GetInhandVisualsEvent(EntityUid user, HandLocation location)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		User = user;
		Location = location;
	}
}
