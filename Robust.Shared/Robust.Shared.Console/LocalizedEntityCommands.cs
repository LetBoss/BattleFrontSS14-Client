using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Console;

public abstract class LocalizedEntityCommands : LocalizedCommands, IEntityConsoleCommand, IConsoleCommand
{
	[Dependency]
	protected readonly EntityManager EntityManager;
}
