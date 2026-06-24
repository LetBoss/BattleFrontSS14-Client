// Decompiled with JetBrains decompiler
// Type: Content.Shared.Electrocution.ElectrocutedEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;

#nullable disable
namespace Content.Shared.Electrocution;

public sealed class ElectrocutedEvent : EntityEventArgs
{
  public readonly EntityUid TargetUid;
  public readonly EntityUid? SourceUid;
  public readonly float SiemensCoefficient;

  public ElectrocutedEvent(EntityUid targetUid, EntityUid? sourceUid, float siemensCoefficient)
  {
    this.TargetUid = targetUid;
    this.SourceUid = sourceUid;
    this.SiemensCoefficient = siemensCoefficient;
  }
}
