using Robust.Shared.Enums;
using Robust.Shared.GameObjects;
using Robust.Shared.Network;

namespace Robust.Shared.Player;

internal interface ICommonSessionInternal : ICommonSession
{
	void SetStatus(SessionStatus status);

	void SetAttachedEntity(EntityUid? uid);

	void SetPing(short ping);

	void SetName(string name);

	void SetChannel(INetChannel channel);
}
