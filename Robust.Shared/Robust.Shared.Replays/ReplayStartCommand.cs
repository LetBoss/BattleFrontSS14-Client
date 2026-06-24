using System;
using Robust.Shared.Console;
using Robust.Shared.ContentPack;
using Robust.Shared.IoC;

namespace Robust.Shared.Replays;

internal sealed class ReplayStartCommand : LocalizedCommands
{
	[Dependency]
	private readonly IReplayRecordingManager _replay;

	[Dependency]
	private readonly IResourceManager _resMan;

	public override string Command => "replay_recording_start";

	public override string Description => LocalizationManager.GetString("cmd-replay-recording-start-desc");

	public override string Help => LocalizationManager.GetString("cmd-replay-recording-start-help");

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (_replay.IsRecording)
		{
			shell.WriteError(base.Loc.GetString("cmd-replay-recording-start-already-recording"));
			return;
		}
		string name = ((args.Length == 0) ? null : args[0]);
		bool result = false;
		if (args.Length > 1 && !bool.TryParse(args[1], out result))
		{
			shell.WriteError(base.Loc.GetString("cmd-parse-failure-bool", ("arg", args[2])));
			return;
		}
		TimeSpan? duration = null;
		if (args.Length > 2)
		{
			if (!float.TryParse(args[2], out var result2))
			{
				shell.WriteError(base.Loc.GetString("cmd-parse-failure-float", ("arg", args[0])));
				return;
			}
			duration = TimeSpan.FromMinutes(result2);
		}
		if (_replay.TryStartRecording(_resMan.UserData, name, result, duration))
		{
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-start-success"));
		}
		else
		{
			shell.WriteLine(base.Loc.GetString("cmd-replay-recording-start-error"));
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-replay-recording-start-hint-name"));
		}
		if (args.Length == 2)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-replay-recording-start-hint-overwrite"));
		}
		if (args.Length == 3)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-replay-recording-start-hint-time"));
		}
		return CompletionResult.Empty;
	}
}
