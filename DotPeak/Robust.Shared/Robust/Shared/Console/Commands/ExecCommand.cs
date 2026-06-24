// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Console.Commands.ExecCommand
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.ContentPack;
using Robust.Shared.IoC;
using Robust.Shared.Utility;
using System.IO;
using System.Text.RegularExpressions;

#nullable enable
namespace Robust.Shared.Console.Commands;

internal sealed class ExecCommand : LocalizedCommands
{
  private static readonly Regex CommentRegex = new Regex("^\\s*#");
  [Dependency]
  private readonly IResourceManager _resources;

  public override string Command => "exec";

  public override void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length < 1)
    {
      shell.WriteError("No file specified!");
    }
    else
    {
      ResPath rootedPath = new ResPath(args[0]).ToRootedPath();
      if (!this._resources.UserData.Exists(rootedPath))
      {
        shell.WriteError("File does not exist.");
      }
      else
      {
        using (StreamReader streamReader = this._resources.UserData.OpenText(rootedPath))
        {
          while (true)
          {
            string str;
            do
            {
              str = streamReader.ReadLine();
              if (str == null)
                goto label_6;
            }
            while (string.IsNullOrWhiteSpace(str) || ExecCommand.CommentRegex.IsMatch(str));
            shell.ConsoleHost.AppendCommand(str);
          }
label_6:;
        }
      }
    }
  }

  public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
  {
    if (args.Length != 1)
      return CompletionResult.Empty;
    string hint = this.Loc.GetString("cmd-exec-arg-filename");
    return CompletionResult.FromHintOptions(CompletionHelper.UserFilePath(args[0], this._resources.UserData), hint);
  }
}
