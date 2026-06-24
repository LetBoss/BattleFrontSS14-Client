// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.PathfindingBreadcrumb
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.NPC;

[NetSerializable]
[Serializable]
public struct PathfindingBreadcrumb(
  Vector2i coordinates,
  int layer,
  int mask,
  float damage,
  PathfindingBreadcrumbFlag flags = PathfindingBreadcrumbFlag.None) : 
  IEquatable<PathfindingBreadcrumb>
{
  public Vector2i Coordinates = coordinates;
  public PathfindingData Data = new PathfindingData(flags, layer, mask, damage);
  public static readonly PathfindingBreadcrumb Invalid = new PathfindingBreadcrumb()
  {
    Data = new PathfindingData(PathfindingBreadcrumbFlag.None, -1, -1, 0.0f)
  };

  public bool Equivalent(PathfindingBreadcrumb other) => this.Data.Equals(other.Data);

  public bool Equals(PathfindingBreadcrumb other)
  {
    return ((Vector2i) ref this.Coordinates).Equals(other.Coordinates) && this.Data.Equals(other.Data);
  }

  public override bool Equals(object? obj)
  {
    return obj is PathfindingBreadcrumb other && this.Equals(other);
  }

  public override int GetHashCode()
  {
    return HashCode.Combine<Vector2i, PathfindingData>(this.Coordinates, this.Data);
  }
}
