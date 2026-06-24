// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleGunnerViewUserComponent
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
[AutoGenerateComponentState(true, false)]
[Access(new Type[] {typeof (VehicleWeaponsSystem), typeof (VehicleGunnerViewSystem)})]
public sealed class VehicleGunnerViewUserComponent : 
  Component,
  ISerializationGenerated<VehicleGunnerViewUserComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float PvsScale;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CursorMaxOffset;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CursorOffsetSpeed = 0.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float CursorPvsIncrease;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleGunnerViewUserComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleGunnerViewUserComponent) target1;
    if (serialization.TryCustomCopy<VehicleGunnerViewUserComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.PvsScale, ref target2, hookCtx, false, context))
      target2 = this.PvsScale;
    target.PvsScale = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CursorMaxOffset, ref target3, hookCtx, false, context))
      target3 = this.CursorMaxOffset;
    target.CursorMaxOffset = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CursorOffsetSpeed, ref target4, hookCtx, false, context))
      target4 = this.CursorOffsetSpeed;
    target.CursorOffsetSpeed = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.CursorPvsIncrease, ref target5, hookCtx, false, context))
      target5 = this.CursorPvsIncrease;
    target.CursorPvsIncrease = target5;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleGunnerViewUserComponent target,
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
    VehicleGunnerViewUserComponent target1 = (VehicleGunnerViewUserComponent) target;
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
    VehicleGunnerViewUserComponent target1 = (VehicleGunnerViewUserComponent) target;
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
    VehicleGunnerViewUserComponent target1 = (VehicleGunnerViewUserComponent) target;
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
  virtual VehicleGunnerViewUserComponent Component.Instantiate()
  {
    return new VehicleGunnerViewUserComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleGunnerViewUserComponent_AutoState : IComponentState
  {
    public float PvsScale;
    public float CursorMaxOffset;
    public float CursorOffsetSpeed;
    public float CursorPvsIncrease;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleGunnerViewUserComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleGunnerViewUserComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleGunnerViewUserComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleGunnerViewUserComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleGunnerViewUserComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleGunnerViewUserComponent.VehicleGunnerViewUserComponent_AutoState()
      {
        PvsScale = component.PvsScale,
        CursorMaxOffset = component.CursorMaxOffset,
        CursorOffsetSpeed = component.CursorOffsetSpeed,
        CursorPvsIncrease = component.CursorPvsIncrease
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleGunnerViewUserComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleGunnerViewUserComponent.VehicleGunnerViewUserComponent_AutoState current))
        return;
      component.PvsScale = current.PvsScale;
      component.CursorMaxOffset = current.CursorMaxOffset;
      component.CursorOffsetSpeed = current.CursorOffsetSpeed;
      component.CursorPvsIncrease = current.CursorPvsIncrease;
      AfterAutoHandleStateEvent args1 = new AfterAutoHandleStateEvent(args.Current);
      this.EntityManager.EventBus.RaiseComponentEvent<AfterAutoHandleStateEvent, VehicleGunnerViewUserComponent>(uid, component, ref args1);
    }
  }
}
