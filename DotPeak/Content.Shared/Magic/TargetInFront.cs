// Decompiled with JetBrains decompiler
// Type: Content.Shared.Magic.TargetInFront
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Magic;

public sealed class TargetInFront : 
  MagicInstantSpawnData,
  ISerializationGenerated<TargetInFront>,
  ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public int Width = 3;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref TargetInFront target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MagicInstantSpawnData target1 = (MagicInstantSpawnData) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (TargetInFront) target1;
    if (serialization.TryCustomCopy<TargetInFront>(this, ref target, hookCtx, false, context))
      return;
    int target2 = 0;
    if (!serialization.TryCustomCopy<int>(this.Width, ref target2, hookCtx, false, context))
      target2 = this.Width;
    target.Width = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref TargetInFront target,
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
    TargetInFront target1 = (TargetInFront) target;
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
    TargetInFront target1 = (TargetInFront) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual TargetInFront MagicInstantSpawnData.Instantiate() => new TargetInFront();
}
