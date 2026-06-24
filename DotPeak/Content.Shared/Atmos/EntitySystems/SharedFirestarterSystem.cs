// Decompiled with JetBrains decompiler
// Type: Content.Shared.Atmos.EntitySystems.SharedFirestarterSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Atmos.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.Atmos.EntitySystems;

public abstract class SharedFirestarterSystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<FirestarterComponent, ComponentInit>(new ComponentEventHandler<FirestarterComponent, ComponentInit>((object) this, __methodptr(OnComponentInit)), (Type[]) null, (Type[]) null);
  }

  private void OnComponentInit(EntityUid uid, FirestarterComponent component, ComponentInit args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    EntityUid performer = uid;
    ref EntityUid? local = ref component.FireStarterActionEntity;
    EntProtoId? fireStarterAction = component.FireStarterAction;
    string actionPrototypeId = fireStarterAction.HasValue ? EntProtoId.op_Implicit(fireStarterAction.GetValueOrDefault()) : (string) null;
    EntityUid container = uid;
    actionsSystem.AddAction(performer, ref local, actionPrototypeId, container);
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }
}
