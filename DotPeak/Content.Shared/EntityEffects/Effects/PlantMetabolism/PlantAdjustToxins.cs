// Decompiled with JetBrains decompiler
// Type: Content.Shared.EntityEffects.Effects.PlantMetabolism.PlantAdjustToxins
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.EntityEffects.Effects.PlantMetabolism;

public sealed class PlantAdjustToxins : 
  PlantAdjustAttribute<PlantAdjustToxins>,
  ISerializationGenerated<PlantAdjustToxins>,
  ISerializationGenerated
{
  public override string GuidebookAttributeName { get; set; } = "plant-attribute-toxins";

  public override bool GuidebookIsAttributePositive { get; protected set; }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref PlantAdjustToxins target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantAdjustAttribute<PlantAdjustToxins> target1 = (PlantAdjustAttribute<PlantAdjustToxins>) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (PlantAdjustToxins) target1;
    serialization.TryCustomCopy<PlantAdjustToxins>(this, ref target, hookCtx, false, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref PlantAdjustToxins target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref PlantAdjustAttribute<PlantAdjustToxins> target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantAdjustToxins target1 = (PlantAdjustToxins) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (PlantAdjustAttribute<PlantAdjustToxins>) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    PlantAdjustToxins target1 = (PlantAdjustToxins) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual PlantAdjustToxins PlantAdjustAttribute<PlantAdjustToxins>.Instantiate()
  {
    return new PlantAdjustToxins();
  }
}
