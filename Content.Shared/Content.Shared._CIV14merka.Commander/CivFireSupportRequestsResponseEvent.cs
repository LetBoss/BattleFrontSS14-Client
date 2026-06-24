using System;
using System.Collections.Generic;
using Content.Shared._CIV14merka.Mortar;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivFireSupportRequestsResponseEvent : EntityEventArgs
{
	public List<CivMortarQueuedRequest> Requests { get; }

	public CivFireSupportRequestsResponseEvent(List<CivMortarQueuedRequest> requests)
	{
		Requests = requests;
	}
}
