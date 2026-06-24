// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dialog.DialogSystem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._RMC14.Dialog;

public sealed class DialogSystem : EntitySystem
{
  [Dependency]
  private IGameTiming _timing;
  [Dependency]
  private SharedUserInterfaceSystem _ui;

  public override void Initialize()
  {
    this.Subs.BuiEvents<DialogComponent>((object) DialogUiKey.Key, (BoundUserInterfaceRegisterExt.BuiEventSubscriber<DialogComponent>) (subs =>
    {
      subs.Event<DialogOptionBuiMsg>(new EntityEventRefHandler<DialogComponent, DialogOptionBuiMsg>(this.OnDialogOption));
      subs.Event<DialogInputBuiMsg>(new EntityEventRefHandler<DialogComponent, DialogInputBuiMsg>(this.OnDialogInput));
      subs.Event<DialogConfirmBuiMsg>(new EntityEventRefHandler<DialogComponent, DialogConfirmBuiMsg>(this.OnDialogConfirm));
      subs.Event<BoundUIClosedEvent>(new EntityEventRefHandler<DialogComponent, BoundUIClosedEvent>(this.OnDialogClosed));
    }));
  }

  private void OnDialogOption(Entity<DialogComponent> ent, ref DialogOptionBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DialogUiKey.Key);
    int index = args.Index;
    DialogOption dialogOption;
    if (index < 0 || !ent.Comp.Options.TryGetValue<DialogOption>(index, out dialogOption))
      return;
    DialogChosenEvent args1 = new DialogChosenEvent(args.Actor, index);
    this.RaiseLocalEvent<DialogChosenEvent>((EntityUid) ent, ref args1);
    if (dialogOption.Event == null)
      return;
    this.RaiseLocalEvent((EntityUid) ent, ref dialogOption.Event, true);
  }

  private void OnDialogInput(Entity<DialogComponent> ent, ref DialogInputBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DialogUiKey.Key);
    if (ent.Comp.InputEvent == (DialogInputEvent) null)
      return;
    string str = args.Input;
    if (str.Length > ent.Comp.CharacterLimit)
      str = str.Substring(0, ent.Comp.CharacterLimit);
    ent.Comp.InputEvent = ent.Comp.InputEvent with
    {
      Message = str
    };
    this.RaiseLocalEvent((EntityUid) ent, (object) ent.Comp.InputEvent);
  }

  private void OnDialogConfirm(Entity<DialogComponent> ent, ref DialogConfirmBuiMsg args)
  {
    this._ui.CloseUi((Entity<UserInterfaceComponent>) ent.Owner, (Enum) DialogUiKey.Key);
    if (ent.Comp.ConfirmEvent == null)
      return;
    this.RaiseLocalEvent((EntityUid) ent, ent.Comp.ConfirmEvent);
  }

  private void OnDialogClosed(Entity<DialogComponent> ent, ref BoundUIClosedEvent args)
  {
    if (this._timing.ApplyingState)
      return;
    this.RemComp<DialogComponent>((EntityUid) ent);
  }

  public void OpenOptions(
    EntityUid target,
    EntityUid actor,
    string title,
    List<DialogOption> options,
    string message = "")
  {
    DialogComponent dialogComponent = this.EnsureComp<DialogComponent>(target);
    dialogComponent.Title = title;
    dialogComponent.Message = new DialogOption(message);
    dialogComponent.DialogType = DialogType.Options;
    dialogComponent.Options = options;
    this.Dirty(target, (IComponent) dialogComponent);
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) target, (Enum) DialogUiKey.Key, actor);
  }

  public void OpenOptions(
    EntityUid actor,
    string title,
    List<DialogOption> options,
    string message = "")
  {
    this.OpenOptions(actor, actor, title, options, message);
  }

  public void OpenInput(
    EntityUid target,
    EntityUid actor,
    string message,
    DialogInputEvent? ev,
    bool largeInput = false,
    int characterLimit = 200,
    bool autoFocus = true)
  {
    DialogComponent dialogComponent = this.EnsureComp<DialogComponent>(target);
    dialogComponent.DialogType = DialogType.Input;
    dialogComponent.Message = new DialogOption(message, (object) ev);
    dialogComponent.InputEvent = ev;
    dialogComponent.LargeInput = largeInput;
    dialogComponent.CharacterLimit = characterLimit;
    dialogComponent.AutoFocus = autoFocus;
    this.Dirty(target, (IComponent) dialogComponent);
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) target, (Enum) DialogUiKey.Key, actor);
  }

  public void OpenInput(
    EntityUid actor,
    string message,
    DialogInputEvent? ev,
    bool largeInput = false,
    int characterLimit = 200,
    bool autoFocus = true)
  {
    this.OpenInput(actor, actor, message, ev, largeInput, characterLimit, autoFocus);
  }

  public void OpenConfirmation(
    EntityUid target,
    EntityUid actor,
    string title,
    string message,
    object ev)
  {
    DialogComponent dialogComponent = this.EnsureComp<DialogComponent>(target);
    dialogComponent.DialogType = DialogType.Confirm;
    dialogComponent.Title = title;
    dialogComponent.Message = new DialogOption(message, ev);
    dialogComponent.ConfirmEvent = ev;
    this.Dirty(target, (IComponent) dialogComponent);
    this._ui.TryOpenUi((Entity<UserInterfaceComponent>) target, (Enum) DialogUiKey.Key, actor);
  }

  public void OpenConfirmation(EntityUid actor, string title, string message, object ev)
  {
    this.OpenConfirmation(actor, actor, title, message, ev);
  }
}
