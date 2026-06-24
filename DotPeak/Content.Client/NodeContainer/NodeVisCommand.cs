// Decompiled with JetBrains decompiler
// Type: Content.Client.NodeContainer.NodeVisCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Administration.Managers;
using Content.Shared.Administration;
using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.NodeContainer;

public sealed class NodeVisCommand : LocalizedEntityCommands
{
  [Dependency]
  private IClientAdminManager _adminManager;
  [Dependency]
  private NodeGroupSystem _nodeSystem;

  public virtual string Command => "nodevis";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (!this._adminManager.HasFlag(AdminFlags.Debug))
      shell.WriteError(((LocalizedCommands) this).Loc.GetString("shell-missing-required-permission", ("perm", (object) "+DEBUG")));
    else
      this._nodeSystem.SetVisEnabled(!this._nodeSystem.VisEnabled);
  }
}
