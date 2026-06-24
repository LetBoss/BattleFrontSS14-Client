// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.ShapeCastInput
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Systems;

internal record struct ShapeCastInput
{
  public Transform Origin;
  public Vector2[] Points;
  public int Count;
  public float Radius;
  public Vector2 Translation;
  public float MaxFraction;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((((EqualityComparer<Transform>.Default.GetHashCode(this.Origin) * -1521134295 + EqualityComparer<Vector2[]>.Default.GetHashCode(this.Points)) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.Count)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.Radius)) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Translation)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.MaxFraction);
  }

  [CompilerGenerated]
  public readonly bool Equals(ShapeCastInput other)
  {
    return EqualityComparer<Transform>.Default.Equals(this.Origin, other.Origin) && EqualityComparer<Vector2[]>.Default.Equals(this.Points, other.Points) && EqualityComparer<int>.Default.Equals(this.Count, other.Count) && EqualityComparer<float>.Default.Equals(this.Radius, other.Radius) && EqualityComparer<Vector2>.Default.Equals(this.Translation, other.Translation) && EqualityComparer<float>.Default.Equals(this.MaxFraction, other.MaxFraction);
  }
}
