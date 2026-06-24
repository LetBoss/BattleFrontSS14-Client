// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.LayerColoringDefinition
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
public sealed class LayerColoringDefinition : 
  ISerializationGenerated<LayerColoringDefinition>,
  ISerializationGenerated
{
  [DataField("type", false, 1, false, false, null)]
  public LayerColoringType? Type = (LayerColoringType) new SkinColoring();
  [DataField("fallbackTypes", false, 1, false, false, null)]
  public List<LayerColoringType> FallbackTypes = new List<LayerColoringType>();
  [DataField("fallbackColor", false, 1, false, false, null)]
  public Color FallbackColor = Color.White;

  public Color GetColor(Color? skin, Color? eyes, MarkingSet markingSet)
  {
    Color? nullable = new Color?();
    if (this.Type != null)
      nullable = this.Type.GetColor(skin, eyes, markingSet);
    if (!nullable.HasValue)
    {
      foreach (LayerColoringType fallbackType in this.FallbackTypes)
      {
        nullable = fallbackType.GetColor(skin, eyes, markingSet);
        if (nullable.HasValue)
          break;
      }
    }
    return nullable ?? this.FallbackColor;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref LayerColoringDefinition target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<LayerColoringDefinition>(this, ref target, hookCtx, false, context))
      return;
    LayerColoringType target1 = (LayerColoringType) null;
    if (!serialization.TryCustomCopy<LayerColoringType>(this.Type, ref target1, hookCtx, true, context))
      target1 = serialization.CreateCopy<LayerColoringType>(this.Type, hookCtx, context);
    target.Type = target1;
    List<LayerColoringType> target2 = (List<LayerColoringType>) null;
    if (this.FallbackTypes == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<LayerColoringType>>(this.FallbackTypes, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<List<LayerColoringType>>(this.FallbackTypes, hookCtx, context);
    target.FallbackTypes = target2;
    Color target3 = new Color();
    if (!serialization.TryCustomCopy<Color>(this.FallbackColor, ref target3, hookCtx, false, context))
      target3 = serialization.CreateCopy<Color>(this.FallbackColor, hookCtx, context);
    target.FallbackColor = target3;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref LayerColoringDefinition target,
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
    LayerColoringDefinition target1 = (LayerColoringDefinition) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public LayerColoringDefinition Instantiate() => new LayerColoringDefinition();
}
