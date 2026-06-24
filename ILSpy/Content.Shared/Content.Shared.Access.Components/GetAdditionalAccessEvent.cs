using System.Collections.Generic;
using Robust.Shared.GameObjects;

namespace Content.Shared.Access.Components;

[ByRefEvent]
public struct GetAdditionalAccessEvent
{
	public HashSet<EntityUid> Entities = new HashSet<EntityUid>();

	public GetAdditionalAccessEvent()
	{
	}
}
