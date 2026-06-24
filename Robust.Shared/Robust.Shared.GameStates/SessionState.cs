using System;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using Robust.Shared.ViewVariables;

namespace Robust.Shared.GameStates;

[Serializable]
[NetSerializable]
public sealed class SessionState
{
	[ViewVariables]
	public NetUserId UserId { get; set; }

	[ViewVariables]
	public string Name { get; set; }

	[ViewVariables]
	public SessionStatus Status { get; set; }

	[Obsolete("Ping data is not currently networked")]
	[ViewVariables]
	public short Ping { get; set; }

	[ViewVariables]
	public NetEntity? ControlledEntity { get; set; }

	public SessionState Clone()
	{
		return new SessionState
		{
			UserId = UserId,
			Name = Name,
			Status = Status,
			ControlledEntity = ControlledEntity
		};
	}
}
