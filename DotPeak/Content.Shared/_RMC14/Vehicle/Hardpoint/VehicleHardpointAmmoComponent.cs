// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleHardpointAmmoComponent
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
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class VehicleHardpointAmmoComponent : 
  Component,
  ISerializationGenerated<VehicleHardpointAmmoComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MagazineSize = 1;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxStoredMagazines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StoredMagazines;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int StoredRounds;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public List<int> StoredRoundSlots = new List<int>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleHardpointAmmoComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleHardpointAmmoComponent) target1;
    if (serialization.TryCustomCopy<VehicleHardpointAmmoComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MagazineSize, ref target2, hookCtx, false, context))
      target2 = this.MagazineSize;
    target.MagazineSize = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxStoredMagazines, ref target3, hookCtx, false, context))
      target3 = this.MaxStoredMagazines;
    target.MaxStoredMagazines = target3;
    int target4 = 0;
    if (!serialization.TryCustomCopy<int>(this.StoredMagazines, ref target4, hookCtx, false, context))
      target4 = this.StoredMagazines;
    target.StoredMagazines = target4;
    int target5 = 0;
    if (!serialization.TryCustomCopy<int>(this.StoredRounds, ref target5, hookCtx, false, context))
      target5 = this.StoredRounds;
    target.StoredRounds = target5;
    List<int> target6 = (List<int>) null;
    if (this.StoredRoundSlots == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<int>>(this.StoredRoundSlots, ref target6, hookCtx, true, context))
      target6 = serialization.CreateCopy<List<int>>(this.StoredRoundSlots, hookCtx, context);
    target.StoredRoundSlots = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleHardpointAmmoComponent target,
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
    VehicleHardpointAmmoComponent target1 = (VehicleHardpointAmmoComponent) target;
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
    VehicleHardpointAmmoComponent target1 = (VehicleHardpointAmmoComponent) target;
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
    VehicleHardpointAmmoComponent target1 = (VehicleHardpointAmmoComponent) target;
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
  virtual VehicleHardpointAmmoComponent Component.Instantiate()
  {
    return new VehicleHardpointAmmoComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleHardpointAmmoComponent_AutoState : IComponentState
  {
    public int MagazineSize;
    public int MaxStoredMagazines;
    public int StoredMagazines;
    public int StoredRounds;
    public List<int> StoredRoundSlots;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleHardpointAmmoComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleHardpointAmmoComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleHardpointAmmoComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleHardpointAmmoComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleHardpointAmmoComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleHardpointAmmoComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleHardpointAmmoComponent.VehicleHardpointAmmoComponent_AutoState()
      {
        MagazineSize = component.MagazineSize,
        MaxStoredMagazines = component.MaxStoredMagazines,
        StoredMagazines = component.StoredMagazines,
        StoredRounds = component.StoredRounds,
        StoredRoundSlots = component.StoredRoundSlots
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleHardpointAmmoComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleHardpointAmmoComponent.VehicleHardpointAmmoComponent_AutoState current))
        return;
      component.MagazineSize = current.MagazineSize;
      component.MaxStoredMagazines = current.MaxStoredMagazines;
      component.StoredMagazines = current.StoredMagazines;
      component.StoredRounds = current.StoredRounds;
      component.StoredRoundSlots = current.StoredRoundSlots == null ? (List<int>) null : new List<int>((IEnumerable<int>) current.StoredRoundSlots);
    }
  }
}
