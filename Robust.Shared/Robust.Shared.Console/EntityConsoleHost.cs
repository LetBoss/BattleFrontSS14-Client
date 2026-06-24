using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Reflection;

namespace Robust.Shared.Console;

internal sealed class EntityConsoleHost
{
	[Dependency]
	private readonly IConsoleHost _consoleHost;

	[Dependency]
	private readonly IReflectionManager _reflectionManager;

	[Dependency]
	private readonly IEntitySystemManager _entitySystemManager;

	private readonly HashSet<string> _entityCommands = new HashSet<string>();

	public bool DiscoverCommands { get; set; } = true;

	public void Startup()
	{
		if (!DiscoverCommands)
		{
			return;
		}
		DependencyCollection systemDependencyCollection = ((EntitySystemManager)_entitySystemManager).SystemDependencyCollection;
		_consoleHost.BeginRegistrationRegion();
		foreach (Type allChild in _reflectionManager.GetAllChildren<IEntityConsoleCommand>())
		{
			IConsoleCommand consoleCommand = (IConsoleCommand)Activator.CreateInstance(allChild);
			systemDependencyCollection.InjectDependencies(consoleCommand, oneOff: true);
			_entityCommands.Add(consoleCommand.Command);
			_consoleHost.RegisterCommand(consoleCommand);
		}
		_consoleHost.EndRegistrationRegion();
	}

	public void Shutdown()
	{
		foreach (string entityCommand in _entityCommands)
		{
			_consoleHost.UnregisterCommand(entityCommand);
		}
		_entityCommands.Clear();
	}
}
