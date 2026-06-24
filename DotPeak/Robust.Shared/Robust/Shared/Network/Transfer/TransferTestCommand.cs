// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferTestCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System;
using System.IO;
using System.Runtime.ExceptionServices;

#nullable enable
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
    INetChannel channel = shell.Player?.Channel;
    if (channel == null)
    {
      shell.WriteError("You do not have a channel");
    }
    else
    {
      int bufferCount = 1024 /*0x0400*/;
      if (args.Length >= 1)
        bufferCount = Parse.Int32(args[0].AsSpan());
      Stream stream = this._transferManager.StartTransfer(channel, new TransferStartInfo()
      {
        MessageKey = "TransferTestManager"
      });
      object obj = (object) null;
      int num = 0;
      byte[] buffer;
      try
      {
        buffer = new byte[16384 /*0x4000*/];
        for (int i = 0; i < bufferCount; ++i)
          await stream.WriteAsync((ReadOnlyMemory<byte>) buffer).ConfigureAwait(false);
        num = 1;
      }
      catch (object ex)
      {
        obj = ex;
      }
      if (stream != null)
        await stream.DisposeAsync();
      object obj1 = obj;
      if (obj1 != null)
      {
        if (!(obj1 is Exception source))
          throw obj1;
        ExceptionDispatchInfo.Capture(source).Throw();
      }
      if (num != 1)
      {
        obj = (object) null;
        stream = (Stream) null;
        buffer = (byte[]) null;
        throw null;
      }
    }
  }
}
