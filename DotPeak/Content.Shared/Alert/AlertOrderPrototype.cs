// Decompiled with JetBrains decompiler
// Type: Content.Shared.Alert.AlertOrderPrototype
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
namespace Content.Shared.Alert;

[Prototype(null, 1)]
[DataDefinition]
public sealed class AlertOrderPrototype : 
  IPrototype,
  IComparer<AlertPrototype>,
  ISerializationGenerated<AlertOrderPrototype>,
  ISerializationGenerated
{
  private readonly Dictionary<ProtoId<AlertPrototype>, int> _typeToIdx = new Dictionary<ProtoId<AlertPrototype>, int>();
  private readonly Dictionary<ProtoId<AlertCategoryPrototype>, int> _categoryToIdx = new Dictionary<ProtoId<AlertCategoryPrototype>, int>();

  [Robust.Shared.ViewVariables.ViewVariables]
  [IdDataField(1, null)]
  public string ID { get; private set; }

  [DataField(null, false, 1, false, false, null)]
  private (string type, string alert)[] Order
  {
    get
    {
      (string, string)[] order = new (string, string)[this._typeToIdx.Count + this._categoryToIdx.Count];
      foreach ((ProtoId<AlertPrototype> key, int num) in this._typeToIdx)
      {
        int index = num;
        order[index] = ("alertType", key.ToString());
      }
      ProtoId<AlertCategoryPrototype> key2;
      foreach ((key2, num) in this._categoryToIdx)
      {
        int index = num;
        order[index] = ("category", key2.ToString());
      }
      return order;
    }
    set
    {
      int num = 0;
      foreach ((string type, string alert) in value)
      {
        switch (type)
        {
          case "alertType":
            this._typeToIdx[ProtoId<AlertPrototype>.op_Implicit(alert)] = num++;
            break;
          case "category":
            this._categoryToIdx[ProtoId<AlertCategoryPrototype>.op_Implicit(alert)] = num++;
            break;
          default:
            throw new ArgumentException();
        }
      }
    }
  }

  private int GetOrderIndex(AlertPrototype alert)
  {
    int orderIndex;
    if (this._typeToIdx.TryGetValue(ProtoId<AlertPrototype>.op_Implicit(alert.ID), out orderIndex))
      return orderIndex;
    ProtoId<AlertCategoryPrototype>? category = alert.Category;
    if (category.HasValue)
    {
      Dictionary<ProtoId<AlertCategoryPrototype>, int> categoryToIdx = this._categoryToIdx;
      category = alert.Category;
      ProtoId<AlertCategoryPrototype> key = category.Value;
      ref int local = ref orderIndex;
      if (categoryToIdx.TryGetValue(key, out local))
        return orderIndex;
    }
    return -1;
  }

  public int Compare(AlertPrototype? x, AlertPrototype? y)
  {
    if (x == null && y == null)
      return 0;
    if (x == null)
      return 1;
    if (y == null)
      return -1;
    int orderIndex1 = this.GetOrderIndex(x);
    int orderIndex2 = this.GetOrderIndex(y);
    if (orderIndex1 == -1 && orderIndex2 == -1)
      return string.Compare(x.ID, y.ID, StringComparison.InvariantCulture);
    if (orderIndex1 == -1)
      return 1;
    if (orderIndex2 == -1)
      return -1;
    int num = orderIndex1 - orderIndex2;
    return num == 0 ? string.Compare(x.ID, y.ID, StringComparison.InvariantCulture) : num;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void InternalCopy(
    ref AlertOrderPrototype target,
    ISerializationManager serialization,
    SerializationHookContext hookCtx,
    ISerializationContext? context = null)
  {
    if (serialization.TryCustomCopy<AlertOrderPrototype>(this, ref target, hookCtx, false, context))
      return;
    string str = (string) null;
    if (this.ID == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<string>(this.ID, ref str, hookCtx, false, context))
      str = this.ID;
    target.ID = str;
    (string, string)[] valueTupleArray = ((string, string)[]) null;
    if (this.Order == null)
      throw new NullNotAllowedException();
    if (!serialization.TryCustomCopy<(string, string)[]>(this.Order, ref valueTupleArray, hookCtx, true, context))
      valueTupleArray = serialization.CreateCopy<(string, string)[]>(this.Order, hookCtx, context, false);
    target.Order = valueTupleArray;
  }

  [Obsolete("Use ISerializationManager.CopyTo instead")]
  public void Copy(
    ref AlertOrderPrototype target,
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
    AlertOrderPrototype target1 = (AlertOrderPrototype) target;
    this.Copy(ref target1, serialization, hookCtx, context);
    target = (object) target1;
  }

  [Obsolete("Use ISerializationManager.CreateCopy instead")]
  public AlertOrderPrototype Instantiate() => new AlertOrderPrototype();
}
