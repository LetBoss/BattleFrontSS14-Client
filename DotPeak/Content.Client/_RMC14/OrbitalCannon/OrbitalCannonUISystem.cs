// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.OrbitalCannon.OrbitalCannonUISystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.OrbitalCannon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.OrbitalCannon;

public sealed class OrbitalCannonUISystem : EntitySystem
{
  public virtual void Initialize()
  {
    // ISSUE: method pointer
    this.SubscribeLocalEvent<OrbitalCannonComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<OrbitalCannonComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnState)), (Type[]) null, (Type[]) null);
  }

  private void OnState(
    Entity<OrbitalCannonComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<OrbitalCannonComputerComponent>.op_Implicit(ent), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is OrbitalCannonComputerBui cannonComputerBui)
          cannonComputerBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"OrbitalCannonComputerBui"}\n{ex}");
    }
  }
}
