// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.OldHelpCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

#nullable enable
namespace Robust.Shared.Console.Commands;

public sealed class OldHelpCommand : LocalizedCommands
{
  public override string Command => "oldhelp";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    HelpCommand.ExecuteStatic(shell, argStr, args, this.Loc);
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return HelpCommand.GetCompletionStatic(shell, args, this.Loc);
  }
}
