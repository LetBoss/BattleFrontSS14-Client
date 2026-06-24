// Decompiled with JetBrains decompiler
// Type: Content.Shared.Explosion.GetExplosionResistanceEvent
// Assembly: Content.Shared, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 5417D05E-B3D9-4989-8630-1DD892BD48BB
// Assembly location: C:\Users\sus\Desktop\SS14_VFS_Dump_20260624_230444\Content.Shared.dll

using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable
namespace Content.Shared.Explosion;

[ByRefEvent]
public record struct GetExplosionResistanceEvent(string ExplosionPrototype) : IInventoryRelayEvent
{
  public float DamageCoefficient = 1f;
  public readonly string ExplosionPrototype = ExplosionPrototype;

  SlotFlags IInventoryRelayEvent.TargetSlots => SlotFlags.WITHOUT_POCKET;

  [CompilerGenerated]
  public override readonly int GetHashCode()
  {
    return EqualityComparer<float>.Default.GetHashCode(this.DamageCoefficient) * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.ExplosionPrototype);
  }

  [CompilerGenerated]
  public readonly bool Equals(GetExplosionResistanceEvent other)
  {
    return EqualityComparer<float>.Default.Equals(this.DamageCoefficient, other.DamageCoefficient) && EqualityComparer<string>.Default.Equals(this.ExplosionPrototype, other.ExplosionPrototype);
  }

  [CompilerGenerated]
  public readonly void Deconstruct(out string ExplosionPrototype)
  {
    ExplosionPrototype = this.ExplosionPrototype;
  }
}
