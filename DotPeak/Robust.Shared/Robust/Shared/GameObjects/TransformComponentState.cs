// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.TransformComponentState
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using Robust.Shared.Serialization;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.GameObjects;

[NetSerializable]
[Serializable]
internal readonly record struct TransformComponentState(
  Vector2 localPosition,
  Angle rotation,
  NetEntity parentId,
  bool noLocalRotation,
  bool anchored) : IComponentState
{
  public readonly NetEntity ParentID = parentId;
  public readonly Vector2 LocalPosition = localPosition;
  public readonly Angle Rotation = rotation;
  public readonly bool NoLocalRotation = noLocalRotation;
  public readonly bool Anchored = anchored;

  [CompilerGenerated]
  public override int GetHashCode()
  {
    return (((EqualityComparer<NetEntity>.Default.GetHashCode(this.ParentID) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.LocalPosition)) * -1521134295 + EqualityComparer<Angle>.Default.GetHashCode(this.Rotation)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.NoLocalRotation)) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Anchored);
  }

  [CompilerGenerated]
  public bool Equals(TransformComponentState other)
  {
    return EqualityComparer<NetEntity>.Default.Equals(this.ParentID, other.ParentID) && EqualityComparer<Vector2>.Default.Equals(this.LocalPosition, other.LocalPosition) && EqualityComparer<Angle>.Default.Equals(this.Rotation, other.Rotation) && EqualityComparer<bool>.Default.Equals(this.NoLocalRotation, other.NoLocalRotation) && EqualityComparer<bool>.Default.Equals(this.Anchored, other.Anchored);
  }
}
