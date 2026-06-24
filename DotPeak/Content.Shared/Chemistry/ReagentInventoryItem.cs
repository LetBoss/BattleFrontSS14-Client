// Decompiled with JetBrains decompiler
// Type: Content.Shared.Chemistry.ReagentInventoryItem
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.FixedPoint;
using Content.Shared.Storage;
using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.Chemistry;

[NetSerializable]
[Serializable]
public sealed class ReagentInventoryItem(
  ItemStorageLocation storageLocation,
  string reagentLabel,
  FixedPoint2 quantity,
  Color reagentColor)
{
  public ItemStorageLocation StorageLocation = storageLocation;
  public string ReagentLabel = reagentLabel;
  public FixedPoint2 Quantity = quantity;
  public Color ReagentColor = reagentColor;
}
