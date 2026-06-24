// Decompiled with JetBrains decompiler
// Type: Content.Shared._CIV14merka.Mortar.CivMortarInterceptAttemptEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable disable
namespace Content.Shared._CIV14merka.Mortar;

[ByRefEvent]
public record struct CivMortarInterceptAttemptEvent(
  int TeamId,
  MapCoordinates Source,
  MapCoordinates Target,
  float PopupRange)
{
  public bool Cancelled = false;

  public int TeamId { get; set; } = TeamId;

  public MapCoordinates Source { get; set; } = Source;

  public MapCoordinates Target { get; set; } = Target;

  public float PopupRange { get; set; } = PopupRange;
}
