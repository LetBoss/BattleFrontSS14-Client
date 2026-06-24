using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class RequestMoveHandItemEvent : EntityEventArgs
{
	public string HandName { get; }

	public RequestMoveHandItemEvent(string handName)
	{
		HandName = handName;
	}
}
