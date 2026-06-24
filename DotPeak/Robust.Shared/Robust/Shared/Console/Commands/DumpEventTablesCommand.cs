// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.DumpEventTablesCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
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
      shell.WriteError(this.Loc.GetString("cmd-dump_event_tables-missing-arg-entity"));
    }
    else
    {
      NetEntity entity1;
      EntityUid? entity2;
      if (!NetEntity.TryParse(args[0].AsSpan(), out entity1) || !this._entities.TryGetEntity(entity1, out entity2) || !this._entities.EntityExists(entity2))
      {
        shell.WriteError(this.Loc.GetString("cmd-dump_event_tables-error-entity"));
      }
      else
      {
        EntityEventBus.EventTable entEventTable = ((EntityEventBus) this._entities.EventBus)._entEventTables[entity2.Value];
        foreach ((Type key, (int Start, int Count) tuple) in entEventTable.EventIndices)
        {
          shell.WriteLine($"{key}:");
          int index = tuple.Start;
          while (index != -1)
          {
            ref EntityEventBus.EventTableListEntry local = ref entEventTable.ComponentLists[index];
            index = local.Next;
            Type type = this._componentFactory.IdxToType(local.Component);
            shell.WriteLine("    " + type.Name);
          }
        }
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHint(this.Loc.GetString("cmd-dump_event_tables-arg-entity")) : CompletionResult.Empty;
  }
}
