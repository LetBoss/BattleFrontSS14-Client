// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.ControlComputer.MarineControlComputerSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Marines.ControlComputer;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines.ControlComputer;

public sealed class MarineControlComputerSystem : SharedMarineControlComputerSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MarineControlComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MarineControlComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnComputerState)), (Type[]) null, (Type[]) null);
  }

  private void OnComputerState(
    Entity<MarineControlComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<MarineControlComputerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is MarineControlComputerBui controlComputerBui)
          controlComputerBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"MarineControlComputerBui"}:\n{ex}");
    }
  }
}
