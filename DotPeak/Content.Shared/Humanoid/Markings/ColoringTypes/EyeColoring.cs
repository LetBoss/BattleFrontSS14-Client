// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.EyeColoring
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public sealed class EyeColoring : 
  LayerColoringType,
  ISerializationGenerated<EyeColoring>,
  ISerializationGenerated
{
  public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet) => eyes;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref EyeColoring target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LayerColoringType target1 = (LayerColoringType) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (EyeColoring) target1;
    serialization.TryCustomCopy<EyeColoring>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref EyeColoring target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref LayerColoringType target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeColoring target1 = (EyeColoring) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (LayerColoringType) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    EyeColoring target1 = (EyeColoring) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual EyeColoring LayerColoringType.Instantiate() => new EyeColoring();
}
