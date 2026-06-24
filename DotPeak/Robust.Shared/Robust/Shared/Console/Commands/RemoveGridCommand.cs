// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.RemoveGridCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map.Components;
using System;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class RemoveGridCommand : LocalizedEntityCommands
{
  public override string Command => "rmgrid";

  public override bool RequireServerOrSingleplayer => true;

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length != 1)
    {
      shell.WriteError("Wrong number of args.");
    }
    else
    {
      EntityUid? entity;
      if (!this.EntityManager.TryGetEntity(NetEntity.Parse(args[0].AsSpan()), out entity) || !this.EntityManager.HasComponent<MapGridComponent>(entity))
      {
        shell.WriteError($"Grid {entity} does not exist.");
      }
      else
      {
        this.EntityManager.DeleteEntity(entity);
        shell.WriteLine($"Grid {entity} was removed.");
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length != 1 ? CompletionResult.Empty : CompletionResult.FromHintOptions(CompletionHelper.Components<MapGridComponent>(args[0], (IEntityManager) this.EntityManager), this.LocalizationManager.GetString("generic-grid"));
  }
}
