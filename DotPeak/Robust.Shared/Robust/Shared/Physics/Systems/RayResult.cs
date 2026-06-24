// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.RayResult
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.Physics.Systems;

public record struct RayResult
{
  public ValueList<RayHit> Results;
  public static readonly RayResult Empty = new RayResult();

  public RayResult() => this.Results = new ValueList<RayHit>();

  public bool Hit => this.Results.Count > 0;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<ValueList<RayHit>>.Default.GetHashCode(this.Results);
  }

  [CompilerGenerated]
  public readonly bool Equals(RayResult other)
  {
    return EqualityComparer<ValueList<RayHit>>.Default.Equals(this.Results, other.Results);
  }
}
