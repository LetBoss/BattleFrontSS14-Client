// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.RobustMemoryManager
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Microsoft.IO;
using System;
using System.IO;

#nullable enable
namespace Robust.Shared.GameObjects;

internal sealed class RobustMemoryManager
{
  private static readonly RecyclableMemoryStreamManager MemStreamManager = new RecyclableMemoryStreamManager(new RecyclableMemoryStreamManager.Options()
  {
    ThrowExceptionOnToArray = true
  });

  public RobustMemoryManager()
  {
    RobustMemoryManager.MemStreamManager.StreamDoubleDisposed += (EventHandler<RecyclableMemoryStreamManager.StreamDoubleDisposedEventArgs>) ((sender, args) =>
    {
      throw new InvalidOperationException("Found double disposed stream.");
    });
    RobustMemoryManager.MemStreamManager.StreamFinalized += (EventHandler<RecyclableMemoryStreamManager.StreamFinalizedEventArgs>) ((sender, args) =>
    {
      throw new InvalidOperationException("Stream finalized but not disposed indicating a leak");
    });
    RobustMemoryManager.MemStreamManager.StreamOverCapacity += (EventHandler<RecyclableMemoryStreamManager.StreamOverCapacityEventArgs>) ((sender, args) =>
    {
      throw new InvalidOperationException("Stream over memory capacity");
    });
  }

  public static MemoryStream GetMemoryStream()
  {
    return (MemoryStream) RobustMemoryManager.MemStreamManager.GetStream(nameof (RobustMemoryManager));
  }

  public static MemoryStream GetMemoryStream(int length)
  {
    return (MemoryStream) RobustMemoryManager.MemStreamManager.GetStream(nameof (RobustMemoryManager), (long) length);
  }
}
