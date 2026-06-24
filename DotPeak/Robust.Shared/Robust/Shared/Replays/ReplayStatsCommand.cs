// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayStatsCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Robust.Shared.Replays;

internal sealed class ReplayStatsCommand : LocalizedCommands
{
  [Dependency]
  private readonly IReplayRecordingManager _replay;

  public override string Command => "replay_recording_stats";

  public override string Description
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-stats-desc");
  }

  public override string Help
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-stats-help");
  }

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._replay.IsRecording)
    {
      ReplayRecordingStats replayStats = this._replay.GetReplayStats();
      float num = (float) replayStats.Size / 1048576f;
      double totalMinutes = replayStats.Time.TotalMinutes;
      shell.WriteLine(this.Loc.GetString("cmd-replay-recording-stats-result", ("time", (object) totalMinutes.ToString("F1")), ("ticks", (object) replayStats.Ticks), ("size", (object) num.ToString("F1")), ("rate", (object) ((double) num / totalMinutes).ToString("F2"))));
    }
    else
      shell.WriteLine(this.Loc.GetString("cmd-replay-recording-stop-not-recording"));
  }
}
