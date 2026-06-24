// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretMuzzleComponent
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
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleTurretMuzzleSystem)})]
public sealed class VehicleTurretMuzzleComponent : 
  Component,
  ISerializationGenerated<VehicleTurretMuzzleComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool Alternate = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseDirectionalOffsets = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetLeft = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetRight = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetLeftNorth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetRightNorth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetLeftEast = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetRightEast = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetLeftSouth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetRightSouth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetLeftWest = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetRightWest = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseRightNext;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleTurretMuzzleComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleTurretMuzzleComponent) target1;
    if (serialization.TryCustomCopy<VehicleTurretMuzzleComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Alternate, ref target2, hookCtx, false, context))
      target2 = this.Alternate;
    target.Alternate = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDirectionalOffsets, ref target3, hookCtx, false, context))
      target3 = this.UseDirectionalOffsets;
    target.UseDirectionalOffsets = target3;
    Vector2 target4 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetLeft, ref target4, hookCtx, false, context))
      target4 = serialization.CreateCopy<Vector2>(this.OffsetLeft, hookCtx, context);
    target.OffsetLeft = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetRight, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.OffsetRight, hookCtx, context);
    target.OffsetRight = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetLeftNorth, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.OffsetLeftNorth, hookCtx, context);
    target.OffsetLeftNorth = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetRightNorth, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.OffsetRightNorth, hookCtx, context);
    target.OffsetRightNorth = target7;
    Vector2 target8 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetLeftEast, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2>(this.OffsetLeftEast, hookCtx, context);
    target.OffsetLeftEast = target8;
    Vector2 target9 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetRightEast, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2>(this.OffsetRightEast, hookCtx, context);
    target.OffsetRightEast = target9;
    Vector2 target10 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetLeftSouth, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Vector2>(this.OffsetLeftSouth, hookCtx, context);
    target.OffsetLeftSouth = target10;
    Vector2 target11 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetRightSouth, ref target11, hookCtx, false, context))
      target11 = serialization.CreateCopy<Vector2>(this.OffsetRightSouth, hookCtx, context);
    target.OffsetRightSouth = target11;
    Vector2 target12 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetLeftWest, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Vector2>(this.OffsetLeftWest, hookCtx, context);
    target.OffsetLeftWest = target12;
    Vector2 target13 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetRightWest, ref target13, hookCtx, false, context))
      target13 = serialization.CreateCopy<Vector2>(this.OffsetRightWest, hookCtx, context);
    target.OffsetRightWest = target13;
    bool target14 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseRightNext, ref target14, hookCtx, false, context))
      target14 = this.UseRightNext;
    target.UseRightNext = target14;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleTurretMuzzleComponent target,
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
    VehicleTurretMuzzleComponent target1 = (VehicleTurretMuzzleComponent) target;
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
    VehicleTurretMuzzleComponent target1 = (VehicleTurretMuzzleComponent) target;
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
    VehicleTurretMuzzleComponent target1 = (VehicleTurretMuzzleComponent) target;
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
  virtual VehicleTurretMuzzleComponent Component.Instantiate()
  {
    return new VehicleTurretMuzzleComponent();
  }

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleTurretMuzzleComponent_AutoState : IComponentState
  {
    public bool Alternate;
    public bool UseDirectionalOffsets;
    public Vector2 OffsetLeft;
    public Vector2 OffsetRight;
    public Vector2 OffsetLeftNorth;
    public Vector2 OffsetRightNorth;
    public Vector2 OffsetLeftEast;
    public Vector2 OffsetRightEast;
    public Vector2 OffsetLeftSouth;
    public Vector2 OffsetRightSouth;
    public Vector2 OffsetLeftWest;
    public Vector2 OffsetRightWest;
    public bool UseRightNext;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleTurretMuzzleComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleTurretMuzzleComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleTurretMuzzleComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleTurretMuzzleComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleTurretMuzzleComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleTurretMuzzleComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleTurretMuzzleComponent.VehicleTurretMuzzleComponent_AutoState()
      {
        Alternate = component.Alternate,
        UseDirectionalOffsets = component.UseDirectionalOffsets,
        OffsetLeft = component.OffsetLeft,
        OffsetRight = component.OffsetRight,
        OffsetLeftNorth = component.OffsetLeftNorth,
        OffsetRightNorth = component.OffsetRightNorth,
        OffsetLeftEast = component.OffsetLeftEast,
        OffsetRightEast = component.OffsetRightEast,
        OffsetLeftSouth = component.OffsetLeftSouth,
        OffsetRightSouth = component.OffsetRightSouth,
        OffsetLeftWest = component.OffsetLeftWest,
        OffsetRightWest = component.OffsetRightWest,
        UseRightNext = component.UseRightNext
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleTurretMuzzleComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleTurretMuzzleComponent.VehicleTurretMuzzleComponent_AutoState current))
        return;
      component.Alternate = current.Alternate;
      component.UseDirectionalOffsets = current.UseDirectionalOffsets;
      component.OffsetLeft = current.OffsetLeft;
      component.OffsetRight = current.OffsetRight;
      component.OffsetLeftNorth = current.OffsetLeftNorth;
      component.OffsetRightNorth = current.OffsetRightNorth;
      component.OffsetLeftEast = current.OffsetLeftEast;
      component.OffsetRightEast = current.OffsetRightEast;
      component.OffsetLeftSouth = current.OffsetLeftSouth;
      component.OffsetRightSouth = current.OffsetRightSouth;
      component.OffsetLeftWest = current.OffsetLeftWest;
      component.OffsetRightWest = current.OffsetRightWest;
      component.UseRightNext = current.UseRightNext;
    }
  }
}
