using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class RequestSetHandEvent : EntityEventArgs
{
	public string HandName { get; }

	public RequestSetHandEvent(string handName)
	{
		HandName = handName;
	}
}
