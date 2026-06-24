// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.VehicleTurretComponent
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
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
[Access(new Type[] {typeof (VehicleWeaponsSystem), typeof (VehicleTurretSystem), typeof (VehicleTurretMuzzleSystem)})]
public sealed class VehicleTurretComponent : 
  Component,
  ISerializationGenerated<VehicleTurretComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RotateToCursor;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float FireWhileRotatingGraceDegrees;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseBarrelDirectionForShots;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float MaxShotCurvatureDegrees;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool StabilizedRotation;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RotationSpeed;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float ReverseDirectionDelay = 0.06f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public float RotationInputDeadzoneDegrees = 1.5f;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ShowOverlay;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool OffsetRotatesWithTurret;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 PixelOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OverlayRsi = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public string OverlayState = string.Empty;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseDirectionalOffsets;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 PixelOffsetNorth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 PixelOffsetEast = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 PixelOffsetSouth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 PixelOffsetWest = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle WorldRotation = Angle.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle TargetRotation = Angle.Zero;
  [NonSerialized]
  public EntityUid? VisualEntity;
  [NonSerialized]
  public Angle? PendingTargetRotation;
  [NonSerialized]
  public TimeSpan PendingTargetApplyAt = TimeSpan.Zero;
  [NonSerialized]
  public int PendingDirectionSign;
  [NonSerialized]
  public int LastAppliedDirectionSign;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref VehicleTurretComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (VehicleTurretComponent) target1;
    if (serialization.TryCustomCopy<VehicleTurretComponent>(this, ref target, hookCtx, false, context))
      return;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.RotateToCursor, ref target2, hookCtx, false, context))
      target2 = this.RotateToCursor;
    target.RotateToCursor = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.FireWhileRotatingGraceDegrees, ref target3, hookCtx, false, context))
      target3 = this.FireWhileRotatingGraceDegrees;
    target.FireWhileRotatingGraceDegrees = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseBarrelDirectionForShots, ref target4, hookCtx, false, context))
      target4 = this.UseBarrelDirectionForShots;
    target.UseBarrelDirectionForShots = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.MaxShotCurvatureDegrees, ref target5, hookCtx, false, context))
      target5 = this.MaxShotCurvatureDegrees;
    target.MaxShotCurvatureDegrees = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.StabilizedRotation, ref target6, hookCtx, false, context))
      target6 = this.StabilizedRotation;
    target.StabilizedRotation = target6;
    float target7 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RotationSpeed, ref target7, hookCtx, false, context))
      target7 = this.RotationSpeed;
    target.RotationSpeed = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ReverseDirectionDelay, ref target8, hookCtx, false, context))
      target8 = this.ReverseDirectionDelay;
    target.ReverseDirectionDelay = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.RotationInputDeadzoneDegrees, ref target9, hookCtx, false, context))
      target9 = this.RotationInputDeadzoneDegrees;
    target.RotationInputDeadzoneDegrees = target9;
    bool target10 = false;
    if (!serialization.TryCustomCopy<bool>(this.ShowOverlay, ref target10, hookCtx, false, context))
      target10 = this.ShowOverlay;
    target.ShowOverlay = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.OffsetRotatesWithTurret, ref target11, hookCtx, false, context))
      target11 = this.OffsetRotatesWithTurret;
    target.OffsetRotatesWithTurret = target11;
    Vector2 target12 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PixelOffset, ref target12, hookCtx, false, context))
      target12 = serialization.CreateCopy<Vector2>(this.PixelOffset, hookCtx, context);
    target.PixelOffset = target12;
    string target13 = (string) null;
    if (this.OverlayRsi == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OverlayRsi, ref target13, hookCtx, false, context))
      target13 = this.OverlayRsi;
    target.OverlayRsi = target13;
    string target14 = (string) null;
    if (this.OverlayState == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.OverlayState, ref target14, hookCtx, false, context))
      target14 = this.OverlayState;
    target.OverlayState = target14;
    bool target15 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDirectionalOffsets, ref target15, hookCtx, false, context))
      target15 = this.UseDirectionalOffsets;
    target.UseDirectionalOffsets = target15;
    Vector2 target16 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PixelOffsetNorth, ref target16, hookCtx, false, context))
      target16 = serialization.CreateCopy<Vector2>(this.PixelOffsetNorth, hookCtx, context);
    target.PixelOffsetNorth = target16;
    Vector2 target17 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PixelOffsetEast, ref target17, hookCtx, false, context))
      target17 = serialization.CreateCopy<Vector2>(this.PixelOffsetEast, hookCtx, context);
    target.PixelOffsetEast = target17;
    Vector2 target18 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PixelOffsetSouth, ref target18, hookCtx, false, context))
      target18 = serialization.CreateCopy<Vector2>(this.PixelOffsetSouth, hookCtx, context);
    target.PixelOffsetSouth = target18;
    Vector2 target19 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.PixelOffsetWest, ref target19, hookCtx, false, context))
      target19 = serialization.CreateCopy<Vector2>(this.PixelOffsetWest, hookCtx, context);
    target.PixelOffsetWest = target19;
    Angle target20 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.WorldRotation, ref target20, hookCtx, false, context))
      target20 = serialization.CreateCopy<Angle>(this.WorldRotation, hookCtx, context);
    target.WorldRotation = target20;
    Angle target21 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.TargetRotation, ref target21, hookCtx, false, context))
      target21 = serialization.CreateCopy<Angle>(this.TargetRotation, hookCtx, context);
    target.TargetRotation = target21;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref VehicleTurretComponent target,
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
    VehicleTurretComponent target1 = (VehicleTurretComponent) target;
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
    VehicleTurretComponent target1 = (VehicleTurretComponent) target;
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
    VehicleTurretComponent target1 = (VehicleTurretComponent) target;
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
  virtual VehicleTurretComponent Component.Instantiate() => new VehicleTurretComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class VehicleTurretComponent_AutoState : IComponentState
  {
    public bool RotateToCursor;
    public float FireWhileRotatingGraceDegrees;
    public bool UseBarrelDirectionForShots;
    public float MaxShotCurvatureDegrees;
    public bool StabilizedRotation;
    public float RotationSpeed;
    public float ReverseDirectionDelay;
    public float RotationInputDeadzoneDegrees;
    public bool ShowOverlay;
    public bool OffsetRotatesWithTurret;
    public Vector2 PixelOffset;
    public string OverlayRsi;
    public string OverlayState;
    public bool UseDirectionalOffsets;
    public Vector2 PixelOffsetNorth;
    public Vector2 PixelOffsetEast;
    public Vector2 PixelOffsetSouth;
    public Vector2 PixelOffsetWest;
    public Angle WorldRotation;
    public Angle TargetRotation;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class VehicleTurretComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<VehicleTurretComponent, ComponentGetState>(new ComponentEventRefHandler<VehicleTurretComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<VehicleTurretComponent, ComponentHandleState>(new ComponentEventRefHandler<VehicleTurretComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      VehicleTurretComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new VehicleTurretComponent.VehicleTurretComponent_AutoState()
      {
        RotateToCursor = component.RotateToCursor,
        FireWhileRotatingGraceDegrees = component.FireWhileRotatingGraceDegrees,
        UseBarrelDirectionForShots = component.UseBarrelDirectionForShots,
        MaxShotCurvatureDegrees = component.MaxShotCurvatureDegrees,
        StabilizedRotation = component.StabilizedRotation,
        RotationSpeed = component.RotationSpeed,
        ReverseDirectionDelay = component.ReverseDirectionDelay,
        RotationInputDeadzoneDegrees = component.RotationInputDeadzoneDegrees,
        ShowOverlay = component.ShowOverlay,
        OffsetRotatesWithTurret = component.OffsetRotatesWithTurret,
        PixelOffset = component.PixelOffset,
        OverlayRsi = component.OverlayRsi,
        OverlayState = component.OverlayState,
        UseDirectionalOffsets = component.UseDirectionalOffsets,
        PixelOffsetNorth = component.PixelOffsetNorth,
        PixelOffsetEast = component.PixelOffsetEast,
        PixelOffsetSouth = component.PixelOffsetSouth,
        PixelOffsetWest = component.PixelOffsetWest,
        WorldRotation = component.WorldRotation,
        TargetRotation = component.TargetRotation
      };
    }

    private void OnHandleState(
      EntityUid uid,
      VehicleTurretComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is VehicleTurretComponent.VehicleTurretComponent_AutoState current))
        return;
      component.RotateToCursor = current.RotateToCursor;
      component.FireWhileRotatingGraceDegrees = current.FireWhileRotatingGraceDegrees;
      component.UseBarrelDirectionForShots = current.UseBarrelDirectionForShots;
      component.MaxShotCurvatureDegrees = current.MaxShotCurvatureDegrees;
      component.StabilizedRotation = current.StabilizedRotation;
      component.RotationSpeed = current.RotationSpeed;
      component.ReverseDirectionDelay = current.ReverseDirectionDelay;
      component.RotationInputDeadzoneDegrees = current.RotationInputDeadzoneDegrees;
      component.ShowOverlay = current.ShowOverlay;
      component.OffsetRotatesWithTurret = current.OffsetRotatesWithTurret;
      component.PixelOffset = current.PixelOffset;
      component.OverlayRsi = current.OverlayRsi;
      component.OverlayState = current.OverlayState;
      component.UseDirectionalOffsets = current.UseDirectionalOffsets;
      component.PixelOffsetNorth = current.PixelOffsetNorth;
      component.PixelOffsetEast = current.PixelOffsetEast;
      component.PixelOffsetSouth = current.PixelOffsetSouth;
      component.PixelOffsetWest = current.PixelOffsetWest;
      component.WorldRotation = current.WorldRotation;
      component.TargetRotation = current.TargetRotation;
    }
  }
}
