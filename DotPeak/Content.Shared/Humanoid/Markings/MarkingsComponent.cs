// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingsComponent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[RegisterComponent]
public sealed class MarkingsComponent : 
  Component,
  ISerializationGenerated<MarkingsComponent>,
  ISerializationGenerated
{
  public Dictionary<HumanoidVisualLayers, List<Marking>> ActiveMarkings = new Dictionary<HumanoidVisualLayers, List<Marking>>();
  [DataField("layerPoints", false, 1, false, false, null)]
  public Dictionary<MarkingCategories, MarkingPoints> LayerPoints = new Dictionary<MarkingCategories, MarkingPoints>();

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarkingsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    Component target1 = (Component) target;
    this.InternalCopy(ref target1, serialization, hookCtx, context);
    target = (MarkingsComponent) target1;
    if (serialization.TryCustomCopy<MarkingsComponent>(this, ref target, hookCtx, false, context))
      return;
    Dictionary<MarkingCategories, MarkingPoints> target2 = (Dictionary<MarkingCategories, MarkingPoints>) null;
    if (this.LayerPoints == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<MarkingCategories, MarkingPoints>>(this.LayerPoints, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<MarkingCategories, MarkingPoints>>(this.LayerPoints, hookCtx, context);
    target.LayerPoints = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarkingsComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref Component target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MarkingsComponent target1 = (MarkingsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (Component) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref object target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MarkingsComponent target1 = (MarkingsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void InternalCopy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    MarkingsComponent target1 = (MarkingsComponent) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (IComponent) target1;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public override void Copy(
    ref IComponent target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    this.InternalCopy(ref target, serialization, hookCtx, context);
  }

  [PreserveBaseOverrides]
  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  virtual MarkingsComponent Component.Instantiate() => new MarkingsComponent();
}
