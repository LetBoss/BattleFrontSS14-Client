// Decompiled with JetBrains decompiler
// Type: Content.Shared.Humanoid.Markings.MarkingPoints
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Humanoid.Markings;

[DataDefinition]
[NetSerializable]
[Serializable]
public sealed class MarkingPoints : ISerializationGenerated<MarkingPoints>, ISerializationGenerated
{
  [DataField(null, false, 1, true, false, null)]
  public int Points;
  [DataField(null, false, 1, true, false, null)]
  public bool Required;
  [DataField(null, false, 1, false, false, null)]
  public bool OnlyWhitelisted;
  [DataField(null, false, 1, false, false, null)]
  public List<ProtoId<MarkingPrototype>> DefaultMarkings = new List<ProtoId<MarkingPrototype>>();

  public static Dictionary<MarkingCategories, MarkingPoints> CloneMarkingPointDictionary(
    Dictionary<MarkingCategories, MarkingPoints> self)
  {
    Dictionary<MarkingCategories, MarkingPoints> dictionary = new Dictionary<MarkingCategories, MarkingPoints>();
    foreach ((MarkingCategories key, MarkingPoints markingPoints) in self)
      dictionary[key] = new MarkingPoints()
      {
        Points = markingPoints.Points,
        Required = markingPoints.Required,
        OnlyWhitelisted = markingPoints.OnlyWhitelisted,
        DefaultMarkings = markingPoints.DefaultMarkings
      };
    return dictionary;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref MarkingPoints target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<MarkingPoints>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Points, ref target1, hookCtx, false, context))
      target1 = this.Points;
    target.Points = target1;
    bool target2 = false;
    if (!serialization.TryCustomCopy<bool>(this.Required, ref target2, hookCtx, false, context))
      target2 = this.Required;
    target.Required = target2;
    bool target3 = false;
    if (!serialization.TryCustomCopy<bool>(this.OnlyWhitelisted, ref target3, hookCtx, false, context))
      target3 = this.OnlyWhitelisted;
    target.OnlyWhitelisted = target3;
    List<ProtoId<MarkingPrototype>> target4 = (List<ProtoId<MarkingPrototype>>) null;
    if (this.DefaultMarkings == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<List<ProtoId<MarkingPrototype>>>(this.DefaultMarkings, ref target4, hookCtx, true, context))
      target4 = serialization.CreateCopy<List<ProtoId<MarkingPrototype>>>(this.DefaultMarkings, hookCtx, context);
    target.DefaultMarkings = target4;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref MarkingPoints target,
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
    MarkingPoints target1 = (MarkingPoints) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public MarkingPoints Instantiate() => new MarkingPoints();
}
