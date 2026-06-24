// Decompiled with JetBrains decompiler
// Type: Content.Client.CartridgeLoader.Cartridges.WantedListUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.CartridgeLoader.Cartridges;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.CartridgeLoader.Cartridges;

public sealed class WantedListUi : 
  UIFragment,
  ISerializationGenerated<WantedListUi>,
  ISerializationGenerated
{
  private WantedListUiFragment? _fragment;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    this._fragment = new WantedListUiFragment();
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is WantedListUiState wantedListUiState))
      return;
    this._fragment?.UpdateState(wantedListUiState.Records);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref WantedListUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (WantedListUi) target1;
    serialization.TryCustomCopy<WantedListUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref WantedListUi target,
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
    WantedListUi target1 = (WantedListUi) target;
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
    WantedListUi target1 = (WantedListUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual WantedListUi UIFragment.Instantiate() => new WantedListUi();
}
