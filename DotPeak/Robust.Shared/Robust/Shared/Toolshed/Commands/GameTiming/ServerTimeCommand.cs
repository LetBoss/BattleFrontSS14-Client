// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.GameTiming.ServerTimeCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.GameTiming;

[ToolshedCommand]
public sealed class ServerTimeCommand : ToolshedCommand
{
  [Dependency]
  private readonly IGameTiming _gameTiming;

  [CommandImplementation(null)]
  public TimeSpan CurTime() => this._gameTiming.ServerTime;
}
