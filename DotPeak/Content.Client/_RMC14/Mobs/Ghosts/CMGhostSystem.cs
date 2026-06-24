// Decompiled with JetBrains decompiler
// Type: Content.Client._RMC14.Mobs.Ghosts.CMGhostSystem
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Shared._RMC14.Mobs;
using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System;

#nullable enable
namespace Content.Client._RMC14.Mobs.Ghosts;

public sealed class CMGhostSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CMGhostComponent, ComponentRemove>(new ComponentEventHandler<CMGhostComponent, ComponentRemove>((object) this, __methodptr(OnCMGhostRemove)), (Type[]) null, (Type[]) null);
  }

  private void OnCMGhostRemove(EntityUid uid, CMGhostComponent comp, ComponentRemove remove)
  {
    SharedActionsSystem actions1 = this._actions;
    Entity<ActionsComponent> performer1 = Entity<ActionsComponent>.op_Implicit(uid);
    EntityUid? nullable = comp.ToggleMarineHudEntity;
    Entity<ActionComponent>? action1 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions1.RemoveAction(performer1, action1);
    SharedActionsSystem actions2 = this._actions;
    Entity<ActionsComponent> performer2 = Entity<ActionsComponent>.op_Implicit(uid);
    nullable = comp.ToggleXenoHudEntity;
    Entity<ActionComponent>? action2 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions2.RemoveAction(performer2, action2);
    SharedActionsSystem actions3 = this._actions;
    Entity<ActionsComponent> performer3 = Entity<ActionsComponent>.op_Implicit(uid);
    nullable = comp.FindParasiteEntity;
    Entity<ActionComponent>? action3 = nullable.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(nullable.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actions3.RemoveAction(performer3, action3);
  }
}
