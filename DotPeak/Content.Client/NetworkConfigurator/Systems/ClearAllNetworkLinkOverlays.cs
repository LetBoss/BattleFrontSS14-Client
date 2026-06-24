// Decompiled with JetBrains decompiler
// Type: Content.Client.NetworkConfigurator.Systems.ClearAllNetworkLinkOverlays
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Robust.Shared.Console;
using Robust.Shared.IoC;

#nullable enable
namespace Content.Client.NetworkConfigurator.Systems;

public sealed class ClearAllNetworkLinkOverlays : LocalizedEntityCommands
{
  [Dependency]
  private NetworkConfiguratorSystem _network;

  public virtual string Command => "clearnetworklinkoverlays";

  public virtual void Execute(IConsoleShell shell, string argStr, string[] args)
  {
    this._network.ClearAllOverlays();
  }
}
