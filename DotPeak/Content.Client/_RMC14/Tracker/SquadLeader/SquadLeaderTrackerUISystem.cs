// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Tracker.SquadLeader.SquadLeaderTrackerUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Tracker.SquadLeader;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Tracker.SquadLeader;

public sealed class SquadLeaderTrackerUISystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SquadLeaderTrackerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<SquadLeaderTrackerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnOverwatchAfterState)), (Type[]) null, (Type[]) null);
  }

  private void OnOverwatchAfterState(
    Entity<SquadLeaderTrackerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<SquadLeaderTrackerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is SquadInfoBui squadInfoBui)
          squadInfoBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"SquadInfoBui"}\n{ex}");
    }
  }
}
