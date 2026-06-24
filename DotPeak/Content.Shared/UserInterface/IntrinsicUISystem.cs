// Decompiled with JetBrains decompiler
// Type: Content.Shared.UserInterface.IntrinsicUISystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Actions;
using Content.Shared.Actions.Components;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Prototypes;
using System;

#nullable enable
namespace Content.Shared.UserInterface;

public sealed class IntrinsicUISystem : EntitySystem
{
  [Dependency]
  private SharedActionsSystem _actionsSystem;
  [Dependency]
  private SharedUserInterfaceSystem _uiSystem;

  public override void Initialize()
  {
    this.SubscribeLocalEvent<IntrinsicUIComponent, MapInitEvent>(new ComponentEventHandler<IntrinsicUIComponent, MapInitEvent>(this.InitActions));
    this.SubscribeLocalEvent<IntrinsicUIComponent, ComponentShutdown>(new ComponentEventRefHandler<IntrinsicUIComponent, ComponentShutdown>(this.OnShutdown));
    this.SubscribeLocalEvent<IntrinsicUIComponent, ToggleIntrinsicUIEvent>(new ComponentEventHandler<IntrinsicUIComponent, ToggleIntrinsicUIEvent>(this.OnActionToggle));
  }

  private void OnActionToggle(
    EntityUid uid,
    IntrinsicUIComponent component,
    ToggleIntrinsicUIEvent args)
  {
    if (args.Key == null)
      return;
    args.Handled = this.InteractUI(uid, args.Key, component);
  }

  private void OnShutdown(
    EntityUid uid,
    IntrinsicUIComponent component,
    ref ComponentShutdown args)
  {
    foreach (IntrinsicUIEntry intrinsicUiEntry in component.UIs.Values)
    {
      EntityUid? toggleActionEntity = intrinsicUiEntry.ToggleActionEntity;
      SharedActionsSystem actionsSystem = this._actionsSystem;
      Entity<ActionsComponent> performer = (Entity<ActionsComponent>) uid;
      EntityUid? nullable = toggleActionEntity;
      Entity<ActionComponent>? action = nullable.HasValue ? new Entity<ActionComponent>?((Entity<ActionComponent>) nullable.GetValueOrDefault()) : new Entity<ActionComponent>?();
      actionsSystem.RemoveAction(performer, action);
    }
  }

  private void InitActions(EntityUid uid, IntrinsicUIComponent component, MapInitEvent args)
  {
    foreach (IntrinsicUIEntry intrinsicUiEntry in component.UIs.Values)
    {
      SharedActionsSystem actionsSystem = this._actionsSystem;
      EntityUid performer = uid;
      ref EntityUid? local = ref intrinsicUiEntry.ToggleActionEntity;
      EntProtoId? toggleAction = intrinsicUiEntry.ToggleAction;
      string valueOrDefault = toggleAction.HasValue ? (string) toggleAction.GetValueOrDefault() : (string) null;
      EntityUid container = new EntityUid();
      actionsSystem.AddAction(performer, ref local, valueOrDefault, container);
    }
  }

  public bool InteractUI(EntityUid uid, Enum key, IntrinsicUIComponent? iui = null)
  {
    if (!this.Resolve<IntrinsicUIComponent>(uid, ref iui))
      return false;
    IntrinsicUIOpenAttemptEvent args = new IntrinsicUIOpenAttemptEvent(uid, key);
    this.RaiseLocalEvent<IntrinsicUIOpenAttemptEvent>(uid, args);
    return !args.Cancelled && this._uiSystem.TryToggleUi((Entity<UserInterfaceComponent>) uid, key, uid);
  }
}
