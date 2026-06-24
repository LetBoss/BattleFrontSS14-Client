// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.TacticalMap.TacticalMapSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client._RMC14.Dropship.Weapon;
using Content.Shared._RMC14.TacticalMap;
using Robust.Client.Player;
using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Player;
using System;

#nullable enable
namespace Content.Client._RMC14.TacticalMap;

public sealed class TacticalMapSystem : SharedTacticalMapSystem
{
  [Dependency]
  private IPlayerManager _player;

  public override void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TacticalMapUserComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<TacticalMapUserComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnUserState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TacticalMapComputerComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<TacticalMapComputerComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnComputerState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TacticalMapLinesComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<TacticalMapLinesComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnLinesState)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<TacticalMapLabelsComponent, AfterAutoHandleStateEvent>(new EntityEventRefHandler<TacticalMapLabelsComponent, AfterAutoHandleStateEvent>((object) this, __methodptr(OnLabelsState)), (Type[]) null, (Type[]) null);
  }

  private void RefreshUser(EntityUid ent)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(ent, ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is TacticalMapUserBui tacticalMapUserBui)
          tacticalMapUserBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"TacticalMapUserBui"}\n{ex}");
    }
  }

  private void RefreshComputer(EntityUid ent)
  {
    try
    {
      UserInterfaceComponent interfaceComponent;
      if (!this.TryComp<UserInterfaceComponent>(ent, ref interfaceComponent))
        return;
      foreach (BoundUserInterface boundUserInterface in interfaceComponent.ClientOpenInterfaces.Values)
      {
        if (boundUserInterface is TacticalMapComputerBui tacticalMapComputerBui)
          tacticalMapComputerBui.Refresh();
        else if (boundUserInterface is DropshipWeaponsBui dropshipWeaponsBui)
          dropshipWeaponsBui.Refresh();
      }
    }
    catch (Exception ex)
    {
      this.Log.Error($"Error refreshing {"TacticalMapComputerBui"}\n{ex}");
    }
  }

  private void OnUserState(Entity<TacticalMapUserComponent> ent, ref AfterAutoHandleStateEvent args)
  {
    EntityUid? localEntity = ((ISharedPlayerManager) this._player).LocalEntity;
    EntityUid entityUid = Entity<TacticalMapUserComponent>.op_Implicit(ent);
    if ((localEntity.HasValue ? (EntityUid.op_Equality(localEntity.GetValueOrDefault(), entityUid) ? 1 : 0) : 0) == 0)
      return;
    this.RefreshUser(Entity<TacticalMapUserComponent>.op_Implicit(ent));
  }

  private void OnComputerState(
    Entity<TacticalMapComputerComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    this.RefreshComputer(Entity<TacticalMapComputerComponent>.op_Implicit(ent));
  }

  private void OnLinesState(
    Entity<TacticalMapLinesComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    if (this.HasComp<TacticalMapUserComponent>(Entity<TacticalMapLinesComponent>.op_Implicit(ent)))
      this.RefreshUser(Entity<TacticalMapLinesComponent>.op_Implicit(ent));
    if (!this.HasComp<TacticalMapComputerComponent>(Entity<TacticalMapLinesComponent>.op_Implicit(ent)))
      return;
    this.RefreshComputer(Entity<TacticalMapLinesComponent>.op_Implicit(ent));
  }

  private void OnLabelsState(
    Entity<TacticalMapLabelsComponent> ent,
    ref AfterAutoHandleStateEvent args)
  {
    if (this.HasComp<TacticalMapUserComponent>(Entity<TacticalMapLabelsComponent>.op_Implicit(ent)))
      this.RefreshUser(Entity<TacticalMapLabelsComponent>.op_Implicit(ent));
    if (!this.HasComp<TacticalMapComputerComponent>(Entity<TacticalMapLabelsComponent>.op_Implicit(ent)))
      return;
    this.RefreshComputer(Entity<TacticalMapLabelsComponent>.op_Implicit(ent));
  }
}
