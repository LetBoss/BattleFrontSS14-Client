// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Events.OnRadiationOverlayUpdateEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Radiation.Systems;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Radiation.Events;

[NetSerializable]
[Serializable]
public sealed class OnRadiationOverlayUpdateEvent(
  double elapsedTimeMs,
  int sourcesCount,
  int receiversCount,
  List<DebugRadiationRay> rays) : EntityEventArgs
{
  public readonly double ElapsedTimeMs = elapsedTimeMs;
  public readonly int SourcesCount = sourcesCount;
  public readonly int ReceiversCount = receiversCount;
  public readonly List<DebugRadiationRay> Rays = rays;
}
