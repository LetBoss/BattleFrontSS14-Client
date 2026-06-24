using System.Threading;
using System.Threading.Tasks;
using Robust.Shared.IoC;
using Robust.Shared.Localization;

namespace Robust.Shared.Console;

public abstract class LocalizedCommands : IConsoleCommand
{
	[Dependency]
	protected readonly ILocalizationManager LocalizationManager;

	public ILocalizationManager Loc => LocalizationManager;

	public abstract string Command { get; }

	public virtual string Description
	{
		get
		{
			if (!LocalizationManager.TryGetString("cmd-" + Command + "-desc", out string value))
			{
				return "";
			}
			return value;
		}
	}

	public virtual string Help
	{
		get
		{
			if (!LocalizationManager.TryGetString("cmd-" + Command + "-help", out string value, ("command", Command)))
			{
				return "";
			}
			return value;
		}
	}

	public virtual bool RequireServerOrSingleplayer => false;

	public abstract void Execute(IConsoleShell shell, string argStr, string[] args);

	public virtual CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		return CompletionResult.Empty;
	}

	public virtual ValueTask<CompletionResult> GetCompletionAsync(IConsoleShell shell, string[] args, string argStr, CancellationToken cancel)
	{
		return ValueTask.FromResult(GetCompletion(shell, args));
	}
}
