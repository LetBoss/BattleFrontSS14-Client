// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.VfsListCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;

#nullable enable
namespace Robust.Shared.Console.Commands;

public sealed class VfsListCommand : LocalizedCommands
{
  [Dependency]
  private readonly IResourceManager _resourceManager;

  public override string Command => "vfs_ls";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length > 1)
    {
      shell.WriteError(this.LocalizationManager.GetString("cmd-vfs_ls-err-args"));
    }
    else
    {
      foreach (string directoryEntry in this._resourceManager.ContentGetDirectoryEntries(new ResPath(args[0])))
        shell.WriteLine(directoryEntry);
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    return args.Length == 1 ? CompletionResult.FromHintOptions(CompletionHelper.ContentDirPath(args[0], this._resourceManager), this.LocalizationManager.GetString("cmd-vfs_ls-hint-path")) : CompletionResult.Empty;
  }
}
