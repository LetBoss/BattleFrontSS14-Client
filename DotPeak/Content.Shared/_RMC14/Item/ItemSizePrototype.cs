// Decompiled with JetBrains decompiler
// Type: Content.Shared.Item.ItemSizePrototype
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Localization;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager.Attributes;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Item;

[Prototype(null, 1)]
public sealed class ItemSizePrototype : IPrototype, IComparable<ItemSizePrototype>
{
  [DataField(null, false, 1, false, false, null)]
  public int Weight = 1;
  [DataField(null, false, 1, false, false, null)]
  public LocId Name;
  [DataField(null, false, 1, true, false, null)]
  public IReadOnlyList<Box2i> DefaultShape = (IReadOnlyList<Box2i>) new List<Box2i>();
  [DataField(null, false, 1, false, false, null)]
  public TimeSpan LatheTime = TimeSpan.FromSeconds(2L);

  [IdDataField(1, null)]
  public string ID { get; private set; }

  public int CompareTo(ItemSizePrototype? other)
  {
    return other != null ? this.Weight.CompareTo(other.Weight) : 0;
  }

  public static bool operator <(ItemSizePrototype a, ItemSizePrototype b) => a.Weight < b.Weight;

  public static bool operator >(ItemSizePrototype a, ItemSizePrototype b) => a.Weight > b.Weight;

  public static bool operator <=(ItemSizePrototype a, ItemSizePrototype b) => a.Weight <= b.Weight;

  public static bool operator >=(ItemSizePrototype a, ItemSizePrototype b) => a.Weight >= b.Weight;
}
