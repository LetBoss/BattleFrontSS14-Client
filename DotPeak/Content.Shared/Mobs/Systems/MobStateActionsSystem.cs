// Decompiled with JetBrains decompiler
// Type: Content.Shared.Mobs.Systems.MobStateActionsSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Mobs.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Mobs.Systems;

public sealed class MobStateActionsSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actions;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<MobStateActionsComponent, MobStateChangedEvent>(new ComponentEventHandler<MobStateActionsComponent, MobStateChangedEvent>(this.OnMobStateChanged));
    this.SubscribeLocalEvent<MobStateComponent, ComponentInit>(new ComponentEventHandler<MobStateComponent, ComponentInit>(this.OnMobStateComponentInit));
  }

  private void OnMobStateChanged(
    EntityUid uid,
    MobStateActionsComponent component,
    MobStateChangedEvent args)
  {
    this.ComposeActions(uid, component, args.NewMobState);
  }

  private void OnMobStateComponentInit(
    EntityUid uid,
    MobStateComponent component,
    ComponentInit args)
  {
    MobStateActionsComponent comp;
    if (!this.TryComp<MobStateActionsComponent>(uid, out comp))
      return;
    this.ComposeActions(uid, comp, component.CurrentState);
  }

  private void ComposeActions(
    EntityUid uid,
    MobStateActionsComponent component,
    MobState newMobState)
  {
    ActionsComponent comp;
    if (!this.TryComp<ActionsComponent>(uid, out comp))
      return;
    foreach (EntityUid grantedAction in component.GrantedActions)
      this.Del(new EntityUid?(grantedAction));
    component.GrantedActions.Clear();
    List<string> stringList;
    if (!component.Actions.TryGetValue(newMobState, out stringList))
      return;
    foreach (string actionPrototypeId in stringList)
    {
      EntityUid? actionId = new EntityUid?();
      if (this._actions.AddAction(uid, ref actionId, actionPrototypeId, uid, comp))
        component.GrantedActions.Add(actionId.Value);
    }
  }
}
