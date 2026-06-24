// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Supply.CivSupplyVendItemAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared._CIV14merka.Supply;

public sealed class CivSupplyVendItemAttemptEvent : CancellableEntityEventArgs
{
  public readonly EntityUid Actor;
  public readonly int Section;
  public readonly int Entry;

  public CivSupplyVendItemAttemptEvent(EntityUid actor, int section, int entry)
  {
    this.Actor = actor;
    this.Section = section;
    this.Entry = entry;
  }
}
