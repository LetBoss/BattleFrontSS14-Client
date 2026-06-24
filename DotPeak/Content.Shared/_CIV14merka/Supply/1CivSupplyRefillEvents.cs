// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Supply.CivSupplyRefillStateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared._CIV14merka.Supply;

[NetSerializable]
[Serializable]
public sealed class CivSupplyRefillStateEvent : EntityEventArgs
{
  public List<CivSupplyRefillEntry> Entries;
  public int RefillThreshold;

  public CivSupplyRefillStateEvent(List<CivSupplyRefillEntry> entries, int refillThreshold)
  {
    this.Entries = entries;
    this.RefillThreshold = refillThreshold;
  }
}
