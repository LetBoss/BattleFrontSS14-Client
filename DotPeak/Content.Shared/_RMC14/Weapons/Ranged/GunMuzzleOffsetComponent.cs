// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Weapons.Ranged.GunMuzzleOffsetComponent
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
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Weapons.Ranged;

[RegisterComponent]
[NetworkedComponent]
[AutoGenerateComponentState(false, false)]
public sealed class GunMuzzleOffsetComponent : 
  Component,
  ISerializationGenerated<GunMuzzleOffsetComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 Offset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseDirectionalOffsets;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool RotateDirectionalOffsets;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetNorth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetEast = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetSouth = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 OffsetWest = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Vector2 MuzzleOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public Angle AngleOffset = Angle.Zero;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseContainerOwner = true;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool UseAimDirection;
  [DataField(null, false, 1, false, false, null)]
  [AutoNetworkedField]
  public bool ApplyToMuzzleFlash = true;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GunMuzzleOffsetComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (GunMuzzleOffsetComponent) target1;
    if (serialization.TryCustomCopy<GunMuzzleOffsetComponent>(this, ref target, hookCtx, false, context))
      return;
    Vector2 target2 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseDirectionalOffsets, ref target3, hookCtx, false, context))
      target3 = this.UseDirectionalOffsets;
    target.UseDirectionalOffsets = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.RotateDirectionalOffsets, ref target4, hookCtx, false, context))
      target4 = this.RotateDirectionalOffsets;
    target.RotateDirectionalOffsets = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetNorth, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.OffsetNorth, hookCtx, context);
    target.OffsetNorth = target5;
    Vector2 target6 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetEast, ref target6, hookCtx, false, context))
      target6 = serialization.CreateCopy<Vector2>(this.OffsetEast, hookCtx, context);
    target.OffsetEast = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetSouth, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.OffsetSouth, hookCtx, context);
    target.OffsetSouth = target7;
    Vector2 target8 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.OffsetWest, ref target8, hookCtx, false, context))
      target8 = serialization.CreateCopy<Vector2>(this.OffsetWest, hookCtx, context);
    target.OffsetWest = target8;
    Vector2 target9 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.MuzzleOffset, ref target9, hookCtx, false, context))
      target9 = serialization.CreateCopy<Vector2>(this.MuzzleOffset, hookCtx, context);
    target.MuzzleOffset = target9;
    Angle target10 = new Angle();
    if (!serialization.TryCustomCopy<Angle>(this.AngleOffset, ref target10, hookCtx, false, context))
      target10 = serialization.CreateCopy<Angle>(this.AngleOffset, hookCtx, context);
    target.AngleOffset = target10;
    bool target11 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseContainerOwner, ref target11, hookCtx, false, context))
      target11 = this.UseContainerOwner;
    target.UseContainerOwner = target11;
    bool target12 = false;
    if (!serialization.TryCustomCopy<bool>(this.UseAimDirection, ref target12, hookCtx, false, context))
      target12 = this.UseAimDirection;
    target.UseAimDirection = target12;
    bool target13 = false;
    if (!serialization.TryCustomCopy<bool>(this.ApplyToMuzzleFlash, ref target13, hookCtx, false, context))
      target13 = this.ApplyToMuzzleFlash;
    target.ApplyToMuzzleFlash = target13;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GunMuzzleOffsetComponent target,
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
    GunMuzzleOffsetComponent target1 = (GunMuzzleOffsetComponent) target;
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
    GunMuzzleOffsetComponent target1 = (GunMuzzleOffsetComponent) target;
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
    GunMuzzleOffsetComponent target1 = (GunMuzzleOffsetComponent) target;
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
  virtual GunMuzzleOffsetComponent Component.Instantiate() => new GunMuzzleOffsetComponent();

  [NetSerializable]
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public sealed class GunMuzzleOffsetComponent_AutoState : IComponentState
  {
    public Vector2 Offset;
    public bool UseDirectionalOffsets;
    public bool RotateDirectionalOffsets;
    public Vector2 OffsetNorth;
    public Vector2 OffsetEast;
    public Vector2 OffsetSouth;
    public Vector2 OffsetWest;
    public Vector2 MuzzleOffset;
    public Angle AngleOffset;
    public bool UseContainerOwner;
    public bool UseAimDirection;
    public bool ApplyToMuzzleFlash;
  }

  [RobustAutoGenerated]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class GunMuzzleOffsetComponent_AutoNetworkSystem : EntitySystem
  {
    public override void Initialize()
    {
      this.SubscribeLocalEvent<GunMuzzleOffsetComponent, ComponentGetState>(new ComponentEventRefHandler<GunMuzzleOffsetComponent, ComponentGetState>(this.OnGetState));
      this.SubscribeLocalEvent<GunMuzzleOffsetComponent, ComponentHandleState>(new ComponentEventRefHandler<GunMuzzleOffsetComponent, ComponentHandleState>(this.OnHandleState));
    }

    private void OnGetState(
      EntityUid uid,
      GunMuzzleOffsetComponent component,
      ref ComponentGetState args)
    {
      args.State = (IComponentState) new GunMuzzleOffsetComponent.GunMuzzleOffsetComponent_AutoState()
      {
        Offset = component.Offset,
        UseDirectionalOffsets = component.UseDirectionalOffsets,
        RotateDirectionalOffsets = component.RotateDirectionalOffsets,
        OffsetNorth = component.OffsetNorth,
        OffsetEast = component.OffsetEast,
        OffsetSouth = component.OffsetSouth,
        OffsetWest = component.OffsetWest,
        MuzzleOffset = component.MuzzleOffset,
        AngleOffset = component.AngleOffset,
        UseContainerOwner = component.UseContainerOwner,
        UseAimDirection = component.UseAimDirection,
        ApplyToMuzzleFlash = component.ApplyToMuzzleFlash
      };
    }

    private void OnHandleState(
      EntityUid uid,
      GunMuzzleOffsetComponent component,
      ref ComponentHandleState args)
    {
      if (!(args.Current is GunMuzzleOffsetComponent.GunMuzzleOffsetComponent_AutoState current))
        return;
      component.Offset = current.Offset;
      component.UseDirectionalOffsets = current.UseDirectionalOffsets;
      component.RotateDirectionalOffsets = current.RotateDirectionalOffsets;
      component.OffsetNorth = current.OffsetNorth;
      component.OffsetEast = current.OffsetEast;
      component.OffsetSouth = current.OffsetSouth;
      component.OffsetWest = current.OffsetWest;
      component.MuzzleOffset = current.MuzzleOffset;
      component.AngleOffset = current.AngleOffset;
      component.UseContainerOwner = current.UseContainerOwner;
      component.UseAimDirection = current.UseAimDirection;
      component.ApplyToMuzzleFlash = current.ApplyToMuzzleFlash;
    }
  }
}
