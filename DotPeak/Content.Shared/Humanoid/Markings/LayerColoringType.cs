// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.LayerColoringType
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using System;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[ImplicitDataDefinitionForInheritors]
public abstract class LayerColoringType : 
  ISerializationGenerated<LayerColoringType>,
  ISerializationGenerated
{
  [DataField("negative", false, 1, false, false, null)]
  public bool Negative { get; private set; }

  public abstract Color? GetCleanColor(Color? skin, Color? eyes, MarkingSet markingSet);

  public Color? GetColor(Color? skin, Color? eyes, MarkingSet markingSet)
  {
    Color? cleanColor = this.GetCleanColor(skin, eyes, markingSet);
    if (!cleanColor.HasValue || !this.Negative)
      return cleanColor;
    Color color = cleanColor.Value;
    color.R = 1f - color.R;
    color.G = 1f - color.G;
    color.B = 1f - color.B;
    return new Color?(color);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void InternalCopy(
    ref LayerColoringType target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<LayerColoringType>(this, ref target, hookCtx, false, context))
      return;
    bool target1 = false;
    if (!serialization.TryCustomCopy<bool>(this.Negative, ref target1, hookCtx, false, context))
      target1 = this.Negative;
    target.Negative = target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref LayerColoringType target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public virtual void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    LayerColoringType target1 = (LayerColoringType) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public virtual LayerColoringType Instantiate() => throw new NotImplementedException();
}
