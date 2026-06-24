// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleAmmoLoaderComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class RMCVehicleAmmoLoaderComponent : 
  Component,
  ISerializationGenerated<RMCVehicleAmmoLoaderComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string HardpointType = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? BulletType;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan LoadDelay = TimeSpan.FromSeconds(0.7);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool EnableShellSelection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntProtoId? SelectedShellType;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleAmmoLoaderComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleAmmoLoaderComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleAmmoLoaderComponent>(this, ref target, hookCtx, false, context))
      return;
    string target2 = (string) null;
    if (this.HardpointType == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.HardpointType, ref target2, hookCtx, false, context))
      target2 = this.HardpointType;
    target.HardpointType = target2;
    EntProtoId? target3 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.BulletType, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntProtoId?>(this.BulletType, hookCtx, context);
    target.BulletType = target3;
    TimeSpan target4 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.LoadDelay, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<TimeSpan>(this.LoadDelay, hookCtx, context);
    target.LoadDelay = target4;
    bool target5 = false;
    if (!serialization.TryCustomCopy<bool>(this.EnableShellSelection, ref target5, hookCtx, false, context))
      target5 = this.EnableShellSelection;
    target.EnableShellSelection = target5;
    EntProtoId? target6 = new EntProtoId?();
    if (!serialization.TryCustomCopy<EntProtoId?>(this.SelectedShellType, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<EntProtoId?>(this.SelectedShellType, hookCtx, context);
    target.SelectedShellType = target6;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleAmmoLoaderComponent target,
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
    RMCVehicleAmmoLoaderComponent target1 = (RMCVehicleAmmoLoaderComponent) target;
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
    RMCVehicleAmmoLoaderComponent target1 = (RMCVehicleAmmoLoaderComponent) target;
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
    RMCVehicleAmmoLoaderComponent target1 = (RMCVehicleAmmoLoaderComponent) target;
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
  virtual RMCVehicleAmmoLoaderComponent Component.Instantiate()
  {
    return new RMCVehicleAmmoLoaderComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class RMCVehicleAmmoLoaderComponent_AutoState : IComponentState
  {
    public string HardpointType;
    public EntProtoId? BulletType;
    public TimeSpan LoadDelay;
    public bool EnableShellSelection;
    public EntProtoId? SelectedShellType;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RMCVehicleAmmoLoaderComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ComponentGetState>(new ComponentEventRefHandler<RMCVehicleAmmoLoaderComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<RMCVehicleAmmoLoaderComponent, ComponentHandleState>(new ComponentEventRefHandler<RMCVehicleAmmoLoaderComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      RMCVehicleAmmoLoaderComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new RMCVehicleAmmoLoaderComponent.RMCVehicleAmmoLoaderComponent_AutoState()
      {
        HardpointType = component.HardpointType,
        BulletType = component.BulletType,
        LoadDelay = component.LoadDelay,
        EnableShellSelection = component.EnableShellSelection,
        SelectedShellType = component.SelectedShellType
      };
    }

    private void OnHandleState(
      EntityUid uid,
      RMCVehicleAmmoLoaderComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is RMCVehicleAmmoLoaderComponent.RMCVehicleAmmoLoaderComponent_AutoState current))
        return;
      component.HardpointType = current.HardpointType;
      component.BulletType = current.BulletType;
      component.LoadDelay = current.LoadDelay;
      component.EnableShellSelection = current.EnableShellSelection;
      component.SelectedShellType = current.SelectedShellType;
    }
  }
}
