// Decompiled with JetBrains decompiler
// Type: Content.Client.Commands.MappingClientSideSetupCommand
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.Mapping;
using Content.Client.Markers;
using Robust.Client.Graphics;
using Robust.Client.State;
using Robust.Shared.Console;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.Commands;

internal sealed class MappingClientSideSetupCommand : LocalizedCommands
{
  [Dependency]
  private IEntitySystemManager _entitySystemManager;
  [Dependency]
  private ILightManager _lightManager;

  public virtual string Command => "mappingclientsidesetup";

  public virtual string Help
  {
    get
    {
      return this.LocalizationManager.GetString($"cmd-{base.Command}-help", ("command", (object) base.Command));
    }
  }

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    if (this._lightManager.LockConsoleAccess)
      return;
    this._entitySystemManager.GetEntitySystem<MarkerSystem>().MarkersVisible = true;
    this._lightManager.Enabled = false;
    shell.ExecuteCommand("showsubfloor");
    IoCManager.Resolve<IStateManager>().RequestStateChange<MappingState>();
  }
}
