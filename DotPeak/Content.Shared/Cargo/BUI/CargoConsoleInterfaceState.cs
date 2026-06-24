// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.BUI.CargoConsoleInterfaceState
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Cargo.Prototypes;
using Robust.Shared.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Cargo.BUI;

[NetSerializable]
[Serializable]
public sealed class CargoConsoleInterfaceState : BoundUserInterfaceState
{
  public string Name;
  public int Count;
  public int Capacity;
  public NetEntity Station;
  public List<CargoOrderData> Orders;
  public List<ProtoId<CargoProductPrototype>> Products;

  public CargoConsoleInterfaceState(
    string name,
    int count,
    int capacity,
    NetEntity station,
    List<CargoOrderData> orders,
    List<ProtoId<CargoProductPrototype>> products)
  {
    this.Name = name;
    this.Count = count;
    this.Capacity = capacity;
    this.Station = station;
    this.Orders = orders;
    this.Products = products;
  }
}
