// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Supply.CivSupplyRefillEntry
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Supply;

[NetSerializable]
[Serializable]
public struct CivSupplyRefillEntry
{
  public string ProtoId;
  public string Name;
  public string Category;
  public int Count;
  public int Periodic;
  public int UnitPrice;
}
