// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.NotifyCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Popups;
using Content.Shared.Popups;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

internal sealed class NotifyCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;

  public virtual string Command => "notify";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    string message = args[0];
    this._entitySystemManager.GetEntitySystem<PopupSystem>().PopupCursor(message, PopupType.Small);
  }
}
