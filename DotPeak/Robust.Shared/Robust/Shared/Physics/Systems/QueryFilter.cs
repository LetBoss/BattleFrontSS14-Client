// Decompiled with JetBrains decompiler
// Type: Robust.Shared.Physics.Systems.QueryFilter
// Assembly: Robust.Shared, Version=272.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 00043EA9-5325-44A7-AF0D-91DD061626DD
// Assembly location: C:\Users\sus\AppData\Roaming\Space Station 14\launcher\engines\Robust.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Robust.Shared.Physics.Systems;

public record struct QueryFilter
{
  public long LayerBits;
  public long MaskBits;
  public Func<EntityUid, bool>? IsIgnored;
  public QueryFlags Flags;

  public QueryFilter()
  {
    this.LayerBits = 0L;
    this.MaskBits = 0L;
    this.IsIgnored = (Func<EntityUid, bool>) null;
    this.Flags = QueryFlags.Dynamic | QueryFlags.Static;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return ((EqualityComparer<long>.Default.GetHashCode(this.LayerBits) * -1521134295 + EqualityComparer<long>.Default.GetHashCode(this.MaskBits)) * -1521134295 + EqualityComparer<Func<EntityUid, bool>>.Default.GetHashCode(this.IsIgnored)) * -1521134295 + EqualityComparer<QueryFlags>.Default.GetHashCode(this.Flags);
  }

  [CompilerGenerated]
  public readonly bool Equals(QueryFilter other)
  {
    return EqualityComparer<long>.Default.Equals(this.LayerBits, other.LayerBits) && EqualityComparer<long>.Default.Equals(this.MaskBits, other.MaskBits) && EqualityComparer<Func<EntityUid, bool>>.Default.Equals(this.IsIgnored, other.IsIgnored) && EqualityComparer<QueryFlags>.Default.Equals(this.Flags, other.Flags);
  }
}
