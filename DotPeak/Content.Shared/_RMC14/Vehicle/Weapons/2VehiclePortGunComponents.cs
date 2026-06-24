// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehiclePortGunOperatorComponent
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
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehiclePortGunSystem)})]
public sealed class VehiclePortGunOperatorComponent : 
  Component,
  ISerializationGenerated<VehiclePortGunOperatorComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Gun;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Vehicle;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public EntityUid? Controller;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehiclePortGunOperatorComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehiclePortGunOperatorComponent) target1;
    if (serialization.TryCustomCopy<VehiclePortGunOperatorComponent>(this, ref target, hookCtx, false, context))
      return;
    EntityUid? target2 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Gun, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<EntityUid?>(this.Gun, hookCtx, context);
    target.Gun = target2;
    EntityUid? target3 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Vehicle, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<EntityUid?>(this.Vehicle, hookCtx, context);
    target.Vehicle = target3;
    EntityUid? target4 = new EntityUid?();
    if (!serialization.TryCustomCopy<EntityUid?>(this.Controller, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<EntityUid?>(this.Controller, hookCtx, context);
    target.Controller = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehiclePortGunOperatorComponent target,
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
    VehiclePortGunOperatorComponent target1 = (VehiclePortGunOperatorComponent) target;
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
    VehiclePortGunOperatorComponent target1 = (VehiclePortGunOperatorComponent) target;
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
    VehiclePortGunOperatorComponent target1 = (VehiclePortGunOperatorComponent) target;
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
  virtual VehiclePortGunOperatorComponent Component.Instantiate()
  {
    return new VehiclePortGunOperatorComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehiclePortGunOperatorComponent_AutoState : IComponentState
  {
    public NetEntity? Gun;
    public NetEntity? Vehicle;
    public NetEntity? Controller;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehiclePortGunOperatorComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehiclePortGunOperatorComponent, ComponentGetState>(new ComponentEventRefHandler<VehiclePortGunOperatorComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehiclePortGunOperatorComponent, ComponentHandleState>(new ComponentEventRefHandler<VehiclePortGunOperatorComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehiclePortGunOperatorComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehiclePortGunOperatorComponent.VehiclePortGunOperatorComponent_AutoState()
      {
        Gun = this.GetNetEntity(component.Gun),
        Vehicle = this.GetNetEntity(component.Vehicle),
        Controller = this.GetNetEntity(component.Controller)
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehiclePortGunOperatorComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehiclePortGunOperatorComponent.VehiclePortGunOperatorComponent_AutoState current))
        return;
      component.Gun = this.EnsureEntity<VehiclePortGunOperatorComponent>(current.Gun, uid);
      component.Vehicle = this.EnsureEntity<VehiclePortGunOperatorComponent>(current.Vehicle, uid);
      component.Controller = this.EnsureEntity<VehiclePortGunOperatorComponent>(current.Controller, uid);
    }
  }
}
