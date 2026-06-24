using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class RequestHandInteractUsingEvent : EntityEventArgs
{
	public string HandName { get; }

	public RequestHandInteractUsingEvent(string handName)
	{
		HandName = handName;
	}
}
