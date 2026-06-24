using System;
using Robust.Shared.Analyzers;
using Robust.Shared.Network;

namespace Robust.Shared.GameObjects;

[NotContentImplementable]
public interface IEntityNetworkManager
{
	event EventHandler<object> ReceivedSystemMessage;

	void SetupNetworking();

	void SendSystemNetworkMessage(EntityEventArgs message, bool recordReplay = true);

	void SendSystemNetworkMessage(EntityEventArgs message, uint sequence)
	{
		throw new NotSupportedException();
	}

	void SendSystemNetworkMessage(EntityEventArgs message, INetChannel channel);
}
