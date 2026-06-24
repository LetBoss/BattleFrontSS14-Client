// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.Ui.Equipment.MechGrabberUi
// Assembly: Content.Client, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: B4469588-B038-4783-B6EC-1EFF6592A364
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Client.dll

using Content.Client.UserInterface.Fragments;
using Content.Shared.Mech;
using Robust.Client.UserInterface;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Client.Mech.Ui.Equipment;

public sealed class MechGrabberUi : 
  UIFragment,
  ISerializationGenerated<MechGrabberUi>,
  ISerializationGenerated
{
  private MechGrabberUiFragment? _fragment;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    if (!fragmentOwner.HasValue)
      return;
    this._fragment = new MechGrabberUiFragment();
    this._fragment.OnEjectAction += (Action<EntityUid>) (e =>
    {
      IEntityManager ientityManager = IoCManager.Resolve<IEntityManager>();
      userInterface.SendMessage((BoundUserInterfaceMessage) new MechGrabberEjectMessage(ientityManager.GetNetEntity(fragmentOwner.Value, (MetaDataComponent) null), ientityManager.GetNetEntity(e, (MetaDataComponent) null)));
    });
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is MechGrabberUiState state1))
      return;
    this._fragment?.UpdateContents(state1);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MechGrabberUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MechGrabberUi) target1;
    serialization.TryCustomCopy<MechGrabberUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MechGrabberUi target,
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
    MechGrabberUi target1 = (MechGrabberUi) target;
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
    MechGrabberUi target1 = (MechGrabberUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MechGrabberUi UIFragment.Instantiate() => new MechGrabberUi();
}
