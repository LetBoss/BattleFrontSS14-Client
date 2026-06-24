// Decompiled with JetBrains decompiler
// Type: Content.Shared.CombatMode.SharedCombatModeSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Content.Shared.Mind;
using Content.Shared.MouseRotator;
using Content.Shared.Movement.Components;
using Content.Shared.Popups;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using System;

#nullable enable
namespace Content.Shared.CombatMode;

public abstract class SharedCombatModeSystem : EntitySystem
{
  [Dependency]
  protected IGameTiming Timing;
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedPopupSystem _popup;
  [Dependency]
  private SharedMindSystem _mind;

  public virtual void Initialize()
  {
    base.Initialize();
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CombatModeComponent, MapInitEvent>(new ComponentEventHandler<CombatModeComponent, MapInitEvent>((object) this, __methodptr(OnMapInit)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CombatModeComponent, ComponentShutdown>(new ComponentEventHandler<CombatModeComponent, ComponentShutdown>((object) this, __methodptr(OnShutdown)), (Type[]) null, (Type[]) null);
    // ISSUE: method pointer
    this.SubscribeLocalEvent<CombatModeComponent, ToggleCombatActionEvent>(new ComponentEventHandler<CombatModeComponent, ToggleCombatActionEvent>((object) this, __methodptr(OnActionPerform)), (Type[]) null, (Type[]) null);
  }

  private void OnMapInit(EntityUid uid, CombatModeComponent component, MapInitEvent args)
  {
    this._actionsSystem.AddAction(uid, ref component.CombatToggleActionEntity, component.CombatToggleAction, new EntityUid());
    this.Dirty(uid, (IComponent) component, (MetaDataComponent) null);
  }

  private void OnShutdown(EntityUid uid, CombatModeComponent component, ComponentShutdown args)
  {
    SharedActionsSystem actionsSystem = this._actionsSystem;
    Entity<ActionsComponent> performer = Entity<ActionsComponent>.op_Implicit(uid);
    EntityUid? toggleActionEntity = component.CombatToggleActionEntity;
    Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
    actionsSystem.RemoveAction(performer, action);
    this.SetMouseRotatorComponents(uid, false);
  }

  private void OnActionPerform(
    EntityUid uid,
    CombatModeComponent component,
    ToggleCombatActionEvent args)
  {
    if (args.Handled)
      return;
    args.Handled = true;
    this.SetInCombatMode(uid, !component.IsInCombatMode, component);
    this._popup.PopupClient(this.Loc.GetString(component.IsInCombatMode ? "action-popup-combat-enabled" : "action-popup-combat-disabled"), args.Performer, new EntityUid?(args.Performer));
  }

  public void SetCanDisarm(EntityUid entity, bool canDisarm, CombatModeComponent? component = null)
  {
    if (!this.Resolve<CombatModeComponent>(entity, ref component, true))
      return;
    component.CanDisarm = new bool?(canDisarm);
  }

  public bool IsInCombatMode(EntityUid? entity, CombatModeComponent? component = null)
  {
    return entity.HasValue && this.Resolve<CombatModeComponent>(entity.Value, ref component, false) && component.IsInCombatMode;
  }

  public virtual void SetInCombatMode(EntityUid entity, bool value, CombatModeComponent? component = null)
  {
    if (!this.Resolve<CombatModeComponent>(entity, ref component, true) || component.IsInCombatMode == value)
      return;
    component.IsInCombatMode = value;
    this.Dirty(entity, (IComponent) component, (MetaDataComponent) null);
    if (component.CombatToggleActionEntity.HasValue)
    {
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid? toggleActionEntity = component.CombatToggleActionEntity;
      Entity<ActionComponent>? action = toggleActionEntity.HasValue ? new Entity<ActionComponent>?(Entity<ActionComponent>.op_Implicit(toggleActionEntity.GetValueOrDefault())) : new Entity<ActionComponent>?();
      int num = component.IsInCombatMode ? 1 : 0;
      actionsSystem.SetToggled(action, num != 0);
    }
    if (!component.ToggleMouseRotator || this.IsNpc(entity) && !this._mind.TryGetMind(entity, out EntityUid _, out MindComponent _))
      return;
    this.SetMouseRotatorComponents(entity, value);
  }

  private void SetMouseRotatorComponents(EntityUid uid, bool value)
  {
    if (value)
    {
      this.EnsureComp<MouseRotatorComponent>(uid);
      this.EnsureComp<NoRotateOnMoveComponent>(uid);
    }
    else
    {
      this.RemComp<MouseRotatorComponent>(uid);
      this.RemComp<NoRotateOnMoveComponent>(uid);
    }
  }

  protected abstract bool IsNpc(EntityUid uid);
}
