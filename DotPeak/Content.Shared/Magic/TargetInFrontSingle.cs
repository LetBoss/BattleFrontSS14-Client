// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.TargetInFrontSingle
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic;

public sealed class TargetInFrontSingle : 
  MagicInstantSpawnData,
  ISerializationGenerated<TargetInFrontSingle>,
  ISerializationGenerated
{
  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TargetInFrontSingle target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MagicInstantSpawnData target1 = (MagicInstantSpawnData) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TargetInFrontSingle) target1;
    serialization.TryCustomCopy<TargetInFrontSingle>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TargetInFrontSingle target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref MagicInstantSpawnData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TargetInFrontSingle target1 = (TargetInFrontSingle) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (MagicInstantSpawnData) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    TargetInFrontSingle target1 = (TargetInFrontSingle) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TargetInFrontSingle MagicInstantSpawnData.Instantiate() => new TargetInFrontSingle();
}
