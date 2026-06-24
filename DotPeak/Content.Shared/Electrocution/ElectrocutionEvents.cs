// Decompiled with JetBrains decompiler
// Type: Content.Shared.Electrocution.ElectrocutionAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Electrocution;

public sealed class ElectrocutionAttemptEvent : CancellableEntityEventArgs, IInventoryRelayEvent
{
  public readonly EntityUid TargetUid;
  public readonly EntityUid? SourceUid;
  public float SiemensCoefficient = 1f;

  public SlotFlags TargetSlots { get; }

  public ElectrocutionAttemptEvent(
    EntityUid targetUid,
    EntityUid? sourceUid,
    float siemensCoefficient,
    SlotFlags targetSlots)
  {
    this.TargetUid = targetUid;
    this.TargetSlots = targetSlots;
    this.SourceUid = sourceUid;
    this.SiemensCoefficient = siemensCoefficient;
  }
}
