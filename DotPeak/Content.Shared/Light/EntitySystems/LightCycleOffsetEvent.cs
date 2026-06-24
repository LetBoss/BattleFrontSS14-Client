// Decompiled with JetBrains decompiler
// Type: Content.Shared.Light.EntitySystems.LightCycleOffsetEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Light.EntitySystems;

[ByRefEvent]
public record struct LightCycleOffsetEvent(TimeSpan Offset)
{
  public readonly TimeSpan Offset = Offset;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<TimeSpan>.Default.GetHashCode(this.Offset);
  }

  [CompilerGenerated]
  public readonly bool Equals(LightCycleOffsetEvent other)
  {
    return EqualityComparer<TimeSpan>.Default.Equals(this.Offset, other.Offset);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out TimeSpan Offset) => Offset = this.Offset;
}
