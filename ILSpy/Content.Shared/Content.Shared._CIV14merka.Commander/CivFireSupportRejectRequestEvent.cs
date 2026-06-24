using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivFireSupportRejectRequestEvent : EntityEventArgs
{
	public int RequestId { get; }

	public CivFireSupportRejectRequestEvent(int requestId)
	{
		RequestId = requestId;
	}
}
