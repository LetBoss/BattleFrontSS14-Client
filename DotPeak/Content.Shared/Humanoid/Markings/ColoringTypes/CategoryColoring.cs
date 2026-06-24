// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.CategoryColoring
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

public sealed class CategoryColoring : 
  LayerColoringType,
  ISerializationGenerated<CategoryColoring>,
  ISerializationGenerated
{
  [DataField("category", false, 1, true, false, null)]
  public MarkingCategories Category;

  public override Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet)
  {
    Color? cleanColor = new Color?();
    IReadOnlyList<Marking> markings;
    if (markingSet.TryGetCategory(this.Category, out markings) && markings.Count > 0)
      cleanColor = new Color?(markings[0].MarkingColors.FirstOrDefault<Color>());
    return cleanColor;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref CategoryColoring target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LayerColoringType target1 = (LayerColoringType) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (CategoryColoring) target1;
    if (serialization.TryCustomCopy<CategoryColoring>(this, ref target, hookCtx, false, context))
      return;
    MarkingCategories target2 = MarkingCategories.Special;
    if (!serialization.TryCustomCopy<MarkingCategories>(this.Category, ref target2, hookCtx, false, context))
      target2 = this.Category;
    target.Category = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref CategoryColoring target,
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
    CategoryColoring target1 = (CategoryColoring) target;
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
    CategoryColoring target1 = (CategoryColoring) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual CategoryColoring LayerColoringType.Instantiate() => new CategoryColoring();
}
