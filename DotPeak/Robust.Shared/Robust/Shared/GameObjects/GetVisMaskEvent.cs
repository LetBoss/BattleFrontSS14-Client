// Decompiled with JetBrains decompiler
// Type: Robust.Shared.GameObjects.GetVisMaskEvent
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Robust.Shared.GameObjects;

[ByRefEvent]
public record struct GetVisMaskEvent
{
  public EntityUid Entity;
  public int VisibilityMask;

  public GetVisMaskEvent()
  {
    this.Entity = new EntityUid();
    this.VisibilityMask = 1;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<EntityUid>.Default.GetHashCode(this.Entity) * -1521134295 + EqualityComparer<int>.Default.GetHashCode(this.VisibilityMask);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetVisMaskEvent other)
  {
    return EqualityComparer<EntityUid>.Default.Equals(this.Entity, other.Entity) && EqualityComparer<int>.Default.Equals(this.VisibilityMask, other.VisibilityMask);
  }
}
