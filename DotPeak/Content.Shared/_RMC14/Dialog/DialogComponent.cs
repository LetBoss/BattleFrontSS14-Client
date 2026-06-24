// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Dialog.DialogComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Dialog;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (DialogSystem)})]
public sealed class DialogComponent : 
  Component,
  ISerializationGenerated<DialogComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DialogType DialogType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string Title;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DialogOption Message = new DialogOption(string.Empty);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<DialogOption> Options = new List<DialogOption>();
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public object? Event;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public DialogInputEvent? InputEvent;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool LargeInput;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public object? ConfirmEvent;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int CharacterLimit = 200;
  [DataField(null, false, 1, false, false, null)]
  public bool AutoFocus = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref DialogComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (DialogComponent) target1;
    if (serialization.TryCustomCopy<DialogComponent>(this, ref target, hookCtx, false, context))
      return;
    DialogType target2 = DialogType.Options;
    if (!serialization.TryCustomCopy<DialogType>(this.DialogType, ref target2, hookCtx, false, context))
      target2 = this.DialogType;
    target.DialogType = target2;
    string target3 = (string) null;
    if (this.Title == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.Title, ref target3, hookCtx, false, context))
      target3 = this.Title;
    target.Title = target3;
    DialogOption target4 = new DialogOption();
    if (!serialization.TryCustomCopy<DialogOption>(this.Message, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<DialogOption>(this.Message, hookCtx, context);
    target.Message = target4;
    List<DialogOption> target5 = (List<DialogOption>) null;
    if (this.Options == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<DialogOption>>(this.Options, ref target5, hookCtx, true, context))
      target5 = serialization.CreateCopy<List<DialogOption>>(this.Options, hookCtx, context);
    target.Options = target5;
    object target6 = (object) null;
    if (!serialization.TryCustomCopy<object>(this.Event, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy(this.Event, hookCtx, context);
    target.Event = target6;
    DialogInputEvent target7 = (DialogInputEvent) null;
    if (!serialization.TryCustomCopy<DialogInputEvent>(this.InputEvent, ref target7, hookCtx, true, context))
      target7 = serialization.CreateCopy<DialogInputEvent>(this.InputEvent, hookCtx, context);
    target.InputEvent = target7;
    bool target8 = false;
    if (!serialization.TryCustomCopy<bool>(this.LargeInput, ref target8, hookCtx, false, context))
      target8 = this.LargeInput;
    target.LargeInput = target8;
    object target9 = (object) null;
    if (!serialization.TryCustomCopy<object>(this.ConfirmEvent, ref target9, hookCtx, true, context))
      target9 = serialization.CreateCopy(this.ConfirmEvent, hookCtx, context);
    target.ConfirmEvent = target9;
    int target10 = 0;
    if (!serialization.TryCustomCopy<int>(this.CharacterLimit, ref target10, hookCtx, false, context))
      target10 = this.CharacterLimit;
    target.CharacterLimit = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoFocus, ref target11, hookCtx, false, context))
      target11 = this.AutoFocus;
    target.AutoFocus = target11;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref DialogComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DialogComponent target1 = (DialogComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DialogComponent target1 = (DialogComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    DialogComponent target1 = (DialogComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual DialogComponent Component.Instantiate() => new DialogComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class DialogComponent_AutoState : IComponentState
  {
    public DialogType DialogType;
    public string Title;
    public DialogOption Message;
    public List<DialogOption> Options;
    public object? Event;
    public DialogInputEvent? InputEvent;
    public bool LargeInput;
    public object? ConfirmEvent;
    public int CharacterLimit;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class DialogComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<DialogComponent, ComponentGetState>(new ComponentEventRefHandler<DialogComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<DialogComponent, ComponentHandleState>(new ComponentEventRefHandler<DialogComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(EntityUid uid, DialogComponent component, ref ComponentGetState args)
    {
      args.State = (IComponentState) new DialogComponent.DialogComponent_AutoState()
      {
        DialogType = component.DialogType,
        Title = component.Title,
        Message = component.Message,
        Options = component.Options,
        Event = component.Event,
        InputEvent = component.InputEvent,
        LargeInput = component.LargeInput,
        ConfirmEvent = component.ConfirmEvent,
        CharacterLimit = component.CharacterLimit
      };
    }

    private void OnHandleState(
      EntityUid uid,
      DialogComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is DialogComponent.DialogComponent_AutoState current))
        return;
      component.DialogType = current.DialogType;
      component.Title = current.Title;
      component.Message = current.Message;
      component.Options = current.Options == null ? (List<DialogOption>) null : new List<DialogOption>((IEnumerable<DialogOption>) current.Options);
      component.Event = current.Event;
      component.InputEvent = current.InputEvent;
      component.LargeInput = current.LargeInput;
      component.ConfirmEvent = current.ConfirmEvent;
      component.CharacterLimit = current.CharacterLimit;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, DialogComponent>(uid, component, ref args1);
    }
  }
}
