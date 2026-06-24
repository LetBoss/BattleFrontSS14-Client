// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayStopCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Robust.Shared.Replays;

internal sealed class ReplayStopCommand : LocalizedCommands
{
  [Dependency]
  private readonly IReplayRecordingManager _replay;

  public override string Command => "replay_recording_stop";

  public override string Description
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-stop-desc");
  }

  public override string Help
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-stop-help");
  }

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._replay.IsRecording)
    {
      this._replay.StopRecording();
      shell.WriteLine(this.Loc.GetString("cmd-replay-recording-stop-success"));
    }
    else
      shell.WriteLine(this.Loc.GetString("cmd-replay-recording-stop-not-recording"));
  }
}
