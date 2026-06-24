// Decompiled with JetBrains decompiler
// Type: Robust.Shared.ViewVariables.Commands.ViewVariablesBaseCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;
using Robust.Shared.Network;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace Robust.Shared.ViewVariables.Commands;

public abstract class ViewVariablesBaseCommand : LocalizedCommands
{
  [Dependency]
  protected readonly INetManager _netMan;
  [Dependency]
  protected readonly IViewVariablesManager _vvm;

  public override async ValueTask<CompletionResult> GetCompletionAsync(
    IConsoleShell shell,
    string[] args,
    string argStr,
    CancellationToken cancel)
  {
    int length = args.Length;
    if (length > 1 || length == 0)
      return CompletionResult.Empty;
    string path1 = args[0];
    if (!this._netMan.IsClient)
      return CompletionResult.FromOptions(this._vvm.ListPath(path1, new VVListPathOptions()).Select<string, CompletionOption>((Func<string, CompletionOption>) (p => new CompletionOption(p, Flags: CompletionOptionFlags.PartialCompletion))));
    if (!path1.StartsWith("/c"))
      return CompletionResult.FromOptions((await this._vvm.ListRemotePath(path1, new VVListPathOptions())).Select<string, CompletionOption>((Func<string, CompletionOption>) (p => new CompletionOption(p, Flags: CompletionOptionFlags.PartialCompletion))).Append<CompletionOption>(new CompletionOption("/c", "Client-side paths", CompletionOptionFlags.PartialCompletion)));
    IViewVariablesManager vvm = this._vvm;
    string str = path1;
    string path2 = str.Substring(2, str.Length - 2);
    VVListPathOptions options = new VVListPathOptions();
    return CompletionResult.FromOptions(vvm.ListPath(path2, options).Select<string, CompletionOption>((Func<string, CompletionOption>) (p => new CompletionOption("/c" + p, Flags: CompletionOptionFlags.PartialCompletion))));
  }
}
