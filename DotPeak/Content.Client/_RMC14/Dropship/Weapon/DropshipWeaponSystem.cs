// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Dropship.Weapon.DropshipWeaponSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Dropship.Weapon;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using System;

#nullable enable
namespace Content.Client._RMC14.Dropship.Weapon;

public sealed class DropshipWeaponSystem : SharedDropshipWeaponSystem
{
  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<DropshipTerminalWeaponsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<DropshipTerminalWeaponsComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnWeaponsState)), (Type[]) null, (Type[]) null);
  }

  private void OnWeaponsState(
    Entity<DropshipTerminalWeaponsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.RefreshWeaponsUI(ent);
  }

  protected override void RefreshWeaponsUI(Entity<DropshipTerminalWeaponsComponent> terminal)
  {
    try
    {
      base.RefreshWeaponsUI(terminal);
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(Entity<DropshipTerminalWeaponsComponent>.op_Implicit(terminal), ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is DropshipWeaponsBui dropshipWeaponsBui)
          dropshipWeaponsBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"DropshipWeaponsBui"}:\n{ex}");
    }
  }
}
