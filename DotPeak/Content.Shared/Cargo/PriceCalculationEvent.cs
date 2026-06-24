// Decompiled with JetBrains decompiler
// Type: Content.Shared.Cargo.PriceCalculationEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable disable
namespace Content.Shared.Cargo;

[ByRefEvent]
public record struct PriceCalculationEvent
{
  public double Price;
  public bool Handled;

  public PriceCalculationEvent()
  {
    this.Price = 0.0;
    this.Handled = false;
  }

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<double>.Default.GetHashCode(this.Price) * -1521134295 + EqualityComparer<bool>.Default.GetHashCode(this.Handled);
  }

  [CompilerGenerated]
  public readonly bool Equals(PriceCalculationEvent other)
  {
    return EqualityComparer<double>.Default.Equals(this.Price, other.Price) && EqualityComparer<bool>.Default.Equals(this.Handled, other.Handled);
  }
}
