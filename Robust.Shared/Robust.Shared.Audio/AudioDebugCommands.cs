using Robust.Shared.Audio.Systems;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Audio;

internal sealed class AudioDebugCommands : LocalizedCommands
{
	[Dependency]
	private readonly IEntitySystemManager _entitySystem;

	public override string Command => "audio_length";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length != 1)
		{
			shell.WriteError(LocalizationManager.GetString("cmd-invalid-arg-number-error"));
		}
		else
		{
			shell.WriteLine(_entitySystem.GetEntitySystem<SharedAudioSystem>().GetAudioLength(new ResolvedPathSpecifier(args[0])).ToString());
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHint(LocalizationManager.GetString("cmd-audio_length-arg-file-name"));
		}
		return CompletionResult.Empty;
	}
}
