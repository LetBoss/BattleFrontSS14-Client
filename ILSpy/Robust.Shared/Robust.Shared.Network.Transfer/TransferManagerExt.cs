using System.IO;

namespace Robust.Shared.Network.Transfer;

public static class TransferManagerExt
{
	public static Stream StartTransfer(this ITransferManager manager, INetChannel channel, string key)
	{
		return manager.StartTransfer(channel, new TransferStartInfo
		{
			MessageKey = key
		});
	}
}
