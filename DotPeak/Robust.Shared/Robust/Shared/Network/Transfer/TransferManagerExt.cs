// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Network.Transfer.TransferManagerExt
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.IO;

#nullable enable
namespace Robust.Shared.Network.Transfer;

public static class TransferManagerExt
{
  public static Stream StartTransfer(
    this ITransferManager manager,
    INetChannel channel,
    string key)
  {
    return manager.StartTransfer(channel, new TransferStartInfo()
    {
      MessageKey = key
    });
  }
}
