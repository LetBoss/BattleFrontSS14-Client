// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleDeployableComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Analyzers;
using Robust.Shared.GameObjects;
using Robust.Shared.GameStates;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleDeploySystem), typeof (VehicleWeaponsSystem)})]
public sealed class VehicleDeployableComponent : 
  Component,
  ISerializationGenerated<VehicleDeployableComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deployed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Deploying;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool DeployingTo;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan DeployTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public TimeSpan UndeployTime = TimeSpan.FromSeconds(10L);
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool AutoTurretEnabled = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AutoTargetRange = 20f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float AutoTargetCooldown = 0.2f;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? Deployer;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan NextAutoTargetTime;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? TargetingDeployer;
  [DataField(null, false, 1, false, false, null)]
  public EntityUid? AutoTarget;
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan DeployEndTime;
  [DataField(null, false, 1, false, false, null)]
  public float AutoSpinSpeed = 90f;
  [DataField(null, false, 1, false, false, null)]
  public Angle AutoSpinWorldRotation = Angle.Zero;
  [DataField(null, false, 1, false, false, null)]
  public bool AutoSpinInitialized;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleDeployableComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleDeployableComponent) target1;
    if (serialization.TryCustomCopy<VehicleDeployableComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deployed, ref target2, hookCtx, false, context))
      target2 = this.Deployed;
    target.Deployed = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.Deploying, ref target3, hookCtx, false, context))
      target3 = this.Deploying;
    target.Deploying = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.DeployingTo, ref target4, hookCtx, false, context))
      target4 = this.DeployingTo;
    target.DeployingTo = target4;
    TimeSpan target5 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployTime, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<TimeSpan>(this.DeployTime, hookCtx, context);
    target.DeployTime = target5;
    TimeSpan target6 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.UndeployTime, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<TimeSpan>(this.UndeployTime, hookCtx, context);
    target.UndeployTime = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoTurretEnabled, ref target7, hookCtx, false, context))
      target7 = this.AutoTurretEnabled;
    target.AutoTurretEnabled = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AutoTargetRange, ref target8, hookCtx, false, context))
      target8 = this.AutoTargetRange;
    target.AutoTargetRange = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AutoTargetCooldown, ref target9, hookCtx, false, context))
      target9 = this.AutoTargetCooldown;
    target.AutoTargetCooldown = target9;
    EntityUid? target10 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Deployer, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<EntityUid?>(this.Deployer, hookCtx, context);
    target.Deployer = target10;
    TimeSpan target11 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.NextAutoTargetTime, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<TimeSpan>(this.NextAutoTargetTime, hookCtx, context);
    target.NextAutoTargetTime = target11;
    EntityUid? target12 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.TargetingDeployer, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<EntityUid?>(this.TargetingDeployer, hookCtx, context);
    target.TargetingDeployer = target12;
    EntityUid? target13 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.AutoTarget, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<EntityUid?>(this.AutoTarget, hookCtx, context);
    target.AutoTarget = target13;
    TimeSpan target14 = new TimeSpan();
    if (!serialization.TryCustomCopy<TimeSpan>(this.DeployEndTime, ref target14, hookCtx, false, context))
      target14 = serialization.CreateCopy<TimeSpan>(this.DeployEndTime, hookCtx, context);
    target.DeployEndTime = target14;
    float target15 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.AutoSpinSpeed, ref target15, hookCtx, false, context))
      target15 = this.AutoSpinSpeed;
    target.AutoSpinSpeed = target15;
    Angle target16 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AutoSpinWorldRotation, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<Angle>(this.AutoSpinWorldRotation, hookCtx, context);
    target.AutoSpinWorldRotation = target16;
    bool target17 = false;
    if (!serialization.TryCustomCopy<bool>(this.AutoSpinInitialized, ref target17, hookCtx, false, context))
      target17 = this.AutoSpinInitialized;
    target.AutoSpinInitialized = target17;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleDeployableComponent target,
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
    VehicleDeployableComponent target1 = (VehicleDeployableComponent) target;
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
    VehicleDeployableComponent target1 = (VehicleDeployableComponent) target;
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
    VehicleDeployableComponent target1 = (VehicleDeployableComponent) target;
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
  virtual VehicleDeployableComponent Component.Instantiate() => new VehicleDeployableComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleDeployableComponent_AutoState : IComponentState
  {
    public bool Deployed;
    public bool Deploying;
    public bool DeployingTo;
    public TimeSpan DeployTime;
    public TimeSpan UndeployTime;
    public bool AutoTurretEnabled;
    public float AutoTargetRange;
    public float AutoTargetCooldown;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleDeployableComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleDeployableComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleDeployableComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleDeployableComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleDeployableComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleDeployableComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleDeployableComponent.VehicleDeployableComponent_AutoState()
      {
        Deployed = component.Deployed,
        Deploying = component.Deploying,
        DeployingTo = component.DeployingTo,
        DeployTime = component.DeployTime,
        UndeployTime = component.UndeployTime,
        AutoTurretEnabled = component.AutoTurretEnabled,
        AutoTargetRange = component.AutoTargetRange,
        AutoTargetCooldown = component.AutoTargetCooldown
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleDeployableComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleDeployableComponent.VehicleDeployableComponent_AutoState current))
        return;
      component.Deployed = current.Deployed;
      component.Deploying = current.Deploying;
      component.DeployingTo = current.DeployingTo;
      component.DeployTime = current.DeployTime;
      component.UndeployTime = current.UndeployTime;
      component.AutoTurretEnabled = current.AutoTurretEnabled;
      component.AutoTargetRange = current.AutoTargetRange;
      component.AutoTargetCooldown = current.AutoTargetCooldown;
    }
  }
}
