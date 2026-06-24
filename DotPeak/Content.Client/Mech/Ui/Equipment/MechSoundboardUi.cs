// Decompiled with JetBrains decompiler
// Type: Content.Client.Mech.Ui.Equipment.MechSoundboardUi
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

public sealed class MechSoundboardUi : 
  UIFragment,
  ISerializationGenerated<MechSoundboardUi>,
  ISerializationGenerated
{
  private MechSoundboardUiFragment? _fragment;

  public override Control GetUIFragmentRoot() => (Control) this._fragment;

  public override void Setup(BoundUserInterface userInterface, EntityUid? fragmentOwner)
  {
    if (!fragmentOwner.HasValue)
      return;
    this._fragment = new MechSoundboardUiFragment();
    this._fragment.OnPlayAction += (Action<int>) (sound => userInterface.SendMessage((BoundUserInterfaceMessage) new MechSoundboardPlayMessage(IoCManager.Resolve<IEntityManager>().GetNetEntity(fragmentOwner.Value, (MetaDataComponent) null), sound)));
  }

  public override void UpdateState(BoundUserInterfaceState state)
  {
    if (!(state is MechSoundboardUiState state1))
      return;
    this._fragment?.UpdateContents(state1);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MechSoundboardUi target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    UIFragment target1 = (UIFragment) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MechSoundboardUi) target1;
    serialization.TryCustomCopy<MechSoundboardUi>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MechSoundboardUi target,
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
    MechSoundboardUi target1 = (MechSoundboardUi) target;
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
    MechSoundboardUi target1 = (MechSoundboardUi) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MechSoundboardUi UIFragment.Instantiate() => new MechSoundboardUi();
}
