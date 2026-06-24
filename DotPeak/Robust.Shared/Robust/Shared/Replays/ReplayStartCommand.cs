// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Replays.ReplayStartCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Robust.Shared.Replays;

internal sealed class ReplayStartCommand : LocalizedCommands
{
  [Dependency]
  private readonly IReplayRecordingManager _replay;
  [Dependency]
  private readonly IResourceManager _resMan;

  public override string Command => "replay_recording_start";

  public override string Description
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-start-desc");
  }

  public override string Help
  {
    get => this.LocalizationManager.GetString("cmd-replay-recording-start-help");
  }

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._replay.IsRecording)
    {
      shell.WriteError(this.Loc.GetString("cmd-replay-recording-start-already-recording"));
    }
    else
    {
      string name = args.Length == 0 ? (string) null : args[0];
      bool result1 = false;
      if (args.Length > 1 && !bool.TryParse(args[1], out result1))
      {
        shell.WriteError(this.Loc.GetString("cmd-parse-failure-bool", ("arg", (object) args[2])));
      }
      else
      {
        TimeSpan? duration = new TimeSpan?();
        if (args.Length > 2)
        {
          float result2;
          if (!float.TryParse(args[2], out result2))
          {
            shell.WriteError(this.Loc.GetString("cmd-parse-failure-float", ("arg", (object) args[0])));
            return;
          }
          duration = new TimeSpan?(TimeSpan.FromMinutes((double) result2));
        }
        if (this._replay.TryStartRecording(this._resMan.UserData, name, result1, duration))
          shell.WriteLine(this.Loc.GetString("cmd-replay-recording-start-success"));
        else
          shell.WriteLine(this.Loc.GetString("cmd-replay-recording-start-error"));
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    if (args.Length == 1)
      return CompletionResult.FromHint(this.Loc.GetString("cmd-replay-recording-start-hint-name"));
    if (args.Length == 2)
      return CompletionResult.FromHint(this.Loc.GetString("cmd-replay-recording-start-hint-overwrite"));
    return args.Length == 3 ? CompletionResult.FromHint(this.Loc.GetString("cmd-replay-recording-start-hint-time")) : CompletionResult.Empty;
  }
}
