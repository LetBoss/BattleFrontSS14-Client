// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferTestManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Log;
using Robust.Shared.Utility;
using System;

#nullable enable
namespace Robust.Shared.Network.Transfer;

internal abstract class TransferTestManager
{
  private readonly ISawmill _sawmill;
  internal const string Key = "TransferTestManager";

  protected TransferTestManager(ITransferManager manager, ILogManager logManager)
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003Cmanager\u003EP = manager;
    this._sawmill = logManager.GetSawmill("net.transfer.test");
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  public void Initialize()
  {
    // ISSUE: reference to a compiler-generated field
    this.\u003Cmanager\u003EP.RegisterTransferMessage(nameof (TransferTestManager), new Action<TransferReceivedEvent>(this.RxCallback));
  }

  private async void RxCallback(TransferReceivedEvent receive)
  {
    byte[] buffer;
    if (!this.PermissionCheck(receive.Channel))
    {
      receive.Channel.Disconnect("Not allowed");
      buffer = (byte[]) null;
    }
    else
    {
      this._sawmill.Info("Receiving debug transfer");
      buffer = new byte[16384 /*0x4000*/];
      long totalRead = 0;
      int num;
      do
      {
        num = await receive.DataStream.ReadAsync(buffer.AsMemory<byte>()).ConfigureAwait(false);
        totalRead += (long) num;
      }
      while (num != 0);
      this._sawmill.Info($"Debug transfer complete for {ByteHelpers.FormatKibibytes(totalRead)} bytes");
      buffer = (byte[]) null;
    }
  }

  protected abstract bool PermissionCheck(INetChannel channel);
}
