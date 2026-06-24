// Decompiled with JetBrains decompiler
// Type: Content.Shared.Guidebook.GuidebookData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Serialization.Manager.Exceptions;
using Robust.Shared.Utility;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Guidebook;

[NetSerializable]
[DataDefinition]
[Serializable]
public sealed class GuidebookData : ISerializationGenerated<GuidebookData>, ISerializationGenerated
{
  [DataField(null, false, 1, false, false, null)]
  public Dictionary<string, Dictionary<string, Dictionary<string, object?>>> Data = new Dictionary<string, Dictionary<string, Dictionary<string, object>>>();
  public FrozenDictionary<string, FrozenDictionary<string, FrozenDictionary<string, object?>>> FrozenData;
  public bool IsFrozen;

  [DataField(null, false, 1, false, false, null)]
  public int Count { get; private set; }

  public void AddData(string prototype, string component, string field, object? value)
  {
    if (this.IsFrozen)
      throw new InvalidOperationException("Attempted to add data to GuidebookData while it is frozen!");
    this.Data.GetOrNew<string, Dictionary<string, Dictionary<string, object>>>(prototype).GetOrNew<string, Dictionary<string, object>>(component).Add(field, value);
    ++this.Count;
  }

  public bool TryGetValue(string prototype, string component, string field, out object? value)
  {
    if (!this.IsFrozen)
      throw new InvalidOperationException("Freeze the GuidebookData before calling TryGetValue!");
    FrozenDictionary<string, FrozenDictionary<string, object>> frozenDictionary1;
    FrozenDictionary<string, object> frozenDictionary2;
    if (this.FrozenData.TryGetValue(prototype, out frozenDictionary1) && frozenDictionary1.TryGetValue(component, out frozenDictionary2) && frozenDictionary2.TryGetValue(field, out value))
      return true;
    value = (object) null;
    return false;
  }

  public void Clear()
  {
    this.Data.Clear();
    this.Count = 0;
    this.IsFrozen = false;
  }

  public void Freeze()
  {
    Dictionary<string, FrozenDictionary<string, FrozenDictionary<string, object>>> source1 = new Dictionary<string, FrozenDictionary<string, FrozenDictionary<string, object>>>();
    foreach ((string key3, Dictionary<string, Dictionary<string, object>> dictionary1) in this.Data)
    {
      string key2 = key3;
      Dictionary<string, Dictionary<string, object>> dictionary2 = dictionary1;
      Dictionary<string, FrozenDictionary<string, object>> source2 = new Dictionary<string, FrozenDictionary<string, object>>();
      Dictionary<string, object> dictionary4;
      foreach ((key3, dictionary4) in dictionary2)
      {
        string key4 = key3;
        Dictionary<string, object> source3 = dictionary4;
        source2.Add(key4, source3.ToFrozenDictionary<string, object>());
      }
      source1.Add(key2, source2.ToFrozenDictionary<string, FrozenDictionary<string, object>>());
    }
    this.FrozenData = source1.ToFrozenDictionary<string, FrozenDictionary<string, FrozenDictionary<string, object>>>();
    this.Data.Clear();
    this.IsFrozen = true;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref GuidebookData target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<GuidebookData>(this, ref target, hookCtx, false, context))
      return;
    int target1 = 0;
    if (!serialization.TryCustomCopy<int>(this.Count, ref target1, hookCtx, false, context))
      target1 = this.Count;
    target.Count = target1;
    Dictionary<string, Dictionary<string, Dictionary<string, object>>> target2 = (Dictionary<string, Dictionary<string, Dictionary<string, object>>>) null;
    if (this.Data == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(this.Data, ref target2, hookCtx, true, context))
      target2 = serialization.CreateCopy<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(this.Data, hookCtx, context);
    target.Data = target2;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref GuidebookData target,
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
    GuidebookData target1 = (GuidebookData) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public GuidebookData Instantiate() => new GuidebookData();
}
