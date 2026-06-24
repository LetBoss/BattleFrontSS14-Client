using System;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared._PUBG.Gulag;

[Serializable]
[NetSerializable]
public sealed class GulagAdminResponseEvent : EntityEventArgs
{
	public bool Accepted { get; }

	public GulagAdminResponseEvent(bool accepted)
	{
		Accepted = accepted;
	}
}
