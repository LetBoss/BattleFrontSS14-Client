// Decompiled with JetBrains decompiler
// Type: Content.Shared.Weapons.Ranged.Systems.AttemptShootEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;

#nullable enable
namespace Content.Shared.Weapons.Ranged.Systems;

[ByRefEvent]
public record struct AttemptShootEvent(
  EntityUid User,
  string? Message,
  EntityCoordinates FromCoordinates,
  EntityCoordinates? ToCoordinates,
  bool Cancelled = false,
  bool ThrowItems = false,
  bool ResetCooldown = false)
;
