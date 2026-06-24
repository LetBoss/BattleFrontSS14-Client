using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._CIV14merka.Commander;

[Serializable]
[NetSerializable]
public sealed class CivFireSupportAcceptRequestEvent : EntityEventArgs
{
	public int RequestId { get; }

	public CivFireSupportAcceptRequestEvent(int requestId)
	{
		RequestId = requestId;
	}
}
