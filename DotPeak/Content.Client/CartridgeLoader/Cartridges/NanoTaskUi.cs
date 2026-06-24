// Decompiled with JetBrains decompiler
// Type: Content.Client.CartridgeLoader.Cartridges.NanoTaskUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NanoTaskUi : 
  UIFragment,
  ISerializationGenerated<NanoTaskUi>,
  ISerializationGenerated
{
  private NanoTaskUiFragment? _fragment;
  private NanoTaskItemPopup? _popup;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    this._fragment = new NanoTaskUiFragment();
    this._popup = new NanoTaskItemPopup();
    this._fragment.NewTask += (Action) (() =>
    {
      this._popup.ResetInputs((NanoTaskItem) null);
      this._popup.SetEditingTaskId(new int?());
      ((BaseWindow) this._popup).OpenCentered();
    });
    this._fragment.OpenTask += (Action<int>) (id =>
    {
      NanoTaskItemAndId nanoTaskItemAndId = this._fragment.Tasks.Find((Predicate<NanoTaskItemAndId>) (task => task.Id == id));
      if (nanoTaskItemAndId == null)
        return;
      this._popup.ResetInputs(nanoTaskItemAndId.Data);
      this._popup.SetEditingTaskId(new int?(nanoTaskItemAndId.Id));
      ((BaseWindow) this._popup).OpenCentered();
    });
    this._fragment.ToggleTaskCompletion += (Action<int>) (id =>
    {
      NanoTaskItemAndId nanoTaskItemAndId = this._fragment.Tasks.Find((Predicate<NanoTaskItemAndId>) (task => task.Id == id));
      if (nanoTaskItemAndId == null)
        return;
      userInterface.SendMessage((BoundUserInterfaceMessage) new CartridgeUiMessage((CartridgeMessageEvent) new NanoTaskUiMessageEvent((INanoTaskUiMessagePayload) new NanoTaskUpdateTask(new NanoTaskItemAndId(id, new NanoTaskItem(nanoTaskItemAndId.Data.Description, nanoTaskItemAndId.Data.TaskIsFor, !nanoTaskItemAndId.Data.IsTaskDone, nanoTaskItemAndId.Data.Priority))))));
    });
    this._popup.TaskSaved += (Action<int, NanoTaskItem>) ((id, data) =>
    {
      userInterface.SendMessage((BoundUserInterfaceMessage) new CartridgeUiMessage((CartridgeMessageEvent) new NanoTaskUiMessageEvent((INanoTaskUiMessagePayload) new NanoTaskUpdateTask(new NanoTaskItemAndId(id, data)))));
      ((BaseWindow) this._popup).Close();
    });
    this._popup.TaskDeleted += (Action<int>) (id =>
    {
      userInterface.SendMessage((BoundUserInterfaceMessage) new CartridgeUiMessage((CartridgeMessageEvent) new NanoTaskUiMessageEvent((INanoTaskUiMessagePayload) new NanoTaskDeleteTask(id))));
      ((BaseWindow) this._popup).Close();
    });
    this._popup.TaskCreated += (Action<NanoTaskItem>) (data =>
    {
      userInterface.SendMessage((BoundUserInterfaceMessage) new CartridgeUiMessage((CartridgeMessageEvent) new NanoTaskUiMessageEvent((INanoTaskUiMessagePayload) new NanoTaskAddTask(data))));
      ((BaseWindow) this._popup).Close();
    });
    this._popup.TaskPrinted += (Action<NanoTaskItem>) (data => userInterface.SendMessage((BoundUserInterfaceMessage) new CartridgeUiMessage((CartridgeMessageEvent) new NanoTaskUiMessageEvent((INanoTaskUiMessagePayload) new NanoTaskPrintTask(data)))));
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is NanoTaskUiState nanoTaskUiState))
      return;
    this._fragment?.UpdateState(nanoTaskUiState.Tasks);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NanoTaskUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NanoTaskUi) target1;
    serialization.TryCustomCopy<NanoTaskUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NanoTaskUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref UIFragment target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NanoTaskUi target1 = (NanoTaskUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (UIFragment) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    NanoTaskUi target1 = (NanoTaskUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NanoTaskUi UIFragment.Instantiate() => new NanoTaskUi();
}
