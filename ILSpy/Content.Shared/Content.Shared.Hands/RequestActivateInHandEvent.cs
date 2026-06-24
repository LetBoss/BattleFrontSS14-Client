using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class RequestActivateInHandEvent : EntityEventArgs
{
	public string HandName { get; }

	public RequestActivateInHandEvent(string handName)
	{
		HandName = handName;
	}
}
