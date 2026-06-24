// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.LocalizedCommands
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.IoC;
using Robust.Shared.Localization;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.Console;

public abstract class LocalizedCommands : IConsoleCommand
{
  [Dependency]
  protected readonly ILocalizationManager LocalizationManager;

  public ILocalizationManager Loc => this.LocalizationManager;

  public abstract string Command { get; }

  public virtual string Description
  {
    get
    {
      string str;
      return !this.LocalizationManager.TryGetString($"cmd-{this.Command}-desc", out str) ? "" : str;
    }
  }

  public virtual string Help
  {
    get
    {
      string str;
      return !this.LocalizationManager.TryGetString($"cmd-{this.Command}-help", out str, ("command", (object) this.Command)) ? "" : str;
    }
  }

  public virtual bool RequireServerOrSingleplayer => false;

  public abstract void Execute(IConsoleShell shell, string argStr, string[] args);

  public virtual CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return CompletionResult.Empty;
  }

  public virtual ValueTask<CompletionResult> GetCompletionAsync(
    IConsoleShell shell,
    string[] args,
    string argStr,
    CancellationToken cancel)
  {
    return ValueTask.FromResult<CompletionResult>(this.GetCompletion(shell, args));
  }
}
