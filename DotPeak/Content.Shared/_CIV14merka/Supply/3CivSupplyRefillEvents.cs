// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Supply.CivSupplySetPeriodicEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared._CIV14merka.Supply;

[NetSerializable]
[Serializable]
public sealed class CivSupplySetPeriodicEvent : EntityEventArgs
{
  public string ProtoId;
  public int Amount;

  public CivSupplySetPeriodicEvent(string protoId, int amount)
  {
    this.ProtoId = protoId;
    this.Amount = amount;
  }
}
