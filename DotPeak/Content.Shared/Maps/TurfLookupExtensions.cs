// Decompiled with JetBrains decompiler
// Type: Content.Shared.Maps.TurfLookupExtensions
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using System.Collections.Generic;

#nullable enable
namespace Content.Shared.Maps;

public static class TurfLookupExtensions
{
  public static void GetEntitiesInTile(
    this EntityLookupSystem lookupSystem,
    TileRef turf,
    HashSet<EntityUid> intersecting,
    LookupFlags flags = LookupFlags.Static)
  {
    Box2Rotated worldBounds = lookupSystem.GetWorldBounds(turf);
    worldBounds.Box = ((Box2) ref worldBounds.Box).Scale(0.9f);
    lookupSystem.GetEntitiesIntersecting(turf.GridUid, worldBounds, intersecting, flags);
  }

  public static HashSet<EntityUid> GetEntitiesInTile(
    this EntityLookupSystem lookupSystem,
    TileRef turf,
    LookupFlags flags = LookupFlags.Static)
  {
    HashSet<EntityUid> intersecting = new HashSet<EntityUid>();
    lookupSystem.GetEntitiesInTile(turf, intersecting, flags);
    return intersecting;
  }
}
