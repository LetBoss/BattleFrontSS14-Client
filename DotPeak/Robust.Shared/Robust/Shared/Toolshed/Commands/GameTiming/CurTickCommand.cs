// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Toolshed.Commands.GameTiming.CurTickCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Timing;

#nullable enable
namespace Robust.Shared.Toolshed.Commands.GameTiming;

[ToolshedCommand]
public sealed class CurTickCommand : ToolshedCommand
{
  [Dependency]
  private readonly IGameTiming _gameTiming;

  [CommandImplementation(null)]
  public GameTick CurTime() => this._gameTiming.CurTick;
}
