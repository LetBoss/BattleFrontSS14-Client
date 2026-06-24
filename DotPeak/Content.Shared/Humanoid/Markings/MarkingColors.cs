// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingColors
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
public sealed class MarkingColors : ISerializationGenerated<MarkingColors>, ISerializationGenerated
{
  [DataField("default", true, 1, false, false, null)]
  public LayerColoringDefinition Default = new LayerColoringDefinition();
  [DataField("layers", true, 1, false, false, null)]
  public Dictionary<string, LayerColoringDefinition>? Layers;

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarkingColors target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MarkingColors>(this, ref target, hookCtx, false, context))
      return;
    LayerColoringDefinition target1 = (LayerColoringDefinition) null;
    if (this.Default == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<LayerColoringDefinition>(this.Default, ref target1, hookCtx, false, context))
    {
      if (this.Default == null)
        target1 = (LayerColoringDefinition) null;
      else
        serialization.CopyTo<LayerColoringDefinition>(this.Default, ref target1, hookCtx, context, true);
    }
    target.Default = target1;
    Dictionary<string, LayerColoringDefinition> target2 = (Dictionary<string, LayerColoringDefinition>) null;
    if (!serialization.TryCustomCopy<Dictionary<string, LayerColoringDefinition>>(this.Layers, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, LayerColoringDefinition>>(this.Layers, hookCtx, context);
    target.Layers = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarkingColors target,
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
    MarkingColors target1 = (MarkingColors) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MarkingColors Instantiate() => new MarkingColors();
}
