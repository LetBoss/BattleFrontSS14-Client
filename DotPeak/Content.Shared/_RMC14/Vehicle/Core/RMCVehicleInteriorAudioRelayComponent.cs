// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Vehicle.RMCVehicleInteriorAudioRelayComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared._RMC14.Vehicle;

[RegisterComponent]
public sealed class RMCVehicleInteriorAudioRelayComponent : 
  Component,
  ISerializationGenerated<RMCVehicleInteriorAudioRelayComponent>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public float ExteriorRange = 10f;
  [DataField(null, false, 1, false, false, null)]
  public float InteriorVolumeOffset = -3f;
  [DataField(null, false, 1, false, false, null)]
  public float InteriorMaxDistance = 18f;
  [DataField(null, false, 1, false, false, null)]
  public float InteriorReferenceDistance = 4f;
  [DataField(null, false, 1, false, false, null)]
  public bool InteriorNoOcclusion = true;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 InsideOffset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float InsideScale = 0.35f;
  [DataField(null, false, 1, false, false, null)]
  public float InsideClamp = 2f;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCVehicleInteriorAudioRelayComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (RMCVehicleInteriorAudioRelayComponent) target1;
    if (serialization.TryCustomCopy<RMCVehicleInteriorAudioRelayComponent>(this, ref target, hookCtx, false, context))
      return;
    float target2 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.ExteriorRange, ref target2, hookCtx, false, context))
      target2 = this.ExteriorRange;
    target.ExteriorRange = target2;
    float target3 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InteriorVolumeOffset, ref target3, hookCtx, false, context))
      target3 = this.InteriorVolumeOffset;
    target.InteriorVolumeOffset = target3;
    float target4 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InteriorMaxDistance, ref target4, hookCtx, false, context))
      target4 = this.InteriorMaxDistance;
    target.InteriorMaxDistance = target4;
    float target5 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InteriorReferenceDistance, ref target5, hookCtx, false, context))
      target5 = this.InteriorReferenceDistance;
    target.InteriorReferenceDistance = target5;
    bool target6 = false;
    if (!serialization.TryCustomCopy<bool>(this.InteriorNoOcclusion, ref target6, hookCtx, false, context))
      target6 = this.InteriorNoOcclusion;
    target.InteriorNoOcclusion = target6;
    Vector2 target7 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.InsideOffset, ref target7, hookCtx, false, context))
      target7 = serialization.CreateCopy<Vector2>(this.InsideOffset, hookCtx, context);
    target.InsideOffset = target7;
    float target8 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideScale, ref target8, hookCtx, false, context))
      target8 = this.InsideScale;
    target.InsideScale = target8;
    float target9 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.InsideClamp, ref target9, hookCtx, false, context))
      target9 = this.InsideClamp;
    target.InsideClamp = target9;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCVehicleInteriorAudioRelayComponent target,
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
    RMCVehicleInteriorAudioRelayComponent target1 = (RMCVehicleInteriorAudioRelayComponent) target;
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
    RMCVehicleInteriorAudioRelayComponent target1 = (RMCVehicleInteriorAudioRelayComponent) target;
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
    RMCVehicleInteriorAudioRelayComponent target1 = (RMCVehicleInteriorAudioRelayComponent) target;
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
  virtual RMCVehicleInteriorAudioRelayComponent Component.Instantiate()
  {
    return new RMCVehicleInteriorAudioRelayComponent();
  }
}
