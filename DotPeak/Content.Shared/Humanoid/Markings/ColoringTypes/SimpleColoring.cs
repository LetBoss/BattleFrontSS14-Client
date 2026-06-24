// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.SimpleColoring
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public sealed class SimpleColoring : 
  LayerColoringType,
  ISerializationGenerated<SimpleColoring>,
  ISerializationGenerated
{
  [DataField("color", false, 1, true, false, null)]
  public Color Color = Color.White;

  public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
  {
    return new Color?(this.Color);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref SimpleColoring target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LayerColoringType target1 = (LayerColoringType) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (SimpleColoring) target1;
    if (serialization.TryCustomCopy<SimpleColoring>(this, ref target, hookCtx, false, context))
      return;
    Color target2 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.Color, ref target2, hookCtx, false, context))
      target2 = serialization.CreateCopy<Color>(this.Color, hookCtx, context);
    target.Color = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref SimpleColoring target,
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
    SimpleColoring target1 = (SimpleColoring) target;
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
    SimpleColoring target1 = (SimpleColoring) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual SimpleColoring LayerColoringType.Instantiate() => new SimpleColoring();
}
