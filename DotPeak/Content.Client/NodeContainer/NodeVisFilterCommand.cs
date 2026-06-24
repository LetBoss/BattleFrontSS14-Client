// Decompiled with JetBrains decompiler
// Type: Content.Client.NodeContainer.NodeVisFilterCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.NodeContainer;

public sealed class NodeVisFilterCommand : LocalizedEntityCommands
{
  [Dependency]
  private NodeGroupSystem _nodeSystem;

  public virtual string Command => "nodevisfilter";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (args.Length == 0)
    {
      foreach (string str in this._nodeSystem.Filtered)
        shell.WriteLine(str);
    }
    else
    {
      string str = args[0];
      if (this._nodeSystem.Filtered.Add(str))
        return;
      this._nodeSystem.Filtered.Remove(str);
    }
  }
}
