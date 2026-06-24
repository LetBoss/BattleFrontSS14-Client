using Robust.Shared.Analyzers;

namespace Robust.Shared.Network;

[NotContentImplementable]
public interface INetStructuredReason
{
	NetDisconnectMessage Message { get; }

	string Reason { get; }

	bool RedialFlag { get; }
}
