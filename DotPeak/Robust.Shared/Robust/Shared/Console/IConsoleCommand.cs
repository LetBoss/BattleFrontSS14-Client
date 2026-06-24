// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.IConsoleCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Console;

public interface IConsoleCommand
{
  string Command { get; }

  string Description { get; }

  string Help { get; }

  bool RequireServerOrSingleplayer => false;

  void Execute(IConsoleShell shell, string argStr, string[] args);

  CompletionResult GetCompletion(IConsoleShell shell, string[] args) => CompletionResult.Empty;

  ValueTask<CompletionResult> GetCompletionAsync(
    IConsoleShell shell,
    string[] args,
    string argStr,
    CancellationToken cancel)
  {
    return ValueTask.FromResult<CompletionResult>(this.GetCompletion(shell, args));
  }
}
