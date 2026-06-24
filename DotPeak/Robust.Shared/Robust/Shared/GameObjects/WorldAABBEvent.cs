// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.WorldAABBEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.Maths;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public record struct WorldAABBEvent
{
  public Box2 AABB;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<Box2>.Default.GetHashCode(this.AABB);
  }

  [CompilerGenerated]
  public readonly bool Equals(WorldAABBEvent other)
  {
    return EqualityComparer<Box2>.Default.Equals(this.AABB, other.AABB);
  }
}
