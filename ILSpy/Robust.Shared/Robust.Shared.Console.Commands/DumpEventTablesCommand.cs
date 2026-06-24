using System;
using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

namespace Robust.Shared.Console.Commands;

internal sealed class DumpEventTablesCommand : LocalizedCommands
{
	[Dependency]
	private readonly EntityManager _entities;

	[Dependency]
	private readonly IComponentFactory _componentFactory;

	public override string Command => "dump_event_tables";

	public override void Execute(IConsoleShell shell, string argStr, string[] args)
	{
		if (args.Length < 1)
		{
			shell.WriteError(base.Loc.GetString("cmd-dump_event_tables-missing-arg-entity"));
			return;
		}
		if (!NetEntity.TryParse(args[0].AsSpan(), out var entity) || !_entities.TryGetEntity(entity, out var entity2) || !_entities.EntityExists(entity2))
		{
			shell.WriteError(base.Loc.GetString("cmd-dump_event_tables-error-entity"));
			return;
		}
		EntityEventBus.EventTable eventTable = ((EntityEventBus)_entities.EventBus)._entEventTables[entity2.Value];
		foreach (var (value, tuple2) in eventTable.EventIndices)
		{
			shell.WriteLine($"{value}:");
			var (num, _) = tuple2;
			while (num != -1)
			{
				ref EntityEventBus.EventTableListEntry reference = ref eventTable.ComponentLists[num];
				num = reference.Next;
				Type type2 = _componentFactory.IdxToType(reference.Component);
				shell.WriteLine("    " + type2.Name);
			}
		}
	}

	public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
	{
		if (args.Length == 1)
		{
			return CompletionResult.FromHint(base.Loc.GetString("cmd-dump_event_tables-arg-entity"));
		}
		return CompletionResult.Empty;
	}
}
