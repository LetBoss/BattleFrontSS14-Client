// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.RayCastInput
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Physics.Systems;

internal record struct RayCastInput
{
  public Vector2 Origin;
  public Vector2 Translation;
  public float MaxFraction;

  public bool IsValidRay()
  {
    return Vector2Helpers.IsValid(this.Origin) && Vector2Helpers.IsValid(this.Translation) && MathHelper.IsValid(this.MaxFraction) && 0.0 <= (double) this.MaxFraction && (double) this.MaxFraction < 3.4028234663852886E+38;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return (EqualityComparer<Vector2>.Default.GetHashCode(this.Origin) * -1521134295 + EqualityComparer<Vector2>.Default.GetHashCode(this.Translation)) * -1521134295 + EqualityComparer<float>.Default.GetHashCode(this.MaxFraction);
  }

  [CompilerGenerated]
  public readonly bool Equals(RayCastInput other)
  {
    return EqualityComparer<Vector2>.Default.Equals(this.Origin, other.Origin) && EqualityComparer<Vector2>.Default.Equals(this.Translation, other.Translation) && EqualityComparer<float>.Default.Equals(this.MaxFraction, other.MaxFraction);
  }
}
