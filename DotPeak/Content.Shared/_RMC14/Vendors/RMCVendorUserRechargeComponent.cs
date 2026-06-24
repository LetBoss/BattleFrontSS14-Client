// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vendors.RMCVendorUserRechargeComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vendors;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (RMCVendorUserRechargeSystem), typeof (SharedCMAutomatedVendorSystem)})]
public sealed class RMCVendorUserRechargeComponent : 
  Component,
  ISerializationGenerated<RMCVendorUserRechargeComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int MaxPoints;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public int PointsPerUpdate;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan TimePerUpdate = TimeSpan.FromMinutes(1L);
  [AutoNetworkedField]
  public TimeSpan LastUpdate = TimeSpan.Zero;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVendorUserRechargeComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVendorUserRechargeComponent) target1;
    if (serialization.TryCustomCopy<RMCVendorUserRechargeComponent>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.MaxPoints, ref target2, hookCtx, false, context))
      target2 = this.MaxPoints;
    target.MaxPoints = target2;
    int target3 = 0;
    if (!serialization.TryCustomCopy<int>(this.PointsPerUpdate, ref target3, hookCtx, false, context))
      target3 = this.PointsPerUpdate;
    target.PointsPerUpdate = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.TimePerUpdate, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.TimePerUpdate, hookCtx, context);
    target.TimePerUpdate = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVendorUserRechargeComponent target,
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
    RMCVendorUserRechargeComponent target1 = (RMCVendorUserRechargeComponent) target;
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
    RMCVendorUserRechargeComponent target1 = (RMCVendorUserRechargeComponent) target;
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
    RMCVendorUserRechargeComponent target1 = (RMCVendorUserRechargeComponent) target;
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
  virtual RMCVendorUserRechargeComponent Component.Instantiate()
  {
    return new RMCVendorUserRechargeComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCVendorUserRechargeComponent_AutoState : IComponentState
  {
    public int MaxPoints;
    public int PointsPerUpdate;
    public TimeSpan TimePerUpdate;
    public TimeSpan LastUpdate;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCVendorUserRechargeComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCVendorUserRechargeComponent, ComponentGetState>(new ComponentEventRefHandler<RMCVendorUserRechargeComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCVendorUserRechargeComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCVendorUserRechargeComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCVendorUserRechargeComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCVendorUserRechargeComponent.RMCVendorUserRechargeComponent_AutoState()
      {
        MaxPoints = component.MaxPoints,
        PointsPerUpdate = component.PointsPerUpdate,
        TimePerUpdate = component.TimePerUpdate,
        LastUpdate = component.LastUpdate
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCVendorUserRechargeComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCVendorUserRechargeComponent.RMCVendorUserRechargeComponent_AutoState current))
        return;
      component.MaxPoints = current.MaxPoints;
      component.PointsPerUpdate = current.PointsPerUpdate;
      component.TimePerUpdate = current.TimePerUpdate;
      component.LastUpdate = current.LastUpdate;
    }
  }
}
