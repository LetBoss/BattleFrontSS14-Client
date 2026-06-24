// Decompiled with JetBrains decompiler
// Type: Content.Shared._RMC14.Deploy.RMCDeploySetup
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Numerics;

#nullable enable
namespace Content.Shared._RMC14.Deploy;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class RMCDeploySetup : 
  ISerializationHooks,
  ISerializationGenerated<RMCDeploySetup>,
  ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public EntProtoId Prototype;
  [DataField(null, false, 1, false, false, null)]
  public RMCDeploySetupMode Mode;
  [DataField(null, false, 1, false, false, null)]
  public bool NeverRedeployableSetup;
  [DataField(null, false, 1, false, false, null)]
  public bool StorageOriginalEntity;
  [DataField(null, false, 1, false, false, null)]
  public Vector2 Offset = Vector2.Zero;
  [DataField(null, false, 1, false, false, null)]
  public float Angle;
  [DataField(null, false, 1, false, false, null)]
  public bool Anchor = true;

  void ISerializationHooks.AfterDeserialization()
  {
    if (!this.StorageOriginalEntity)
      return;
    this.StorageOriginalEntity = false;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref RMCDeploySetup target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<RMCDeploySetup>(this, ref target, hookCtx, true, context))
      return;
    EntProtoId target1 = new EntProtoId();
    if (!serialization.TryCustomCopy<EntProtoId>(this.Prototype, ref target1, hookCtx, false, context))
      target1 = serialization.CreateCopy<EntProtoId>(this.Prototype, hookCtx, context);
    target.Prototype = target1;
    RMCDeploySetupMode target2 = RMCDeploySetupMode.Default;
    if (!serialization.TryCustomCopy<RMCDeploySetupMode>(this.Mode, ref target2, hookCtx, false, context))
      target2 = this.Mode;
    target.Mode = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.NeverRedeployableSetup, ref target3, hookCtx, false, context))
      target3 = this.NeverRedeployableSetup;
    target.NeverRedeployableSetup = target3;
    bool target4 = false;
    if (!serialization.TryCustomCopy<bool>(this.StorageOriginalEntity, ref target4, hookCtx, false, context))
      target4 = this.StorageOriginalEntity;
    target.StorageOriginalEntity = target4;
    Vector2 target5 = new Vector2();
    if (!serialization.TryCustomCopy<Vector2>(this.Offset, ref target5, hookCtx, false, context))
      target5 = serialization.CreateCopy<Vector2>(this.Offset, hookCtx, context);
    target.Offset = target5;
    float target6 = 0.0f;
    if (!serialization.TryCustomCopy<float>(this.Angle, ref target6, hookCtx, false, context))
      target6 = this.Angle;
    target.Angle = target6;
    bool target7 = false;
    if (!serialization.TryCustomCopy<bool>(this.Anchor, ref target7, hookCtx, false, context))
      target7 = this.Anchor;
    target.Anchor = target7;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref RMCDeploySetup target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    RMCDeploySetup target1 = (RMCDeploySetup) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public RMCDeploySetup Instantiate() => new RMCDeploySetup();
}
