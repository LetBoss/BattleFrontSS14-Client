// Decompiled with JetBrains decompiler
// Type: Content.Client.CartridgeLoader.Cartridges.NotekeeperUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class NotekeeperUi : 
  UIFragment,
  ISerializationGenerated<NotekeeperUi>,
  ISerializationGenerated
{
  private NotekeeperUiFragment? _fragment;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    this._fragment = new NotekeeperUiFragment();
    this._fragment.OnNoteRemoved += (Action<string>) (note => this.SendNotekeeperMessage(NotekeeperUiAction.Remove, note, userInterface));
    this._fragment.OnNoteAdded += (Action<string>) (note => this.SendNotekeeperMessage(NotekeeperUiAction.Add, note, userInterface));
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is NotekeeperUiState notekeeperUiState))
      return;
    this._fragment?.UpdateState(notekeeperUiState.Notes);
  }

  private void SendNotekeeperMessage(
    NotekeeperUiAction action,
    string note,
    BoundUserInterface userInterface)
  {
    CartridgeUiMessage cartridgeUiMessage = new CartridgeUiMessage((CartridgeMessageEvent) new NotekeeperUiMessageEvent(action, note));
    userInterface.SendMessage((BoundUserInterfaceMessage) cartridgeUiMessage);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref NotekeeperUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (NotekeeperUi) target1;
    serialization.TryCustomCopy<NotekeeperUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref NotekeeperUi target,
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
    NotekeeperUi target1 = (NotekeeperUi) target;
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
    NotekeeperUi target1 = (NotekeeperUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual NotekeeperUi UIFragment.Instantiate() => new NotekeeperUi();
}
