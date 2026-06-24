using Robust.Shared.Console;
using Robust.Shared.IoC;

namespace Robust.Shared.Configuration;

internal sealed class ConfigApplyRollbackCommand : IConsoleCommand
{
	[Dependency]
	private readonly IConfigurationManager _cfg;

	public string Command => "config_rollback_apply";

	public string Description => "";

	public string Help => "";

	public void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		_cfg.ApplyRollback();
	}
}
