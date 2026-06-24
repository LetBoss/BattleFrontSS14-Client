using System;
using System.Collections.Generic;
using Robust.Shared.Analyzers;
using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Network;

namespace Robust.Shared.Player;

[NotContentImplementable]
public interface ICommonSession
{
	SessionStatus Status { get; }

	EntityUid? AttachedEntity { get; }

	NetUserId UserId { get; }

	string Name { get; }

	short Ping { get; }

	INetChannel Channel { get; set; }

	LoginType AuthType { get; }

	HashSet<EntityUid> ViewSubscriptions { get; }

	DateTime ConnectedTime { get; set; }

	SessionState State { get; }

	SessionData Data { get; }

	bool ClientSide { get; set; }
}
