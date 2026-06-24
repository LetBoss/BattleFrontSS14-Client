// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Marines.MarineAnnounceSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Marines.Announce;
using Content.Shared._RMC14.Marines.Announce;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Marines;

public sealed class MarineAnnounceSystem : SharedMarineAnnounceSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<MarineCommunicationsComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<MarineCommunicationsComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnCommunicationsComputerState)), (Type[]) null, (Type[]) null);
  }

  private void OnCommunicationsComputerState(
    Entity<MarineCommunicationsComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<MarineCommunicationsComputerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is MarineCommunicationsComputerBui communicationsComputerBui)
          communicationsComputerBui.OnStateUpdate();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"MarineCommunicationsComputerBui"}\n{ex}");
    }
  }
}
