using System;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Network;

[NotContentImplementable]
public interface IServerNetManager : INetManager
{
	public delegate Task<NetApproval> NetApprovalDelegate(NetApprovalEventArgs eventArgs);

	byte[]? CryptoPublicKey { get; }

	AuthMode Auth { get; }

	Func<string, Task<NetUserId?>>? AssignUserIdCallback { get; set; }

	NetApprovalDelegate? HandleApprovalCallback { get; set; }

	void DisconnectChannel(INetChannel channel, string reason);
}
