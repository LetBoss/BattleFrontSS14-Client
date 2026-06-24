// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.GcFullCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System;
using System.Runtime;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class GcFullCommand : LocalizedCommands
{
  public override string Command => "gcf";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
    GC.Collect(2, GCCollectionMode.Forced, true, true);
  }
}
