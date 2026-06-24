using System;
using Robust.Shared.IoC;

namespace Robust.Shared.Console.Commands;

internal sealed class DumpDependencyInjectors : LocalizedCommands
{
	[Dependency]
	private readonly IDependencyCollection _dependencies;

	public override string Command => "dump_dependency_injectors";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		Type[] cachedInjectorTypes = ((DependencyCollection)_dependencies).GetCachedInjectorTypes();
		Type[] array = cachedInjectorTypes;
		foreach (Type type in array)
		{
			shell.WriteLine(type.FullName ?? "");
		}
		shell.WriteLine(base.Loc.GetString("cmd-dump_dependency_injectors-total-count", ("total", cachedInjectorTypes.Length)));
	}
}
