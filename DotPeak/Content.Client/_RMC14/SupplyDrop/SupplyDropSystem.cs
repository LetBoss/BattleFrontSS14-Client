// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.SupplyDrop.SupplyDropSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Overwatch;
using Content.Shared._RMC14.SupplyDrop;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.SupplyDrop;

public sealed class SupplyDropSystem : SharedSupplyDropSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<SupplyDropComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<SupplyDropComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnSupplyDropComputerState)), (Type[]) null, (Type[]) null);
  }

  private void OnSupplyDropComputerState(
    Entity<SupplyDropComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<SupplyDropComputerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is SupplyDropComputerBui supplyDropComputerBui)
          supplyDropComputerBui.Refresh();
        if (boundUserInterface is OverwatchConsoleBui overwatchConsoleBui)
          overwatchConsoleBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"SupplyDropComputerBui"}\n{ex}");
    }
  }
}
