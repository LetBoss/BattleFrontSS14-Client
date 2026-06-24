// Decompiled with JetBrains decompiler
// Type: Content.Shared.Radiation.Systems.RadiationRay
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;

#nullable enable
namespace Content.Shared.Radiation.Systems;

public struct RadiationRay(
  MapId mapId,
  EntityUid sourceUid,
  Vector2 source,
  EntityUid destinationUid,
  Vector2 destination,
  float rads)
{
  public MapId MapId = mapId;
  public EntityUid SourceUid = sourceUid;
  public Vector2 Source = source;
  public EntityUid DestinationUid = destinationUid;
  public Vector2 Destination = destination;
  public float Rads = rads;
  public Dictionary<NetEntity, List<(Vector2i, float)>>? Blockers = (Dictionary<NetEntity, List<(Vector2i, float)>>) null;

  public bool ReachedDestination => (double) this.Rads > 0.0;
}
