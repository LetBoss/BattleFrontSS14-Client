using System;
using Robust.Shared.Log;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Transfer;

internal abstract class TransferTestManager(ITransferManager manager, ILogManager logManager)
{
	private readonly ISawmill _sawmill = logManager.GetSawmill("net.transfer.test");

	internal const string Key = "TransferTestManager";

	public void Initialize()
	{
		manager.RegisterTransferMessage("TransferTestManager", RxCallback);
	}

	private async void RxCallback(TransferReceivedEvent receive)
	{
		if (!PermissionCheck(receive.Channel))
		{
			receive.Channel.Disconnect("Not allowed");
			return;
		}
		_sawmill.Info("Receiving debug transfer");
		byte[] buffer = new byte[16384];
		long totalRead = 0L;
		int num;
		do
		{
			num = await receive.DataStream.ReadAsync(buffer.AsMemory()).ConfigureAwait(continueOnCapturedContext: false);
			totalRead += num;
		}
		while (num != 0);
		_sawmill.Info("Debug transfer complete for " + ByteHelpers.FormatKibibytes(totalRead) + " bytes");
	}

	protected abstract bool PermissionCheck(INetChannel channel);
}
