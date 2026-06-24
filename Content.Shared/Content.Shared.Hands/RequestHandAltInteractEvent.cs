using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.Hands;

[Serializable]
[NetSerializable]
public sealed class RequestHandAltInteractEvent : EntityEventArgs
{
	public string HandName { get; }

	public RequestHandAltInteractEvent(string handName)
	{
		HandName = handName;
	}
}
