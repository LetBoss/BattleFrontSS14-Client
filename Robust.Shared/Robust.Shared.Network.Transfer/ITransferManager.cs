using System;
using System.IO;
using System.Threading.Tasks;
using Robust.Shared.Analyzers;

namespace Robust.Shared.Network.Transfer;

[NotContentImplementable]
public interface ITransferManager
{
	internal event Action ClientHandshakeComplete;

	Stream StartTransfer(INetChannel channel, TransferStartInfo startInfo);

	void RegisterTransferMessage(string key, Action<TransferReceivedEvent>? rxCallback = null, NetMessageAccept accept = NetMessageAccept.Both);

	internal void Initialize();

	internal void FrameUpdate();

	internal Task ServerHandshake(INetChannel channel);
}
