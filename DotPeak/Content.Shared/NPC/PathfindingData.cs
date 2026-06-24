// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.PathfindingData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.Serialization;
using System;

#nullable enable
namespace Content.Shared.NPC;

[NetSerializable]
[Serializable]
public struct PathfindingData(PathfindingBreadcrumbFlag flag, int layer, int mask, float damage) : 
  IEquatable<PathfindingData>
{
  public PathfindingBreadcrumbFlag Flags = flag;
  public int CollisionLayer = layer;
  public int CollisionMask = mask;
  public float Damage = damage;

  public bool IsFreeSpace
  {
    get => this.Flags == PathfindingBreadcrumbFlag.None && this.Damage.Equals(0.0f);
  }

  public bool IsEquivalent(PathfindingData other)
  {
    return this.CollisionLayer.Equals(other.CollisionLayer) && this.CollisionMask.Equals(other.CollisionMask) && this.Flags.Equals((object) other.Flags);
  }

  public bool Equals(PathfindingData other)
  {
    return this.CollisionLayer.Equals(other.CollisionLayer) && this.CollisionMask.Equals(other.CollisionMask) && this.Flags.Equals((object) other.Flags) && this.Damage.Equals(other.Damage);
  }

  public override bool Equals(object? obj) => obj is PathfindingData other && this.Equals(other);

  public override int GetHashCode()
  {
    return HashCode.Combine<int, int, int>((int) this.Flags, this.CollisionLayer, this.CollisionMask);
  }
}
