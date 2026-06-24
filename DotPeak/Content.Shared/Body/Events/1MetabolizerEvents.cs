// Decompiled with JetBrains decompiler
// Type: Content.Shared.Body.Events.ApplyMetabolicMultiplierEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Body.Events;

[ByRefEvent]
public readonly record struct ApplyMetabolicMultiplierEvent(float Multiplier)
{
  public readonly float Multiplier = Multiplier;

  [CompilerGenerated]
  public override int GetHashCode() => EqualityComparer<float>.Default.GetHashCode(this.Multiplier);

  [CompilerGenerated]
  public bool Equals(ApplyMetabolicMultiplierEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.Multiplier, other.Multiplier);
  }

  [CompilerGenerated]
  public void Deconstruct(out float Multiplier) => Multiplier = this.Multiplier;
}
