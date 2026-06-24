// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.MemCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class MemCommand : LocalizedCommands
{
  public override string Command => "mem";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    long heapSizeBytes = GC.GetGCMemoryInfo().HeapSizeBytes;
    long totalMemory = GC.GetTotalMemory(false);
    shell.WriteLine(this.Loc.GetString("cmd-mem-report", ("heapSize", (object) heapSizeBytes), ("totalAllocated", (object) totalMemory)));
  }
}
