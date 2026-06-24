// Decompiled with JetBrains decompiler
// Type: Content.Shared.NPC.Events.NPCSteeringDebugData
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.NPC.Events;

[NetSerializable]
[Serializable]
public readonly record struct NPCSteeringDebugData(
  NetEntity EntityUid,
  Vector2 Direction,
  float[] Interest,
  float[] Danger,
  List<Vector2> DangerPoints)
{
  public readonly NetEntity EntityUid = EntityUid;
  public readonly Vector2 Direction = Direction;
  public readonly float[] Interest = Interest;
  public readonly float[] Danger = Danger;
  public readonly List<Vector2> DangerPoints = DangerPoints;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((EqualityComparer<NetEntity>.Default.GetHashCode(this.EntityUid) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Direction)) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Interest)) * -1521134295 + EqualityComparer<float[]>.Default.GetHashCode(this.Danger)) * -1521134295 + EqualityComparer<List<Vector2>>.Default.GetHashCode(this.DangerPoints);
  }

  [CompilerGenerated]
  public bool Equals(NPCSteeringDebugData other)
  {
    return EqualityComparer<NetEntity>.Default.Equals(this.EntityUid, other.EntityUid) && EqualityComparer<Vector2>.Default.Equals(this.Direction, other.Direction) && EqualityComparer<float[]>.Default.Equals(this.Interest, other.Interest) && EqualityComparer<float[]>.Default.Equals(this.Danger, other.Danger) && EqualityComparer<List<Vector2>>.Default.Equals(this.DangerPoints, other.DangerPoints);
  }

  [CompilerGenerated]
  public void Deconstruct(
    out NetEntity EntityUid,
    out Vector2 Direction,
    out float[] Interest,
    out float[] Danger,
    out List<Vector2> DangerPoints)
  {
    EntityUid = this.EntityUid;
    Direction = this.Direction;
    Interest = this.Interest;
    Danger = this.Danger;
    DangerPoints = this.DangerPoints;
  }
}
