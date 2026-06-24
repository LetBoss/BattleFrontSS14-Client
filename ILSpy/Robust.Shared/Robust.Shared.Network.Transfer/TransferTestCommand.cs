using System;
using System.IO;
using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

namespace Robust.Shared.Network.Transfer;

internal sealed class TransferTestCommand : IConsoleCommand
{
	internal const string CommandKey = "transfer_test";

	[Dependency]
	private readonly ITransferManager _transferManager;

	public string Command => "transfer_test";

	public string Description => "";

	public string Help => "Usage: transfer_test <buffer count>";

	public async void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		INetChannel netChannel = shell.Player?.Channel;
		if (netChannel == null)
		{
			shell.WriteError("You do not have a channel");
			return;
		}
		int bufferCount = 1024;
		if (args.Length >= 1)
		{
			bufferCount = Parse.Int32(args[0].AsSpan());
		}
		await using Stream stream = _transferManager.StartTransfer(netChannel, new TransferStartInfo
		{
			MessageKey = "TransferTestManager"
		});
		byte[] buffer = new byte[16384];
		for (int i = 0; i < bufferCount; i++)
		{
			await stream.WriteAsync(buffer).ConfigureAwait(continueOnCapturedContext: false);
		}
	}
}
